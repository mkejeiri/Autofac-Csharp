using System;
using Autofac;
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
            Console.WriteLine(message);
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

    internal class Program
    {
        static void Main(string[] args)
        {

            //without a DI
            //var log = new ConsoleLog();
            //var engine = new Engine(log);
            //var car = new Car(engine, log);
            //var log = container.Resolve<ConsoleLog>(); //need as "AsSelf"
            //var ilog = container.Resolve<ILog>();




            //With DI
            /* I N F O:
             * use .AsSelf() to get also a type reference of object instead of interface!
             * In case of several Ilog's, the container only picks up the last one
             * builder.RegisterType<EmailLog>().As<ILog>().As<IConsole>().PreserveExistingDefaults(): previous one (if any) 
               get selected as a default
             * When it comes to different ctr for a single object, the autofac will use the ctr with the most argument
             *  builder.RegisterType<Car>().UsingConstructor(typeof(Engine)); will pick up the ctr with only an engine,
                thus the emailLog is used instead of consoleLog for the car, but in the container we will use the default log (consoleLog)                
             
             * 
             */
            var builder = new ContainerBuilder();
            //builder.RegisterType<EmailLog>().As<ILog>().AsSelf();            
            //Email log is the default for a request of ILog and IConsole
            builder.RegisterType<ConsoleLog>().As<ILog>().As<IConsole>();
            //builder.RegisterType<Fax>().As<ILog>().As<IConsole>().PreserveExistingDefaults();
            //builder.RegisterType<EmailLog>().As<ILog>().PreserveExistingDefaults();                  
            builder.RegisterType<Engine>();
            builder.RegisterType<Car>().UsingConstructor(typeof(Engine));            
            //builder.RegisterType<Car>().UsingConstructor(typeof(Engine), typeof(ILog));            


            IContainer container = builder.Build();
            var car = container.Resolve<Car>();
            car.go();
            Console.ReadKey();          
        }
    }
}
