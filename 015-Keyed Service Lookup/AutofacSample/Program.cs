using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Features.Indexed;
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

    
    public class Reporting
    {
        private IIndex<string, ILog> indexedList;

        public Reporting(IIndex<string, ILog> index)
        {
            this.indexedList = index ?? throw new ArgumentNullException(nameof(index));
        }

        internal void Report()
        {
            indexedList["sms"].Write($"indexed key: ");
        }
    }

    internal class Program {
        public static void Main(string[] args) {

            var builder = new ContainerBuilder();
            builder.RegisterType<ConsoleLog>().Keyed<ILog>("cmd");
            builder.Register<SMSLog>(c => new SMSLog("+45445454")).Keyed<ILog>("sms");
            builder.RegisterType<Reporting>();
            using (var container = builder.Build())
            {
                container.Resolve<Reporting>().Report();
            }

            Console.ReadKey();
        }
    }
}
