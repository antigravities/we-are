using System.Text.Json;
using System.Net.Http;

namespace WeAre;

public class WebhookPayload {
    public class RichEmbed {
        public class RichEmbedImage {
            public string url {get;set;}
        }

        public string title {get;set;}
        public string description {get;set;}
        public string url {get;set;}
        public RichEmbedImage thumbnail {get;set;}
    }

    public RichEmbed[] embeds {get;set;}

    public WebhookPayload(string title, string description, string url, string imageUrl){
        embeds = [new RichEmbed(){
            title = title,
            description = description,
            url = url,
            thumbnail = new RichEmbed.RichEmbedImage(){
                url = imageUrl
            }
        }];
    }

    public async Task SendAsync(string webhook){
        var client = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(this));
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        await client.PostAsync(webhook, content);
    }
}