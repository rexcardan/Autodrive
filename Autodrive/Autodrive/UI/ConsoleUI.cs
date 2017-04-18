using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    namespace Cardan.ConsoleLib
    {
        /// <summary>
        /// Helpful methods to get UI responses using console
        /// </summary>
        public class ConsoleUI
        {
            public ConsoleUI()
            {
                PromptColor = ConsoleColor.Yellow;
                ProgressBarColor = ConsoleColor.Green;
                ErrorColor = ConsoleColor.Red;
            }

            public ConsoleColor PromptColor { get; set; }
            public ConsoleColor ProgressBarColor { get; set; }
            public ConsoleColor ErrorColor { get; set; }
            /// <summary>
            /// Gets yes or no response. Returns true if yes
            /// </summary>
            /// <param name="prompt"></param>
            /// <returns></returns>
            public bool GetYesNoResponse(string prompt = "Yes (Y) or no (N)?")
            {
                Console.WriteLine(prompt, PromptColor);
                var key = Console.ReadKey().Key;
                Console.WriteLine(string.Empty);
                return key == ConsoleKey.Y;
            }

            /// <summary>
            /// Gets a response from a list of string possible answers. Returns the string the user selects
            /// </summary>
            /// <param name="answers"></param>
            /// <returns></returns>
            public string GetStringResponse(string prompt, string[] answers)
            {
                SkipLines(1);
                Console.ForegroundColor = PromptColor;
                WritePrompt(prompt);
                Console.ResetColor();
                Console.WriteLine("-----------------------------------------------------");
                answers
                .Select((p, i) => string.Format("{0} - {1}", i, p))
                .ToList()
                .ForEach(a => Console.WriteLine(a, PromptColor));
                if (answers.Length == 0)
                {
                    Console.WriteLine("-Nothing found");
                }
                Console.WriteLine("-----------------------------------------------------");
                Console.WriteLine("");

                Console.ForegroundColor = PromptColor;
                var result = GetIntInput("");

                if (answers.Length >= result)
                {
                    Console.Write(" - " + answers[result]);
                    Console.WriteLine("");
                    Console.ResetColor();
                    return answers[result];
                }

                Console.WriteLine("");
                Console.ResetColor();
                WriteError($"Not a valid selection, must be less than {answers.Length}");
                return GetStringResponse("Please try again", answers);
            }

            public double GetDoubleInput(string prompt)
            {
                double response = double.NaN;
                if (!string.IsNullOrEmpty(prompt))
                    WritePrompt(prompt);
                var valid = false;
                while (!valid)
                {
                    var possibleInt = Console.ReadLine();

                    if (!double.TryParse(possibleInt, out response))
                    {
                        WritePrompt($"Please enter a valid number value, last value {possibleInt}");
                    }
                    else { valid = true; }
                }
                return response;
            }

            public int GetResponse(string[] answers)
            {
                var resp = GetStringResponse("Please select one of the following", answers);
                return answers.ToList().IndexOf(resp);
            }

            /// <summary>
            /// Returns an action from a list of possible options
            /// </summary>
            /// <param name="options"></param>
            /// <returns>the action to execute</returns>
            public Action GetResponse(Dictionary<string, Action> options)
            {
                var answers = options.Select(o => o.Key).ToArray();
                var resp = GetStringResponse("Please select one of the following", answers);
                return options[resp];
            }

            /// <summary>
            /// Gets a progress bar at the specified width. Use the report method of the progress bar to update
            /// </summary>
            /// <param name="blockCount"></param>
            /// <returns></returns>
            public ProgressBar ProgressBar(int blockCount = 20) { return new ProgressBar(ProgressBarColor, blockCount); }

            public string GetStringInput(string prompt)
            {
                Console.ForegroundColor = PromptColor;
                Console.WriteLine(prompt);
                Console.WriteLine("");
                var resp = Console.ReadLine();
                Console.ResetColor();
                return resp;
            }

            public string GetSaveFilePath(string fileName = "")
            {
                Console.WriteLine("Where do you want to save?");
                Thread.Sleep(1000);
                SaveFileDialog fd = new SaveFileDialog();
                fd.FileName = fileName;
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    return fd.FileName;
                }
                return null;
            }

            public string GetOpenFilePath(string fileName = "")
            {
                Console.WriteLine("Which file do you want?");
                Thread.Sleep(1000);
                OpenFileDialog fd = new OpenFileDialog();
                fd.FileName = fileName;
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    return fd.FileName;
                }
                return null;
            }

            public void WritePrompt(string prompt)
            {
                Console.ForegroundColor = PromptColor;
                Console.WriteLine(prompt);
                Console.ResetColor();
                SkipLines(1);
            }

            public void WriteError(string prompt)
            {
                Console.ForegroundColor = ErrorColor;
                Console.WriteLine(prompt);
                Console.ResetColor();
            }

            public void Write(string line)
            {
                Console.WriteLine(line);
            }

            /// <summary>
            /// Gets an integer input value
            /// </summary>
            public int GetIntInput(string prompt)
            {
                int response = 0;
                if (!string.IsNullOrEmpty(prompt))
                    WritePrompt(prompt);
                var valid = false;
                while (!valid)
                {
                    var possibleInt = Console.ReadLine();

                    if (!int.TryParse(possibleInt, out response))
                    {
                        WritePrompt($"Please enter a valid integer value, last value {possibleInt}");
                    }
                    else { valid = true; }
                }
                return response;
            }

            public void WriteSectionHeader(string section, ConsoleColor color = ConsoleColor.Cyan)
            {
                var lastColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.Write($"================{section}====================");
                Console.WriteLine("");
                Console.ForegroundColor = lastColor;
            }

            public void SkipLines(int numOfLinesToSkip)
            {
                for (int i = 0; i < numOfLinesToSkip; i++)
                {
                    Console.WriteLine(string.Empty);
                }
            }
        }
    }

}
