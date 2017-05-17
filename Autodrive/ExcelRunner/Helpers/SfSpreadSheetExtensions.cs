using Autodrive;
using Autodrive.Jobs;
using Autodrive.Jobs.IO;
using Cardan.XCel;
using Syncfusion.UI.Xaml.Spreadsheet;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExcelRunner.Helpers
{
    public static class SfSpreadSheetExtensions
    {
        /// <summary>
        /// Parses the spreadsheet to pull Autodrive jobs
        /// </summary>
        /// <param name="ss"></param>
        /// <returns></returns>
        public static List<ExcelJob> GetExcelJobs(this SfSpreadsheet ss)
        {
            var rows = ss.ToXCelRows();
            List<ExcelJob> jobs = new List<ExcelJob>();
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
                var excelJob = new ExcelJob(state,index);
                excelJob.Bias = XCelRowParser.GetBias(header, row);
                excelJob.Notification = XCelRowParser.GetNotification(header, row);
                foreach (var measurement in XCelRowParser.ReadMeasurements(header, row))
                {
                    excelJob.AddMeasurement(measurement);
                }
                excelJob.DepthOfMeasurentMM = XCelRowParser.GetMeasurementDepth(header, row);
                excelJob.NumberOfMeasurementsDesired = XCelRowParser.GetNMeasurements(header, row);
                jobs.Add(excelJob);
            }
            return jobs;
        }

        /// <summary>
        /// Converts to Cardan XCel format for processing
        /// </summary>
        /// <param name="ss"></param>
        /// <returns></returns>
        public static List<XCelData> ToXCelRows(this SfSpreadsheet ss)
        {
            List<XCelData> rows = new List<XCelData>();

            foreach (var r in ss.ActiveSheet.Rows)
            {
                var row = new XCelData();
                foreach (var c in r.Cells)
                {
                    row.Add(c.Value);
                }
                rows.Add(row);
            }
            return rows;
        }

        /// <summary>
        /// Highlights the row to show we are working on it
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="rowIndex"></param>
        /// <param name="highlightColor"></param>
        public static void HighlightRow(this SfSpreadsheet ss, int rowIndex, ExcelKnownColors highlightColor)
        {
            var activeRow = ss.ActiveSheet.Rows[rowIndex];
            Application.Current.Dispatcher.Invoke(() =>

            {
                activeRow.CellStyle.ColorIndex = highlightColor;
                foreach (var cell in activeRow.Cells)
                {
                    ss.ActiveGrid.InvalidateCell(cell.Row, cell.Column);
                }
            });
        }
    }
}
