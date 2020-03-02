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
            appHost.Container.AddTransient<CreateFileService>();
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
        public void CaseSensitive()
        {
            Expression e = new Expression("x+X");
            e.addArguments(new Argument("x"));
            e.addArguments(new Argument("X"));
            e.setArgumentValue("x", 1);
            e.setArgumentValue("X", 2);
            Assert.AreEqual(3, e.calculate());

        }

    }
}
