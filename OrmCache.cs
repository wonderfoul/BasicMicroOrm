using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Reflection.Emit;
using System.Data;
using System.Collections.Concurrent;

namespace BasicMicroOrm
{
    /// <summary>   Orm cache. </summary>
    ///
    /// <remarks>   Nsl, 08.01.2013. </remarks>

    public class OrmCache
    {
        private static readonly MethodInfo _getValue = typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo _isDBNull = typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });
        private static ConcurrentDictionary<Type, Delegate> _maps = new ConcurrentDictionary<Type, Delegate>();

        /// <summary>   Gets a map. </summary>
        ///
        /// <remarks>   Nsl, 08.01.2013. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="dataRecord">   The data record. </param>
        ///
        /// <returns>   The map&lt; t&gt; </returns>

        public static Func<SqlDataReader, T> GetMap<T>(IDataRecord dataRecord)
        {
            Delegate cachedDelegate;

            if (_maps.TryGetValue(typeof(T), out cachedDelegate) == true)
            {
                return (Func<SqlDataReader, T>)cachedDelegate;
            }

            DynamicMethod method = new DynamicMethod("DynamicCreate", typeof(T), new Type[] { typeof(IDataRecord) }, typeof(T), true);
            ILGenerator generator = method.GetILGenerator();

            LocalBuilder result = generator.DeclareLocal(typeof(T));
            generator.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);

            for (int i = 0; i < dataRecord.FieldCount; i++)
            {
                PropertyInfo propertyInfo = typeof(T).GetProperty(dataRecord.GetName(i));

                if (propertyInfo.GetCustomAttributes(typeof(IgnoreMapping), false).Length == 0)
                {
                    Label endIfLabel = generator.DefineLabel();

                    if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                    {
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldc_I4, i);
                        generator.Emit(OpCodes.Callvirt, _isDBNull);
                        generator.Emit(OpCodes.Brtrue, endIfLabel);

                        generator.Emit(OpCodes.Ldloc, result);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldc_I4, i);
                        generator.Emit(OpCodes.Callvirt, _getValue);

                        if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null)
                        {
                            generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                        }
                        else
                        {
                            generator.Emit(OpCodes.Unbox_Any, dataRecord.GetFieldType(i));
                        }


                        generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

                        generator.MarkLabel(endIfLabel);
                    }
                }
            }

            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);

            cachedDelegate = method.CreateDelegate(typeof(Func<SqlDataReader, T>));

            _maps[typeof(T)] = cachedDelegate;

            return (Func<SqlDataReader, T>)cachedDelegate;
        }

        /// <summary>   Clears the cache. </summary>
        ///
        /// <remarks>   Nsl, 08.01.2013. </remarks>

        public static void ClearCache()
        {
            _maps.Clear();
        }
    }
}
