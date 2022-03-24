using DotNetCommon.Extensions;
using DotNetCommon.WebApiClient;
using PiModel;
using PiModel.Search;

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

        //ASSET FRAMEWORK SEARCHING, PI HIERARCHY
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

        private async Task<List<Asset>> AssetSearchAllInternal(List<Asset> parents, List<Asset> assets, int depthLimit, int currentDepth)
        {
            if (currentDepth > depthLimit) return assets;

            foreach (Asset parent in parents.ToList())
            {
                List<Asset> children = (await this.GetAllAsync<Asset>(parent.Links.Elements, ITEMS_PROPERTY)).ToList();
                foreach (var child in children) { child.Parent = parent; }
                parent.ChildAssets = children;
                assets.AddRange(children);
                await AssetSearchAllInternal(children, assets, depthLimit, currentDepth + 1);
            }

            return assets;
        }

        //QUERY SEARCHING
        public async Task<PiPoint> SearchPiPoint(string piPointName)
        {
            if (String.IsNullOrWhiteSpace(piPointName)) throw new ArgumentNullException(nameof(piPointName));
            string url = "search/query".WithQueryParameter("name", piPointName);
            PiSearchResult searchResult = await this.GetAsync<PiSearchResult>(url);

            if (searchResult.Errors.Count > 0) throw new Exception("Error with request: " + searchResult.Errors.Select(x => x.Message).ToDelimitedList(';'));
            if (searchResult.TotalHits < 1) return null;
            PiSearchItem item = searchResult.Items.FirstOrDefault(x => x.Name.CapsAndTrim() == piPointName.CapsAndTrim() && x.ItemType.CapsAndTrim() == ItemTypes.PI_POINT.CapsAndTrim());
            if (item == null) return null;

            PiPoint piPoint = await this.GetByDirectLink<PiPoint>(item.Links.Self);
            return piPoint;
        }

        //LOAD TIME SERIES DATA
        public async Task LoadInterpolatedValues(IHaveTimeSeriesData item, int daysBack, bool convertTimeStampsToLocalTime = false)
        {
            string url = item.TimeSeriesLinks.InterpolatedData;
            url = url.WithParameter("startTime", $"*-{daysBack}d").WithParameter("interval", "1h");
            item.InterpolatedDataPoints = await this.GetAllAsync<InterpolatedDataPoint>(url, ITEMS_PROPERTY);

            if (convertTimeStampsToLocalTime)
            {
                foreach (InterpolatedDataPoint point in item.InterpolatedDataPoints)
                {
                    point.Timestamp = point.Timestamp.ToLocalTime();
                }
            }
        }

        public async Task LoadInterpolatedValues(IHaveTimeSeriesData item, DateTime startDate, DateTime endDate)
        {
            if (startDate.Kind != DateTimeKind.Utc && startDate.Kind != DateTimeKind.Local) throw new ArgumentException("The incoming start date must have a DateTimeKind of Utc or Local. " +
     "The DateTimeKind of loaded interpolated values will match that of the incoming date range");
            if (endDate.Kind != DateTimeKind.Utc && endDate.Kind != DateTimeKind.Local) throw new ArgumentException("The incoming end date must have a DateTimeKind of Utc or Local. " +
                "The DateTimeKind of loaded interpolated values will match that of the incoming date range");
            if (startDate.Kind != endDate.Kind) throw new ArgumentException("The startDate and endDate must have the same DateTimeKind");

            string url = item.TimeSeriesLinks.InterpolatedData;

            startDate = startDate.Date.AddMinutes(30); //30 minutes after the start of the start date
            endDate = endDate.AddDays(1).Date.AddMinutes(-30); //30 minutes before the end of the end date
            string startDateString;
            string endDateString;

            if (startDate.Kind == DateTimeKind.Utc) { startDateString = startDate.ToStringWithNoOffset(); endDateString = endDate.ToStringWithNoOffset(); }
            else { startDateString = startDate.ToStringWithLocalOffset(); endDateString = endDate.ToStringWithLocalOffset(); }

            url = url.WithParameter("startTime", $"{startDateString}")
                .WithParameter("endTime", $"{endDateString}")
                .WithParameter("interval", "1h");

            item.InterpolatedDataPoints = await this.GetAllAsync<InterpolatedDataPoint>(url, ITEMS_PROPERTY);

            if (startDate.Kind == DateTimeKind.Local)
            {
                foreach (InterpolatedDataPoint point in item.InterpolatedDataPoints)
                {
                    point.Timestamp = point.Timestamp.ToLocalTime(); //Pi gives back all dates in UTC, so a call to ToLocalTime() should take care of it
                }
            }

            item.InterpolatedDataPoints = item.InterpolatedDataPoints.Where(x => x.Timestamp.Date.Date >= startDate.Date && x.Timestamp.Date.Date <= endDate.Date).ToList();
        }
    }
}