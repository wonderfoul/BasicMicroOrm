using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace BasicMicroOrm
{
    /// <summary>   Command parameters. </summary>
    ///
    /// <remarks>   Nsl, 08.01.2013. </remarks>

    public class CommandParameters
    {
        private Dictionary<string, SqlParameter> _parameters = new Dictionary<string, SqlParameter>();

        /// <summary>   Gets options for controlling the operation. </summary>
        ///
        /// <value> The parameters. </value>

        public Dictionary<string, SqlParameter> Parameters
        {
            get
            {
                return _parameters;
            }
        }


        /// <summary>   Adds name.. </summary>
        ///
        /// <remarks>   Nsl, 16.01.2013. </remarks>
        ///
        /// <param name="parameter">    The SqlParameter to add. </param>

        public void Add(SqlParameter parameter)
        {
            _parameters.Add(parameter.ParameterName, parameter);
        }

        /// <summary>   Adds name.. </summary>
        ///
        /// <remarks>   Nsl, 16.01.2013. </remarks>
        ///
        /// <param name="name">     The name. </param>
        /// <param name="value">    The value. </param>

        public void Add(string name, object value)
        {
            SqlParameter parameter = new SqlParameter("@" + name, value ?? DBNull.Value);

            _parameters.Add(name, parameter);
        }

        /// <summary>   Adds name.. </summary>
        ///
        /// <remarks>   Nsl, 31.08.2013. </remarks>
        ///
        /// <param name="name">     The name. </param>
        /// <param name="value">    The value. </param>

        public void Add(string name, DateTime value)
        {
            SqlParameter parameter;

            if (value != DateTime.MinValue)
            {
                parameter = new SqlParameter("@" + name, value);
            }
            else
            {
                parameter = new SqlParameter("@" + name, DBNull.Value);
            }


            _parameters.Add(name, parameter);
        }

        /// <summary>   Adds name.. </summary>
        ///
        /// <remarks>   Nsl, 17.01.2013. </remarks>
        ///
        /// <param name="name">     The name. </param>
        /// <param name="value">    The value. </param>
        /// <param name="type">     The type. </param>

        public void Add(string name, object value, SqlDbType type)
        {
            SqlParameter parameter = new SqlParameter("@" + name, value ?? DBNull.Value);

            parameter.ParameterName = "@" + name;
            parameter.SqlDbType = type;

            _parameters.Add(name, parameter);
        }

        /// <summary>   Adds name.. </summary>
        ///
        /// <remarks>   Nsl, 05.02.2013. </remarks>
        ///
        /// <param name="name">     The name. </param>
        /// <param name="value">    The value. </param>
        /// <param name="type">     The type. </param>

        public void AddOut(string name, object value, SqlDbType type)
        {
            SqlParameter parameter = new SqlParameter("@" + name, value ?? DBNull.Value);

            parameter.SqlDbType = type;
            parameter.Direction = ParameterDirection.Output;

            _parameters.Add(name, parameter);
        }

        /// <summary>   Adds name.. </summary>
        ///
        /// <remarks>   Nsl, 05.02.2013. </remarks>
        ///
        /// <param name="name"> The name. </param>
        /// <param name="type"> The type. </param>

        public void AddOut(string name, SqlDbType type)
        {
            SqlParameter parameter = new SqlParameter("@" + name, type);

            parameter.Direction = ParameterDirection.Output;

            _parameters.Add(name, parameter);
        }

        /// <summary>   Adds name.. </summary>
        ///
        /// <remarks>   Nsl, 05.02.2013. </remarks>
        ///
        /// <param name="name"> The name. </param>
        /// <param name="type"> The type. </param>
        /// <param name="size"> The size. </param>
        ///

        public void AddOut(string name, SqlDbType type, int size)
        {
            SqlParameter parameter = new SqlParameter("@" + name, type);

            parameter.Direction = ParameterDirection.Output;
            parameter.Size = size;

            _parameters.Add(name, parameter);
        }


        /// <summary>   Gets. </summary>
        ///
        /// <remarks>   Nsl, 08.01.2013. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="parameterName">    Name of the parameter. </param>
        ///
        /// <returns>   . </returns>

        public T Get<T>(string parameterName)
        {
            return (T)_parameters[parameterName].Value;
        }
    }
}
