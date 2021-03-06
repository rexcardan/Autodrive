﻿
using Autodrive;
using Autodrive._1DScanners.StandardImaging;
using Autodrive.Electrometers.StandardImaging;
using Autodrive.Jobs.IO;
using Autodrive.Jobs.Mechanical;
using Autodrive.Jobs.Output;
using Autodrive.Jobs.Processor;
using Autodrive.Linacs;
using Autodrive.Linacs.Varian.CSeries;
using Autodrive.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var linac = new CSeriesLinac();
            linac.Initialize("COM9");
            var logger = new Logger();
            logger.Logged += (log) => Console.WriteLine(log);
            linac.Logger = logger;

            var ms = MachineState.InitNew();
            ms.Energy = Energy._6X;
            ms.X1 = 5;
            ms.X2 = 5;
            linac.SetMachineState(ms);

            var ss = ServiceModeSession.Instance;
            ss.Logger = logger;
            ss.AddWaitTime("test", 7000);
            ss.Wait();


            var time = new TimerLogger("Test", 7000, 1000, logger);
            time.CompletionEvent.WaitOne();
            Console.WriteLine("Timer complete");
            Console.Read();

            var max = new Max4000();
            max.Initialize("COM1");

            var verify = max.Verify();
            var status = max.GetStatus();
            var dv = new DoseView1D();
            dv.Initialize("COM3");
           
            dv.GoToDepth(64.2).Wait();
            var test = dv.GetOrigin();
            //var linac = new CSeriesLinac();
            //linac.Initialize("COM5");
         // linac.OverrideDefaultInterlocks();
      
         
            //var of = @"C:\Users\variansupport\Desktop\photonOoutputFactors.txt";
            //var edwOF = @"C:\Users\variansupport\Desktop\edwFactors.txt";
            //var jobs = JobResultReader.Read(of);
            //var edwJobs = JobResultReader.Read(edwOF).ToList();
            //var table = AccessoryOFProcessor.GetTableRows(edwJobs);
            //foreach(var t in table)
            //{
            //    t.PrintToConsole();
            //}
            //Console.ReadLine();

            //var dv = new DoseView1D();
            //dv.Initalize("COM12");

            //var max = new Max4000();
            //max.Initialize("COM9");
            //max.Verify();

            ////  max.Zero().Wait();

            //var bias = max.SetBias(Autodrive.Electrometers.Bias.NEG_100PERC);
            //max.SetMode(Autodrive.Electrometers.MeasureMode.CHARGE);

            //var linac = new CSeriesLinac();
            //linac.Initialize("COM10");

            //var ofTest = new EDWFactors(linac, max, dv);
            //ofTest.Logger.Logged += Logger_Logged;
            //ofTest.Run();

            // var session = ServiceModeSession.Instance;
            // session.Keyboard = new VetraKeyboard("COM3");
            // session.KeySpeedMs = 100;
            //// session.EnterDefaultPassword();
            // session.ResetConsoleState();
            // //session.ToggleDefaultInterlocks();


            // session.MachineState.GantryRot = 180;
            // session.MachineState.CollimatorRot = 180;
            // session.MachineState.X1 = 5.0;
            // session.MachineState.X2 = 5.0;
            // session.MachineState.Y1 = 5.0;
            // session.MachineState.Y2 = 5.0;
            // session.MachineState.CouchLat = 100.2;
            // session.MachineState.CouchVert = 127.9;
            // session.MachineState.CouchLng = 54.4;

            //MonthlyMechanicals.InitializePosition();
            //MonthlyMechanicals.CouchStarShot();
            Console.Read();
            //  var tasks = new List<ITask>();
        }


        private static void Logger_Logged(string toLog)
        {
            Console.WriteLine(toLog);
        }
    }
}
