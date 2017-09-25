
namespace NixJqGridFramework.GenericExtention
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using NixJqGridFramework.Entities;
    using NixJqGridFramework.Entities.Enums;
    using NixJqGridFramework.Models;
    using NixJqGridFramework.NixJqGridCore;
    using NixJqGridFramework.NixJqGridSettings;

    /// <summary>
    /// NixJqGrid.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class NixJqGrid<T> : NixJqGrid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NixJqGrid&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        public NixJqGrid(NixUri url)
            : base(GetgenericTypeName(), url, GetDataSourceSettings())
        {
            this.GetDefaultProperties();   
        }

        /// <summary>
        /// Get current type name
        /// </summary>
        /// <returns>name of generic type</returns>
        private static string GetgenericTypeName()
        {
            return typeof(T).Name;
        }

        /// <summary>
        /// Get data source settings automaticly from generic type
        /// </summary>
        /// <returns></returns>
        private static DataSourceSettings GetDataSourceSettings()
        {
            DataSourceSettings dataSourceSettings = new DataSourceSettings();
            PropertyInfo[] properties = GetAllPublicProperties();
            dataSourceSettings.Columns = GetColumns(properties);
            dataSourceSettings.DataType = DataTypes.json;
            dataSourceSettings.MethodType = MethodTypes.POST;
            return dataSourceSettings;
        }


        /// <summary>
        /// Get all bublic properties from type wich has a get accessor
        /// </summary>
        /// <returns>property information list</returns>
        private static PropertyInfo[] GetAllPublicProperties()
        {
            Type currentType = typeof(T);
            return currentType.GetProperties();
        }


        /// <summary>
        /// Create column list with help a property list
        /// </summary>
        /// <param name="properties"></param>
        /// <returns>List of NixJqGridColumn</returns>
        private static IList<NixJqGridColumn> GetColumns(PropertyInfo[] properties)
        {
            return properties.Select(propertyInfo => new NixJqGridColumn()
                {
                    Name = propertyInfo.Name, 
                    NixJqGridColumnModel = new NixJqGridColumnModel()
                        {
                            Name = propertyInfo.Name, 
                            Index = propertyInfo.Name
                        }, 
                    ColumnType = propertyInfo.PropertyType.Name.ToLower().Replace("64", string.Empty)
                }).ToList();
        }

        /// <summary>
        /// Sets the default properties in base JqGrid
        /// </summary>
        private void GetDefaultProperties()
        {            
            base.Sortname = typeof(T)
                .GetProperties()
                .First()
                .Name;

            base.Viewrecords = true;
        }
    }
}
