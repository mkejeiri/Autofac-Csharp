using System;
using System.Collections.Generic;
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
            var builder = new ContainerBuilder();
            builder.RegisterType<Parent>();
            //builder.RegisterType<Child>().PropertiesAutowired();
            //builder.RegisterType<Child>().WithProperty("Parent", new Parent());

            //1- Method injection
            builder.Register(c =>
                {
                    var child = new Child();
                    child.setParent(c.Resolve<Parent>());
                    return child;
                });

            //2- Using an event handler: fire an evt whenever a child is activated
            builder.RegisterType<Child>().OnActivated((IActivatedEventArgs<Child> e) =>
           {
               var p = e.Context.Resolve<Parent>();
               e.Instance.setParent(p);
           });

            var container = builder.Build();
            var parent = container.Resolve<Child>().Parent;
            Console.WriteLine(parent);
            Console.ReadKey();          
        }

    }
}
