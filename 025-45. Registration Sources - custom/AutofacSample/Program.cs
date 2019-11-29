using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;

namespace AutofacSample
{

    public abstract class BaseHandler {
        public virtual string Handle(string message) {
            return $"Handled {message}";
        }
    }

    public class HandlerA: BaseHandler
    {
        public override string Handle(string message)
        {
            return $"Handled by A {message}";
        }
    }
    public class HandlerB: BaseHandler
    {
        public override string Handle(string message)
        {
            return $"Handled by B {message}";
        }
    }

    public interface IHandlerFactory {
        T GetHandler<T>() where T : BaseHandler; 
    }

    public class HandlerFactory : IHandlerFactory
    {
        public T GetHandler<T>() where T : BaseHandler
        {
            return Activator.CreateInstance<T>();
        }
    }

    /*
     * Consumers
     */

    public class ConsumerA
    {
        private HandlerA handlerA;

        public ConsumerA(HandlerA handlerA)
        {
            this.handlerA = handlerA ?? throw new ArgumentNullException(nameof(handlerA));
        }

        public void DoWork() {
            Console.WriteLine(handlerA.Handle("ConsumerA"));
        }
    }
    public class ConsumerB
    {
        private HandlerB handlerB;

        public ConsumerB(HandlerB handlerB)
        {
            this.handlerB = handlerB ?? throw new ArgumentNullException(nameof(handlerB));
        }

        public void DoWork() {
            Console.WriteLine(handlerB.Handle("ConsumerB"));
        }
    }

    /*
     * Registration source
     */
    public class HandleRegistrationSource : IRegistrationSource
    {
        public bool IsAdapterForIndividualComponents => false;

        public IEnumerable<IComponentRegistration> RegistrationsFor(
            Service service,
            Func<Service,IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            var swt = service as IServiceWithType;
            if (swt == null 
                || swt.ServiceType == null 
                || !swt.ServiceType.IsAssignableTo<BaseHandler>())
            {
                yield break; //break out IEnumerable
            }
            //else
            yield return new ComponentRegistration(
                Guid.NewGuid(),
                //public DelegateActivator(Type limitType, Func<IComponentContext, IEnumerable<Parameter>, object> activationFunction);
                new DelegateActivator(
                    swt.ServiceType,
                    (c,p) => {
                        var provider = c.Resolve<IHandlerFactory>();
                        var method = provider.GetType().GetMethod("GetHandler").MakeGenericMethod(swt.ServiceType);
                        return method.Invoke(provider, null);
                    }),

                new CurrentScopeLifetime(),
                InstanceSharing.None,
                InstanceOwnership.OwnedByLifetimeScope,
                new[] {service},
                new ConcurrentDictionary<string, object>()
                );
        }
    }


    internal class Program
    {
        public static void Main(string[] args)
        {
            var b = new ContainerBuilder();
            /*
             We don't register the handlers but only the factory!
             */
            b.RegisterType<HandlerFactory>().As<IHandlerFactory>();
            b.RegisterSource(new HandleRegistrationSource());
            b.RegisterType<ConsumerA>();
            b.RegisterType<ConsumerB>();

            using (var c = b.Build())
            {
                c.Resolve<ConsumerA>().DoWork();
                c.Resolve<ConsumerB>().DoWork();
            }               
            Console.ReadKey();
        }
    }
}
