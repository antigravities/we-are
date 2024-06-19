using System.Text.Json;

namespace WeAre;

public class Asset {
    private static readonly string AssetBase = "https://services.facepunch.com/sbox/package/get/";
    
    public class AssetInfo {
        public string Ident {get;set;}
        public string Title {get;set;}
        public string Thumb {get;set;}
        public VersionInfo Version {get;set;}
        public OrgInfo Org {get;set;}
    }

    public class VersionInfo {
        public ulong Id {get;set;}
        public string Changes {get;set;}
        public ulong FileCount {get;set;}
        public ulong TotalSize {get;set;}
    }

    public class OrgInfo {
        public string Ident {get;set;}
        public string Title {get;set;}
    }

    public AssetInfo Package {get;set;}

    public static async Task<Asset> FetchAsync(string ident){
        var client = new HttpClient();
        var response = await client.GetAsync(AssetBase + ident);
        var responseString = await response.Content.ReadAsStringAsync();
        var asset = JsonSerializer.Deserialize<Asset>(responseString);
        return asset;
    }
}