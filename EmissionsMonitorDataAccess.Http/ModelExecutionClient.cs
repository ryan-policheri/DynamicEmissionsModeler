using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCommon.WebApiClient;
using EmissionsMonitorModel.ProcessModeling;
using EmissiosMonitorDataAccess.Http;

namespace EmissionsMonitorDataAccess.Http
{
    public class ModelExecutionClient : EmissionsMonitorClient
    {
        public ModelExecutionClient()
        {
            this.Client.Timeout = new TimeSpan(12, 0 , 0);
        }

        public async Task<ModelExecutionResult> RemoteExecuteAsync(ModelExecutionSpec spec)
        {
            var result = await this.PostAsync<ModelExecutionSpec, ModelExecutionResult>("ModelExecution", spec, (response) => { return response.AsAsync<ModelExecutionResult>(); });
            return result;
        }
    }
}
