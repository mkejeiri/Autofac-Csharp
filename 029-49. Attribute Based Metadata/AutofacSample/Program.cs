using System;
using System.ComponentModel.Composition;
using Autofac;
using Autofac.Extras.AttributeMetadata;
using Autofac.Features.AttributeFilters;

namespace AutofacSample
{

    [MetadataAttribute]
    public class AgeMetadataAttribute : Attribute
    {
        public int Age { get; private set; }

        public AgeMetadataAttribute(int age)
        {
            Age = age;
        }
    }

    public interface IArtwork {
        void Display();
    }

    [AgeMetadata(100)]
    public class CenturyArtwork : IArtwork
    {
        public void Display()
        {
            Console.WriteLine("displaying a century-old piece");
        }
    }

    [AgeMetadata(1000)]
    public class MillinialArtwork : IArtwork
    {
        public void Display()
        {
            Console.WriteLine("displaying a really old piece of art");
        }
    }

    public class ArtDisplay
    {
        private IArtwork artwork;

        public ArtDisplay([MetadataFilter("Age",1000)] IArtwork artwork)
        {
            this.artwork = artwork ?? throw new ArgumentNullException(nameof(artwork));
        }

        public void Display() {
            artwork.Display();
        }
    }



    public class AttributeBasedMetadata
    {      
        public static void Main(string[] args)
        {
           
            var b = new ContainerBuilder();
            b.RegisterModule<AttributedMetadataModule>();
            b.RegisterType<CenturyArtwork>().As<IArtwork>();
            b.RegisterType<MillinialArtwork>().As<IArtwork>();
            b.RegisterType<ArtDisplay>().WithAttributeFiltering(); ;

            using (IContainer c= b.Build())
            {
                var ad= c.Resolve<ArtDisplay>();
                ad.Display();

            }
            Console.ReadKey();
        }
    }
}
