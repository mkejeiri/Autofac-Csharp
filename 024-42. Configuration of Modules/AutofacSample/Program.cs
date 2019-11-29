using System;
using System.Collections.Generic;
using System.IO;
using Autofac;
using Autofac.Configuration;
using Microsoft.Extensions.Configuration;

namespace AutofacSample
{


    public interface IOperation
    {
        float calculate(float a, float b);
    }

    public class Addition : IOperation
    {
        public float calculate(float a, float b)
        {
            return a + b;
        }
    }
    public class Multiplication : IOperation
    {
        public float calculate(float a, float b)
        {
            return a * b;
        }
    }

    public class CalculationModule : Module {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Addition>().As<IOperation>();
            builder.RegisterType<Multiplication>().As<IOperation>();
        }

    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            /*
             * Ms config : Json file MS config
             */
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json");
            var configuration = configBuilder.Build();


            /*
             Autofac config: wrapper around Json file MS config
             */
            var containerBuilder = new ContainerBuilder();
            var configModule = new ConfigurationModule(configuration);
            containerBuilder.RegisterModule(configModule);

            using (var container = containerBuilder.Build())
            {
                float a = 3, b = 4;
                foreach (IOperation op in container.Resolve<IList<IOperation>>())
                {
                    Console.WriteLine($"{op.GetType().Name} of {a} & {b} = {op.calculate(a, b)}");
                }
            }

            Console.ReadKey();
        }
    }
}
