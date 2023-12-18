using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Realty_Connect.DTO;

namespace Realty_Connect.Services
{
    public class PropertyService
    {
        private readonly IAmazonDynamoDB _dynamoDBClient;
        private const string TableName = "RCProperty";

        //public UserService(IAmazonDynamoDB dynamoDBClient) { _dynamoDBClient = dynamoDBClient; }

        public PropertyService()
        {
            var credentials = new BasicAWSCredentials("AKIAZEBKIIPTURPQU5VT", "E4rc3kAaTfa0a1l5mAG6BK8fNveb6egEJMy6jlqk");
            var config = new AmazonDynamoDBConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.CACentral1 // Replace YourRegion with the appropriate AWS region
            };

            _dynamoDBClient = new AmazonDynamoDBClient(credentials, config);
        }

        public async Task<List<Property>> GetAllProperties()
        {
            var request = new ScanRequest
            {
                TableName = TableName
            };

            var response = await _dynamoDBClient.ScanAsync(request);

            var properties = response.Items.Select(MapToPropertyObject).ToList();
            return properties;
        }


        public async Task<Property> GetPropertyById(int id)
        {
            var request = new GetItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "id", new AttributeValue { N = id.ToString() } }
                }
            };

            var response = await _dynamoDBClient.GetItemAsync(request);

            return response.Item != null ? MapToPropertyObject(response.Item) : null;
        }


        public async Task AddProperty(Property property)
        {
            var request = new PutItemRequest
            {
                TableName = TableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "id", new AttributeValue { N = property.Id.ToString() } },
                    { "title", new AttributeValue { S = property.Title } },
                    { "description", new AttributeValue { S = property.Description } },
                    { "address", new AttributeValue { S = property.Address } },
                    { "bedrooms", new AttributeValue { N = property.Bedrooms.ToString() } },
                    { "bathrooms", new AttributeValue { N = property.Bathrooms.ToString() } },
                    { "price", new AttributeValue { N = property.Price.ToString() } },
                    { "userid", new AttributeValue { N = property.UserId.ToString() } } // Include userid here
                }
            };

            await _dynamoDBClient.PutItemAsync(request);
        }



        public async Task UpdateProperty(Property property)
        {
            var request = new UpdateItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "id", new AttributeValue { N = property.Id.ToString() } }
                },
                UpdateExpression = "SET title = :tl, description = :desc, address = :addr, bedrooms = :bd, bathrooms = :ba, price = :pr, userid = :uid",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":tl", new AttributeValue { S = property.Title } },
                    { ":desc", new AttributeValue { S = property.Description } },
                    { ":addr", new AttributeValue { S = property.Address } },
                    { ":bd", new AttributeValue { N = property.Bedrooms.ToString() } },
                    { ":ba", new AttributeValue { N = property.Bathrooms.ToString() } },
                    { ":pr", new AttributeValue { N = property.Price.ToString() } },
                    { ":uid", new AttributeValue { N = property.UserId.ToString() } }
                }
            };

            await _dynamoDBClient.UpdateItemAsync(request);
        }




        public async Task DeleteProperty(int id)
        {
            var request = new DeleteItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "id", new AttributeValue { N = id.ToString() } }
                }
            };

            await _dynamoDBClient.DeleteItemAsync(request);
        }

        private Property MapToPropertyObject(Dictionary<string, AttributeValue> map)
        {
            return new Property
            {
                Id = int.Parse(map["id"].N),
                Title = map["title"].S,
                Description = map["description"].S,
                Address = map["address"].S,
                Bedrooms = int.Parse(map["bedrooms"].N),
                Bathrooms = int.Parse(map["bathrooms"].N),
                Price = double.Parse(map["price"].N),
                UserId = int.Parse(map["userid"].N)
            };
        }

        public async Task PatchProperty(int id, Dictionary<string, string> updates)
        {
            var updateExpression = "SET ";
            var expressionAttributeValues = new Dictionary<string, AttributeValue>();

            foreach (var update in updates)
            {
                if (int.TryParse(update.Value, out int intValue))
                {
                    updateExpression += $"{update.Key} = :{update.Key}, ";
                    expressionAttributeValues.Add($":{update.Key}", new AttributeValue { N = intValue.ToString() });
                }
                else
                {
                    updateExpression += $"{update.Key} = :{update.Key}, ";
                    expressionAttributeValues.Add($":{update.Key}", new AttributeValue { S = update.Value });
                }
            }

            updateExpression = updateExpression.TrimEnd(',', ' ');

            var request = new UpdateItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
        {
            { "id", new AttributeValue { N = id.ToString() } }
        },
                UpdateExpression = updateExpression,
                ExpressionAttributeValues = expressionAttributeValues
            };

            await _dynamoDBClient.UpdateItemAsync(request);
        }
    }
}
