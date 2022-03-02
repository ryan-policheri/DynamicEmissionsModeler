using System;
using System.Collections.Generic;
using System.Linq;
using DotNetCommon.MVVM;
using PiModel;

namespace UnifiedDataExplorer.ModelWrappers
{
    public class ServerDatabaseAssetWrapper : ILazyTreeItemBackingModel
    {
        private readonly AssetServer _assetServer;
        private readonly Database _database;
        private readonly Asset _asset;

        public const string ASSET_SERVER_TYPE = "ASSET_SERVER";
        public const string DATABASE_TYPE = "DATABASE";
        public const string ASSET_TYPE = "ASSET";

        public ServerDatabaseAssetWrapper(AssetServer assetServer)
        {
            if (assetServer == null) throw new ArgumentNullException();
            _assetServer = assetServer;
        }

        public ServerDatabaseAssetWrapper(Database database)
        {
            if (database == null) throw new ArgumentNullException();
            _database = database;
        }

        public ServerDatabaseAssetWrapper(Asset asset)
        {
            if (asset == null) throw new ArgumentNullException();
            _asset = asset;
        }

        public ItemBase ItemBase
        {
            get
            {
                ICollection<ItemBase> all = new List<ItemBase>() { _assetServer, _database, _asset };
                return all.Where(x => x != null).First();
            }
        }

        public ICollection<ServerDatabaseAssetWrapper> Children { get; } = new List<ServerDatabaseAssetWrapper>();

        public string GetLinkToSelf()
        {
            if (IsAssetServer()) return AsAssetServer().Links.Self;
            else if (IsDatabase()) return AsDatabase().Links.Self;
            else if (IsAsset()) return AsAsset().Links.Self;
            else throw new InvalidOperationException("Link to self not implemented");
        }

        public string GetId()
        {
            return ItemBase.Id;
        }

        public string GetItemName()
        {
            return ItemBase.Name;
        }

        public bool IsKnownLeaf()
        {
            if (this.IsAsset()) return !_asset.HasChildren;
            return false;
        }

        public bool IsAssetServer()
        {
            return _assetServer != null;
        }

        public AssetServer AsAssetServer()
        {
            return _assetServer;
        }

        public bool IsDatabase()
        {
            return _database != null;
        }

        public Database AsDatabase()
        {
            return _database;
        }

        public bool IsAsset()
        {
            return _asset != null;
        }

        public Asset AsAsset()
        {
            return _asset;
        }

        public string GetTypeTag()
        {
            if (IsAssetServer()) return ASSET_SERVER_TYPE;
            else if (IsDatabase()) return DATABASE_TYPE;
            else if (IsAsset()) return ASSET_TYPE;
            else throw new InvalidOperationException("Type tag not implemented");
        }
    }
}
