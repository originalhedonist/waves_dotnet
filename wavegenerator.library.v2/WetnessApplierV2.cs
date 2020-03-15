using org.mariuszgromada.math.mxparser;
using System;
using System.Threading.Tasks;
using wavegenerator.library.common;

namespace wavegenerator.library
{
    public class WetnessApplierV2 : IAmplitude
    {
#nullable enable
        private readonly Expression? wetnessExpression;
        private readonly IAmplitude delegation;
#nullable disable

        public WetnessApplierV2(IAmplitude delegation, string expressionString, Function[] functions, Constant[] constants)
        {
            if(expressionString != null)
            {
                this.wetnessExpression = new Expression(expressionString);
                this.wetnessExpression.addConstants(constants);
                this.wetnessExpression.addFunctions(functions);
                this.wetnessExpression.addArguments(new Argument("t"), new Argument("n"), new Argument("channel"));
                this.wetnessExpression.verifySyntax();
            }

            this.delegation = delegation;
        }
        public async Task<double> Amplitude(double t, int n, int channel)
        {
            var baseA = await delegation.Amplitude(t, n, channel);
            var apos = (1 - baseA) / 2;
            if (wetnessExpression == null) return apos;
            else
            {
                //apply wetness
                this.wetnessExpression.setArgumentValue("t", t);
                this.wetnessExpression.setArgumentValue("n", n);
                this.wetnessExpression.setArgumentValue("channel", channel);
                var wetnessVal = this.wetnessExpression.calculateAndVerify(0, 1);
                var dryness = 1 - wetnessVal;
                var a = 1 - dryness * apos;
                return a;
            }
        }
    }
}