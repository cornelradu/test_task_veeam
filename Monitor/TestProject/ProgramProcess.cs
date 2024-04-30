using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    internal class ProgramProcess
    {
        private string ExecutableLocation = null;
        ProcessStartInfo startInfo;
        Process process;

        public ProgramProcess(String ExecutableLocation) 
        {
            this.ExecutableLocation = ExecutableLocation;
        }

        public void Setup()
        {
            startInfo = new ProcessStartInfo();
            startInfo.FileName = ExecutableLocation;
        }
        public void Start()
        {
            try
            {
                process = Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public void Stop()
        {
            process.Kill();
        }
    }
}
