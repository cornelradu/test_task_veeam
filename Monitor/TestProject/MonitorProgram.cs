using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMonitor
{
    internal class MonitorProgram
    {
        private ProcessStartInfo StartInfo;
        private Process Process;
        private List<String> output;
        
        public void Setup(String processToMonitor, double maximumLifetime, double frequency)
        {
            StartInfo = new ProcessStartInfo();

            string project1BinDirectory = Path.GetFullPath(Path.Combine("..", "..", "..", "..", "Monitor", "bin", "Debug", "net8.0"));

            string executablePath = Path.Combine(project1BinDirectory, "Monitor.exe");

            StartInfo.FileName = executablePath;
            StartInfo.UseShellExecute = false;
            StartInfo.RedirectStandardOutput = true;
            
            String[] args = new string[] { processToMonitor, maximumLifetime.ToString(), frequency.ToString() };
            StartInfo.Arguments = string.Join(" ", args);

            output = new List<string>();
        }

        public int GetOutput(String programName)
        {
            int result = 0;
            foreach(String outputLine in output)
            {
                if (outputLine.Contains(programName))
                {
                    result++;
                }
            }
            return result;
        }

        public void Start()
        {
            try
            {
                Process = Process.Start(StartInfo);
                Process.OutputDataReceived += (sender, args) => output.Add(args.Data);
                Process.BeginOutputReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public void Stop()
        {
            Process.Kill();
        }
    }
}
