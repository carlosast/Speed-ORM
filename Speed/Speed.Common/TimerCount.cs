using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Speed
{
    /// <summary>
    /// Classe de medição de intervalos de tempo
    /// Útil e temppo de desenvolvimento para o programador
    /// </summary>
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class TimerCount
    {
        List<TimerCountInfo> timers;
        TimerCountInfo info;
        TimerCountInfo info0;
        /// <summary>
        /// Inicializa uma nova instância de TimerCount e já começa a medir o tempo
        /// </summary>
        public TimerCount()
            : this("", true)
        {
        }
        /// <summary>
        /// Inicializa uma nova instância de TimerCount
        /// </summary>
        /// <param name="start">Se start=true, começa a medir o tempo. Se start=false, não começa a mediro tempo</param>
        public TimerCount(bool start)
            : this("", start)
        {
        }
        /// <summary>
        /// Inicializa uma nova instância de TimerCount e já começa a medir o tempo
        /// </summary>
        /// <param name="title">Título do timer</param>
        public TimerCount(string title)
            : this(title, true)
        {
        }
        /// <summary>
        /// Inicializa uma nova instância de TimerCount
        /// </summary>
        /// <param name="title">Título do timer</param>
        /// <param name="start">Se start=true, começa a medir o tempo. Se start=false, não começa a mediro tempo</param>
        public TimerCount(string title, bool start)
        {
            timers = new List<TimerCountInfo>();
            info0 = info = new TimerCountInfo(title, start);
            timers.Add(info);
        }
        private void Stop()
        {
            if (info != null)
            {
                info.Stop();
                if (info != info0)
                    timers.Add(info);
                info = null;
            }
        }
        /// <summary>
        /// Cria um novo timer, sem título
        /// </summary>
        public void Next()
        {
            Next("", true);
        }
        /// <summary>
        /// Cria um novo timer, sem título
        /// </summary>
        /// <param name="start">Se start=true, começa a medir o tempo. Se start=false, não começa a mediro tempo</param>
        public void Next(bool start)
        {
            Next("", start);
        }
        
        /// <summary>
        /// Cria um novo timer
        /// </summary>
        /// <param name="title">Título do timer</param>
        public void Next(string title)
        {
            Next(title, true);
        }

        /// <summary>
        /// Cria um novo timer e executa uma Action
        /// </summary>
        /// <param name="title">Título do timer</param>
        public void Next(string title, Action action)
        {
            Next(title, true);
            action();
        }

        /// <summary>
        /// Cria um novo time
        /// </summary>
        /// <param name="title">Título do timer</param>
        /// <param name="start">Se start=true, começa a medir o tempo. Se start=false, não começa a mediro tempo</param>
        public void Next(string title, bool start)
        {
            Stop();
            info = new TimerCountInfo(title, start);
        }
        /// <summary>
        /// Pára e retorna o relatório do timer
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            Stop();
            string report = "";
            TimeSpan total = TimeSpan.Zero;
            var maxWidth = timers.Max(p => p.ProcessName.Length);
            maxWidth = Math.Max(maxWidth, "Total".Length);

            var ts = timers.Where(p => p.isValid).ToList();
            for (int i = 0; i < ts.Count; i++)
                total += timers[i].TimeElapsed;
            for (int i = 0; i < ts.Count; i++)
                report += timers[i].FormatTotal(maxWidth) + (ts.Count > 1 ? " --> " + string.Format("{0:p2}", timers[i].TimeElapsed.TotalMilliseconds / total.TotalMilliseconds).PadLeft(8) : "") + Environment.NewLine;
            if (ts.Count > 1 && total != TimeSpan.Zero)
            {
                report += TimerCountInfo.FormatTotal("Total", total, maxWidth);
            }
            return report;
        }
    }
    internal class TimerCountInfo
    {
        internal DateTime start, end;
        string title = "";
        private bool isStoped;
        const string format = "HH:mm:ss.ffffff";
        internal bool isValid = true;
        public TimerCountInfo()
            : this("", true)
        {
        }
        public TimerCountInfo(string title)
            : this(title, true)
        {
        }
        public TimerCountInfo(string title, bool isValid)
        {
            this.start = DateTime.Now;
            isStoped = false;
            this.isValid = isValid;
            this.title = title;
        }
        public void Stop()
        {
            this.end = DateTime.Now;
            isStoped = true;
        }
        public TimeSpan TimeElapsed
        {
            get
            {
                return (TimeSpan)end.Subtract(start);
            }
        }
        public string ProcessName
        {
            get { return title; }
        }
        public DateTime Start
        {
            get { return start; }
        }
        public DateTime End
        {
            get { return end; }
        }
        public bool IsStoped
        {
            get { return isStoped; }
        }
        public string FormatTotal(int maxWidth)
        {
            return FormatTotal(title, end.Subtract(start), maxWidth);
        }
        public static string FormatTotal(string title, TimeSpan ts, int maxWidth)
        {
            var total = Math.Round(ts.TotalSeconds, 6).ToString().PadRight(10);
            return title.PadRight(maxWidth) + " : " + total + "s";
        }
    }
}
