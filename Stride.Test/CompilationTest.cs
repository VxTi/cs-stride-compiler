using Stride.Common;

namespace Stride.Test;

[TestClass]
public sealed class CompilationTest
{
    [TestMethod]
    public void TestCompilation()
    {
        try
        {
            Compiler.Compiler.CompileSource("test-path.sr", Globals.DefaultMainFileContent, []);
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }
}