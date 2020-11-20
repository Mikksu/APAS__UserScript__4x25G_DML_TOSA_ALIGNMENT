using System;
using System.Collections.Generic;
using System.Linq;

namespace UserScript
{
    public class DataAnalysis
    {
        public enum SlopeTrendEnum
        {
            Positive,
            Nagetive,
            Ripple
        }

        public static void CheckSlope(double[] Data, out SlopeTrendEnum Trend)
        {
            var slope = new List<double>();
            var dataArray = Data.ToList();

            if (Data.Length < 2)
                throw new ArgumentException("there are no enough data.", nameof(Data));

            for (var i = 0; i < dataArray.Count - 1; i++) slope.Add(dataArray[i + 1] - dataArray[i]);

            slope.Sort();

            if (slope.First() >= 0)
                Trend = SlopeTrendEnum.Positive;
            else if (slope.Last() < 0)
                Trend = SlopeTrendEnum.Nagetive;
            else
                Trend = SlopeTrendEnum.Ripple;
        }
    }
}