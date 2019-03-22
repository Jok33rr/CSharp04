using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NikitchenkoCSharp04
{
    internal static class Years
    {
        public static int YearsOld(this DateTime todayDateTime, DateTime userDateTime)
        {
            return (todayDateTime.Year - userDateTime.Year) - (todayDateTime.DayOfYear >= userDateTime.DayOfYear ? 0 : 1);
        }
    }
}
