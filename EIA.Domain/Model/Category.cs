using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using EIA.Domain.DataBind;

namespace EIA.Domain.Model
{
    public class Category
    {
        [JsonConverter(typeof(ToIntConverter))]
        [JsonPropertyName("category_id")]
        public int CategoryId { get; set; }

        [JsonPropertyName("name")]
        public string CategoryName { get; set; }

        [JsonPropertyName("parent_category_id")]
        public string ParentCategoryId { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }

        [JsonPropertyName("childcategories")]
        public ICollection<Category> ChildCategories { get; set; }

        [JsonPropertyName("childseries")]
        public ICollection<Series> ChildSeries { get; set; }
    }
}
