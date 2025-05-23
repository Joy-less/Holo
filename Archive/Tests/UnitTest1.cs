using System.Numerics;

namespace Holo.Testing;

[TestClass]
public class Test1 {
    [TestMethod]
    public void Test() {
        Console.WriteLine(BigInteger.Parse("12_400"));
    }
}