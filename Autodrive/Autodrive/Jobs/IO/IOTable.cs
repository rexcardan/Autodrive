using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Jobs.IO
{
    public class IOTable : List<dynamic[]>
    {
        public Dictionary<string, dynamic> Metadata { get; set; } = new Dictionary<string, dynamic>();

        public void PrintToConsole()
        {
            Console.WriteLine(""); // Add buffer
            //Write metadata (table id)
            foreach (var d in Metadata)
            {
                Console.WriteLine($"{d.Key} = {d.Value}");
            }

            var rowsAsString = this.Select(r => GetRowAsString(r));

            var maxCellWidth = rowsAsString.Select(r => r.Max(cell => cell.Length)).Max();

            foreach (var row in this)
            {
                var rowAsString = row
                    .Select(r => r == null ? "" : r) // Convert null to empty string
                    .Select(r => (string)(r.ToString()));

                rowAsString = rowAsString.Select(r => r.PadRight(maxCellWidth + 1)).ToArray();
                Console.WriteLine(string.Join("|", rowAsString));
            }
        }

        private string[] GetRowAsString(dynamic[] row)
        {
            return row
                    .Select(r => r == null ? "" : r) // Convert null to empty string
                    .Select(r => (string)(r.ToString())).ToArray();
        }

    }
}
