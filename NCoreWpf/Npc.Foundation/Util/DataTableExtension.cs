using Npc.Foundation.Logger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Npc.Foundation.Util
{
    /// <summary>
    /// DataTable Extension
    /// </summary>
    public static class DataTableExtension
    {
        /// <summary>
        /// DataTable to List<typeparamref name="T"/>
        /// </summary>
        public static List<T> ToList<T>(this DataTable dt)
        {
            List<T> result = new List<T>();

            Type type = typeof(T);
            PropertyInfo[] propertyInfos = type.GetProperties();

            var cols = dt.Columns.Cast<DataColumn>().ToList();

            foreach (DataRow row in dt.Rows)
            {
                result.Add(row.ToModel<T>(cols, type, propertyInfos));
            }

            return result;
        }

        /// <summary>
        /// DataTable to List<typeparamref name="T"/>
        /// </summary>
        /// <param name="columnNames"></param>
        public static List<T> ToList<T>(this DataTable dt, List<string> columnNames)
        {
            List<T> result = new List<T>();

            Type type = typeof(T);
            PropertyInfo[] propertyInfos = type.GetProperties();

            var allCols = dt.Columns.Cast<DataColumn>().ToList();

            List<DataColumn> cols = null;
            if (columnNames != null)
            {
                cols = new List<DataColumn>();
                foreach (var col in allCols)
                {
                    if (columnNames.Contains(col.ColumnName))
                    {
                        cols.Add(col);
                    }
                }
            }
            else
            {
                cols = allCols;
            }


            foreach (DataRow row in dt.Rows)
            {
                result.Add(row.ToModel<T>(cols, type, propertyInfos));
            }

            return result;
        }

        /// <summary>
        /// DataTable to ObservableCollection<typeparamref name="T"/>
        /// </summary>
        public static ObservableCollection<T> ToObservableCollection<T>(this DataTable dt)
        {
            ObservableCollection<T> result = new ObservableCollection<T>();

            Type type = typeof(T);
            PropertyInfo[] propertyInfos = type.GetProperties();

            var cols = dt.Columns.Cast<DataColumn>().ToList();

            foreach (DataRow row in dt.Rows)
            {
                result.Add(row.ToModel<T>(cols, type, propertyInfos));
            }

            return result;
        }

        /// <summary>
        /// DataRow to Model<typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="cols"></param>
        /// <param name="type"></param>
        /// <param name="propertyInfos"></param>
        /// <returns></returns>
        private static T ToModel<T>(this DataRow row, List<DataColumn> cols, Type type, PropertyInfo[] propertyInfos)
        {
            T instance = (T)Activator.CreateInstance(type);

            foreach (PropertyInfo pi in propertyInfos)
            {
                if (cols.Any(c => c.ColumnName.ToUpper() == pi.Name.ToUpper()))
                {
                    object value = row[pi.Name];
                    if (pi.PropertyType.ToString() != value.GetType().ToString())
                    {
                        if (value is DBNull) { continue; }

                        try
                        {
                            pi.SetValue(instance, Convert.ChangeType(value, Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType), null);

                        }
                        catch (Exception ex)
                        {
                            LogHub.Write(ex, LogTypes.NpcFatal);
                        }
                    }
                    else
                    {
                        pi.SetValue(instance, value, null);
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// DataTable 에서 string value 조회
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static string GetValueString(this DataTable dt, int row, int column)
        {
            if (dt != null && dt.Rows.Count > row && dt.Columns.Count > column)
            {
                return dt.Rows[row][column].ToString();
            }

            return null;
        }

        /// <summary>
        /// DataTable 에서 int value 조회
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static int GetValueInt(this DataTable dt, int row, int column)
        {
            int value = 0;

            if (dt != null && dt.Rows.Count > row && dt.Columns.Count > column)
            {
                if (dt.Rows[row][column] != DBNull.Value)
                {
                    int.TryParse(dt.Rows[row][column].ToString(), out value);
                }
            }

            return value;
        }

        /// <summary>
        /// DataTable 에서 string value 조회
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static string GetValueString(this DataTable dt, int row, string columnName)
        {
            if (dt != null && dt.Rows.Count > row && dt.Columns.Contains(columnName))
            {
                return dt.Rows[row][columnName].ToString();
            }

            return null;
        }

        /// <summary>
        /// DataTable 에서 int value 조회
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static int GetValueInt(this DataTable dt, int row, string columnName)
        {
            int value = 0;

            if (dt != null && dt.Rows.Count > row && dt.Columns.Contains(columnName))
            {
                if (dt.Rows[row][columnName] != DBNull.Value)
                {
                    int.TryParse(dt.Rows[row][columnName].ToString(), out value);
                }
            }

            return value;
        }
    }
}
