using System;
using Autofac;

namespace AutofacSample
{
    public interface IReportingService
    {
        void Report();
    }

    public class ReportingService : IReportingService
    {
        public void Report()
        {
            Console.WriteLine("Here is your report!");
        }
    }
    public class ReportingServiceWithLogging : IReportingService
    {
        private IReportingService decorated;

        public ReportingServiceWithLogging(IReportingService decorated)
        {
            this.decorated = decorated;
        }

        public void Report()
        {
            Console.WriteLine("logging begins ....");
            decorated.Report(); 
            Console.WriteLine("logging ends...");
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            var b = new ContainerBuilder();
            //PB indefinite injections
            b.RegisterType<ReportingService>().Named<IReportingService>("reporting");

            //RegisterDecorator<TService>(this ContainerBuilder builder, 
            //    Func<IComponentContext, IEnumerable<Parameter>, TService, TService> decorator, 
            //    object fromKey, 
            //    object toKey = null);

            b.RegisterDecorator<IReportingService>(
                (c, p) => new ReportingServiceWithLogging(p),
                "reporting");

            using (IContainer c= b.Build())
            {
                var r = c.Resolve<IReportingService>();
                r.Report();
            }
            Console.ReadKey();
        }
    }
}
