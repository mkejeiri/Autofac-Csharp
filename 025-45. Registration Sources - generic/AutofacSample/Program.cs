using System;
using System.Collections.Generic;
using System.IO;
using Autofac;
using Autofac.Configuration;
using Autofac.Features.ResolveAnything;
using Microsoft.Extensions.Configuration;

namespace AutofacSample
{

    interface ICanSpeak
    {
        void speak();
    }

    public class Person : ICanSpeak
    {
        public void speak()
        {
            Console.WriteLine("Hello I can speak");
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            using (var container = builder.Build())
            {
                container.Resolve<Person>().speak();
            }
            
            Console.ReadKey();
        }
    }
}
