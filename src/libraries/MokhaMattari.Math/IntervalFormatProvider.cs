using System;

namespace MokhaMattari.Math
{
    class IntervalFormatProvider<T> : IFormatProvider, ICustomFormatter
        where T : IComparable<T>
    {
        object IFormatProvider.GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// FIXME: 気力がない
        /// </summary>
        /// <remarks>
        /// 精度指定子は1（小数点以下1桁）固定です。
        /// </remarks>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            Interval<T> interval = arg as Interval<T>;

            if (interval != null)
            {
                string leftSuffix = (interval.IntervalType & IntervalType.LeftOpen) == IntervalType.LeftOpen ? "超" : "以上";
                string rightSuffix = (interval.IntervalType & IntervalType.RightOpen) == IntervalType.RightOpen ? "未満" : "以下";

                switch (format)
                {
                    case "a":
                    case "A":
                        if (interval.EndPointA == null)
                            return string.Empty;

                        return string.Format(formatProvider, "{0:F1}{1}", interval.EndPointA, leftSuffix);

                    case "b":
                    case "B":
                        if (interval.EndPointB == null)
                            return string.Empty;

                        return string.Format(formatProvider, "{0:F1}{1}", interval.EndPointB, rightSuffix);

                    case "p":
                    case "P":
                        if (interval.EndPointA == null)
                            return string.Empty;

                        return string.Format(formatProvider, "{0:P1}％{1}", interval.EndPointA, leftSuffix);

                    case "q":
                    case "Q":
                        if (interval.EndPointB == null)
                            return string.Empty;

                        return string.Format(formatProvider, "{0:P1}％{1}", interval.EndPointB, rightSuffix);

                    default:
                        // 書式指定子が指定されていない場合は、区間文字列表現（例：[a, b)）を返しておく
                        if (string.IsNullOrEmpty(format))
                        {
                            string leftParenthesis = (interval.IntervalType & IntervalType.LeftOpen) == IntervalType.LeftOpen ? "(" : "[";
                            string rightParenthesis = (interval.IntervalType & IntervalType.RightOpen) == IntervalType.RightOpen ? ")" : "]";

                            return string.Format(formatProvider, "{2}{0}, {1}{3}", interval.EndPointA, interval.EndPointB, leftParenthesis, rightParenthesis);
                        }
                        else
                        {
                            throw new FormatException(string.Format("'{0}' は不正な書式指定子です。A, B, P, Q のいずれかを使用してください。", format));
                        }
                }
            }

            // 引数が Interval 型でない場合
            if (arg is IFormattable)
            {
                return ((IFormattable)arg).ToString(format, formatProvider);
            }

            // 引数が IFormattable を実装していない場合は、Object.ToString メソッドで文字列化する
            return arg.ToString();
        }
    }
}