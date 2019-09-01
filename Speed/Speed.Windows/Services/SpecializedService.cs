using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Speed.Windows.Services
{


    /// <summary>
    /// Classe especializada de serviço
    /// Para usá-la, deve-se criar uma classe que herde dessa e sobrepor o método DoWork.
    /// Exemplo:
    /// class ServiceConnector : SpecializedService
    /// {
    ///     public ServiceConnector()
    ///     {
    ///     }
    ///     
    ///     public override void DoWork()
    ///     {
    ///         ...
    ///     }
    /// }
    public partial class SpecializedService : System.ServiceProcess.ServiceBase
    {

        #region Declarations

        private static int userCount = 0;
        private static ManualResetEvent pause = new ManualResetEvent(false);
        public bool LogEvents { get; set; }
        /// <summary>
        /// Modo de executar o serviço. se Continued, fica rodando indefinidamente, 
        /// chamando DoWork sucessivamente.
        /// </summary>
        public RunServiceMode RunMode = RunServiceMode.Continued;
        /// <summary>
        /// Intervalo, em mili-segundos, entre uma chamada de DoWork e outra
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Intervalo, em minutos, entre uma chamada de DoWork e outra
        /// </summary>
        public int IntervalMinutes
        {
            get { return this.Interval / (1000 * 60); }
            set { this.Interval = value * (1000 * 60); }
        }

        /// <summary>
        /// Se setado a true, sai
        /// </summary>
        public bool Cancel { get; set; }
        public string DisplayName { get; set; }

        private bool isRunning;

        [DllImport("ADVAPI32.DLL", EntryPoint = "SetServiceStatus")]
        public static extern bool SetServiceStatus(
                        IntPtr hServiceStatus,
                        SERVICE_STATUS lpServiceStatus
                        );
        private SERVICE_STATUS myServiceStatus;

        /// <summary>
        /// Thread usado pela classe fazer execução.
        /// Se desejar sobrepor a funcionalidade básica, sobreponha o método OnStart e,
        /// antes de chamar base.OnStart(args), defina um valor para WorkerThread
        /// </summary>
        public Thread WorkerThread = null;
        bool useWorkTrhead;

        #endregion Declarations

        #region Constructors

        public SpecializedService(bool useWorkTrhead)
            : base()
        {
            this.useWorkTrhead = useWorkTrhead;
            Cancel = false;
            RunMode = RunServiceMode.Continued;
            Interval = 0;
            LogEvents = false;

            CanPauseAndContinue = true;
            CanHandleSessionChangeEvent = true;
            CanShutdown = true;
            CanStop = true;
            ServiceName = "";
        }

        #endregion Constructors

        #region Override

        // Start the service.
        protected override void OnStart(string[] args)
        {
            Cancel = false;
            IntPtr handle = this.ServiceHandle;
            myServiceStatus.currentState = (int)State.SERVICE_START_PENDING;
            // SetServiceStatus(handle, myServiceStatus);

            // Start a separate thread that does the actual work.

            if ((WorkerThread == null && useWorkTrhead) ||
                ((WorkerThread.ThreadState &
                 (System.Threading.ThreadState.Unstarted | System.Threading.ThreadState.Stopped)) != 0))
            {
                writeEvent("OnStart", "Starting the service worker thread.");
                WorkerThread = new Thread(new ThreadStart(run));
                WorkerThread.Start();
            }
            if (WorkerThread != null)
            {
                writeEvent("OnStart", "Worker thread state = " + WorkerThread.ThreadState.ToString());
            }

            myServiceStatus.currentState = (int)State.SERVICE_RUNNING;
            // SetServiceStatus(handle, myServiceStatus);
            writeEvent("OnStart", "Starting ...");

            /*
            // Get arguments from the ImagePath string value for the service's registry 
            // key (HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\MYSERVICENAMEService).
            // These arguments are not used by this sample, this code is only intended to
            // demonstrate how to obtain the arguments.
            string[] imagePathArgs = Environment.GetCommandLineArgs();
            if (imagePathArgs.Length > 1)
            {
                writeEvent("ImagePath", "argument 1: " + imagePathArgs[1]);
                if (imagePathArgs.Length > 2)
                    writeEvent("ImagePath", "argument 2: " + imagePathArgs[2]);
            }
            */

            // Get values for arguments passed in from the Services control panel or
            // by the ServiceController class Start(string[]) method.
            // Note:  The arguments are not persisted by the control panel. You must
            // open the properties for the service, set the arguments, then start the
            // service. You may find this functionality useful when debugging a service.
            // These arguments are not used by this sample, this code is only
            // intended to demonstrate how to obtain the arguments.
            if (args.Length > 0)
            {
                writeEvent("OnStart", "Arguments: " + args[0]);
                if (args.Length > 1)
                    writeEvent("Arguments", args[1]);
            }

            //if (!useWorkTrhead)
            //{
            //    string arg = (args != null && args.Length >= 2) ? args[1].ToUpper() : "";
            //    if (!"/R/L/I/U".Contains(arg))
            //        DoWork();
            //}
        }

        // Stop this service.
        protected override void OnStop()
        {
            // New in .NET Framework version 2.0.
            writeEvent("OnStop", "RequestAdditionalTime");
            this.RequestAdditionalTime(1000);
            // Signal the worker thread to exit.
            if ((WorkerThread != null) && (WorkerThread.IsAlive))
            {
                writeEvent("OnStop", "Stopping the service worker thread.");
                Cancel = true;
                pause.Reset();
                Thread.Sleep(1000);
                WorkerThread.Abort();
            }
            if (WorkerThread != null)
            {
                writeEvent("OnStop", "Worker thread state = " + WorkerThread.ThreadState.ToString());
            }
            // Indicate a successful exit.
            this.ExitCode = 0;
        }

        // Pause the service.
        protected override void OnPause()
        {
            // Pause the worker thread.
            if ((WorkerThread != null) &&
                (WorkerThread.IsAlive) &&
                ((WorkerThread.ThreadState &
                 (System.Threading.ThreadState.Suspended | System.Threading.ThreadState.SuspendRequested)) == 0))
            {
                writeEvent("OnPause", "Pausing the service worker thread.");

                pause.Reset();
                Thread.Sleep(5000);
            }

            if (WorkerThread != null)
            {
                writeEvent("OnPause", "Worker thread state = " +
                    WorkerThread.ThreadState.ToString());
            }
        }

        protected override void OnShutdown()
        {
            writeEvent("OnShutdown", "Stopping ...");
        }

        // Continue a paused service.
        protected override void OnContinue()
        {

            // Signal the worker thread to continue.
            if ((WorkerThread != null) &&
                ((WorkerThread.ThreadState &
                 (System.Threading.ThreadState.Suspended | System.Threading.ThreadState.SuspendRequested)) != 0))
            {
                writeEvent("OnContinue", "Resuming the service worker thread.");

                pause.Set();

            }
            if (WorkerThread != null)
            {
                writeEvent("OnContinue", "Worker thread state = " +
                    WorkerThread.ThreadState.ToString());
            }
        }

        // Handle a custom command.
        protected override void OnCustomCommand(int command)
        {
            writeEvent("OnCustomCommand", "Custom command received: " +
                command.ToString());

            // If the custom command is recognized,
            // signal the worker thread appropriately.

            switch (command)
            {
                case (int)SpecializedCustomCommands.StopWorker:
                    // Signal the worker thread to terminate.
                    // For this custom command, the main service
                    // continues to run without a worker thread.
                    pause.Reset();
                    break;

                case (int)SpecializedCustomCommands.RestartWorker:

                    // Restart the worker thread if necessary.
                    pause.Set();
                    break;

                case (int)SpecializedCustomCommands.CheckWorker:
                    // Log the current worker thread state.
                    writeEvent("OnCustomCommand", "Worker thread state = " +
                        WorkerThread.ThreadState.ToString());

                    break;

                default:
                    writeEvent("OnCustomCommand",
                        "default");
                    break;
            }
        }

        // Handle a session change notice
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            writeEvent("OnSessionChange", "Session change notice received: " +
                changeDescription.Reason.ToString() + "  Session ID: " +
                changeDescription.SessionId.ToString());

            switch (changeDescription.Reason)
            {
                case SessionChangeReason.SessionLogon:
                    userCount += 1;
                    writeEvent("OnSessionChange",
                        "SessionLogon, total users: " +
                        userCount.ToString());
                    break;
                case SessionChangeReason.SessionLogoff:

                    userCount -= 1;
                    writeEvent("OnSessionChange",
                        "SessionLogoff, total users: " +
                        userCount.ToString());
                    break;
                case SessionChangeReason.RemoteConnect:
                    userCount += 1;
                    writeEvent("OnSessionChange",
                        "RemoteConnect, total users: " +
                        userCount.ToString());
                    break;
                case SessionChangeReason.RemoteDisconnect:
                    userCount -= 1;
                    writeEvent("OnSessionChange",
                        "RemoteDisconnect, total users: " +
                        userCount.ToString());
                    break;
                case SessionChangeReason.SessionLock:
                    writeEvent("OnSessionChange",
                        "SessionLock");
                    break;
                case SessionChangeReason.SessionUnlock:
                    writeEvent("OnSessionChange",
                        "SessionUnlock");
                    break;
                default:
                    break;
            }
        }

        #endregion Override

        #region Properties

        public bool IsRunning
        {
            get { return isRunning; }
        }

        #endregion Properties

        #region DoWork

        // Define a simple method that runs as the worker thread for 
        // the service.  
        private void run()
        {
            writeEvent("WorkerThread", "Starting the service worker thread.");

            try
            {
                do
                {
                    isRunning = true;
                    DoWork();
                    isRunning = false;

                    // pause.WaitOne();

                    if (!Cancel && Interval > 0 && RunMode == RunServiceMode.Continued)
                    {
                        // Log.Debug("SpecializedService.run", "Sleeping " + Interval);
                        Thread.Sleep(Interval);
                    }
                }
                while (!Cancel && RunMode == RunServiceMode.Continued);
            }
            catch (ThreadAbortException)
            {
                isRunning = false;

                // Another thread has signalled that this worker
                // thread must terminate.  Typically, this occurs when
                // the main service thread receives a service stop 
                // command.

                // Write a trace line indicating that the worker thread
                // is exiting.  Notice that this simple thread does
                // not have any local objects or data to clean up.
                writeEvent("WorkerThread", "Thread abort signaled.");
            }

            writeEvent("WorkerThread", "Exiting the service worker thread.");
        }

        public virtual void DoWork()
        {
        }

        public void Execute()
        {
            bool isInteractive = false;
            try
            {
                Log.Clear();
                var args = System.Environment.GetCommandLineArgs();

                string arg = (args != null && args.Length >= 2) ? args[1].ToUpper() : "";

                switch (arg)
                {
                    case "/R":
                        isInteractive = false;
                        RunMode = RunServiceMode.OneTime;
                        Log.Info("Executando em modo console.");
                        run();
                        break;

                    case "/L":
                        isInteractive = true;
                        RunMode = RunServiceMode.Continued;
                        Log.Info("Executando em modo console.");
                        run();
                        break;

                    case "/I":
                        isInteractive = true;
                        if (Install())
                        {
                            Log.Info("Serviço '{0}' instalado com sucesso.", ServiceName);
                            if (isInteractive)
                                ProgramBase.ShowInformation(string.Format("Serviço '{0}' instalado com sucesso.", ServiceName));
                        }
                        else
                        {
                            Log.Info("Erro ao instalar o serviço '{0}'.", ServiceName);
                            if (isInteractive)
                                ProgramBase.ShowError(string.Format("Erro ao instalar o serviço '{0}'.", ServiceName));
                        }
                        break;

                    case "/U":
                        isInteractive = true;
                        if (UnInstall())
                        {
                            Log.Info("Serviço desinstalado com sucesso.");
                            if (isInteractive)
                                ProgramBase.ShowInformation(string.Format("Serviço '{0}' desinstalado com sucesso.", ServiceName));
                        }
                        else
                        {
                            Log.Info("Erro ao desinstalar o serviço.");
                            if (isInteractive)
                                ProgramBase.ShowError(string.Format("Erro ao desinstalar o serviço '{0}'.", ServiceName));
                        }
                        break;

                    default:
                        System.ServiceProcess.ServiceBase.Run(this);
                        break;

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Execute");
                if (isInteractive)
                    ProgramBase.ShowError(ex);
            }

            Log.Info(ServiceName + ".Main - Saindo...");

        }

        #endregion DoWork

        //string GetName()
        //{
        //    return Conv.HasData(this.DisplayName) ? DisplayName : ServiceName;
        //}

        #region Events

        private void writeEvent(string source, string message)
        {
            if (LogEvents)
                WriteEvent(source, message);
        }

        public void WriteEvent(string source, string message)
        {
            EventLog.WriteEntry(ServiceName + " - " + source, DateTime.Now.ToLongTimeString() + " - " + message);
        }

        #endregion Events

        #region Install

        public bool Install()
        {
            return DirectServiceInstaller.InstallService(Assembly.GetEntryAssembly().Location, ServiceName, DisplayName);
        }

        public bool UnInstall()
        {
            return DirectServiceInstaller.UnInstallService(ServiceName);
        }

        #endregion Install

    }

    #region SpecializedCustomCommands

    /// <summary>
    /// Define custom commands for the SimpleService.
    /// </summary>
    public enum SpecializedCustomCommands
    {
        StopWorker = 128,
        RestartWorker,
        CheckWorker
    };

    #endregion SpecializedCustomCommands

    #region SERVICE_STATUS

    [StructLayout(LayoutKind.Sequential)]
    public struct SERVICE_STATUS
    {
        public int serviceType;
        public int currentState;
        public int controlsAccepted;
        public int win32ExitCode;
        public int serviceSpecificExitCode;
        public int checkPoint;
        public int waitHint;
    }

    #endregion SERVICE_STATUS

    #region State

    public enum State
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    #endregion State

    #region RunServiceMode

    /// <summary>
    /// Modo de executar o serviço
    /// </summary>
    public enum RunServiceMode
    {
        /// <summary>
        /// Chama o método DoWork indefinidamente, uma vez após a outra
        /// </summary>
        Continued,
        /// <summary>
        /// Chama o método DoWork apenas uma vez e sai
        /// </summary>
        OneTime
    }

    #endregion ServiceMode



}
