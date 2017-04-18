using Autodrive._1DScanners.StandardImaging;
using Autodrive.Electrometers.StandardImaging;
using Autodrive.Jobs.IO;
using Autodrive.Linacs.Varian.CSeries;
using Autodrive.Logging;
using Autodrive.UI.Cardan.ConsoleLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.UI
{
    public class ExcelRunnerUI
    {
        static ConsoleUI ui;
        private static DoseView1D _1ds;
        private static Max4000 _el;
        private static CSeriesLinac _linac;

        public static void Run()
        {
            ui = new ConsoleUI();
            //Header
            ui.WriteSectionHeader("AUTODRIVE EXCEL RUNNER");
            ui.WriteSectionHeader("By Rex Cardan | UAB ");
            ui.SkipLines(2);

            //Connect to RS232
            var ports = SerialPort.GetPortNames();
            var adPort = ui.GetStringResponse("Which port is the Autodrive on?", ports);
            ports = ports.Where(p => p != adPort).ToArray();

            var _1dport = ui.GetStringResponse("Which port is the 1D scanner on?", ports);
            ports = ports.Where(p => p != _1dport).ToArray();

            var elPort = ui.GetStringResponse("Which port is the electrometer on?", ports);

            ui.Write("Connecting to Autodrive...");
            _linac = new CSeriesLinac();
            _linac.Initialize(adPort);
            ui.Write("Connecting to DsoeView1D...");
            _1ds = new DoseView1D();
            _1ds.Initialize(_1dport);
            ui.Write($"Connected to DoseView {_1ds.GetVersion()}");

            ui.Write("Connecting to Max4000...");
            _el = new Max4000();
            _el.Initialize(elPort);
            if (_el.Verify()) { ui.Write($"Connected to Max4000"); }
            else { ui.WriteError($"Could not connect to Max4000! Check connections"); }

            //Find Excel Sheet to key tasks from
            string excel = null;
            while (excel == null)
            {
                ui.WritePrompt("Please select the Excel file where the tasks are located.");
                var startingLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "toDoList.xlsx");
                excel = ui.GetOpenFilePath(startingLocation);
                if (excel == null) { ui.WriteError("You must select a file!"); }
            }

            //Read Excel
            var jobs = XCelJobList.Read(excel);
            var toDo = jobs.RowJobs.Where(j => !j.Item1.IsComplete());
            ui.Write($"Found {toDo}/{jobs.RowJobs.Count} jobs left to complete");

            var logger = new Logger();
            logger.Logged += Logger_Logged;
            jobs.Run(_linac, _el, _1ds,logger);
        }

        private static void Logger_Logged(string toLog)
        {
            ui.Write(toLog);
        }
    }
}
