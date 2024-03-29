﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetCommon.Extensions;
using DotNetCommon.WebApiClient;
using EIA.Domain.Extensions;
using EIA.Domain.Model;

namespace EIA.Services.Clients
{
    public class EiaClient : WebApiClientBase
    {
        public EiaClient() : base()
        {
            this.Client = new HttpClient();
        }

        public void Initialize(IEiaConnectionInfo connectionInfo)
        {
            Initialize(connectionInfo.BaseUrl, connectionInfo.SubscriptionKey);
        }

        public void Initialize(string baseAddress, string subscriptionKey)
        {
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

        public async Task TestAsync()
        {
            await this.GetAsync("".WithQueryString("api_key", SubscriptionKey));
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

        public async Task<Series> GetSeriesByIdAsync(string seriesId, int numberOfDays = -1)
        {
            string pathPreDayFilter = $"/v2/seriesid/{seriesId}".WithQueryString("api_key", SubscriptionKey);
            string path = pathPreDayFilter;
            if (numberOfDays > 0)
            {
                DateTime startDate = DateTime.UtcNow.AddDays(-numberOfDays);
                string dateFilterValue = startDate.ToString("yyyyMMddTHHZ");
                path = pathPreDayFilter.WithQueryString("start", dateFilterValue);
            }
            Series series = await this.GetAsync<Series>(path, "response");
            if (!series.Frequency.IsHourlyFrequency()) //This is smelly, but essentially we are saying that if the data isn't hourly, remove the numberOfDays filter
            {
                series = await this.GetAsync<Series>(pathPreDayFilter, "response");
            }
            //series.ParseAllDates();
            return series;
        }

        public async Task<Series> GetSeriesByIdAsync(string seriesId, IBuildEiaTimeSeriesQueryString settings)
        {
            //TODO, make this work better
            string preUserQuery = $"/v2/seriesid/{seriesId}".WithQueryString("api_key", SubscriptionKey);
            string queryString = settings.BuildEiaQueryString();
            string path = preUserQuery + "&" + queryString.TrimStart('?');
            Series series = await this.GetAsync<Series>(path, "response");
            return series;
        }

        public async Task<Series> GetHourlySeriesByIdAsync(string seriesId, DateTimeOffset startDate, DateTimeOffset endDate, TimeSpan seriesDataOffset)
        {//This method assumes you know the offset of your series data. It is hourly data after all.
            if (seriesDataOffset.Hours != 0) throw new NotImplementedException("Have not implemented querying of non-utc data yet");

            string startDateString = startDate.ToStringWithNoOffset("yyyyMMddTHHZ");
            string endDateString  = endDate.ToStringWithNoOffset("yyyyMMddTHHZ");

            string path = $"/v2/seriesid/{seriesId}".WithQueryString("api_key", SubscriptionKey);
            path = path.WithQueryString("start", startDateString);
            path = path.WithQueryString("end", endDateString);
            Series series = await this.GetFirstAsync<Series>(path, "response");

            if (!series.Frequency.IsHourlyFrequency()) throw new ArgumentException("This method is only for series that are hourly in nature");

            return series;
        }
    }
}