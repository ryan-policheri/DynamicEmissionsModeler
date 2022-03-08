using DotNetCommon.Extensions;
using DotNetCommon.WebApiClient;
using PiModel;
using System.Text.Json;

namespace PiServices
{
    public class PiHttpClient : WebApiClientBase
    {
        private const string ITEMS_PROPERTY = "Items";

        public PiHttpClient(string baseAddress, string userName, string password, string defaultAssetServer = null) : base()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            this.Client = new HttpClient(handler);

            this.Client.BaseAddress = new Uri(baseAddress);
            this.DefaultAssetServer = defaultAssetServer;
            this.UserName = userName;
            this.Password = password;
            this.AddAuthorizationHeader();
            //this.NestedPropertyToStartFrom = "Items"; //Using the self link does have an items property
        }

        public string BaseAddress => this.Client.BaseAddress.ToString();

        public string DefaultAssetServer { get; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool HasAuthorization => this.Client.DefaultRequestHeaders.Contains("Authorization");

        public void AddAuthorizationHeader()
        {
            if (!String.IsNullOrWhiteSpace(this.UserName) && !String.IsNullOrWhiteSpace(this.Password) && !HasAuthorization)
            {
                string asBase64 = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(UserName + ":" + Password));
                this.Client.DefaultRequestHeaders.Add("Authorization", asBase64);
            }
        }

        public async Task DoSomething()
        {
            string data = await this.GetAsync("assetservers");
        }

        public async Task<T> GetByDirectLink<T>(string link)
        {
            return await this.GetAsync<T>(link);
        }

        public async Task<AssetServer> AssetServerSearch(string assetServer)
        {
            IEnumerable<AssetServer> assetServers = await this.GetAllAsync<AssetServer>("assetservers", ITEMS_PROPERTY);
            return assetServers.Where(x => x.Name.CapsAndTrim() == assetServer.CapsAndTrim()).FirstOrDefault();
        }

        public async Task<IEnumerable<Database>> GetAssetServerDatabases(string assetServer)
        {
            AssetServer server = await AssetServerSearch(assetServer);
            if (server == null) return null;
            IEnumerable<Database> databases = await this.GetAllAsync<Database>(server.Links.Databases, ITEMS_PROPERTY);
            return databases;
        }

        public async Task<Database> DatabaseSearch(string assetServer, string database)
        {
            IEnumerable<Database> databases = await this.GetAssetServerDatabases(assetServer);
            return databases.Where(x => x.Name.CapsAndTrim() == database.CapsAndTrim()).FirstOrDefault();
        }

        public async Task<IEnumerable<Asset>> GetDatabaseAssets(Database database)
        {
            return await this.GetAllAsync<Asset>(database.Links.Elements, ITEMS_PROPERTY);
        }

        public async Task<IEnumerable<Asset>> AssetSearchAll(string assetServer, string database, int depthLimit = 3)
        {
            Database owningDatabase = await DatabaseSearch(assetServer, database);
            if (owningDatabase == null) return null;
            List<Asset> databaseChildren = (await this.GetAllAsync<Asset>(owningDatabase.Links.Elements, ITEMS_PROPERTY)).ToList();
            owningDatabase.ChildAssets = databaseChildren;
            await AssetSearchAllInternal(databaseChildren, databaseChildren, depthLimit, 1);
            return databaseChildren.ToList();
        }

        public async Task<IEnumerable<Asset>> GetChildAssets(Asset asset)
        {
            return await this.GetAllAsync<Asset>(asset.Links.Elements, ITEMS_PROPERTY);
        }

        public async Task LoadAssetValueList(Asset asset)
        {
            ICollection<AssetValue> assetValues = (await this.GetAllAsync<AssetValue>(asset.Links.Value, ITEMS_PROPERTY)).ToList();
            asset.ChildValues = assetValues;
        }

        public async Task<AssetValue> LoadAssetValueDetail(AssetValue assetValue)
        {
            assetValue = await this.GetByDirectLink<AssetValue>(assetValue.Links.Source);
            return assetValue;
        }

        public async Task LoadInterpolatedValues(AssetValue value, bool convertTimeStampsToLocalTime = false)
        {
            string url = value.Links.InterpolatedData;
            url = url.WithParameter("startTime", "*-16d").WithParameter("interval", "1h");
            value.InterpolatedDataPoints = await this.GetAllAsync<InterpolatedDataPoint>(url, ITEMS_PROPERTY);

            if(convertTimeStampsToLocalTime)
            {
                foreach(InterpolatedDataPoint point in value.InterpolatedDataPoints)
                {
                    point.Timestamp = point.Timestamp.ToLocalTime();
                }
            }
        }

        public async Task LoadInterpolatedValues(AssetValue value, DateTime startDate, DateTime endDate, bool useLocalTime = false)
        {
            string url = value.Links.InterpolatedData;
            int startDaysAgo = (DateTime.Today - startDate.Date).Days;
            int endDaysAgo = (DateTime.Today - endDate.Date).Days;
            //The api seems to cut off day filtering with the current time. I.E. if i want yesterday i'd enter -1d, but i'd only see timestamps that are before today's current time
            //It seems like it should be possible to pass in a timestamp for your startTime and endTime and have everything filter perfectly, but i'm seeing odd behavior with that as well
            //Anyway the workaround for now is to buffer the query with an extra day on each end and trim unwanted dates client side.
            startDaysAgo++;
            endDaysAgo--; 
            url = url.WithParameter("startTime", $"*-{startDaysAgo}d").WithParameter("interval", "1h");
            if (endDaysAgo > 0) url = url.WithParameter("endTime", $"*-{endDaysAgo}d");
            value.InterpolatedDataPoints = await this.GetAllAsync<InterpolatedDataPoint>(url, ITEMS_PROPERTY);

            if (useLocalTime)
            {
                foreach (InterpolatedDataPoint point in value.InterpolatedDataPoints)
                {
                    point.Timestamp = point.Timestamp.ToLocalTime(); //Pi gives back all dates in UTC, so a call to ToLocalTime() should take care of it
                }
            }

            value.InterpolatedDataPoints = value.InterpolatedDataPoints.Where(x => x.Timestamp.Date.Date >= startDate.Date && x.Timestamp.Date.Date <= endDate.Date).ToList();
        }

        private async Task<List<Asset>> AssetSearchAllInternal(List<Asset> parents, List<Asset> assets, int depthLimit, int currentDepth)
        {
            if (currentDepth > depthLimit) return assets;

            foreach(Asset parent in parents.ToList())
            {
                List<Asset> children = (await this.GetAllAsync<Asset>(parent.Links.Elements, ITEMS_PROPERTY)).ToList();
                foreach (var child in children) { child.Parent = parent; }
                parent.ChildAssets = children;
                assets.AddRange(children);
                await AssetSearchAllInternal(children, assets, depthLimit, currentDepth + 1);
            }

            return assets;
        }
    }
}