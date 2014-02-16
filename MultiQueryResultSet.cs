using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicMicroOrm
{
    /// <summary>   Multi query result set. </summary>
    ///
    /// <remarks>   Nsl, 25.01.2013. </remarks>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>
    /// <typeparam name="U">    Generic type parameter. </typeparam>
    /// <typeparam name="V">    Generic type parameter. </typeparam>
    /// <typeparam name="W">    Type of the w. </typeparam>

    public class MultiQueryResultSet<T, U, V, W>
    {

        /// <summary>   Gets or sets the first. </summary>
        ///
        /// <value> The first. </value>

        public IEnumerable<T> FirstResult { get; set; }

        /// <summary>   Gets or sets the second result. </summary>
        ///
        /// <value> The second result. </value>

        public IEnumerable<U> SecondResult { get; set; }

        /// <summary>   Gets or sets the third result. </summary>
        ///
        /// <value> The third result. </value>

        public IEnumerable<V> ThirdResult { get; set; }

        /// <summary>   Gets or sets the fourth result. </summary>
        ///
        /// <value> The fourth result. </value>

        public IEnumerable<W> FourthResult { get; set; }



    }
}
