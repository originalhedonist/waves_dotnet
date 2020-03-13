using org.mariuszgromada.math.mxparser;
using Xunit;

namespace waveweb.Tests
{
    public class ExpressionParserTests
    {
        [Fact]
        public void CaseSensitive()
        {
            Expression e = new Expression("x+X");
            e.addArguments(new Argument("x"));
            e.addArguments(new Argument("X"));
            e.setArgumentValue("x", 1);
            e.setArgumentValue("X", 2);
            Assert.Equal(3, e.calculate());
        }
    }
}
