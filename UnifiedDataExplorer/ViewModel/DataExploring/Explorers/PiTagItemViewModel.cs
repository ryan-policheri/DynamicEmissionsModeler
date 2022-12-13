using System.Linq;
using DotNetCommon.Extensions;
using PiModel;

namespace UnifiedDataExplorer.ViewModel.DataExploring.Explorers
{
    public class PiTagItemViewModel
    {
        private PiPoint _piPoint;

        public PiTagItemViewModel()
        {
        }

        public PiTagItemViewModel(PiPoint piPoint)
        {
            _piPoint = piPoint;
            TagName = _piPoint.Name;
            EngineeringUnits = _piPoint.EngineeringUnits;
            Description = _piPoint.Descriptor;
            ShortenedDescription = Description.First(25, true);
            Link = _piPoint.Links.Self;
        }

        public string TagName { get; set; }
        public string EngineeringUnits { get; set; }
        public string Description { get; set; }
        public string ShortenedDescription { get; set; }
        public string Link { get; set; }
    }
}