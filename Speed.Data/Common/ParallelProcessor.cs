namespace Speed.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class ParallelProcessor
    {
        private readonly int _maxDegreeOfParallelism;
        private readonly bool _continueOnError;

        private readonly List<Action> _actions = new List<Action>();

        private long _totalProcessed = 0;
        private long _totalErrors = 0;
        private readonly List<Exception> _exceptions = new List<Exception>();
        private readonly object _exceptionsLock = new object();

        public ParallelProcessor(int maxDegreeOfParallelism = 10, bool continueOnError = true)
        {
            _maxDegreeOfParallelism = Math.Max(1, maxDegreeOfParallelism);
            _continueOnError = continueOnError;
        }

        /// <summary>
        /// Adiciona um método/action para ser executado em paralelo
        /// </summary>
        public void AddAction(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");
            _actions.Add(action);
        }

        /// <summary>
        /// Adiciona vários métodos de uma vez
        /// </summary>
        public void AddActions(IEnumerable<Action> actions)
        {
            if (actions == null) return;
            _actions.AddRange(actions);
        }

        /// <summary>
        /// Executa todos os Actions adicionados em paralelo (com limite de simultaneidade)
        /// </summary>
        public ParallelResult Process(CancellationToken cancellationToken = default)
        {
            if (_actions.Count == 0)
                return new ParallelResult();

            var semaphore = new SemaphoreSlim(_maxDegreeOfParallelism);
            var tasks = new List<Task>();

            foreach (var action in _actions)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var localAction = action; // evita closure problem

                var task = Task.Factory.StartNew(() =>
                {
                    semaphore.Wait(cancellationToken);

                    try
                    {
                        localAction();
                        Interlocked.Increment(ref _totalProcessed);
                    }
                    catch (Exception ex)
                    {
                        Interlocked.Increment(ref _totalErrors);

                        lock (_exceptionsLock)
                        {
                            _exceptions.Add(ex);
                        }

                        if (!_continueOnError)
                            throw;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                tasks.Add(task);
            }

            // Aguarda todas as tarefas terminarem
            Task.WaitAll(tasks.ToArray());

            semaphore.Dispose();

            // Limpa a lista após executar (opcional)
            _actions.Clear();

            return new ParallelResult
            {
                TotalProcessed = _totalProcessed,
                TotalErrors = _totalErrors,
                Exceptions = _exceptions
            };
        }

        /// <summary>
        /// Limpa todas as actions adicionadas
        /// </summary>
        public void Clear()
        {
            _actions.Clear();
        }
    }
    
    // ======================= RESULTADO =======================
    public class ParallelResult
    {
        public long TotalProcessed { get; set; }
        public long TotalErrors { get; set; }
        public List<Exception> Exceptions { get; set; } = new List<Exception>();

        public bool HasErrors => TotalErrors > 0;
    }
}
