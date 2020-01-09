using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

#pragma warning disable 1591 // no warning for missing comments

namespace IntoTheCode.Basic.Util
{
    /// <summary>Utility methods.</summary>
    public static class DotNetUtil
    {
        #region odd functions

        /// <summary>Get username from full AD username.</summary>
        /// <param name="fullUsername">Full AD-Username.</param>
        /// <returns>The username.</returns>
        public static string GetUsername(string fullUsername)
        {
            int pos = fullUsername.IndexOf('\\') + 1;
            string username = fullUsername.Substring(pos).ToLower();
            return username;
        }

        /// <summary>Generate a random string of characters.</summary>
        /// <param name="length">Length of wanted string.</param>
        /// <param name="chars">Allowed chars.</param>
        /// <returns>A new string of length.</returns>
        public static string GetRandomString(int length, string chars = null)
        {
            // Default characters
            const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            if (string.IsNullOrEmpty(chars)) chars = Chars;
            var random = new Random();
            string id = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return id;
        }

        /// <summary>Escapes sql string to sql script streng. Empty string if null.</summary>
        /// <param name="sql">Sql string.</param>
        /// <returns>Sql script string.</returns>
        public static string EscapeSql(string sql)
        {
            if (sql == null) return string.Empty;

            const string Newline = "' + CHAR(13)+CHAR(10) + '";
            const string Newlinetag = "<nl>";
            sql = sql.Replace("'", "''");
            sql = sql.Replace("\r\n", Newlinetag).Replace("\n", Newlinetag).Replace("\r", Newlinetag);
            sql = sql.Replace(Newlinetag, Newline);
            return sql;
        }

        /// <summary>Convert object to xml-string without xml declaration or namespaces.</summary>
        /// <param name="obj">The object.</param>
        /// <returns>Xml string.</returns>
        public static string ToPlainXmlString<T>(T obj)
        {
            var xml = "";
            XmlSerializer rqSerializer = new XmlSerializer(typeof(T));
            var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww, settings))
                {
                    rqSerializer.Serialize(writer, obj, emptyNamespaces);
                    xml = sww.ToString(); // Your XML
                }
            }
            return xml;
        }

        /// <summary>Newline for output.</summary>
        public static string NL(this string str)
        {
            return (str == null ? String.Empty : str) + "\r\n";
        }

        public static string Res(Expression<Func<string>> resourceExpression, params object[] parm)
        {
            string resId, resString;
            resId = GetMemberName(resourceExpression);
            resString = resourceExpression.Compile().Invoke();

            return resId + ": " + string.Format(resString, parm);
        }

        #endregion odd functions

        #region GetMemberName

        /// <summary>Get the name of a class member.
        /// str = DotNetUtil.GetMemberName(() => ((AClass)null).PropString);
        /// str = DotNetUtil.GetMemberName(() => a.PropString);
        /// str = DotNetUtil.GetMemberName(() => AClass.StaticPropObject);
        /// str = DotNetUtil.GetMemberName(() => AClass.StaticPropInt); 
        /// </summary>
        public static string GetMemberName(Expression<Func<object>> expression)
        {
            if (expression == null)
                throw new ArgumentException("The expression cannot be null.");

            return GetMemberName(expression.Body);
        }

        /// <summary>
        /// Use this when a string must be returned (Resources).
        /// str = DotNetUtil.GetMemberName(o => AClass.StaticPropString);
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetMemberName(Expression<Func<string>> expression)
        {
            if (expression == null)
                throw new ArgumentException("The expression cannot be null.");

            return GetMemberName(expression.Body);
        }

        ////public static string GetMemberName(Expression<Func<ValueType>> expression)

        private static string GetMemberName(Expression expression)
        {
            if (expression == null)
                throw new ArgumentException("The expression cannot be null.");

            if (expression is MemberExpression)
            {
                // Reference type property or field
                var memberExpression =
                    (MemberExpression)expression;
                return memberExpression.Member.Name;
            }

            if (expression is MethodCallExpression)
            {
                // Reference type method
                var methodCallExpression =
                    (MethodCallExpression)expression;
                return methodCallExpression.Method.Name;
            }

            if (expression is UnaryExpression)
            {
                // Property, field of method returning value type
                var unaryExpression = (UnaryExpression)expression;
                if (unaryExpression.Operand is MethodCallExpression)
                {
                    var methodExpression =
                        (MethodCallExpression)unaryExpression.Operand;
                    return methodExpression.Method.Name;
                }

                return ((MemberExpression)unaryExpression.Operand).Member.Name;
            }

            throw new ArgumentException("Invalid expression");
        }

        #endregion GetMemberName

        #region reflection


        /// <summary>Not tested! Get static property value.</summary>

        internal static T StaticGet<T>(Type theClass, string propName, bool throwOnError = true)
        {
            Func<string, T> returnErr = s => { if (throwOnError) throw new Exception(s); else return default(T); };
            //string compName = string.Empty;

            PropertyInfo[] infos = theClass.GetProperties();

            PropertyInfo compNameProp = infos.FirstOrDefault(i => i.Name == propName);

            if (compNameProp == null) return returnErr(string.Format("The property '{0}' does not exists", propName));

            // Func to find static, public, properties
            if (compNameProp.PropertyType != typeof(T)) return returnErr(string.Format("The property '{0}' does not have the right type", propName));
            MethodInfo getter = compNameProp.GetGetMethod();
            if (getter == null) return returnErr(string.Format("The property '{0}' getter isn't public", propName));
            if (!getter.IsStatic) return returnErr(string.Format("The property '{0}' getter isn't static", propName));

            // BindingFlags.FlattenHierarchy ??


            //    { get { return CompName_ + (IsTrial ? ".Trial" : string.Empty); } }
            return (T)compNameProp.GetValue(null, null);
        }

#endregion reflection
    }
}
