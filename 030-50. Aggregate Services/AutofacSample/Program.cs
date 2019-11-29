using System;
using Autofac;
using Autofac.Extras.AggregateService;

namespace AutofacSample
{
    public interface IService1 { }
    public interface IService2 { }
    public interface IService3 { }
    public interface IService4 { }
    public interface IService5 { }

    public class Service1 : IService1 { }
    public class Service2 : IService2 { }
    public class Service3 : IService3 { }
    public class Service4 : IService4 { }

    public class Service5 : IService5,IService4
    {

        private string name;

        public Service5()
        {
        }

        public Service5(string name)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }


    public interface IMyAggregateService
    {
        IService1 service1 { get; }
        IService2 service2 { get; }
        IService3 service3 { get; }
        IService4 service4 { get; }
        //this will be proxied to ctr of class Service5
        IService5 GetFourthService(string name);
    }

    

    public class Consumer
    {
        public IMyAggregateService allServices;

        public Consumer(IMyAggregateService allServices)
        {
            this.allServices = allServices ?? throw new ArgumentNullException(nameof(allServices));
        }

       
        //private IService1 service1;
        //private IService2 service2;
        //private IService3 service3;
        //private IService4 service4;

        //public Consumer(IService1 service1, IService2 service2, IService3 service3, IService4 service4)
        //{
        //    this.service1 = service1;
        //    this.service2 = service2;
        //    this.service3 = service3;
        //    this.service4 = service4;
        //}
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var cb = new ContainerBuilder();
            cb.RegisterAggregateService<IMyAggregateService>();
            cb.RegisterAssemblyTypes(typeof(Program).Assembly)
              .Where(t => t.Name.StartsWith("Service"))
              .AsImplementedInterfaces();
            cb.RegisterType<Consumer>();

       
            using (var container = cb.Build())
            {
                var consumer = container.Resolve<Consumer>();
                Console.WriteLine(consumer.allServices.service3.GetType().Name); //output class Service3

                //GetFourthService of type IService5 will be proxied to ctr of class Service5
                Console.WriteLine(consumer.allServices.GetFourthService("test").GetType().Name); //output class Service5
                Console.WriteLine(consumer.allServices.service4.GetType().Name);
            }

           

            Console.ReadKey();
        }
    }
}
