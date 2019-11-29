using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core;
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

    public class Reporting {
        private IList<ILog> allLogs;
        public Reporting(IList<ILog> allLogs)
        {
            this.allLogs = allLogs ?? throw new ArgumentNullException(paramName: nameof(IList<ILog>));         
        }
         public void Report() {
            foreach (var log in allLogs)
            {
                log.Write($"Hello, this {log.GetType().Name}");
            }
        }
    }

    internal class Program {
        public static void Main(string[] args) {
            var builder = new ContainerBuilder();
            builder.RegisterType<ConsoleLog>().As<ILog>();
            //builder.RegisterType<ConsoleLog>();

            //builder.RegisterType<SMSLog>().WithParameter("phoneNumber","+245455454");
            //builder.RegisterType<SMSLog>();
            builder.Register(c => new SMSLog("+215878")).As<ILog>();
            builder.RegisterType<Reporting>();
            using (var container = builder.Build()) {
                 container.Resolve<Reporting>().Report();
                //Console.WriteLine("Done reporting!");
                 //container.Resolve<SMSLog>()                   
                 //   .Write("Hi there!");

            } 
            Console.ReadKey();
        }
    }
}
