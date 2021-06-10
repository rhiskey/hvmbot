using Rollbar;
using System;
using System.IO;

namespace hvmbot
{
    class Logging
    {
        static readonly string strPath = "log.txt";
        public static void ErrorLogging(Exception ex)
        {
            RollbarLocator.RollbarInstance.Configure(new RollbarConfig(Configuration.rollbarToken));

            //string strPath = "log.txt";
            if (!File.Exists(strPath))
            {
                File.Create(strPath).Dispose();
            }
            using (StreamWriter sw = File.AppendText(strPath))
            {
                sw.WriteLine("=============Error Logging ===========");
                sw.WriteLine("===========Start============= " + DateTime.Now);
                sw.WriteLine("Error Message: " + ex.Message);
                sw.WriteLine("Stack Trace: " + ex.StackTrace);
                sw.WriteLine("===========End============= " + DateTime.Now);

            }
            RollbarLocator.RollbarInstance.Error(ex);
        }

        public static void ErrorLogging(string ex)
        {
            RollbarLocator.RollbarInstance.Configure(new RollbarConfig(Configuration.rollbarToken));
            //string strPath = "log.txt";
            if (!File.Exists(strPath))
            {
                File.Create(strPath).Dispose();
            }
            using (StreamWriter sw = File.AppendText(strPath))
            {
                sw.WriteLine("=============Error Logging ===========");
                sw.WriteLine("===========Start============= " + DateTime.Now);
                sw.WriteLine("Error Message: " + ex);
                // sw.WriteLine("Stack Trace: " + ex.StackTrace);
                sw.WriteLine("===========End============= " + DateTime.Now);

            }
            RollbarLocator.RollbarInstance.Error(ex);
        }
        public static void ReadError()
        {
            //string strPath = "log.txt";
            using (StreamReader sr = new StreamReader(strPath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }

    }
}
