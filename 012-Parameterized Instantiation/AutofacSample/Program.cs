using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Features.OwnedInstances;

namespace AutofacSample
{

    public interface Ilog : IDisposable {
        void Write(string message);
    }

    public class ConsoleLog : Ilog
    {
        public void Dispose()
        {
            Console.WriteLine($"{this.ToString()} is disposed!");
        }

        public void Write(string message)
        {
            Console.WriteLine($"{message}: {DateTime.Now.Ticks}");
        }
    }

    public class SMSLog : Ilog
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

    public class Reporting {
        //private Lazy<ConsoleLog> log;

        //public Reporting(Lazy<ConsoleLog> log)
        //{
        //    this.log = log ?? throw new ArgumentNullException(nameof(log));
        //    Console.WriteLine("Reporting Component created");
        //}
        //public void Report()
        //{
        //    log.Value.Write("Log started");
        //}

        //public void ReportOnce()
        //{
        //    log.Value.Write("Log started");
        //    //Dispose come from Owned which is an autofac!, that why we don't use 'log.Value.Dispose()'
        //    log.Dispose();
        //}

        /*
         * Owned dependency: object is used once and thrown away
         */
        //private Owned<ConsoleLog> log;

        //public Reporting(Owned<ConsoleLog> log)
        //{
        //    this.log = log ?? throw new ArgumentNullException(nameof(log));
        //}

        /*
         * Dynamic Instantiation: this allows us to create ConsoleLog withount calling any autofac API!!!
         * by just using autofac
         */


        private Func<ConsoleLog> consoleLog;
        //inject a fun with arguments: Parameterized Instantiation
        //limitiation: lack of support to duplicate type in params list (e.g. string more than once!!!)
        private Func<string, SMSLog> smsLog;

        //public Reporting(Func<ConsoleLog> log)
        //{
        //    this.consoleLog = log ?? throw new ArgumentNullException(nameof(log));
        //}

        //public Reporting(Func<string, SMSLog> smsLog)
        //{
        //    this.smsLog = smsLog ?? throw new ArgumentNullException(nameof(smsLog));
        //}

        public Reporting(Func<ConsoleLog> consoleLog, Func<string, SMSLog> smsLog)
        {
            this.consoleLog = consoleLog ?? throw new ArgumentNullException(nameof(consoleLog));
            this.smsLog = smsLog ?? throw new ArgumentNullException(nameof(smsLog)); 
        }

        public void Report()
        {
            //invoke the func log()
            consoleLog().Write("reporting to console");
            consoleLog().Write("And again");
            //params: +1236674
            smsLog("+1236674").Write("Texting Admins ...");
        }
    }

    internal class Program {
        public static void Main(string[] args) {
            var builder = new ContainerBuilder();
            builder.RegisterType<ConsoleLog>();
            //builder.RegisterType<SMSLog>().WithParameter("phoneNumber","+245455454");
            builder.RegisterType<SMSLog>();
            builder.RegisterType<Reporting>();
            using (var container = builder.Build()) {
                 container.Resolve<Reporting>().Report();
                Console.WriteLine("Done reporting!");
                 //container.Resolve<SMSLog>()                   
                 //   .Write("Hi there!");

            } 
            Console.ReadKey();
        }
    }
}
