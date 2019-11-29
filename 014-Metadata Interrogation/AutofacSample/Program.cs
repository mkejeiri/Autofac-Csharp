using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Features.Metadata;
using Autofac.Features.OwnedInstances;

namespace AutofacSample
{

    public interface ILog : IDisposable {
        void Write(string message);
    }

    public class ConsoleLog : ILog
    {
        public void Dispose()
        {
            Console.WriteLine($"{this.ToString()} is disposed!");
        }

        public void Write(string message)
        {
            Console.WriteLine($"{message}: {DateTime.Now.Ticks}");
        }
        //public ConsoleLog() => Console.WriteLine("inside ConsoleLog ctr");
    }

    public class SMSLog : ILog
    {
        public readonly string phoneNumber;
        public SMSLog(string phoneNumber)
        {           
            this.phoneNumber = phoneNumber;
        }
        public void Dispose()
        {
            Console.WriteLine($"{this.ToString()} is disposed!");
        }

        public void Write(string message)
        {
            Console.WriteLine($"{phoneNumber} : {message}");
        }
    }

    /*
       2- Strongly typed
     */
    public class Settings {
        public string logMode { get; set; }    
    }


    public class Reporting
    {
        /*
                1- loosely type 
         */
        //private Meta<ConsoleLog> log;
        private Meta<ConsoleLog,Settings> logST;

        public Reporting(Meta<ConsoleLog, Settings> logST) => this.logST = logST ?? throw new ArgumentNullException(nameof(logST));

        //public Reporting(Meta<ConsoleLog> log)
        //{
        //    this.log = log ?? throw new ArgumentNullException(nameof(log));
        //}

        internal void Report()
        {
            /*
                1- loosely type 
             */
            //log.Value.Write("Starting report");
            //if (log.Metadata["mode"] as string =="verbose")
            //{
            //    log.Value.Write($"VERBOSE MODE : LOGGER started on {DateTime.Now}");
            //}

            /*
                2- Strongly typed
             */
            logST.Value.Write("Starting report");
            if (logST.Metadata.logMode == "verbose")
            {
                logST.Value.Write($"VERBOSE MODE : LOGGER started on {DateTime.Now}");
            }
        }
    }

    internal class Program {
        public static void Main(string[] args) {
            var builder = new ContainerBuilder();
            /*
                1- loosely type 
             */
            //builder.RegisterType<ConsoleLog>()
            //    .WithMetadata("mode","info");

            /*
               2- Strongly typed
            */
            builder.RegisterType<ConsoleLog>()
                .WithMetadata<Settings>(c => c.For(x => x.logMode, "verbose"));

            builder.RegisterType<Reporting>();

            //resolution
            using (var container = builder.Build()) {
                 container.Resolve<Reporting>().Report();        
            } 
            Console.ReadKey();
        }
    }
}
