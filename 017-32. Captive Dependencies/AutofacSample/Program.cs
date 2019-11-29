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

    public interface IResource { }
    public class Singleton : IResource, IDisposable
    {
        public Singleton() => Console.WriteLine("Singleton created...");
        public void Dispose()
        {
            Console.WriteLine("Singleton disposed!");
        }
    }
    public class InstancePerDependencyResource : IResource, IDisposable
    {
        public InstancePerDependencyResource() => Console.WriteLine("InstancePerDependencyResource created...");
        public void Dispose()
        {
            Console.WriteLine("InstancePerDependencyResource disposed!");
        }
    }

    public class ResourceManager 
    {
        public ResourceManager(IEnumerable<IResource> resources)
        {
            this.resources = resources ?? throw new ArgumentNullException(nameof(resources));
        }

        public IEnumerable<IResource> resources { get; set; }
    }

    internal class Program {
        public static void Main(string[] args)
        {
            //Captive dependency : when a longlife component holds to a shortlife component
            var builder = new ContainerBuilder();
            builder.RegisterType<ResourceManager>();
            builder.RegisterType<Singleton>().As<IResource>().SingleInstance();
            //builder.RegisterType<InstancePerDependencyResource>().As<IResource>();
            builder.RegisterType<InstancePerDependencyResource>().As<IResource>().SingleInstance();
            using (IContainer container = builder.Build())
            {
                for (int i = 0; i < 3; i++)
                {
                    using (ILifetimeScope scope = container.BeginLifetimeScope())
                    {
                        Console.WriteLine($"iter {i}");
                        var resourceManager = scope.Resolve<ResourceManager>();
                    }

                }               

            }
            Console.ReadKey();
        }
    }
}
