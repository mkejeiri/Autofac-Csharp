using System;
using Autofac;

namespace AutofacSample
{
    public interface ILog : IDisposable
    {
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


    public class Parent:IDisposable {
        public Parent() => Console.WriteLine($"Parent created at {DateTime.Now}");
        public override string ToString()
        {
            return $"I'm the Parent";
        }
        public void Dispose()
        {
            Console.WriteLine($"Parent disposed");
        }
    }
    public class Child : IDisposable {
        public string Name { get; set; }
        public Parent Parent { get; set; }

        public void SetParent(Parent parent)
        {
            Parent = parent ?? throw new ArgumentNullException(paramName:nameof(Parent));
        }
        public Child() => Console.WriteLine($"Child created at {DateTime.Now}");
        public void Dispose()
        {
            Message();
        }

        public virtual void Message()
        {
            Console.WriteLine($"************************Child disposed");
        }

        public override string ToString()
        {
            return "I'm the child";
        }
    }

    /*
     * allow the child to do parent property injection!
     */
    public class ParentChildModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Parent>();
            builder.Register(c => new Child() { Parent = c.Resolve<Parent>() });
        }
    }

    public class BadChild : Child {
        public override string ToString()
        {
            return "I'm the BadChild!";
        }

        public override void Message()
        {
            Console.WriteLine($"*********************************************BadChild disposed!!!");
        }
    }

    public class MyClass : IStartable
    {
        public MyClass()
        {
            Console.WriteLine($"MyClass ctor: {this}");
        }
        public void Start()
        {
            Console.WriteLine($"Myclass being started : {this}");
        }
    }

    internal class Program {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MyClass>().AsSelf().As<IStartable>().SingleInstance();
            using (var scope = builder.Build())
            {
                scope.Resolve<MyClass>();
           }
           Console.ReadKey();
        }
    }
}
