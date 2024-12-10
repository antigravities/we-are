using Amazon.Lambda.Core;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace WeAre;

public class Function
{
    public static AmazonDynamoDBClient client = new AmazonDynamoDBClient();
    private static readonly string tableName = Environment.GetEnvironmentVariable("TABLE_NAME");
    private static readonly string webhook = Environment.GetEnvironmentVariable("WEBHOOK");

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(object input, ILambdaContext context)
    {
        foreach( var item in (await client.ScanAsync(tableName, new List<string>(){ "ident", "last" })).Items ){
            var asset = await Asset.FetchAsync(item["ident"].S);

            if( item.TryGetValue("last", out AttributeValue val) && val.N == asset.Version.Id.ToString() ){
                Console.WriteLine($"No new version for {asset.Title}");
            } else {
                Console.WriteLine($"New version for {asset.Title}");
                item["last"] = new AttributeValue(){ N = asset.Version.Id.ToString() };
                await client.PutItemAsync(tableName, item);

                await new WebhookPayload(
                    asset.Title,
                    asset.Version.Changes,
                    $"https://asset.party/{asset.Org.Ident}/{asset.Ident}",
                    asset.Thumb
                ).SendAsync(webhook);
            }
        }

        return "";
    }
}

public class Program {
    public static void Main(string[] args){
        var function = new Function();
        function.FunctionHandler(null, null).Wait();
    }
}