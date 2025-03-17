global using System;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;


namespace ConsoleApp3;

public class Meysam
{
    public const int PageCount = 1;
    public readonly int PageCount2 = 2;
    public string? Isbn
    {
        get {return Isbn;}
        set {Isbn = value;}
    }
    public required string Isbn2 = "asd";

    [SetsRequiredMembers]
    public Meysam(int id)
    {
        int id1 = id;
    }

    public (bool state, string status) test(int a, out int b)
    {
        Console.WriteLine("test");
        b = 1;
        return (state: true, status: "asd");
    } 
}

partial class Program
{
    static async Task Main(string[] args)
    {
        char a = 'a';
        string aa = "asd";
        string b = $"asdas{aa}";
        string filePath = @"C:\televisions\sony\bravia.txt";
        int x = 2;
        int? y = null;
        y ??= 3; // if is null then 3
        string result = x > 3 ? "Greater than 3" : "Less than or equal to 3";
        int maxLength = y ?? 30; // if is null then 30

        bool q = false, q1 = true;
        aa = (q & q1) ? "t" : "f";
        aa = (q && q1) ? "t" : "f"; // if q was flase then it dont check the q1

        double cc = 9.8;
        int dd = (int)cc;
        dd = int.Parse("27");
        if (int.TryParse(aa, out int count))
        {
            Console.WriteLine($"There are {count} eggs.");
        }
        else
        {
           Console.WriteLine("I could not parse the input.");
        }

        string amount = "12$";
        if (string.IsNullOrEmpty(amount)) return;
        try
        {
            decimal amountValue = decimal.Parse(amount);
        }
        catch (FormatException) when (amount.Contains("$"))
        {
            Console.WriteLine("Amounts cannot use the dollar sign!");
        }
        catch (FormatException)
        {
            Console.WriteLine("Amounts must only contain digits!");
        }

        Console.WriteLine($"Hello, World! {maxLength}");
        HttpClient client = new();

        Task<HttpResponseMessage> responseTask = client.GetAsync("http://www.google.com/");
        
        List<int> meysamInts = [1, 2, 3, 4, 5, 6, 7, 8, 9];

        List<int> oddInts = meysamInts.Where(i => i % 2 == 1).ToList();
        IEnumerable<int> evenInts = from i in meysamInts where i % 2 == 0 select i;

        foreach (var i in oddInts)
        {
            Console.WriteLine(i);
        }

        foreach (var i in evenInts)
        {
            Console.WriteLine(i);
        }


        HttpResponseMessage response = await client.GetAsync("http://www.google.com/");
        Console.WriteLine("google's home page has {0:N0} bytes.", response.Content.Headers.ContentLength);

        var response2 = await responseTask;

        int[] sixNumbers = { 9, 7, 5, 4, 2, 10 };
        // Console.WriteLine($"{nameof(sixNumbers)}: {CheckSwitch(sixNumbers)}");

        // public string Greeting => $"{Name} says 'Hello!'"; // lambda
        Console.WriteLine($"{nameof(sixNumbers)}: {CheckSwitch(sixNumbers)}");

        WhatsMyNamespace();

        
    }

    static string CheckSwitch(int[] values) => values switch // declarative style
    {
        /// <summary>
        /// Pass a 32-bit unsigned integer and it will be converted into its ordinal equivalent.
        /// </summary>
        /// <param name="number">Number as a cardinal value e.g. 1, 2, 3, and so on.</param>
        /// <returns>Number as an ordinal value e.g. 1st, 2nd, 3rd, and so on.</returns>
        [] => "Empty array",
        [1, 2, _, 10] => "Contains 1, 2, any single number, 10.",
        [1, 2, .., 10] => "Contains 1, 2, any range including empty, 10.",
        [1, 2] => "Contains 1 then 2.",
        [int item1, int item2, int item3] =>
        $"Contains {item1} then {item2} then {item3}.",
        [0, _] => "Starts with 0, then one other number.",
        [0, ..] => "Starts with 0, then any range of numbers.",
        [2, .. int[] others] => $"Starts with 2, then {others.Length} more numbers.",
        [..] => "Any items in any order.",
    };
}

#region
// asdasd
/*
asdasd
*/
#endregion