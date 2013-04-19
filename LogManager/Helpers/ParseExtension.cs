using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace LogManager.Helpers
{
    public static class ParseExtension
    {
        /// <summary>
        /// Parse any string to a base type utulizing the types built in TryParse Method
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="thingToParse">The string you want to parse</param>
        /// <returns></returns>
        public static T Parse<T>(this string thingToParse)
        {
            return thingToParse.Parse<T>(default(T));
        }

        /// <summary>
        /// Parse any string to a base type utulizing the types built in TryParse Method
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="thingToParse">The string you want to parse</param>
        /// <param name="defaultValue">The default value to return if parsing fails</param>
        /// <returns></returns>
        public static T Parse<T>(this string thingToParse, T defaultValue)
        {
            var retType = typeof(T);

            if (KnownParsers.ContainsKey(retType) != true)
            {
                KnownParsers[retType] = GetTryParse(retType);
            }

            MethodInfo tParse = KnownParsers[retType];

            if (tParse != null)
            {
                var parameters = new object[] { thingToParse, null };
                var success = (bool)tParse.Invoke(null, parameters);
                if (success)
                {
                    return (T)parameters[1];
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Parse any string into something else by providing a custom parsing function
        /// </summary>
        /// <typeparam name="T">Complex OutPut Type expected back</typeparam>
        /// <param name="thingToParse">The string you want to parse</param>
        /// <param name="parser">The func that takes your sting and turns it into your output type</param>
        /// <returns></returns>
        public static T Parse<T>(this string thingToParse, Func<string, T> parser)
        {
            return parser.Invoke(thingToParse);
        }

        /// <summary>
        /// An in memory collection of nuts we've already cracked
        /// </summary>
        private static Dictionary<Type, MethodInfo> _knownParsers;
        public static Dictionary<Type, MethodInfo> KnownParsers
        {
            get { return _knownParsers ?? (_knownParsers = GetNullableParsers()); }
            set { _knownParsers = value; }
        }

        /// <summary>
        /// A collection of transformations to map nullable types to there sisters
        /// </summary>
        /// <returns></returns>
        private static Dictionary<Type, MethodInfo> GetNullableParsers()
        {
            Dictionary<Type, MethodInfo> nullableParsers = new Dictionary<Type, MethodInfo>();
            nullableParsers[typeof(int?)] = GetTryParse(typeof(int));
            nullableParsers[typeof(Int32?)] = GetTryParse(typeof(Int32));
            nullableParsers[typeof(Int64?)] = GetTryParse(typeof(Int64));
            nullableParsers[typeof(decimal?)] = GetTryParse(typeof(decimal));
            nullableParsers[typeof(Decimal?)] = GetTryParse(typeof(Decimal));
            nullableParsers[typeof(double?)] = GetTryParse(typeof(double));
            nullableParsers[typeof(Double?)] = GetTryParse(typeof(Double));
            nullableParsers[typeof(float?)] = GetTryParse(typeof(float));
            nullableParsers[typeof(byte?)] = GetTryParse(typeof(byte));
            nullableParsers[typeof(Byte?)] = GetTryParse(typeof(Byte));
            nullableParsers[typeof(DateTime?)] = GetTryParse(typeof(DateTime));

            return nullableParsers;
        }

        /// <summary>
        /// Looks Up The Tryparse method of the provided type
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public static MethodInfo GetTryParse(Type sourceType)
        {
            return sourceType.GetMethod("TryParse",
                BindingFlags.Public | BindingFlags.Static, null,
                new[] { typeof(string), sourceType.MakeByRefType() }, null);
        }
    }
}