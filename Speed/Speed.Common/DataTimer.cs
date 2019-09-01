using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Speed
{


    /// <summary>
    /// Classe útil, que se atualiza automaticamente de tempos em tempos.
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class DataTimer<T>
    {

        T value;
        int milliseconds;
        Func<T> funcReturnValue;
        DateTime? last; // última vez que foi carregado o Value
        bool isUpdating;
        object locker = new object();
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="milliseconds">Intervalo, em milliseconds, que a classe deve se auto-atualizar</param>
        /// <param name="funcReturnValue">Func usada para a auto-atualização</param>
        public DataTimer(int milliseconds, Func<T> funcReturnValue)
        {
            this.milliseconds = milliseconds;
            this.funcReturnValue = funcReturnValue;
        }

        /// <summary>
        /// Valor 
        /// </summary>
        public T Value
        {
            get
            {
                while (isUpdating)
                {
                    System.Threading.Thread.Sleep(100); 
                }

                if (!isUpdating)
                {
                    if (!last.HasValue || DateTime.Now.Subtract(last.Value).TotalMilliseconds > milliseconds)
                        Update();
                }
                return this.value;
            }
            set
            {
                this.value = value;
            }

        }

        /// <summary>
        /// força uma atualização
        /// </summary>
        public void Update()
        {
            try
            {
                isUpdating = true;
                lock (locker)
                    this.value = funcReturnValue();
            }
            finally
            {
                isUpdating = false;
                last = DateTime.Now;
            }
        }

        public void Clear()
        {
            isUpdating = false;
            last = null;
        }

    }

}

