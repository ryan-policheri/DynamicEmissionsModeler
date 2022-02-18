using System.Text.Json.Serialization;

namespace PiModel
{
    public class Database : ItemBase
    {
        public DatabaseLinks Links { get; set; }

        [JsonIgnore]
        public ICollection<Asset> ChildAssets { get; set; }
    }
}
