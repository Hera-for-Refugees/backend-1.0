using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Mvc;


public static class GenericExtension
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            Random rnd = new Random();
            while (n > 1)
            {
                int k = (rnd.Next(0, n) % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static IQueryable<T> Search<T>(this IQueryable<T> source, string filterText)
        {
            if (string.IsNullOrEmpty(filterText))
            {
                return source;
            }

            string[] properties = typeof(T).GetProperties().Where(p => p.PropertyType == typeof(string)).Select(p => p.Name).ToArray();
            properties = RemoveProp(properties, new List<string> { "Latitude", "Longitude", "Photo" });
            ParameterExpression parameter = Expression.Parameter(typeof(T));
            Expression[] propertyExpressions = properties.Select(
                x => !string.IsNullOrEmpty(x) ? GetDeepPropertyExpression(parameter, x) : null).ToArray();

            Expression like = propertyExpressions.Select(expression => Expression.Call(expression, typeof(string).GetMethod("ToLower", Type.EmptyTypes))).Select(toLower => Expression.Call(toLower, typeof(string).GetMethod("Contains"), Expression.Constant(filterText.ToLower()))).Aggregate<MethodCallExpression, Expression>(null, (current, ex) => BuildOrExpression(current, ex));

            return source.Where(Expression.Lambda<Func<T, bool>>(like, parameter));
        }

        private static Expression BuildOrExpression(Expression existingExpression, Expression expressionToAdd)
        {
            if (existingExpression == null)
            {
                return expressionToAdd;
            }

            //Build 'OR' expression for each property
            return Expression.OrElse(existingExpression, expressionToAdd);
        }

        private static Expression GetDeepPropertyExpression(Expression initialInstance, string property)
        {
            Expression result = null;
            foreach (string propertyName in property.Split('.'))
            {
                Expression instance = result ?? initialInstance;
                result = Expression.Property(instance, propertyName);
            }
            return result;
        }

        static string[] RemoveProp(string[] properties, List<string> propList)
        {
            var tempList = properties.ToList();
            foreach (var item in propList)
            {
                var index = tempList.IndexOf(item);
                if (index > -1)
                {
                    tempList.RemoveAt(index);
                }
            }
            return tempList.ToArray();
        }

        public static List<SelectListItem> ConvertToSelectList<T>(this List<T> source, string displayMember, string valueMember)
        {
            var result = new List<SelectListItem>();
            if (source != null && source.Count > 0)
            {
                foreach (var item in source)
                {
                    result.Add(new SelectListItem() { Text = GetPropertyValue(displayMember, item).ToString(), Value = GetPropertyValue(valueMember, item).ToString() });
                }
            }
            return result;
        }
        static object GetPropertyValue(string property, Object entity)
        {
            var objInfo = entity.GetType().GetProperty(property);
            if (objInfo != null)
                return objInfo.GetValue(entity, null);
            else
                return "";
        }

        public static string GetDateText(this DateTime firstDate, DateTime lastDate)
        {
            const double ApproxDaysPerMonth = 30.4375;
            const double ApproxDaysPerYear = 365.25;
            int iDays = (lastDate - firstDate).Days;
            int iYear = (int)(iDays / ApproxDaysPerYear);
            iDays -= (int)(iYear * ApproxDaysPerYear);
            int iMonths = (int)(iDays / ApproxDaysPerMonth);
            iDays -= (int)(iMonths * ApproxDaysPerMonth);
            if (iYear > 0)
                return string.Format("{0} yıl, {1} ay, {2} gün", iYear, iMonths, iDays);
            else if (iMonths > 0)
                return string.Format("{0} ay, {1} gün", iMonths, iDays);
            else
                return string.Format("{0} gün", iDays);
        }


        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> list)
        {
            Random r = new Random();
            return list.OrderBy(x => (r.Next()));
        }
        /// <summary>
        /// Generic list için klonlama yapar
        /// </summary>
        /// <returns></returns>
        public static T Clone<T>(this object item)
        {
            if (item != null)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                formatter.Serialize(stream, item);
                stream.Seek(0, SeekOrigin.Begin);
                T result = (T)formatter.Deserialize(stream);
                stream.Close();
                return result;
            }
            else
                return default(T);
        }

        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> @this, Func<T, TKey> keySelector)
        {
            return @this.GroupBy(keySelector).Select(grps => grps).Select(e => e.First());
        }
    }

