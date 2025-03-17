using System;
namespace ConsoleApp3;

partial class Program
{
    private static void WhatsMyNamespace() // Define a static function.
    {
        Console.WriteLine("Namespace of Program class: {0}", arg0: typeof(Program).Namespace ?? "null");
    }
}
