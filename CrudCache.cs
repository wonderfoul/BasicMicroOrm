using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Concurrent;

namespace BasicMicroOrm
{
    /// <summary>   Crud cache. </summary>
    ///
    /// <remarks>   Nsl, 08.01.2013. </remarks>

    public static class CrudCache
    {
        private static Dictionary<Type, string> _createSqlCache = new Dictionary<Type, string>();
        private static Dictionary<Type, string> _readSqlCache = new Dictionary<Type, string>();
        private static Dictionary<Type, string> _readAllSqlCache = new Dictionary<Type, string>();
        private static Dictionary<Type, string> _updateSqlCache = new Dictionary<Type, string>();
        private static Dictionary<Type, string> _deleteSqlCache = new Dictionary<Type, string>();

        /// <summary>   Gets a create sql. </summary>
        ///
        /// <remarks>   Nsl, 08.01.2013. </remarks>
        ///
        /// <param name="type"> The type. </param>
        ///
        /// <returns>   The create sql. </returns>

        public static string GetCreateSql(Type type)
        {
            string result;

            if (_createSqlCache.TryGetValue(type, out result) == true)
            {
                return result;
            }


            List<string> columnNames = new List<string>();
            List<string> parameterNames = new List<string>();
            bool containsIdentity = false;

            foreach (PropertyInfo property in type.GetProperties())
            {
                bool isIdentity = property.GetCustomAttributes(typeof(Identity), false).Count() > 0;

                if (isIdentity == false)
                {
                    columnNames.Add(property.Name);
                    parameterNames.Add("@" + property.Name);
                }
                else
                {
                    containsIdentity = true;
                }
            }

            string insertBase = "INSERT INTO {0} ({1}) VALUES ({2})";

            if (containsIdentity == true)
            {
                insertBase += " SELECT CAST(SCOPE_IDENTITY() AS INT)";
            }
            else
            {
                insertBase += " SELECT 0";
            }

            result = string.Format(insertBase, type.Name, columnNames.ToFormattedString(","), parameterNames.ToFormattedString(","));

            _createSqlCache[type] = result;

            return result;
        }

        /// <summary>   Gets a read sql. </summary>
        ///
        /// <remarks>   Nsl, 08.01.2013. </remarks>
        ///
        /// <param name="type"> The type. </param>
        ///
        /// <returns>   The read sql. </returns>

        public static string GetReadSql(Type type)
        {
            string result;

            if (_readSqlCache.TryGetValue(type, out result) == true)
            {
                return result;
            }

            //string readBase = "SELECT {0} FROM {1} WHERE {2}";

            //List<string> columnNames = new List<string>();
            List<string> parameterNames = new List<string>();


            foreach (PropertyInfo property in type.GetProperties())
            {
                bool isKey = property.GetCustomAttributes(typeof(PrimaryKey), false).Count() > 0;

                // columnNames.Add(property.Name);

                if (isKey == true)
                {
                    parameterNames.Add(property.Name + "=@" + property.Name);
                }
            }

            //result = string.Format(readBase, columnNames.ToFormattedString(","), type.Name, parameterNames.ToFormattedString(" AND "));

            result = string.Format("SELECT * FROM {0} WHERE {1}", type.Name, parameterNames.ToFormattedString(" AND "));
            
            _readSqlCache[type] = result;

            return result;
        }

        /// <summary>   Gets a read all sql. </summary>
        ///
        /// <remarks>   Nsl, 08.01.2013. </remarks>
        ///
        /// <param name="type"> The type. </param>
        ///
        /// <returns>   The read all sql. </returns>

        public static string GetReadAllSql(Type type)
        {
            string result;

            if (_readAllSqlCache.TryGetValue(type, out result) == true)
            {
                return result;
            }

            /*string readAllBase = "SELECT {0} FROM {1}";

            
            List<string> columnNames = new List<string>();


            foreach (PropertyInfo property in type.GetProperties())
            {
                columnNames.Add(property.Name);
            }

            result = string.Format(readAllBase, columnNames.ToFormattedString(","), type.Name);
            */

            result = string.Format("SELECT * FROM {0}", type.Name);

            _readAllSqlCache[type] = result;

            return result;
        }

        /// <summary>   Gets an update sql. </summary>
        ///
        /// <remarks>   Nsl, 08.01.2013. </remarks>
        ///
        /// <param name="type"> The type. </param>
        ///
        /// <returns>   The update sql. </returns>

        public static string GetUpdateSql(Type type)
        {
            string result;

            if (_updateSqlCache.TryGetValue(type, out result) == true)
            {
                return result;
            }

            string updateBase = "UPDATE {0} SET {1} WHERE {2}";

            List<string> columnNames = new List<string>();
            List<string> parameterNames = new List<string>();

            foreach (PropertyInfo property in type.GetProperties())
            {
                bool isKey = property.GetCustomAttributes(typeof(PrimaryKey), false).Count() > 0;

                if (isKey == false)
                {
                    columnNames.Add(property.Name + "=@" + property.Name);
                }
                else
                {
                    parameterNames.Add(property.Name + "=@" + property.Name);
                }
            }

            result = string.Format(updateBase, type.Name, columnNames.ToFormattedString(","), parameterNames.ToFormattedString(" AND "));

            _updateSqlCache[type] = result;

            return result;
        }

        /// <summary>   Gets a delete sql. </summary>
        ///
        /// <remarks>   Nsl, 08.01.2013. </remarks>
        ///
        /// <param name="type"> The type. </param>
        ///
        /// <returns>   The delete sql. </returns>

        public static string GetDeleteSql(Type type)
        {
            string result;

            if (_deleteSqlCache.TryGetValue(type, out result) == true)
            {
                return result;
            }

            string deleteBase = "DELETE FROM {0} WHERE {1}";

            List<string> parameterNames = new List<string>();


            foreach (PropertyInfo property in type.GetProperties())
            {
                bool isKey = property.GetCustomAttributes(typeof(PrimaryKey), false).Count() > 0;

                if (isKey == true)
                {
                    parameterNames.Add(property.Name + "=@" + property.Name);
                }
            }

            result = string.Format(deleteBase, type.Name, parameterNames.ToFormattedString(" AND "));

            _deleteSqlCache[type] = result;

            return result;
        }

        /// <summary>   Clears the cache. </summary>
        ///
        /// <remarks>   Nsl, 08.01.2013. </remarks>

        public static void ClearCache()
        {
            _createSqlCache.Clear();
            _readSqlCache.Clear();
            _readAllSqlCache.Clear();
            _updateSqlCache.Clear();
            _deleteSqlCache.Clear();
        }
    }
}
