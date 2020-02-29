using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using NUnit.Framework;
using waveweb.ServiceInterface;

namespace waveweb.Tests
{
    public class MappingTests
    {
        [Test]
        public void ConfigurationIsValid()
        {
            var mappingConfig = Mapping.CreateMapperConfiguration();
            mappingConfig.AssertConfigurationIsValid();
        }
    }
}
