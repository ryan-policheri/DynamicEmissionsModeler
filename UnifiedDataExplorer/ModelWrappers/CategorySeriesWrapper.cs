using System;
using System.Collections.Generic;
using DotNetCommon.MVVM;
using EIA.Domain.Model;

namespace EIADataViewer.ModelWrappers
{
    public class CategorySeriesWrapper : Category, ILazyTreeItemBackingModel
    {
        private readonly Category _category;
        private readonly Series _series;

        public CategorySeriesWrapper(Category category)
        {
            _category = category;
            _series = null;
            this.Children = new List<CategorySeriesWrapper>();

            if (_category.ChildCategories != null)
            {
                foreach (Category child in _category.ChildCategories)
                {
                    Children.Add(new CategorySeriesWrapper(child));
                }
            }

            if (_category.ChildSeries != null)
            {
                foreach (Series child in _category.ChildSeries)
                {
                    Children.Add(new CategorySeriesWrapper(child));
                }
            }
        }

        private CategorySeriesWrapper(Series series)
        {
            _series = series;
            _category = null;
            this.Children = new List<CategorySeriesWrapper>();
        }

        public ICollection<CategorySeriesWrapper> Children { get; }

        public string GetId()
        {
            if (_category != null) return _category.CategoryId.ToString();
            else if (_series != null) return _series.Id.ToString();
            else throw new InvalidOperationException();
        }

        public string GetItemName()
        {
            if (_category != null) return _category.CategoryName;
            else if (_series != null) return _series.Name;
            else throw new InvalidOperationException();
        }

        public bool IsKnownLeaf()
        {//A category could have a child category so we don't know if it is a leaf. A series will always be a leaf.
            if (_category == null && _series != null) return true;
            else return false;
        }

        public bool IsSeries()
        {
            return _series != null;
        }

        public bool IsCategory()
        {
            return _category != null;
        }
    }
}
