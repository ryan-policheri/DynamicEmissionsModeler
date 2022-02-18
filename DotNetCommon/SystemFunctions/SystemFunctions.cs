using DotNetCommon.Extensions;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DotNetCommon.SystemFunctions
{
    public class SystemFunctions
    {
        public static string User { get; set; }

        private static string SystemProcessFile
        {
            get
            {
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return "cmd.exe";
                else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return "/bin/bash";
                else throw new PlatformNotSupportedException();
            }
        }

        private static string SystemProcessArgParameter
        {
            get
            {
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return "/C";
                else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return "-c";
                else throw new PlatformNotSupportedException();
            }
        }

        private static readonly char PathChar = Path.DirectorySeparatorChar;

        private static string WrapSystemArgs(string[] args)
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string argString = "";
                foreach (string arg in args)
                {
                    argString += arg.Quotify() + " ";
                }
                argString = argString.TrimEnd();
                return SystemProcessArgParameter + " " + argString.Quotify();
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                string argString = "";
                foreach (string arg in args)
                {
                    argString += "\\\"" + arg + "\\\" ";
                }
                argString = argString.TrimEnd();
                return SystemProcessArgParameter + " " + argString.Quotify();
            }
            else throw new PlatformNotSupportedException();
        }

        public static ProcessStats RunSystemProcess(string[] args, string workingDirectory = null, string senstiveParameterInfoToReplace = null)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.UseShellExecute = false;
            startInfo.FileName = SystemProcessFile;
            if (!String.IsNullOrWhiteSpace(User)) startInfo.UserName = SystemFunctions.User;
            string wrappedArgs = WrapSystemArgs(args);
            startInfo.Arguments = wrappedArgs;
            if (!String.IsNullOrWhiteSpace(workingDirectory)) startInfo.WorkingDirectory = workingDirectory;
            process.StartInfo = startInfo;
            int exitCode = Int32.MinValue;

            try
            {
                process.Start();
                DateTime startTime = process.StartTime; //do this up here for linux compatibility
                process.WaitForExit();
                exitCode = process.ExitCode;
                if (exitCode != 0) throw new Exception(); //Let the catch handle and throw the deats
                else return new ProcessStats(startTime, process.ExitTime, true);
            }
            catch (Exception ex)
            {
                if (!String.IsNullOrWhiteSpace(senstiveParameterInfoToReplace)) wrappedArgs = wrappedArgs.Replace(senstiveParameterInfoToReplace, "***");
                throw new Exception("An error occured while executing the following process: " + SystemProcessFile +
                                    " " + wrappedArgs + ". The exit code was " + exitCode +
                                    ". The user was: " + SystemFunctions.User + " The working directory was: " + workingDirectory);
            }
        }

        public static void RunSystemProcess(string arguments, string workingDirectory = null, string senstiveCommandInfoToReplace = null)
        {
            RunCustomProcess(SystemProcessFile, arguments, workingDirectory, senstiveCommandInfoToReplace);
        }

        public static void RunCustomProcess(string processFile, string arguments, string workingDirectory = null, string senstiveParameterInfoToReplace = null)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.UseShellExecute = false;
            startInfo.FileName = processFile;
            startInfo.Arguments = processFile.StartsWith(SystemProcessFile) ? $"{SystemProcessArgParameter} " + arguments.Quotify() : arguments;
            if (!String.IsNullOrWhiteSpace(workingDirectory)) startInfo.WorkingDirectory = workingDirectory;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                if (!String.IsNullOrWhiteSpace(senstiveParameterInfoToReplace)) arguments = arguments.Replace(senstiveParameterInfoToReplace, "***");
                throw new Exception("An error occured while executing the following process: " + processFile + " " + arguments + ". The exit code was " + process.ExitCode);
            }
        }

        public static void CreateFreshDirectory(string directory)
        {
            DeleteDirectory(directory);
            Directory.CreateDirectory(directory);
        }

        public static void DeleteDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }

        public static void CreateDirectory(string directory)
        {
            Directory.CreateDirectory(directory);
        }

        public static string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        public static void DeleteFile(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        public static string RenameFile(string file, string newFileName)
        {
            FileInfo fileInfo = new FileInfo(file);
            string newFile = $"{fileInfo.Directory}{PathChar}{newFileName}{fileInfo.Extension}";
            File.Move(file, newFile);
            return newFile;
        }

        public static string GetDateTimeAsFileNameSafeString()
        {
            string fileNameSafeDateTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt").Replace("/", ".").Replace(":", "-").Replace(" ", "_");
            return fileNameSafeDateTime;
        }

        public static void CreateFile(string file, byte[] bytes)
        {
            File.WriteAllBytes(file, bytes);
        }

        public static void CreateFile(string file, string text)
        {
            File.WriteAllText(file, text);
        }

        public static void OpenFile(string file)
        {
            Process.Start($"{SystemProcessFile} ", $"{SystemProcessArgParameter} " + "\"" + file + "\"");
        }

        public static string ReadAllText(string file)
        {
            return File.ReadAllText(file);
        }

        public static void CopyDirectory(string sourceDirectory, string targetDirectory)
        {
            Directory.CreateDirectory(targetDirectory);
            string[] files = Directory.GetFiles(sourceDirectory);
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                string newFile = $"{targetDirectory}{PathChar}{fileInfo.Name}";
                File.Copy(file, newFile);
            }

            string[] directories = Directory.GetDirectories(sourceDirectory);

            foreach (string directory in directories)
            {
                string subDirectory = directory.Split(PathChar).Last();
                string targetSubDirectory = $"{targetDirectory}{PathChar}{subDirectory}";
                CopyDirectory(directory, targetSubDirectory);
            }
        }

        public static string CombineDirectoryComponents(string directory, string directoryOrFile)
        {
            return directory.TrimEnd(PathChar) + PathChar + directoryOrFile.TrimStart(PathChar);
        }

        public static string CombineDirectoryComponents(string directory1, string directory2, string directoryOrFile)
        {
            return CombineDirectoryComponents(CombineDirectoryComponents(directory1, directory2), directoryOrFile);
        }

        public static string CombineDirectoryComponents(string directory1, string directory2, string directory3, string directoryOrFile)
        {
            return CombineDirectoryComponents(CombineDirectoryComponents(directory1, directory2, directory3), directoryOrFile);
        }
    }
}
