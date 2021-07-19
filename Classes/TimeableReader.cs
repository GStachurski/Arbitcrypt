using ArbitCrypt.Enums;
using ArbitCrypt.Helpers;
using System;
using System.Threading;

namespace ArbitCrypt.Classes
{
    public class TimeableReader
    {
        private static readonly Thread inputThread;
        private static readonly AutoResetEvent getInput, gotInput;
        private static string input;

        static TimeableReader()
        {
            getInput = new AutoResetEvent(false);
            gotInput = new AutoResetEvent(false);
            inputThread = new Thread(timedReader) { IsBackground = true };
            inputThread.Start();
        }

        private static void timedReader()
        {
            while (true)
            {
                getInput.WaitOne();
                input = Console.ReadLine();
                gotInput.Set();
            }
        }

        public static string ReadLine(int timeOutSeconds)
        {
            getInput.Set();
            bool success = gotInput.WaitOne(timeOutSeconds * 1000);
            if (success) return input;
            else throw new TimeoutException(string.Format(Dialog.GetDialog(ConsoleDialogs.ConsoleTimeoutDialog), timeOutSeconds));
        }
    }
}
