using System.Text.Json.Serialization;

namespace PiModel
{
    public class Asset : ItemBase
    {
        public string TemplateName { get; set; }
        public bool HasChildren { get; set; }
        public List<object> CategoryNames { get; set; }
        public AssetLinks Links { get; set; }

        [JsonIgnore]
        public Asset Parent { get; set; }

        [JsonIgnore]
        public ICollection<Asset> Children { get; set; }
    }
}
