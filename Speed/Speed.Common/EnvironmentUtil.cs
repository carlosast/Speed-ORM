using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Speed
{
    class EnvironmentUtil
    {
    }
}

namespace EnvironmentTestConsole
{    ///     /// Summary description for Class1.    ///     
    class Class1
    {        ///         /// The main entry point for the application.        ///        [STAThread]
        public static void Main(string[] args)
        {            // reading the entire command line            
            // including the path to the application:            
            String s = Environment.CommandLine;
            Console.WriteLine(s);
            // reading each argument individually:            
            foreach (String s1 in Environment.GetCommandLineArgs())
                Console.WriteLine(s1 + "\n");
            // reading a specific argument:            
            if (Environment.GetCommandLineArgs().Length > 0)
            {
                s = Environment.GetCommandLineArgs().GetValue(0).ToString();
                Console.WriteLine(s + "\n");
            }
            // manipulating the current working directory:            
            Environment.CurrentDirectory = @"C:\Temp";
            Console.WriteLine("Current directory is: " + Environment.CurrentDirectory);
            // getting the computer and user names:            
            Console.WriteLine("Machine= " + Environment.MachineName);
            Console.WriteLine("User= " + Environment.UserDomainName + "\\" + Environment.UserName);
            Console.WriteLine("Is Interactive= " + Environment.UserInteractive.ToString());
            // reading all environment variables            
            foreach (String s2 in Environment.GetEnvironmentVariables().Keys)
                Console.WriteLine(s2 + "=" + Environment.GetEnvironmentVariable(s2).ToString());
            // reading a specific variable            
            s = Environment.GetEnvironmentVariable("PATH").ToString();
            // translating or expanding strings containing            
            // variable references            
            Console.WriteLine(Environment.ExpandEnvironmentVariables(@"User%userdomain%\%username% on %computername%\n"));
            // identifying logical drive letters            
            foreach (String s3 in Environment.GetLogicalDrives())
                Console.WriteLine("Drive: " + s3 + "\n");
            // locating the system folder            
            Console.WriteLine("System Dir= " + Environment.SystemDirectory + "\n");
            // locating all system and other special folders            
            String sFolderName = "";
            String sFolderPath = "";
            foreach (Environment.SpecialFolder eFolderID in Enum.GetValues(typeof(System.Environment.SpecialFolder)))
            {
                sFolderName = Enum.GetName(typeof(System.Environment.SpecialFolder), eFolderID);
                sFolderPath = Environment.GetFolderPath(eFolderID);
                Console.WriteLine(sFolderName + "=" + sFolderPath + "\n");
            }
            // locating a specific special folder            
            sFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            // identifying OS parameters:            
            s = Environment.NewLine;// New Line sequence for the current OS
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    Console.WriteLine("Running under Windows NT or Windows 2000\n");
                    break;
                case PlatformID.Win32S:
                    Console.WriteLine("Running under Win32s\n");
                    break;
                case PlatformID.Win32Windows:
                    Console.WriteLine("Running under win9x\n");
                    break;
            }
            Console.WriteLine("OS Version= " + Environment.OSVersion.Version.ToString() + "\n");
            Console.WriteLine("Stack size= " + Environment.StackTrace + "\n");
            Console.WriteLine("Tick Count= " + Environment.TickCount.ToString() + "\n");
            Debug.WriteLine("CLR Version= " + Environment.Version.ToString() + "\n");
            Debug.WriteLine("WorkingSet size= " + Environment.WorkingSet.ToString() + "\n");
            // setting an exit code for the current process            
            Environment.ExitCode = 19;
            Console.WriteLine("Exit code=" + Environment.ExitCode.ToString() + "\n");
            Console.WriteLine("\nHit any key to continue\n");
            Console.ReadLine();
            //Setting an exit code and terminating immediately:            
            Environment.Exit(1919);
            // the exit codes are ignored in debugging and are only valid for releases.        
        }
    }
}
