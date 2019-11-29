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

    //public class SMSLog : ILog
    //{
    //    public readonly string phoneNumber;
    //    public SMSLog(string phoneNumber)
    //    {           
    //        this.phoneNumber = phoneNumber;
    //    }
    //    public void Dispose()
    //    {
    //        Console.WriteLine($"{this.ToString()} is disposed!");
    //    }

    //    public void Write(string message)
    //    {
    //        Console.WriteLine($"{phoneNumber} : {message}");
    //    }
    //}
    public interface ILog : IDisposable {
        void Write(string message);
    }

    public interface IConsoleLog { }
    public class ConsoleLog : IConsoleLog, IDisposable
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
    
    internal class Program {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            //builder.RegisterType<ConsoleLog>();

            //AutoFac won't dispose this and you need to manage this by yourself, same as RegisterInstance 
            //- NO AUTO DISPOSE!!! until the container is disposed  
            //builder.RegisterType<ConsoleLog>().ExternallyOwned();
            builder.RegisterInstance(new ConsoleLog());
            //once the container is disposed everything related is disposed
            using (IContainer container = builder.Build())
            {
                for (int i = 0; i < 10; i++)
                {
                    using (ILifetimeScope scope = container.BeginLifetimeScope())
                    {
                        scope.Resolve<ConsoleLog>().Write($"iter {i}");
                    }
                }
            } 
           Console.ReadKey();
        }
    }
}
