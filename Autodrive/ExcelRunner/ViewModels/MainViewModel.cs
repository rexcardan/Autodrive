﻿using Autodrive;
using Autodrive._1DScanners.StandardImaging;
using Autodrive.Electrometers.StandardImaging;
using Autodrive.Interfaces;
using Autodrive.Jobs;
using Autodrive.Jobs.IO;
using Autodrive.Linacs.Varian.CSeries;
using Autodrive.Logging;
using Cardan.XCel;
using ExcelRunner.Helpers;
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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ExcelRunner.ViewModels
{
    public class MainViewModel : BindableBase
    {
        #region FIELDS
        private SfSpreadsheet spreadsheet;
        private SfSpreadsheetRibbon ribbon;
        CSeriesLinac linac;
        IElectrometer el;
        DoseView1D scan1D;
        Logger logger = null;
        #endregion

        #region PROPERTIES
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
        #endregion

        #region BINDING PARAMS
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
        private List<ExcelJob> jobs = new List<ExcelJob>();

        public string DVComPort
        {
            get { return dVComPort; }
            set { SetProperty(ref dVComPort, value); }
        }

        private string status;

        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        private string dvConected;

        public string DVConnected
        {
            get { return dvConected; }
            set { SetProperty(ref dvConected, value); }
        }

        private string adConnected;

        public string ADConnected
        {
            get { return adConnected; }
            set { SetProperty(ref adConnected, value); }
        }

        private string elConected;

        public string ELConnected
        {
            get { return elConected; }
            set { SetProperty(ref elConected, value); }
        }
        #endregion


        public MainViewModel()
        {
            logger = new Logger();
            logger.Logged += Logger_Logged;

            SetDefaultComPorts();

            SetCommands();

            RunTasksCommand = new DelegateCommand(async () =>
            {
                jobs = spreadsheet.GetExcelJobs();

                logger.Log($"{jobs.Count} found. {jobs.Sum(j => j.Measurements.Count())}/{jobs.Sum(j => j.NumberOfMeasurementsDesired)} measurements to do.");

                foreach (var job in jobs.Where(j => !j.IsComplete()))
                {
                    //Highlight row to show we are working on it
                    spreadsheet.HighlightRow(job.RowIndex, Syncfusion.XlsIO.ExcelKnownColors.Yellow);

                    Task linacTask = SetLinacState(job);
                    Task scannerTask = Set1DScannerState(job);

                    //Don't start measuring until complete
                    await linacTask;
                    await scannerTask;

                    this.logger.Log("Starting measurement...");
                    if (el != null)
                    {
                        for (int i = 0; i < job.MeasurementsLeft; i++)
                        {
                            double val = job.TakeMeasurement(el, linac, repeatBeam: i != 0);
                            this.logger.Log($"Measurement complete : {val}");

                            //Update Spreadsheet
                            var mNumber = job.Measurements.Count();
                            var mHeader = $"M{mNumber}";
                            var column = spreadsheet.ActiveSheet.Rows[0].Cells.Select(c => c.Value).ToList().IndexOf(mHeader);

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                try
                                {
                                    var activer = spreadsheet.ActiveSheet.Rows[job.RowIndex];
                                    var activeCell = activer.Cells[column];
                                    spreadsheet.ActiveGrid.SetCellValue(activeCell, val.ToString());
                                    spreadsheet.ActiveGrid.InvalidateCell(activeCell.Row, activeCell.Column);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            });
                        }
                    }
                    spreadsheet.HighlightRow(job.RowIndex, Syncfusion.XlsIO.ExcelKnownColors.White);
                }

                this.logger.Log("Tasks complete!");
            });
        }

        private void SetCommands()
        {
            RelaySpreadsheetControlCommand = new DelegateCommand<SfSpreadsheet>((sp) =>
            {
                this.spreadsheet = sp;
            });

            ConnectADCommand = new DelegateCommand(() =>
            {
                this.linac = new CSeriesLinac();
                this.linac.Logger = logger;
                try { linac.Initialize(ADComPort); ADConnected = "(Connected)"; }
                catch(Exception e)
                {
                    ADConnected = "(Error)";
                }
            });

            ConnectELCommand = new DelegateCommand(() =>
            {
                this.el = new Max4000();
                el.Logger = logger;
                try
                {
                    el.Initialize(ELComPort);
                    if (!el.Verify())
                    {
                        MessageBox.Show("Couldn't find Max 4000!");
                    }
                    else
                    {
                        ELConnected = "(Connected)";
                    }
                }
                catch(Exception e) { ELConnected = "(Error)"; }
              
            });

            Connect1DCommand = new DelegateCommand(() =>
            {
                this.scan1D = new DoseView1D();
                scan1D.Logger = logger;
                try
                {
                    scan1D.Initialize(DVComPort);
                    var version = scan1D.GetVersion();
                    if (string.IsNullOrEmpty(version))
                    {
                        MessageBox.Show("Couldn't find DoseView 1D!");
                    }
                    else
                    {
                        DVConnected = "(Connected)";
                    }
                }
                catch(Exception e) { DVConnected = "(Error)"; }            
            });
        }

        private void SetDefaultComPorts()
        {
            SerialPort.GetPortNames().ToList().ForEach(ComPorts.Add);

            //Autoselect for EdgePort hookup
            if (ComPorts.Count > 3)
            {
                var number = new Regex(@"(\d+)$");

                //Take last 4 
                var edgeport = ComPorts.Select(c => new { Port = c, Numer = int.Parse(number.Match(c).Value) })
                    .OrderByDescending(c => c.Numer).Take(4).OrderBy(c => c.Numer).ToArray();
                // .Take(4)
                // (n => ).ToArray();
                ELComPort = edgeport[1].Port;
                ADComPort = edgeport[0].Port;
                DVComPort = edgeport[2].Port;
            }
        }

        private Task Set1DScannerState(ExcelJob job)
        {
            return Task.Run(() =>
            {
                if (scan1D != null)
                {
                    scan1D.GoToDepth(job.DepthOfMeasurentMM).Wait();
                }
            });
        }

        private Task SetLinacState(ExcelJob job)
        {
            return Task.Run(() =>
            {
                if (linac != null)
                {
                    job.MachineStateRun.Time = 99;
                    linac.SetMachineState(job.MachineStateRun);
                }
            });
        }

        private void Logger_Logged(string toLog)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Status = toLog;
            });
        }
    }
}
