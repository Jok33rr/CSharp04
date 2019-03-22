using System;
using System.Collections.Generic;
using System.Linq;

namespace NikitchenkoCSharp04
{
    public static class SorterByParam
    {
        public static readonly string[] FilterSortParams =
                 Array.ConvertAll(typeof(User).GetProperties(), (property) => property.Name);

        public static List<User> SortByParam(this List<User> users, string property, bool ascending)
        {
            return Array.IndexOf(FilterSortParams, property) >= 0
                ?
                    (ascending
                        ?
                        (from p in users orderby p.GetType().GetProperty(property)?.GetValue(p, null) ascending select p).ToList()
                        :
                        (from p in users orderby p.GetType().GetProperty(property)?.GetValue(p, null) descending select p).ToList()
                    )
                : users;
        }

        public static List<User> FilterByParam(this List<User> users, string property, string query)
        {
            if (Array.IndexOf(FilterSortParams, property) < 0) return new List<User>();

            query = query.ToLower();
            return (from p in users
                    where (p.GetType().GetProperty(property)?.GetValue(p, null)).ToString().ToLower().Contains(query) select p).ToList();
        }
    }
}
