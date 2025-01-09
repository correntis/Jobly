using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MessagesService.DataAccess.Configuration.Providers
{
    public class GuidSerializationProvider : IBsonSerializationProvider
    {
        public IBsonSerializer GetSerializer(Type type)
        {
            if (type == typeof(Guid))
            {
                return new GuidSerializer(GuidRepresentation.Standard);
            }

            return null;
        }
    }
}
