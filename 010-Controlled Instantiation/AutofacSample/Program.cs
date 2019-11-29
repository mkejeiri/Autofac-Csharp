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
            Console.WriteLine($"ConsoleLog created at : {DateTime.Now.Ticks}");
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

        public void ReportOnce()
        {
            log.Value.Write("Log started");
            log.Dispose();
        }

        /*
         * Owned dependency: object is used once and thrown away
         */
        private Owned<ConsoleLog> log;

        public Reporting(Owned<ConsoleLog> log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }
    }

    internal class Program {
        public static void Main(string[] args) {
            var builder = new ContainerBuilder();
            builder.RegisterType<ConsoleLog>();
            builder.RegisterType<SMSLog>().WithParameter("phoneNumber","+245455454");
            builder.RegisterType<Reporting>();
            using (var container = builder.Build()) {
                 container.Resolve<Reporting>().ReportOnce();
                Console.WriteLine("Done reporting!");
                 container.Resolve<SMSLog>()                   
                    .Write("Hi there!");

            } 
            Console.ReadKey();
        }
    }
}
