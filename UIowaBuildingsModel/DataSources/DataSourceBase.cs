using DotNetCommon.Extensions;

namespace EmissionsMonitorModel.DataSources
{
    public class DataSourceBase
    {
        public int SourceId { get; set; }

        public string SourceName { get; set; }

        public DataSourceType SourceType { get; set; }

        public string SourceDetailsJson { get; set; }

        public virtual string ToSourceDetails() => null;

        public virtual DataSourceBase FromSourceDetails()
        {
            switch (SourceType)
            {
                case DataSourceType.Pi:
                    var piSource = SourceDetailsJson.ConvertJsonToObject<PiDataSource>();
                    InternalMap(piSource);
                    return piSource;
                case DataSourceType.Eia:
                    var eiaSource = SourceDetailsJson.ConvertJsonToObject<EiaDataSource>();
                    InternalMap(eiaSource);
                    return eiaSource;
                default:
                    throw new NotImplementedException();
            }
        }

        private void InternalMap(DataSourceBase source)
        {
            source.SourceId = this.SourceId;
            source.SourceName = this.SourceName;
            source.SourceType = this.SourceType;
            source.SourceDetailsJson = this.SourceDetailsJson;
        }
    }
}
