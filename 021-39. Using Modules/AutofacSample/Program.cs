using System;
using Autofac;

namespace AutofacSample
{


    public interface IDrive
    {
        void Drive();
    }

    public interface IVehicule {
        void Go();
    }

    public class CrazyDriver : IDrive
    {
        public void Drive()
        {
            Console.WriteLine("Driving over speed limit and hiting a tree!");
        }
    }

    public class SaneDrive : IDrive
    {
        public void Drive()
        {
            Console.WriteLine("Drive safely to Destination!");
        }
    }

    public class Truck : IVehicule
    {
        public Truck(IDrive driver)
        {
            Driver = driver;
        }

        public IDrive Driver { get; set; }
        public void Go()
        {
            Driver.Drive();
        }
    }

    public class TransportModule : Module {
        public bool respectSpeedLimit { get; set; }
        protected override void Load(ContainerBuilder builder)
        {
            if (respectSpeedLimit)
                builder.RegisterType<SaneDrive>().As<IDrive>();
            else
                builder.RegisterType<CrazyDriver>().As<IDrive>();

            builder.RegisterType<Truck>().As<IVehicule>();
        }
    }
    internal class Program { 
        public static void Main(string[] args)
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule(new TransportModule { respectSpeedLimit =false});
            using (IContainer container = builder.Build())
            {
                container.Resolve<IVehicule>().Go();
            }

           Console.ReadKey();
        }
    }
}
