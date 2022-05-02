using DotNetCommon.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIowaBuildingsModel;

namespace UnifiedDataExplorer.ViewModel
{
    public class CampusSnapshotViewModel : ViewModelBase
    {
        private CampusSnapshot snapshot;

        public CampusSnapshotViewModel(CampusSnapshot snapshot)
        {
            this.snapshot = snapshot;
        }
    }
}
