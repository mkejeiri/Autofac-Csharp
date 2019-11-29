using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Features.Metadata;

namespace AutofacSample
{

    public interface ICommand
    {
        void Execute();
    }

    class SaveCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("Saving a file");
        }
    }

    class OpenCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("Opening a file");
        }
    }

    class Button
    {
        private ICommand command;
        private string name;

        //public Button(ICommand command)
        //{
        //    this.command = command ?? throw new ArgumentNullException(nameof(command));
        //}

        public Button(ICommand command, string name)
        {
            this.command = command ?? throw new ArgumentNullException(nameof(command));
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }
        public void PrintMe()
        {
            Console.WriteLine($"I'm a {name} button ");
        }

        public void Click()
        {
            command.Execute();
        }
    }

    class Editor
    {
        private IEnumerable<Button> buttons;

        public Editor(IEnumerable<Button> buttons)
        {
            this.buttons = buttons ?? throw new ArgumentNullException(nameof(buttons)); ;
        }

        internal IEnumerable<Button> Buttons { get => buttons; }

        public void clickAll() {
            foreach (var btn in Buttons)
            {
                btn.Click();
            }
        }
    }                 

    internal class Program
    {
        public static void Main(string[] args)
        {
            var b = new ContainerBuilder();
            b.RegisterType<SaveCommand>().As<ICommand>().WithMetadata("Name", "Save");
            b.RegisterType<OpenCommand>().As<ICommand>().WithMetadata("Name", "Open");
            //b.RegisterType<Button>(); //we get only defaut when resolve editor
            //b.RegisterAdapter<ICommand, Button>(cmd => new Button(cmd)); //we get as many button as ICommand! when resolve editor
            b.RegisterAdapter<Meta<ICommand>, Button>(cmd => new Button(cmd.Value,(string) cmd.Metadata["Name"])); //we get as many button as ICommand! when resolve editor
            b.RegisterType<Editor>();
            using (IContainer c = b.Build())
            {
               var editor = c.Resolve<Editor>();
                //editor.clickAll();
                foreach (var btn  in editor.Buttons)
                {
                    btn.PrintMe();
                }
            }  

            Console.ReadKey();
        }
    }
}
