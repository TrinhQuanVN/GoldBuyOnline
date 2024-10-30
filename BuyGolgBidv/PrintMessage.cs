using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyGolgBidv
{
    public static class PrintMessage
    {
        public static void Print(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        public static void Information(string message)
        {
            Print(message, ConsoleColor.Blue);
        }
        public static void Error(string message)
        {
            Print(message, ConsoleColor.Red);
        }
        public static void Success(string message)
        {
            Print(message, ConsoleColor.Green);
        }
    }
}
