using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EIA.Domain.Model;
using DotNetCommon.Extensions;
using DotNetCommon.WebApiClient;

namespace EIA.Services.Clients
{
    public class EiaClient : WebApiClientBase
    {
        private readonly string _subscriptionKey;

        public EiaClient(HttpClient client) : base(client)
        {
            _subscriptionKey = Client.DefaultRequestHeaders.Where(x => x.Key == "Subscription-Key").First().Value.First();
        }

        public async Task<Category> GetCategoryTreeAsync(int rootCategoryId)
        {
            Category root = await GetCategoryByIdAsync(rootCategoryId);
            await PopulateCategoryChildren(root);
            return root;
        }

        private async Task PopulateCategoryChildren(Category category)
        {
            if (category.ChildCategories == null || category.ChildCategories.Count() == 0) return;
            else
            {
                IEnumerable<Category> copiedChildren = category.ChildCategories.Copy();
                category.ChildCategories.Clear();
                foreach (Category child in copiedChildren)
                {
                    Category populatedChild = await GetCategoryByIdAsync(child.CategoryId);
                    category.ChildCategories.Add(populatedChild);
                }

                foreach (Category child in category.ChildCategories)
                {
                    await PopulateCategoryChildren(child);
                }
            }
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            string path = "category/".WithQueryString("api_key", _subscriptionKey).WithQueryString("category_id", categoryId.ToString());
            return await this.GetAsync<Category>(path, "category");
        }

        public async Task<Series> GetSeriesByIdAsync(string seriesId)
        {
            string path = "series/".WithQueryString("api_key", _subscriptionKey).WithQueryString("series_id", seriesId);
            return await this.GetFirstAsync<Series>(path, "series");
        }
    }
}
