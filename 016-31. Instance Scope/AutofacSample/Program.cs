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
            Console.WriteLine($"{message}: {DateTime.Now}");
        }
        public ConsoleLog() => Console.WriteLine("\n\t\t\t ----------- Creating ConsoleLog -------");
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
       
    }

    internal class Program {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            //builder.RegisterType<ConsoleLog>().As<ILog>()
            //    .InstancePerLifetimeScope(); //calling resolve
            builder.RegisterType<ConsoleLog>().As<ILog>()
                .InstancePerMatchingLifetimeScope("customScope"); //calling resolve
            //builder.RegisterType<ConsoleLog>().As<ILog>()
            //    .InstancePerDependency();

            ILog log;
            using (IContainer container = builder.Build())            {
                //using (ILifetimeScope scope = container.BeginLifetimeScope())
                //{
                //    log = scope.Resolve<ILog>();
                //    log.Write("ILifetimeScope block - 1st call: ILog msg");

                //    log = scope.Resolve<ILog>();
                //    log.Write("ILifetimeScope block 2nd call: ILog msg");
                //}

                //log = container.Resolve<ILog>();
                //log.Write("outside ILifetimeScope block: ILog msg");


                using (ILifetimeScope scope = container.BeginLifetimeScope("customScope"))
                {
                    log = scope.Resolve<ILog>();
                    log.Write("customScope- ILifetimeScope block - 01st call: ILog msg");

                    log = scope.Resolve<ILog>();
                    log.Write("customScope - ILifetimeScope block 02nd call: ILog msg");

                    log = scope.Resolve<ILog>();
                    log.Write("customScope - ILifetimeScope block 03th call: ILog msg");
                }

                using (ILifetimeScope scope = container.BeginLifetimeScope("customScope"))
                {
                    log = scope.Resolve<ILog>();
                    log.Write("customScope- ILifetimeScope block - 001st call: ILog msg");

                    log = scope.Resolve<ILog>();
                    log.Write("customScope - ILifetimeScope block 002nd call: ILog msg");

                    log = scope.Resolve<ILog>();
                    log.Write("customScope - ILifetimeScope block 003th call: ILog msg");
                }

                using (ILifetimeScope scope = container.BeginLifetimeScope("customScope"))
                {
                    log = scope.Resolve<ILog>();
                    log.Write("customScope- ILifetimeScope block - 0001st call: ILog msg");

                    log = scope.Resolve<ILog>();
                    log.Write("customScope - ILifetimeScope block 0002nd call: ILog msg");

                    log = scope.Resolve<ILog>();
                    log.Write("customScope - ILifetimeScope block 0003th call: ILog msg");

                    using (var subScope = scope.BeginLifetimeScope())
                    {
                        log = scope.Resolve<ILog>();
                        log.Write("subScope- ILifetimeScope block - 01st call: ILog msg");

                        log = scope.Resolve<ILog>();
                        log.Write("subScope - ILifetimeScope block 02nd call: ILog msg");

                        log = scope.Resolve<ILog>();
                        log.Write("subScope - ILifetimeScope block 03th call: ILog msg");

                    }

                    using (var subScope = scope.BeginLifetimeScope())
                    {
                        log = scope.Resolve<ILog>();
                        log.Write("subScope- ILifetimeScope block - 0001st call: ILog msg");

                        log = scope.Resolve<ILog>();
                        log.Write("subScope - ILifetimeScope block 0002nd call: ILog msg");

                        log = scope.Resolve<ILog>();
                        log.Write("subScope - ILifetimeScope block 0003th call: ILog msg");

                    }
                }

            }




            Console.WriteLine("left using block");
            Console.ReadKey();
        }
    }
}
