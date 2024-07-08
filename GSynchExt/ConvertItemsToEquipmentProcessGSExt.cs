using PX.Data;
using PX.Objects.AR;
using PX.Objects.IN;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static PX.Data.BQL.BqlPlaceholder;

namespace PX.Objects.FS
{
    public static class SharedFunctionsGSExt
    {

        public static T Create<T>(this T @this) where T : class, new()
        {
            return Utility<T>.Create();
        }
       
    }
    public static class Utility<T> where T : class, new()
    {
        static Utility()
        {
            Create = Expression.Lambda<Func<T>>(Expression.New(typeof(T).GetConstructor(Type.EmptyTypes))).Compile();
        }
        public static Func<T> Create { get; private set; }
    }
}
