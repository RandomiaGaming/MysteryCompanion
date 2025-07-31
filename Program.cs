namespace MysteryCompanion
{
    public static class Program
    {
        public const string HostIP = "192.168.1.143";
        public const string GuestIP = "192.168.1.119";
        public const ushort PostPort = 37737;
        public const ushort GetPort = 47747;
        public static string PostBinding => $"http://{GuestIP}:{PostPort}/";
        public static string GetBinding => $"http://{HostIP}:{GetPort}/";
        public const string MysteryMemewareBuildDir = "D:\\Programming Projects\\MysteryMemeware\\bin\\Debug";
        public static readonly string ExecutePacketPath = $"{MysteryMemewareBuildDir}\\MysteryGo";
        public static readonly string FileSourcePath = $"{MysteryMemewareBuildDir}\\MysteryMemeware.exe";
        public static readonly string FileDestinationPath = $"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory)}\\PentestingPortal\\MysteryMemeware.exe";
        public enum MysteryRole { Messenger, Host, Guest };
        public static void Main(string[] args)
        {
            MysteryRole role = MysteryRole.Messenger;
            bool hitError = false;

            try
            {
                if (VMCheck())
                {
                    role = MysteryRole.Guest;
                }

                if (!(args is null) && args.Length != 0)
                {
                    if (args.Length == 1 && args[0] == "/host")
                    {
                        role = MysteryRole.Host;
                    }
                    else if (args.Length == 1 && args[0] == "/guest")
                    {
                        role = MysteryRole.Guest;
                    }
                    else if (args.Length == 1 && args[0] == "/messenger")
                    {
                        role = MysteryRole.Messenger;
                    }
                    else
                    {
                        throw new System.Exception($"Invalid command line args \"{System.Environment.CommandLine}\".");
                    }
                }

                if (role is MysteryRole.Host)
                {
                    if (!AdminCheck())
                    {
                        RelaunchWithUAC();
                    }
                    Host.Run();
                }
                else if (role is MysteryRole.Guest)
                {
                    if (!AdminCheck())
                    {
                        RelaunchWithUAC();
                    }
                    Guest.Run();
                }
                else
                {
                    Messenger.Run();
                }
            }
            catch (System.Exception ex)
            {
                WriteError(ex);
                hitError = true;
            }

            if (role is MysteryRole.Host || role is MysteryRole.Guest || hitError)
            {
                PressAnyKeyToClose();
            }

            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        public static void PressAnyKeyToClose()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            WriteLine("Press any key to exit...");
            stopwatch.Start();
            while (true)
            {
                System.Console.ReadKey(true);
                if (stopwatch.ElapsedTicks > 10000000)
                {
                    break;
                }
            }
        }
        public static void WriteError(System.Exception ex)
        {
            System.ConsoleColor originalColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = System.ConsoleColor.Red;
            System.Console.WriteLine($"Error: {ex.Message} at {ex.StackTrace}.");
            System.Console.ForegroundColor = originalColor;
        }
        public static void WriteLine(string line = null)
        {
            if (line is null || line.Length <= 0)
            {
                System.Console.WriteLine();
            }
            else
            {
                System.ConsoleColor originalColor = System.Console.ForegroundColor;
                System.Console.ForegroundColor = System.ConsoleColor.White;
                System.Console.WriteLine(line);
                System.Console.ForegroundColor = originalColor;
            }
        }
        public static bool VMCheck()
        {
            System.Management.ManagementObjectSearcher searcher1 = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
            foreach (System.Management.ManagementBaseObject obj in searcher1.Get())
            {
                string manufacturer = obj["Manufacturer"]?.ToString().ToLower();
                string model = obj["Model"]?.ToString().ToLower();

                if ((manufacturer.Contains("microsoft") && model.Contains("virtual")) || model.Contains("virtualbox") || model.Contains("vmware") || model.Contains("qemu") || model.Contains("bochs") || model.Contains("parallels") || model.Contains("xen"))
                {
                    return true;
                }
            }
            searcher1.Dispose();

            System.Management.ManagementObjectSearcher searcher2 = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            foreach (System.Management.ManagementBaseObject obj in searcher2.Get())
            {
                string manufacturer = obj["Manufacturer"]?.ToString().ToLower();

                if (manufacturer.Contains("microsoft") || manufacturer.Contains("vmware") || manufacturer.Contains("virtualbox"))
                {
                    return true;
                }
            }
            searcher2.Dispose();

            System.Management.ManagementObjectSearcher searcher3 = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_Bios");
            foreach (System.Management.ManagementBaseObject obj in searcher3.Get())
            {
                string manufacturer = obj["Manufacturer"]?.ToString().ToLower();
                string version = obj["Version"]?.ToString();

                if (manufacturer.Contains("innotek") && version.Contains("virtualbox"))
                {
                    return true;
                }
            }
            searcher3.Dispose();

            return false;
        }
        public static bool AdminCheck()
        {
            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);

            bool output = principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);

            identity.Dispose();

            return output;
        }
        public static void RelaunchWithUAC()
        {
            System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            startInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory();
            startInfo.Arguments = ConstructArgs();
            startInfo.FileName = typeof(Program).Assembly.Location;
            startInfo.Verb = "runas";

            System.Diagnostics.Process.Start(startInfo);

            currentProcess.Kill();
        }
        public static string ConstructArgs()
        {
            string output = "";

            string[] args = System.Environment.GetCommandLineArgs();

            for (int i = 1; i < args.Length; i++)
            {
                output += $"\"{args[i]}\"";
                if (i + 1 < args.Length)
                {
                    output += " ";
                }
            }

            return output;
        }
    }
}