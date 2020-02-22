using NUnit.Framework;
using org.mariuszgromada.math.mxparser;
using ServiceStack;
using ServiceStack.Testing;
using System;
using waveweb.ServiceInterface;
using waveweb.ServiceModel;

namespace waveweb.Tests
{
    public class UnitTest
    {
        private readonly ServiceStackHost appHost;

        public UnitTest()
        {
            appHost = new BasicAppHost().Init();
            appHost.Container.AddTransient<MyServices>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() => appHost.Dispose();

        [Test]
        public void Can_call_MyServices()
        {
            //var service = appHost.Container.Resolve<MyServices>();

            //var response = (HelloResponse)service.Any(new Hello { Name = "World" });

            //Assert.That(response.Result, Is.EqualTo("Hello, World!"));
        }

        [Test]
        public void ParserTest()
        {
            Expression e = new Expression("sin(x)");
            e.setVerboseMode();
            string[] missingArgs = e.getMissingUserDefinedArguments();
            string[] missingUnits = e.getMissingUserDefinedUnits();
            string[] missingFunctions = e.getMissingUserDefinedFunctions();

            Console.Out.WriteLine(e.getErrorMessage());
            Assert.True(e.checkSyntax());
            //e.addArguments(new Argument("x", Math.PI / 2));
            double result = e.calculate();
            Assert.AreEqual(1.0, result);
        }
    }
}
