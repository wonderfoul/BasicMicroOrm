using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicMicroOrm
{
    /// <summary>   Attribute to identify if the column is a primary key. </summary>
    ///
    /// <remarks>   Nsl, 08.01.2013. </remarks>

    public class PrimaryKey : Attribute
    {
    }

    /// <summary>   Attribute to identify if the column is identity. </summary>
    ///
    /// <remarks>   Nsl, 08.01.2013. </remarks>

    public class Identity : Attribute
    {
    }

    /// <summary>   Attibute for the mappers to Ignore mapping. </summary>
    ///
    /// <remarks>   Nsl, 26.02.2013. </remarks>

    public class IgnoreMapping : Attribute
    {
    }
}
