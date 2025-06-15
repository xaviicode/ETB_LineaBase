using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LineaBaseETB_V2.Models;

namespace LineaBaseETB_V2.Services
{
    public class AzureDevOpsService
    {
        private readonly Uri _uri;
        private readonly VssBasicCredential _credentials;

        // Campos a consultar, ajusta según tus necesidades
        private readonly string[] fields = new[]
        {
            "System.Id",
            "System.Title",
            "System.State",
            "System.WorkItemType",
            "System.AssignedTo",
            "Custom.NumeroIniciativa",
            "Custom.Sistema_1",
            "Custom.Sistema_2",
            "Custom.Sistema_3",
            "Custom.Sistema_4",
            "Custom.Sistema_5",
            "Custom.Sistema_6",
            "Custom.Sistema_7",
            "Custom.Sistema_8",
            "Custom.Sistema_9",
            "Custom.Sistema_10",
            "Custom.Sistema_11",
            "Custom.Sistema_12",
            "Custom.SistemasAfectados",
            "Custom.Fecha_PnP"
        };

        private const int MaxBatchSize = 200;
        private const int MaxItemsToFetch = 20000;

        public AzureDevOpsService(string organization, string personalAccessToken)
        {
            _uri = new Uri($"https://dev.azure.com/{organization}");
            _credentials = new VssBasicCredential(string.Empty, personalAccessToken);
        }

        public async Task<List<string>> ObtenerProyectosAsync()
        {
            using var connection = new VssConnection(_uri, new VssCredentials(_credentials));
            var projectClient = connection.GetClient<ProjectHttpClient>();
            var proyectos = await projectClient.GetProjects();
            return proyectos.Select(p => p.Name).ToList();
        }

        public async Task<List<WorkItemModel>> ObtenerWorkItemsAsync(string proyecto)
        {
            using var client = new WorkItemTrackingHttpClient(_uri, new VssCredentials(_credentials));

            var wiql = new Wiql
            {
                Query = $@"
                    SELECT [System.Id] FROM WorkItems 
                    WHERE [System.TeamProject] = '{proyecto}'
                      AND [System.State] NOT IN ('Done', 'Closed')
                      AND [System.CreatedDate] >= @StartOfYear
                    ORDER BY [System.Id] DESC"
            };

            var result = await client.QueryByWiqlAsync(wiql);

            if (result.WorkItems == null || !result.WorkItems.Any())
                return new List<WorkItemModel>();

            var ids = result.WorkItems.Select(wi => wi.Id).Take(MaxItemsToFetch).ToArray();
            var allWorkItems = new List<WorkItemModel>();

            for (int i = 0; i < ids.Length; i += MaxBatchSize)
            {
                var batchIds = ids.Skip(i).Take(MaxBatchSize).ToArray();
                try
                {
                    var batch = await client.GetWorkItemsAsync(batchIds, fields);
                    foreach (var wi in batch)
                    {
                        var model = MapWorkItemToModel(wi);
                        // Seguridad: si por alguna razón el estado es Done/Closed, no lo agregues
                        if (model.State != "Done" && model.State != "Closed")
                            allWorkItems.Add(model);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener el batch de Work Items (IDs: {string.Join(",", batchIds)}): {ex.Message}");
                }
            }

            return allWorkItems;
        }

        private WorkItemModel MapWorkItemToModel(WorkItem wi)
        {
            return new WorkItemModel
            {
                Id = wi.Id ?? 0,
                Title = GetFieldValue<string>(wi, "System.Title"),
                State = GetFieldValue<string>(wi, "System.State"),
                Type = GetFieldValue<string>(wi, "System.WorkItemType"),
                AssignedTo = GetFieldValue<string>(wi, "System.AssignedTo"),
                NumeroIniciativa = GetFieldValue<string>(wi, "Custom.NumeroIniciativa"),
                Sistema1 = GetFieldValue<string>(wi, "Custom.Sistema_1"),
                Sistema2 = GetFieldValue<string>(wi, "Custom.Sistema_2"),
                Sistema3 = GetFieldValue<string>(wi, "Custom.Sistema_3"),
                Sistema4 = GetFieldValue<string>(wi, "Custom.Sistema_4"),
                Sistema5 = GetFieldValue<string>(wi, "Custom.Sistema_5"),
                Sistema6 = GetFieldValue<string>(wi, "Custom.Sistema_6"),
                Sistema7 = GetFieldValue<string>(wi, "Custom.Sistema_7"),
                Sistema8 = GetFieldValue<string>(wi, "Custom.Sistema_8"),
                Sistema9 = GetFieldValue<string>(wi, "Custom.Sistema_9"),
                Sistema10 = GetFieldValue<string>(wi, "Custom.Sistema_10"),
                Sistema11 = GetFieldValue<string>(wi, "Custom.Sistema_11"),
                Sistema12 = GetFieldValue<string>(wi, "Custom.Sistema_12"),
                SistemasAfectados = GetFieldValue<string>(wi, "Custom.SistemasAfectados"),
                FechaPnP = GetFieldValueDateTime(wi, "Custom.Fecha_PnP")
            };
        }

        private T GetFieldValue<T>(WorkItem wi, string fieldName)
        {
            if (wi.Fields.TryGetValue(fieldName, out object value))
            {
                if (value is T variable)
                    return variable;
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return default;
                }
            }
            return default;
        }

        private DateTime? GetFieldValueDateTime(WorkItem wi, string fieldName)
        {
            var val = GetFieldValue<object>(wi, fieldName);
            if (val == null)
                return null;
            if (val is DateTime dt)
                return dt;
            if (DateTime.TryParse(val.ToString(), out DateTime parsed))
                return parsed;
            return null;
        }
    }
}
