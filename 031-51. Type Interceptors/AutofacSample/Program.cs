using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using System;
using System.IO;
using System.Linq;

namespace AutofacSample
{

    public class CallLogger : IInterceptor
    {

        private TextWriter output;

        public CallLogger(TextWriter output)
        {
            this.output = output ?? throw new ArgumentNullException(nameof(output));
        }

        public void Intercept(IInvocation invocation)
        {
            var methodName = invocation.Method.Name;            

            output.WriteLine("Calling method {0} with args {1}", methodName,
                string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString())
                )
               );

            invocation.Proceed();
            output.WriteLine("Done calling {0}, result was {1} ", methodName, invocation.ReturnValue);
        }
    }
    public interface IAudit
    {
        int Start(DateTime reportDate);
    }


    /*
     Whenever someone used audit please CallLogger intercept (class) method!
         */
    [Intercept(typeof(CallLogger))]
    public class Audit : IAudit
    {
        /*
         * this intercept work through subclassing mechanism that why we need to use virtual!!!
         */
        public virtual int Start(DateTime reportDate)
        {
            Console.WriteLine($"Starting report on {reportDate}");
            return 1;
        }
    }

    public class TypeInterceptors
    {
        static void Main(string[] args)
        {
            var cb = new ContainerBuilder();
            cb.Register(c => new CallLogger(Console.Out))
                .As<IInterceptor>()
                .AsSelf();
            cb.RegisterType<Audit>().As<IAudit>().EnableClassInterceptors();

            using (var c=cb.Build())
            {
                var audit = c.Resolve<IAudit>();
                audit.Start(DateTime.Now);
            }

            Console.ReadKey();
        }
    }
}

