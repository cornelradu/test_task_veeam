using System.Diagnostics;
using TestMonitor;

namespace TestProject
{
    public class Tests
    {
        Dictionary<String, String> ProgramsForTesting = null;

        [SetUp]
        public void Setup()
        {
            ProgramsForTesting = new Dictionary<String, String>()
            {
                {"notepad", "C:\\Windows\\System32\\notepad.exe" },
                {"mspaint", @"C:\Windows\System32\mspaint.exe"}
            };

            Process[] processes = Process.GetProcessesByName("Monitor");
            foreach(Process process in processes)
            {
                process.Kill();
            }
        }

        [Test]
        public void TestIfNotepadIsKilled()
        {
            MonitorProgram monitorProgram = new MonitorProgram();
            monitorProgram.Setup("Notepad", 0.1, 0.1);
            monitorProgram.Start();

            ProgramProcess processToTest = new ProgramProcess(ProgramsForTesting["notepad"]);
            processToTest.Setup();
            processToTest.Start();

            Thread.Sleep(1000 * 10);

            Process[] processes = Process.GetProcessesByName("notepad");
            
            monitorProgram.Stop();
            
            Assert.That(processes.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestIfNotepadIsKilledAfterLifetimeExpired()
        {
            MonitorProgram monitorProgram = new MonitorProgram();
            monitorProgram.Setup("Notepad", 1, 0.1);
            monitorProgram.Start();

            ProgramProcess processToTest = new ProgramProcess(ProgramsForTesting["notepad"]);
            processToTest.Setup();
            processToTest.Start();

            Thread.Sleep(1000 * 30);

            Process[] processes = Process.GetProcessesByName("notepad");
            Assert.That(processes.Length, Is.GreaterThan(0));

            Thread.Sleep(1000 * 40);

            monitorProgram.Stop();
            processes = Process.GetProcessesByName("notepad");

            Assert.That(processes.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestFrequency()
        {
            Process[] processes;
            MonitorProgram monitorProgram = new MonitorProgram();
            monitorProgram.Setup("Notepad", 0.8, 0.5);
            monitorProgram.Start();

            ProgramProcess processToTest = new ProgramProcess(ProgramsForTesting["notepad"]);
            processToTest.Setup();
            processToTest.Start();

            Thread.Sleep(1000 * 30);

            processes = Process.GetProcessesByName("notepad");
            Assert.That(processes.Length, Is.EqualTo(1));

            Thread.Sleep(1000 * 60);

            processes = Process.GetProcessesByName("notepad");
            Assert.That(processes.Length, Is.EqualTo(0));

            processToTest = new ProgramProcess(ProgramsForTesting["notepad"]);
            processToTest.Setup();
            processToTest.Start();
            Thread.Sleep(1000 * 90);

            processes = Process.GetProcessesByName("notepad");

            Assert.That(processes.Length, Is.EqualTo(0));
        }

        [Test]
        public void Test2Monitors()
        {
            Process[] processesNotepad, processedPaint;
            MonitorProgram monitorProgramNotepad = new MonitorProgram();
            monitorProgramNotepad.Setup("Notepad", 0.8, 0.5);
            monitorProgramNotepad.Start();

            MonitorProgram monitorProgramPaint = new MonitorProgram();
            monitorProgramPaint.Setup("mspaint", 1, 1.5);
            monitorProgramPaint.Start();

            ProgramProcess processToTestNotepad = new ProgramProcess(ProgramsForTesting["notepad"]);
            processToTestNotepad.Setup();
            processToTestNotepad.Start();

            ProgramProcess processToTestPaint = new ProgramProcess(ProgramsForTesting["mspaint"]);
            processToTestPaint.Setup();
            processToTestPaint.Start();

            Thread.Sleep(1000 * 30);

            processesNotepad = Process.GetProcessesByName("notepad");
            Assert.That(processesNotepad.Length, Is.EqualTo(1));

            processedPaint = Process.GetProcessesByName("mspaint");
            Assert.That(processedPaint.Length, Is.EqualTo(1));
            
            Thread.Sleep(1000 * 40);

            processesNotepad = Process.GetProcessesByName("notepad");
            Assert.That(processesNotepad.Length, Is.EqualTo(0));

            processedPaint = Process.GetProcessesByName("mspaint");
            Assert.That(processedPaint.Length, Is.EqualTo(1));

            Thread.Sleep(1000 * 80);

            processedPaint = Process.GetProcessesByName("mspaint");
            Assert.That(processedPaint.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestOutputLogs()
        {
            Process[] processesNotepad, processedPaint;
            MonitorProgram monitorProgramNotepad = new MonitorProgram();
            monitorProgramNotepad.Setup("Notepad", 0.1, 0.1);
            monitorProgramNotepad.Start();

            MonitorProgram monitorProgramPaint = new MonitorProgram();
            monitorProgramPaint.Setup("mspaint", 0.1, 0.1);
            monitorProgramPaint.Start();

            for (int i = 0; i < 3; i++)
            {
                ProgramProcess processToTestNotepad = new ProgramProcess(ProgramsForTesting["notepad"]);
                processToTestNotepad.Setup();
                processToTestNotepad.Start();
            }

            for (int i = 0; i < 2; i++)
            {
                ProgramProcess processToTestPaint = new ProgramProcess(ProgramsForTesting["mspaint"]);
                processToTestPaint.Setup();
                processToTestPaint.Start();
            }

            Thread.Sleep(20000);

            int notepadClosedRecords = monitorProgramNotepad.GetOutput("Notepad");
            Assert.That(notepadClosedRecords, Is.EqualTo(3));

            int paintClosedRecords = monitorProgramPaint.GetOutput("mspaint");
            Assert.That(paintClosedRecords, Is.EqualTo(2));
        }

        [TearDown]
        public void Cleanup()
        {
            Process[] processes = Process.GetProcessesByName("Monitor");
            foreach (Process process in processes)
            {
                process.Kill();
            }

            foreach(String program in ProgramsForTesting.Keys)
            {
                processes = Process.GetProcessesByName(program);
                foreach(Process process in processes)
                {
                    process.Kill();
                }
            }
        }
    }
}