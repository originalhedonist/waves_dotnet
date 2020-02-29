using AutoMapper;

namespace waveweb.ServiceInterface
{
    public class Mapping
    {
        public static MapperConfiguration CreateMapperConfiguration()
        {
            var mappingConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<MappingProfile>();
            });
            return mappingConfig;
        }
    }
}