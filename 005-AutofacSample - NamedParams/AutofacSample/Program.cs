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


    internal class Program
    {
        static void Main(string[] args)
        {
           var builder = new ContainerBuilder();
            //1- named parameters: hardcoded
            builder.RegisterType<SMSLog>().As<ILog>()
                .WithParameter("phoneNumber", "+2123695847");


            //2- type parameters : this will crash if string is not found
            builder.RegisterType<SMSLog>().As<ILog>()
                .WithParameter(new TypedParameter(typeof(string), "+2123695847"));

            //3- Resolved parameter: most accurate!!!
            builder.RegisterType<SMSLog>().As<ILog>()
                .WithParameter(
                    new ResolvedParameter(
                        //predicate
                        (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "phoneNumber",
                        //vlaue accessor
                        (pi, ctx) => "+2123695847"
                        )
                );

            IContainer container1 = builder.Build();
            var log1 = container1.Resolve<ILog>();
            log1.Write("hi there");


            //4- do everything at resolution time not at registration time: 
            //postpone /delay the params that we pass to the container untill the resolution
            builder = new ContainerBuilder();
            Random random = new Random();
            builder.Register((c, p) => new SMSLog(p.Named<string>("phoneNumber"))).As<ILog>();
            Console.WriteLine("---------about to build a container--------");
            IContainer container2 = builder.Build();
            var log2 = container2.Resolve<ILog>(new NamedParameter("phoneNumber", random.Next().ToString()));
            log2.Write("Testing ...");

            Console.ReadKey();          
        }
    }
}
