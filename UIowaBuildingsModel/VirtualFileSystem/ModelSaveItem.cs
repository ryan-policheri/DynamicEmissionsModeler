using DotNetCommon.Extensions;
using EmissionsMonitorModel.ProcessModeling;

namespace EmissionsMonitorModel.VirtualFileSystem
{
    public class ModelSaveItem : SaveItem
    {
        public ModelSaveItem()
        {
            SaveItemType = SaveItemTypes.ModelSaveItem;
        }

        public string ProcessModelJsonDetails { get; set; }

        public ProcessModel ToProcessModel()
        {
            var model = this.ProcessModelJsonDetails.ConvertJsonToObject<ProcessModel>();
            model.ModelName = this.SaveItemName;
            return model;
        }
    }
}
