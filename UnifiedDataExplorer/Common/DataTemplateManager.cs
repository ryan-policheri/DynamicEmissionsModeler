using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using DotNetCommon.Extensions;

namespace EIADataViewer.Common
{
    public class DataTemplateRegistrar
    {
        private readonly Assembly _assembly;
        private readonly string _viewModelRootNamespace;
        private readonly string _viewRootNamespace;
        private readonly DataTemplateManager _dataTemplateManager;

        public DataTemplateRegistrar(Assembly assembly, string viewModelRootNamespace, string viewRootNamespace)
        {
            _assembly = assembly;
            _viewModelRootNamespace = viewModelRootNamespace;
            _viewRootNamespace = viewRootNamespace;
            _dataTemplateManager = new DataTemplateManager();
        }

        public void RegisterAllTemplatesByConvention()
        {
            IEnumerable<Type> viewModelTypes = _assembly.GetTypes().Where(x => x.Namespace.StartsWith(_viewModelRootNamespace) && x.Name.EndsWith("ViewModel"));
            IEnumerable<Type> viewTypes = _assembly.GetTypes().Where(x => x.Namespace.StartsWith(_viewRootNamespace) && x.Name.EndsWith("View"));

            foreach(Type viewModelType in viewModelTypes)
            {
                string rootName = viewModelType.Name.TrimEnd("ViewModel");
                string viewName = rootName + "View";
                Type viewType = viewTypes.Where(x => x.Name == viewName).FirstOrDefault();

                if(viewType != null)
                {
                    _dataTemplateManager.RegisterDataTemplate(viewModelType, viewType);
                }
            }
        }
    }

    public class DataTemplateManager
    {//Source: https://ikriv.com/dev/wpf/DataTemplateCreation/DataTemplateManager.cs
        public void RegisterDataTemplate<TViewModel, TView>() where TView : FrameworkElement
        {
            RegisterDataTemplate(typeof(TViewModel), typeof(TView));
        }

        public void RegisterDataTemplate(Type viewModelType, Type viewType)
        {
            var template = CreateTemplate(viewModelType, viewType);
            var key = template.DataTemplateKey;
            Application.Current.Resources.Add(key, template);
        }

        private DataTemplate CreateTemplate(Type viewModelType, Type viewType)
        {
            const string xamlTemplate = "<DataTemplate DataType=\"{{x:Type vm:{0}}}\"><v:{1} /></DataTemplate>";
            var xaml = String.Format(xamlTemplate, viewModelType.Name, viewType.Name, viewModelType.Namespace, viewType.Namespace);

            var context = new ParserContext();

            context.XamlTypeMapper = new XamlTypeMapper(new string[0]);
            context.XamlTypeMapper.AddMappingProcessingInstruction("vm", viewModelType.Namespace, viewModelType.Assembly.FullName);
            context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace, viewType.Assembly.FullName);

            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            context.XmlnsDictionary.Add("vm", "vm");
            context.XmlnsDictionary.Add("v", "v");

            var template = (DataTemplate)XamlReader.Parse(xaml, context);
            return template;
        }
    }
}
