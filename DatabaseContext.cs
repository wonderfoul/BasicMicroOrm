using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

// namespace: Nias.Mentorkit.Data
//
// summary:	.

namespace BasicMicroOrm
{
    /// <summary>   Dummy class for the multiquery generic arguments. </summary>
    ///
    /// <remarks>   Nsl, 25.01.2013. </remarks>

    public class DoNotMap { }

    /// <summary>   Database context. </summary>
    ///
    /// <remarks>   Nsl, 11.05.2012. </remarks>

    public class DatabaseContext : IDisposable
    {
        private static string _connectionString;
        private SqlConnection _connection;
        private SqlTransaction _transaction;

        /// <summary>   Static constructor. </summary>
        ///
        /// <remarks>   Nsl, 11.05.2012. </remarks>

        static DatabaseContext()
        {
            _connectionString = System.Configuration.ConfigurationManager.AppSettings["SQLConnectionString"];
        }

        /// <summary>   Gets the connection. </summary>
        ///
        /// <remarks>   Nsl, 11.05.2012. </remarks>
        ///
        /// <returns>   The connection. </returns>

        public SqlConnection GetConnection()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            }

            return _connection;
        }

        /// <summary>   Gets the transaction. </summary>
        ///
        /// <remarks>   Nsl, 14.05.2012. </remarks>
        ///
        /// <returns>   The transaction. </returns>

        public SqlTransaction GetTransaction()
        {
            return _transaction;
        }

        /// <summary>   Starts a transaction. </summary>
        ///
        /// <remarks>   Nsl, 11.05.2012. </remarks>

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_transaction == null)
            {
                _transaction = GetConnection().BeginTransaction(isolationLevel);
            }
            else
            {
                throw new InvalidOperationException("SqlServer does not support nested transactions.");
            }
        }

        /// <summary>   Saves a transaction. </summary>
        ///
        /// <remarks>   Nsl, 25.02.2013. </remarks>
        ///
        /// <exception cref="InvalidOperationException">
        ///     Thrown when the requested operation is invalid.
        /// </exception>
        ///
        /// <param name="savePointName">    Name of the save point. </param>

        public void SaveTransaction(string savePointName)
        {
            if (_transaction != null)
            {
                _transaction.Save(savePointName);
            }
            else
            {
                throw new InvalidOperationException("You can not create a savepoint on a null transaction.");
            }
        }

        /// <summary>   Stops a transaction. </summary>
        ///
        /// <remarks>   Nsl, 11.05.2012. </remarks>

        public void RollbackTransaction(string savepointName = null)
        {
            if (_transaction != null)
            {
                if (savepointName == null)
                {
                    _transaction.Rollback();
                    _transaction.Dispose();
                    _transaction = null;
                }
                else
                {
                    _transaction.Rollback(savepointName);
                }
            }
        }

        /// <summary>   Saves this object. </summary>
        ///
        /// <remarks>   Nsl, 11.05.2012. </remarks>

        public void CommitTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Dispose();
                _transaction = null;
            }
            else
            {
                throw new InvalidOperationException("You can not commit a null transaction.");
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ///
        /// <remarks>   Nsl, 14.05.2012. </remarks>

        public void Dispose()
        {
            RollbackTransaction();

            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        /// <summary>   Creates this object. </summary>
        ///
        /// <remarks>   Nsl, 08.01.2013. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="parameters">   Options for controlling the operation. </param>
        ///
        /// <returns>   . </returns>

        public int Create<T>(object parameters) where T : class
        {
            string sql = CrudCache.GetCreateSql(typeof(T));

            return Query<int>(sql, parameters).First();
        }

        /// <summary>   Gets. </summary>
        ///
        /// <remarks>   Nsl, 08.01.2013. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="parameters">   Options for controlling the operation. </param>
        ///
        /// <returns>   . </returns>

        public T Get<T>(object parameters) where T : class
        {
            string sql = CrudCache.GetReadSql(typeof(T));

            return Query<T>(sql, parameters).FirstOrDefault<T>();
        }

        /// <summary>   Enumerates get all in this collection. </summary>
        ///
        /// <remarks>   Nsl, 08.01.2013. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="parameters">   Options for controlling the operation. </param>
        ///
        /// <returns>
        ///     An enumerator that allows foreach to be used to process get all&lt; t&gt; in this
        ///     collection.
        /// </returns>

        public IEnumerable<T> GetAll<T>(object parameters = null) where T : class
        {
            string sql = CrudCache.GetReadAllSql(typeof(T));

            return Query<T>(sql, parameters);
        }

        /// <summary>   Updates the given parameters. </summary>
        ///
        /// <remarks>   Nsl, 08.01.2013. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="parameters">   Options for controlling the operation. </param>
        ///
        /// <returns>   . </returns>

        public int Update<T>(object parameters)
        {
            string sql = CrudCache.GetUpdateSql(typeof(T));

            using (SqlCommand command = new SqlCommand(sql, GetConnection(), _transaction))
            {
                if (parameters != null)
                {
                    AddParameters(command, parameters);
                }

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>   Deletes the given parameters. </summary>
        ///
        /// <remarks>   Nsl, 08.01.2013. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="parameters">   Options for controlling the operation. </param>
        ///
        /// <returns>   . </returns>

        public int Delete<T>(object parameters)
        {
            string sql = CrudCache.GetDeleteSql(typeof(T));

            using (SqlCommand command = new SqlCommand(sql, GetConnection(), _transaction))
            {
                if (parameters != null)
                {
                    AddParameters(command, parameters);
                }

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>   Executes the non query operation. </summary>
        ///
        /// <remarks>   Nsl, 08.01.2013. </remarks>
        ///
        /// <param name="sql">          The sql. </param>
        /// <param name="parameters">   Options for controlling the operation. </param>
        /// <param name="commandType">  (optional) type of the command. </param>
        ///
        /// <returns>   . </returns>

        public int ExecuteNonQuery(string sql, object parameters = null, CommandType commandType = CommandType.Text)
        {
            using (SqlCommand command = new SqlCommand(sql, GetConnection(), _transaction))
            {
                command.CommandType = commandType;

                if (parameters != null)
                {
                    AddParameters(command, parameters);
                }

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>   Enumerates query in this collection. </summary>
        ///
        /// <remarks>   Nsl, 08.01.2013. </remarks>
        ///
        /// <param name="sql">          The sql. </param>
        /// <param name="parameters">   Options for controlling the operation. </param>
        ///
        /// <returns>
        ///     An enumerator that allows foreach to be used to process query in this collection.
        /// </returns>

        public IEnumerable<object> Query(string sql, object parameters = null)
        {
            using (SqlCommand command = new SqlCommand(sql, GetConnection(), _transaction))
            {
                if (parameters != null)
                {
                    AddParameters(command, parameters);
                }

                List<object> result = new List<object>();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Dictionary<string, object> prop = new Dictionary<string, object>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            prop.Add(reader.GetName(i), reader[i]);
                        }

                        result.Add((object)prop);
                    }
                }

                return result;
            }
        }


        /// <summary>   Enumerates query in this collection. </summary>
        ///
        /// <remarks>   Nsl, 08.01.2013. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="sql">          The sql. </param>
        /// <param name="parameters">   Options for controlling the operation. </param>
        /// <param name="commandType">  (optional) type of the command. </param>
        ///
        /// <returns>
        ///     An enumerator that allows foreach to be used to process query&lt; t&gt; in this
        ///     collection.
        /// </returns>

        public IEnumerable<T> Query<T>(string sql, object parameters = null, CommandType commandType = CommandType.Text)
        {
            using (SqlCommand command = new SqlCommand(sql, GetConnection(), _transaction))
            {
                command.CommandType = commandType;

                if (parameters != null)
                {
                    AddParameters(command, parameters);
                }

                List<T> list = new List<T>();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (typeof(T).IsValueType == false)
                    {
                        Func<SqlDataReader, T> map = OrmCache.GetMap<T>(reader);

                        while (reader.Read())
                        {
                            T obj = map.Invoke(reader);

                            list.Add(obj);
                        }
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            T obj = (T)reader.GetValue(0);

                            list.Add(obj);
                        }
                    }
                }

                return list.AsEnumerable<T>();
            }
        }

        /// <summary>   Multi query. </summary>
        ///
        /// <remarks>   Nsl, 25.01.2013. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <typeparam name="U">    Generic type parameter. </typeparam>
        /// <param name="sql">          The sql. </param>
        /// <param name="parameters">   Options for controlling the operation. </param>
        /// <param name="commandType">  (optional) type of the command. </param>
        ///
        /// <returns>   . </returns>

        public MultiQueryResultSet<T, U, DoNotMap, DoNotMap> Query<T, U>(string sql, object parameters = null, CommandType commandType = CommandType.Text)
        {
            return ReadMultiQueryResultSet<T, U, DoNotMap, DoNotMap>(sql, 2, parameters, commandType);
        }

        /// <summary>   Multi query. </summary>
        ///
        /// <remarks>   Nsl, 25.01.2013. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <typeparam name="U">    Generic type parameter. </typeparam>
        /// <typeparam name="V">    Generic type parameter. </typeparam>
        /// <param name="sql">          The sql. </param>
        /// <param name="parameters">   Options for controlling the operation. </param>
        /// <param name="commandType">  (optional) type of the command. </param>
        ///
        /// <returns>   . </returns>

        public MultiQueryResultSet<T, U, V, DoNotMap> Query<T, U, V>(string sql, object parameters = null, CommandType commandType = CommandType.Text)
        {
            return ReadMultiQueryResultSet<T, U, V, DoNotMap>(sql, 3, parameters, commandType);
        }

        /// <summary>   Multi query. </summary>
        ///
        /// <remarks>   Nsl, 25.01.2013. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <typeparam name="U">    Generic type parameter. </typeparam>
        /// <typeparam name="V">    Generic type parameter. </typeparam>
        /// <typeparam name="W">    Type of the w. </typeparam>
        /// <param name="sql">          The sql. </param>
        /// <param name="parameters">   Options for controlling the operation. </param>
        /// <param name="commandType">  (optional) type of the command. </param>
        ///
        /// <returns>   . </returns>

        public MultiQueryResultSet<T, U, V, W> Query<T, U, V, W>(string sql, object parameters = null, CommandType commandType = CommandType.Text)
        {
            return ReadMultiQueryResultSet<T, U, V, W>(sql, 4, parameters, commandType);
        }

        /// <summary>   Reads a multi query result set. </summary>
        ///
        /// <remarks>   Nsl, 25.01.2013. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <typeparam name="U">    Generic type parameter. </typeparam>
        /// <typeparam name="V">    Generic type parameter. </typeparam>
        /// <typeparam name="W">    Type of the w. </typeparam>
        /// <param name="sql">              The sql. </param>
        /// <param name="numberOfResults">  Number of results. </param>
        /// <param name="parameters">       Options for controlling the operation. </param>
        /// <param name="commandType">      (optional) type of the command. </param>
        ///
        /// <returns>   The multi query result set&lt; t,u,v,w&gt; </returns>

        private MultiQueryResultSet<T, U, V, W> ReadMultiQueryResultSet<T, U, V, W>(string sql, int numberOfResults, object parameters = null, CommandType commandType = CommandType.Text)
        {
            MultiQueryResultSet<T, U, V, W> multiResultSet = new MultiQueryResultSet<T, U, V, W>();

            using (SqlCommand command = new SqlCommand(sql, GetConnection(), _transaction))
            {
                command.CommandType = commandType;

                if (parameters != null)
                {
                    AddParameters(command, parameters);
                }

                using (SqlDataReader reader = command.ExecuteReader())
                {

                    multiResultSet.FirstResult = ReadMultiQueryRecord<T>(reader);

                    if (numberOfResults >= 2)
                    {
                        reader.NextResult();

                        multiResultSet.SecondResult = ReadMultiQueryRecord<U>(reader);

                        if (numberOfResults >= 3)
                        {
                            reader.NextResult();

                            multiResultSet.ThirdResult = ReadMultiQueryRecord<V>(reader);

                            if (numberOfResults >= 4)
                            {
                                reader.NextResult();

                                multiResultSet.FourthResult = ReadMultiQueryRecord<W>(reader);
                            }
                        }
                    }
                }
            }

            return multiResultSet;
        }

        /// <summary>   Enumerates read multi query record in this collection. </summary>
        ///
        /// <remarks>   Nsl, 25.01.2013. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="reader">   The reader. </param>
        ///
        /// <returns>
        ///     An enumerator that allows foreach to be used to process read multi query record&lt; t&gt;
        ///     in this collection.
        /// </returns>

        private IEnumerable<T> ReadMultiQueryRecord<T>(SqlDataReader reader)
        {
            List<T> result = new List<T>();

            if (typeof(T).IsValueType == false)
            {
                Func<SqlDataReader, T> map = OrmCache.GetMap<T>(reader);

                while (reader.Read())
                {
                    T obj = map.Invoke(reader);

                    result.Add(obj);
                }
            }
            else
            {
                while (reader.Read())
                {
                    T obj = (T)reader.GetValue(0);

                    result.Add(obj);
                }
            }

            return result;
        }



        /// <summary>   Clears the cache. </summary>
        ///
        /// <remarks>   Nsl, 08.01.2013. </remarks>

        public void ClearCache()
        {
            CrudCache.ClearCache();
            OrmCache.ClearCache();
        }

        /// <summary>   Adds the parameters to 'parameters'. </summary>
        ///
        /// <remarks>   Nsl, 08.01.2013. </remarks>
        ///
        /// <param name="command">      The command. </param>
        /// <param name="parameters">   Options for controlling the operation. </param>

        private void AddParameters(SqlCommand command, object parameters)
        {
            if (parameters != null)
            {
                CommandParameters commandParameters = parameters as CommandParameters;

                if (commandParameters == null)
                {
                    foreach (PropertyInfo propertyInfo in parameters.GetType().GetProperties())
                    {
                        SqlParameter param = new SqlParameter("@" + propertyInfo.Name, propertyInfo.GetValue(parameters, null) ?? DBNull.Value);

                        command.Parameters.Add(param);
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, SqlParameter> parameter in commandParameters.Parameters)
                    {
                        command.Parameters.Add(parameter.Value);
                    }
                }
            }
        }
    }
}
