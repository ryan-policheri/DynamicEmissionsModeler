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
    }
}
