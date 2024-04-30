using System;
using System.Diagnostics;
using System.Threading;
using System.Timers;

class Program
{
    private static String processName;
    private static Double maximumLifetime;
    private static Double frequency;
    private static System.Timers.Timer timer;
    static void Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Args: program_name(string) lifetime(double) frequency(double)");
        }

        processName = args[0];
        maximumLifetime = Double.Parse( args[1]);
        frequency = Double.Parse(args[2]);

        int freq = (int)(1000.0 * frequency * 60.0);
        timer = new System.Timers.Timer(freq);
        
        SetupTimer();

        Console.WriteLine("Press q to stop...");
        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.KeyChar == 'q')
            {
                timer.Stop();
                return;
            }
        }
    }

    static void SetupTimer()
    {
        timer.Elapsed += OnTimerElapsed;
        timer.Enabled = true;
        timer.AutoReset = true;
        timer.Start();
    }

    static void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        Process[] processes = Process.GetProcessesByName(processName);
        if (processes.Length == 0)
        {
            return;
        }

        foreach (Process process in processes)
        {
            TimeSpan elapsedTime = DateTime.Now - process.StartTime;
            if (elapsedTime.TotalMinutes > maximumLifetime)
            {
                try
                {
                    process.Kill();
                    Console.WriteLine($"Killed process '{processName}' with ID {process.Id} killed.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to kill process '{processName}': {ex.Message}");
                }
            }
        }
    }

}