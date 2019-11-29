using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core;

namespace AutofacSample
{

    public interface ILog {
        void Write(string message);
    }
    public interface IConsole
    {
    }

    public class ConsoleLog : ILog, IConsole
    {
        public void Write(string message)
        {
            //Console.WriteLine($"Inside ConsoleLog .... \n{message}");
            Console.WriteLine($"{message}");
        }
    }

    public class Engine
    {
        private ILog log;
        private int id;

        public Engine(ILog log)
        {
            this.log = log;
            id = new Random().Next();
        }

        public Engine(ILog log, int id)
        {
            this.log = log;
            this.id = id;
        }

        public void Ahead(int power) {
            log.Write($"Engine[{id}] ahead {power}");
        }
        
    }

    public class EmailLog : ILog,IConsole
    {
        private const string adminEmail= "admin@foo.com";
        public void Write(string message)
        {
            //Console.WriteLine($"Email sent to {adminEmail} .... \n{message}");
            Console.WriteLine($"Email sent to {adminEmail}");
        }
    }

    public class Fax : ILog, IConsole
    {
        private const string faxReceptionist = " Fax receptionist";
        public void Write(string message)
        {
            Console.WriteLine($"Fax sent to {faxReceptionist}");
        }
    }
    public class Car {
        private Engine engine;
        private ILog log;
        private IConsole Console;

        public Car(Engine engine)
        {
            this.engine = engine;
            this.log = new EmailLog();
            System.Console.WriteLine($"Type of engine only!");
        }

        public Car(Engine engine, ILog log)
        {
            this.engine = engine;
            this.log = log;
        }

        public Car(Engine engine, ILog log, IConsole console)
        {
            this.engine = engine;
            this.log = log;
            Console = console;
            System.Console.WriteLine($"most argument is used!!!");
        }

        public void go() {
            engine.Ahead(100);
            log.Write("Car going forward...");
        }
       
    }

    public class SMSLog : ILog
    {
        private readonly string phoneNumber;

        public SMSLog(string phoneNumber)
        {
            this.phoneNumber = phoneNumber;
        }

        public void Write(string message)
        {
            Console.WriteLine($"SMS sent to {phoneNumber}: {message}");
        }
    }

    public class Parent {
        public override string ToString()
        {
            return $"I'm you father";
        }
    }
    public class Child {
        public string  Name{ get; set; }
        public Parent Parent { get; set; }

        public void setParent(Parent Parent) {
            this.Parent = Parent;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(assembly)
                .Where(t=> t.Name.EndsWith("Log")).Except<SMSLog>()
                .Except<ConsoleLog>(c=> c.As<ILog>().SingleInstance()
                .AsSelf()
                );

            //regiter all ending as Log and grab them as the 1st interface they implement
            builder.RegisterAssemblyTypes(assembly)
                .Except<SMSLog>()
                .Where(t => t.Name.EndsWith("Log"))
                .As(t => t.GetInterfaces()[0]);

            Console.ReadKey();
         }

    }
}
