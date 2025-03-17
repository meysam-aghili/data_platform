using ConsoleApp3;


namespace TestProject1;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        Meysam mey = new(id: 1)
        {
            Isbn = "978-1803237800"
        };
        bool expected = true;
        int a = 1;
        var result = mey.test(ref a, out int b);
        Assert.Equal(expected, result.state);
    }
}