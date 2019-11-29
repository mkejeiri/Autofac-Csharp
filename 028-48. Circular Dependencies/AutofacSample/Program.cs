using System;
using Autofac;

namespace AutofacSample
{
    class ParentWithProperty
    {
        public ChildWithProperty  Child { get; set; }
        public override string ToString()
        {
            return "Parent";
        }
    }
    class ChildWithProperty
    {
        public ParentWithProperty Parent { get; set; }
        public override string ToString()
        {
            return "Child";
        }
    }

    class ParentWithConstructor
    {
        public ParentWithConstructor(ChildWithProperty1 child) => Child = child;

        public ChildWithProperty1 Child { get; set; }
        public override string ToString()
        {
            return "Parent with a child property";
        }
    }

    class ChildWithProperty1
    {
        public ParentWithConstructor Parent { get; set; }
        public override string ToString()
        {
            return "Child ChildWithProperty1";
        }
    }



    internal class Program
    {

        /*
         Autofac can't manage CircularDependencies when it happens in both contructors (parent & child)
         */

        /*         
      CircularDependencies contructor & Property example   : 
        */
        static void Main(string[] args)
        {
            var b = new ContainerBuilder();
            b.RegisterType<ParentWithConstructor>()
                .InstancePerLifetimeScope();
            
            b.RegisterType<ChildWithProperty1>()
                .InstancePerLifetimeScope()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            using (var c = b.Build())
            {
                Console.WriteLine(c.Resolve<ParentWithConstructor>().Child.Parent);                
            }
            Console.ReadKey();
        }

         /*         
            CircularDependencies child & parent Properties example   :          
           */
        public static void Main_(string[] args)
        {
            //we want to avoid a loop
            var b = new ContainerBuilder();
            b.RegisterType<ParentWithProperty>()
                .InstancePerLifetimeScope()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            b.RegisterType<ChildWithProperty>()
                .InstancePerLifetimeScope()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
                       
            using (IContainer c= b.Build())
            {
                Console.WriteLine(c.Resolve<ParentWithProperty>());
                Console.WriteLine(c.Resolve<ChildWithProperty>());
                
            }
            Console.ReadKey();
        }
    }
}
