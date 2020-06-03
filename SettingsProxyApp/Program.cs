using System;
using System.Reflection;

namespace SettingsProxyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var foo = DispatchProxy.Create<IFoo, SettingsProxy>();
            var bar = DispatchProxy.Create<IBar, SettingsProxy>();
            var foo2 = new SettingsProxyBuilder<IFoo>()
                .With(x => x.A, "Alvin")
                .With(x => x.C, "Simon")
                .Build();

            Console.WriteLine(foo.A);
            Console.WriteLine(foo.B);
            Console.WriteLine(foo.C);

            Console.WriteLine(bar.D);
            Console.WriteLine(bar.E);
            Console.WriteLine(bar.B);

            Console.WriteLine(foo2.A);
            Console.WriteLine(foo2.B);
            Console.WriteLine(foo2.C);
        }
    }

    public interface IFoo
    {
        string A { get; }
        [SettingName("Barnaby")]
        string B { get; }
        string C { get; }
    }

    public interface IBar
    {
        [SettingName("Dmitry")]
        string D { get; }
        string E { get; }
        [SettingName("Boris")]
        string B { get; }
    }
}
