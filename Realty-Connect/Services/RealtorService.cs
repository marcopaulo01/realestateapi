using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Realty_Connect.DTO;

namespace Realty_Connect.Services
{
    public class RealtorService
    {
        private readonly IAmazonDynamoDB _dynamoDBClient;
        private const string TableName = "RCRealtor";

        public RealtorService()
        {
            var credentials = new BasicAWSCredentials("AKIAZEBKIIPTURPQU5VT", "E4rc3kAaTfa0a1l5mAG6BK8fNveb6egEJMy6jlqk");
            var config = new AmazonDynamoDBConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.CACentral1 // Replace with appropriate AWS region
            };

            _dynamoDBClient = new AmazonDynamoDBClient(credentials, config);
        }

        public async Task<List<Realtor>> GetAllRealtors()
        {
            var request = new ScanRequest
            {
                TableName = TableName
            };

            var response = await _dynamoDBClient.ScanAsync(request);

            var realtors = response.Items.Select(MapToRealtorObject).ToList();
            return realtors;
        }

        public async Task<Realtor> GetRealtorById(int id)
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

            return response.Item != null ? MapToRealtorObject(response.Item) : null;
        }

        public async Task AddRealtor(Realtor realtor)
        {
            var request = new PutItemRequest
            {
                TableName = TableName,
                Item = new Dictionary<string, AttributeValue>
            {
                { "id", new AttributeValue { N = realtor.Id.ToString() } },
                { "firstName", new AttributeValue { S = realtor.FirstName } },
                { "lastName", new AttributeValue { S = realtor.LastName } },
                { "email", new AttributeValue { S = realtor.Email } }
                // Add other realtor properties here
            }
            };

            await _dynamoDBClient.PutItemAsync(request);
        }

        public async Task UpdateRealtor(Realtor realtor)
        {
            var request = new UpdateItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
        {
            { "id", new AttributeValue { N = realtor.Id.ToString() } }
        },
                UpdateExpression = "SET firstName = :fn, lastName = :ln, email = :em",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            { ":fn", new AttributeValue { S = realtor.FirstName } },
            { ":ln", new AttributeValue { S = realtor.LastName } },
            { ":em", new AttributeValue { S = realtor.Email } }
        }
            };

            await _dynamoDBClient.UpdateItemAsync(request);
        }


        public async Task DeleteRealtor(int id)
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

        private Realtor MapToRealtorObject(Dictionary<string, AttributeValue> map)
        {
            return new Realtor
            {
                Id = int.Parse(map["id"].N),
                FirstName = map["firstName"].S,
                LastName = map["lastName"].S,
                Email = map["email"].S
            };
        }

        public async Task PatchRealtor(int id, Dictionary<string, string> updates)
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
