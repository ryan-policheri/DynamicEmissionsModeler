using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetCommon.Extensions;
using DotNetCommon.WebApiClient;
using EIA.Domain.Model;

namespace EIA.Services.Clients
{
    public class EiaClient : WebApiClientBase
    {
        public EiaClient(string baseAddress, string subscriptionKey) : base()
        {
            this.Client = new HttpClient();
            this.Client.BaseAddress = new Uri(baseAddress);
            SubscriptionKey = subscriptionKey;
            AddAuthorizationHeader();
        }

        public string BaseAddress => this.Client.BaseAddress.ToString();

        public string SubscriptionKey { get; set; }

        public bool HasAuthorization => this.Client.DefaultRequestHeaders.Contains("Subscription-Key");

        public void AddAuthorizationHeader()
        {
            if (!String.IsNullOrWhiteSpace(SubscriptionKey) && !this.HasAuthorization)
            {
                this.Client.DefaultRequestHeaders.Add("Subscription-Key", SubscriptionKey);
            }
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
            string path = "category/".WithQueryString("api_key", SubscriptionKey).WithQueryString("category_id", categoryId.ToString());
            return await this.GetAsync<Category>(path, "category");
        }

        public async Task<Series> GetSeriesByIdAsync(string seriesId)
        {
            string path = "series/".WithQueryString("api_key", SubscriptionKey).WithQueryString("series_id", seriesId);
            return await this.GetFirstAsync<Series>(path, "series");
        }
    }
}
