using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var sp = new SerialPort("COM3");
            sp.BaudRate = 9600;
            sp.DataBits = 8;
            sp.StopBits = StopBits.One;
            sp.Open();

            while (true)
            {
                var message = Console.ReadLine();
                var key = Task.Run(() =>
                {
                    var keyPressed = Console.ReadKey();
                    return keyPressed;
                });
                sp.WriteLine(message);
                var k = key.Result;
            }
        }
    }
}
