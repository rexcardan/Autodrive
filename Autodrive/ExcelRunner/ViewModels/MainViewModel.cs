using Autodrive;
using Autodrive._1DScanners.StandardImaging;
using Autodrive.Electrometers.StandardImaging;
using Autodrive.Interfaces;
using Autodrive.Jobs;
using Autodrive.Jobs.IO;
using Autodrive.Linacs.Varian.CSeries;
using Autodrive.Logging;
using Cardan.XCel;
using Prism.Commands;
using Prism.Mvvm;
using Syncfusion.UI.Xaml.Grid.Utility;
using Syncfusion.UI.Xaml.Spreadsheet;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ExcelRunner.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private SfSpreadsheet spreadsheet;
        private SfSpreadsheetRibbon ribbon;
        CSeriesLinac linac;
        IElectrometer el;
        DoseView1D scan1D;
        Logger logger = null;


        public DelegateCommand OpenCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand SaveAsCommand { get; set; }
        public DelegateCommand ConnectADCommand { get; set; }
        public DelegateCommand ConnectELCommand { get; set; }
        public DelegateCommand Connect1DCommand { get; set; }
        public DelegateCommand RunTasksCommand { get; set; }
        public DelegateCommand<object> RelayRibbonControlCommand { get; private set; }

        public DelegateCommand<SfSpreadsheet> RelaySpreadsheetControlCommand { get; set; }

        public ObservableCollection<string> ComPorts { get; set; } = new ObservableCollection<string>();

        private string eLComPort;

        public string ELComPort
        {
            get { return eLComPort; }
            set { SetProperty(ref eLComPort, value); }
        }

        private string aDComPort;

        public string ADComPort
        {
            get { return aDComPort; }
            set { SetProperty(ref aDComPort, value); }
        }

        private string dVComPort;
        private List<Tuple<Job, int>> jobs = new List<Tuple<Job, int>>();

        public string DVComPort
        {
            get { return dVComPort; }
            set { SetProperty(ref dVComPort, value); }
        }


        public MainViewModel()
        {
            SerialPort.GetPortNames().ToList().ForEach(ComPorts.Add);
            ELComPort = ComPorts[0];

            RelaySpreadsheetControlCommand = new DelegateCommand<SfSpreadsheet>((sp) =>
            {
                this.spreadsheet = sp;
            });

            ConnectADCommand = new DelegateCommand(() =>
            {
                this.linac = new CSeriesLinac();
                linac.Initialize(ADComPort);
            });

            ConnectELCommand = new DelegateCommand(() =>
            {
                spreadsheet.ActiveSheet.Rows[5].Cells[10].Value2 = 5;
                //this.el = new Max4000();
                //el.Initialize(ELComPort);
                //if (!el.Verify())
                //{
                //    MessageBox.Show("Couldn't find Max 4000!");
                //}
            });

            Connect1DCommand = new DelegateCommand(() =>
            {
                this.scan1D = new DoseView1D();
                scan1D.Initialize(DVComPort);
                if (string.IsNullOrEmpty(scan1D.GetVersion()))
                {
                    MessageBox.Show("Couldn't find DoseView 1D!");
                }
            });

            RunTasksCommand = new DelegateCommand(async () =>
            {
                List<XCelData> rows = new List<XCelData>();

                foreach (var r in spreadsheet.ActiveSheet.Rows)
                {
                    var row = new XCelData();
                    foreach (var c in r.Cells)
                    {
                        row.Add(c.Value);
                    }
                    rows.Add(row);
                }

                await Task.Run(() =>
                {
                    foreach (var row in rows.Skip(1).Where(r => r[0] != null && !((string)r[0]).StartsWith("//")).ToList())
                    {
                        var state = new MachineState();
                        var index = rows.IndexOf(row);
                        var header = rows[0];
                        state.Accessory = XCelRowParser.GetAccessory(header, row);
                        state.CollimatorRot = XCelRowParser.GetCollimatorRot(header, row);
                        state.CouchLat = XCelRowParser.GetCouchLat(header, row);
                        state.CouchVert = XCelRowParser.GetCouchVert(header, row);
                        state.CouchLng = XCelRowParser.GetCouchLng(header, row);
                        state.CouchRot = XCelRowParser.GetCouchRot(header, row);
                        state.DoseRate = XCelRowParser.GetDoseRate(header, row);
                        state.Energy = XCelRowParser.GetEnergy(header, row);
                        state.GantryRot = XCelRowParser.GetGantryRot(header, row);
                        state.MU = XCelRowParser.GetMU(header, row);
                        state.Time = XCelRowParser.GetTime(header, row);
                        state.X1 = XCelRowParser.GetX1(header, row);
                        state.X2 = XCelRowParser.GetX2(header, row);
                        state.Y1 = XCelRowParser.GetY1(header, row);
                        state.Y2 = XCelRowParser.GetY2(header, row);
                        var job = new Job(state);
                        foreach (var measurement in XCelRowParser.ReadMeasurements(header, row))
                        {
                            job.AddMeasurement(measurement);
                        }
                        job.DepthOfMeasurentMM = XCelRowParser.GetMeasurementDepth(header, row);
                        job.NumberOfMeasurementsDesired = XCelRowParser.GetNMeasurements(header, row);
                        var jobIndex = new Tuple<Job, int>(job, index);
                        jobs.Add(new Tuple<Job, int>(job, index));
                    }
                });


                await Task.Run(() =>
                {
                    foreach (var job in jobs)
                    {
                        var activeRow = spreadsheet.ActiveSheet.Rows[job.Item2];
                        activeRow.CellStyle.FillBackground = Syncfusion.XlsIO.ExcelKnownColors.LightGreen;
                        var measurementsLeft = job.Item1.NumberOfMeasurementsDesired - job.Item1.Measurements.Length;
                        if (measurementsLeft > 0)
                        {
                            //linac.SetMachineState(job.Item1.MachineStateRun);
                        }
                        if (scan1D != null)
                        {
                            scan1D.GoToDepth(job.Item1.DepthOfMeasurentMM).Wait();
                        }
                        //if (el != null && measurementsLeft > 0)
                        //{
                        for (int i = 0; i < measurementsLeft; i++)
                        {
                            //el.StartMeasurement();
                            //linac.BeamOn();
                            //el.StopMeasurement();
                            //var val = el.GetValue().Measurement;
                            Thread.Sleep(1000);
                            var val = 5.0;
                            job.Item1.AddMeasurement(val);
                            //Save

                            var mNumber = job.Item1.Measurements.Count();
                            var mHeader = $"M{mNumber}";
                            var column = spreadsheet.ActiveSheet.Rows[0].Cells.Select(c => c.Value).ToList().IndexOf(mHeader);
                            activeRow.Cells[column].Value2 = 5;
                        }
                        //}
                        activeRow.CellStyle.FillBackground = Syncfusion.XlsIO.ExcelKnownColors.White;
                    }
                });





            });

        }
    }
}
