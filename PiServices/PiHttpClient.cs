using DotNetCommon.Extensions;
using DotNetCommon.WebApiClient;
using PiModel;
using System.Net;

namespace PiServices
{
    public class PiHttpClient : WebApiClientBase
    {
        public PiHttpClient(string baseAddress, string basicAuthString) : base()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            this.Client = new HttpClient(handler);
            this.Client.DefaultRequestHeaders.Add("Authorization", basicAuthString);
            this.Client.BaseAddress = new Uri(baseAddress);
            this.NestedPropertyToStartFrom = "Items";
        }

        public async Task DoSomething()
        {
            string data = await this.GetAsync("assetservers");
        }

        public async Task<AssetServer> AssetServerSearch(string assetServer)
        {
            IEnumerable<AssetServer> assetServers = await this.GetAllAsync<AssetServer>("assetservers");
            return assetServers.Where(x => x.Name.CapsAndTrim() == assetServer.CapsAndTrim()).FirstOrDefault();
        }

        public async Task<Database> DatabaseSearch(string assetServer, string database)
        {
            AssetServer server = await AssetServerSearch(assetServer);
            if (server == null) return null;
            IEnumerable<Database> databases = await this.GetAllAsync<Database>(server.Links.Databases);
            return databases.Where(x => x.Name.CapsAndTrim() == database.CapsAndTrim()).FirstOrDefault();
        }

        public async Task<IEnumerable<Asset>> AssetSearchAll(string assetServer, string database, int depthLimit = 3)
        {
            Database owningDatabase = await DatabaseSearch(assetServer, database);
            if (owningDatabase == null) return null;
            List<Asset> databaseChildren = (await this.GetAllAsync<Asset>(owningDatabase.Links.Elements)).ToList();
            owningDatabase.ChildAssets = databaseChildren;
            await AssetSearchAllInternal(databaseChildren, databaseChildren, depthLimit, 1);
            return databaseChildren.ToList();
        }

        private async Task<List<Asset>> AssetSearchAllInternal(List<Asset> parents, List<Asset> assets, int depthLimit, int currentDepth)
        {
            if (currentDepth > depthLimit) return assets;

            foreach(Asset parent in parents.ToList())
            {
                List<Asset> children = (await this.GetAllAsync<Asset>(parent.Links.Elements)).ToList();
                foreach (var child in children) { child.Parent = parent; }
                parent.Children = children;
                assets.AddRange(children);
                await AssetSearchAllInternal(children, assets, depthLimit, currentDepth + 1);
            }

            return assets;
        }
    }
}