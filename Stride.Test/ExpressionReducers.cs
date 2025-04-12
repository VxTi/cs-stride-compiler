using Stride.Compiler.Ast;
using Stride.Compiler.Ast.Nodes;

namespace Stride.Test;

[TestClass]
public class ExpressionReducers
{

    private readonly AtomicValueNode intNode = new(1);
    private readonly AtomicValueNode floatNode = new(3.0f);
    private readonly AtomicValueNode stringNode = new("test");

    [TestMethod]
    public void StringReduction()
    {
        var addition = new Expression.AdditionNode(stringNode, stringNode);

        try
        {
            addition.Reduce();
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }
    
    [TestMethod]
    public void AdditionReduction()
    {
        var addition = new Expression.AdditionNode(intNode, stringNode);

        try
        {
            addition.Reduce();
        }
        catch (Exception e)
        {
            return;
        }

        Assert.Fail("Cannot append string to numeric type");
    }
}