using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MokhaMattari.Math
{
    /// <summary>
    /// 開区間／閉区間を表現するジェネリッククラスです。
    /// 以下のいずれかの区間を表現できます。
    /// <list>
    ///  <item>閉区間: [a, b]</item>
    ///  <item>半開区間: (a, b]</item>
    ///  <item>半開区間: [a, b)</item>
    ///  <item>開区間: (a, b)</item>
    /// </list>
    /// </summary>
    public class Interval<T> : IFormattable
        where T : IComparable<T>
    {
        /// <summary>
        /// 端点a の値を設定または取得します。
        /// </summary>
        /// <value>端点a の値</value>
        public T EndPointA { get; private set; }

        /// <summary>
        /// 端点b の値を設定または取得します。
        /// </summary>
        /// <value>端点b の値</value>
        public T EndPointB { get; private set; }

        /// <summary>
        /// 区間種別を設定または取得します。
        /// </summary>
        /// <value>区間種別</value>
        public IntervalType IntervalType { get; private set; }

        /// <summary>
        /// 区間定義のインスタンスを作成します。
        /// </summary>
        /// <param name="endpointA">端点a の値</param>
        /// <param name="endpointB">端点b の値</param>
        /// <param name="aIsOpen">端点a 側が開区間（a < x）なら true, 閉区間（a <= x）なら false を指定する</param>
        /// <param name="bIsOpen">端点b 側が開区間（x < b）なら true, 閉区間（x <= b）なら false を指定する</param>
        public Interval(T endpointA, T endpointB, bool aIsOpen, bool bIsOpen)
        {
            this.EndPointA = endpointA;
            this.EndPointB = endpointB;

            this.IntervalType = (bIsOpen ? IntervalType.RightOpen : 0) | (aIsOpen ? IntervalType.LeftOpen : 0);
        }

        /// <summary>
        /// <paramref name="val"> が区間内に存在するか判定します。
        /// </summary>
        /// <returns>区間内なら <see langword="true"/>, そうでなければ <see langword="false"/>. </returns>
        public bool Contains(T val)
        {
            bool isRightToEndpointA = false;
            bool isLeftToEndpointB = false;

            // T a = this.EndPointA ?? ReadStaticField("MinValue");
            // T b = this.EndPointB ?? ReadStaticField("MaxValue");

            // 端点a 側開区間なら (a < x)、閉区間なら (a <= x) で判定
            if ((this.IntervalType & IntervalType.LeftOpen) == IntervalType.LeftOpen)
            {
                isRightToEndpointA = this.EndPointA.CompareTo(val) < 0;
            }
            else
            {
                isRightToEndpointA = this.EndPointA.CompareTo(val) <= 0;
            }

            // 端点b 側開区間なら (x < b)、閉区間なら (x <= b) で判定
            if ((this.IntervalType & IntervalType.RightOpen) == IntervalType.RightOpen)
            {
                isLeftToEndpointB = this.EndPointB.CompareTo(val) > 0;
            }
            else
            {
                isLeftToEndpointB = this.EndPointB.CompareTo(val) >= 0;
            }

            return isRightToEndpointA && isLeftToEndpointB;
        }

        /// <summary>
        /// 既定のフォーマットを用いて、この区間の自然言語における文字列表現を取得します。
        /// 既定のフォーマットは "{0:A} {0:B}" です。
        /// "[0.1, 0.2)" という区間に対しては、 "0.1以上 0.2未満" という文字列を返却します。
        /// </summary>
        /// <returns>区間の自然言語における文字列表現</returns>
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        /// この区間の自然言語における文字列表現を取得します。
        /// </summary>
        /// <returns>区間の自然言語における文字列表現</returns>
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        /// <summary>
        /// <p>
        ///  この区間の自然言語における文字列表現を取得します。
        /// </p>
        /// <p>
        ///  <paramref name="format"/> が空文字または <see langword="null"/> の場合、既定のフォーマットが使用されます。
        ///  既定のフォーマットは "{0:A} {0:B}" です。フォーマットの各プレースホルダにはこの区間のインスタンスが引数として与えられます。
        /// </p>
        /// <p>
        ///  <paramref name="formatProvider"/> が <see langword="null"/> の場合、<see cref="IntervalFormatProvider{T}"/> が使用されます。
        ///  このフォーマットプロバイダは、書式指定子に従って、「端点A, B のどちらか」を、「標準形式またはパーセント書式指定で」文字列化します。
        ///  使用できる書式指定子については、<see cref="IntervalFormatProvider{T}"/> を参照してください。
        ///  なお、このフォーマットプロバイダは、パーセント記号（％）と数値の間に空白を挿入しません。
        ///  細かい動作を変更したい場合は、 <see cref="IntervalFormatProvider{T}"/> 側を修正してください。
        /// </p>
        /// </summary>
        /// <returns>区間の自然言語における文字列表現</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "{0:A} {0:B}";
            }

            if (formatProvider == null)
            {
                formatProvider = new IntervalFormatProvider<T>();
            }

            return string.Format(formatProvider, format, this);
        }

        /// <summary>
        /// 区間表現文字列（例： "(1, 10]" ）をパースして新しい <see cref="Interval"/> のインスタンスを生成します。
        /// 端点A の文字列（例："(1, 10]" ）なら、"1"）が空文字である場合、その型の最小値として扱われます。
        /// 端点B の文字列（例："(1, 10]" ）なら、"10"）が空文字である場合、その型の最大値として扱われます。
        /// 最小値・最大値は、<typeparamref name="T"> 型の static な MinValue・MaxValue プロパティに対応します。
        /// このとき、<typeparamref name="T"> 型にそれらのプロパティが存在しないと <see cref="NotSupportedException"/> がスローされます。
        /// </summary>
        /// <param name="s">区間表現の文字列</param>
        /// <typeparam name="T">端点の値の型パラメータ</typeparam>
        /// <returns>パースされた <see cref="Interval"/> インスタンス</returns>
        /// <exception cref="InvalidOperationException"><typeparamref name="T"/> に対応する <see cref="TypeConverter"/> が存在しない場合</exception>
        /// <exception cref="FormatException"><paramref name="c"> が、区間表現文字列ではない場合</exception>
        /// <exception cref="NotSupportedException">端点の文字列が、型 <typeparamref="T"/> に変換できない場合</exception>
        /// <exception cref="NotSupportedException">端点の文字列が空文字の場合で、かつ型 <typeparamref="T"/> に MinValue または MaxValue プロパティが存在しない場合</exception>
        public static Interval<T> Parse(string s)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter == null)
            {
                throw new InvalidOperationException($"型 {typeof(T)} には TypeConverter が定義されていないため、文字列から変換出来ません。");
            }

            const string pattern = @"(\[|\()([^,]*?),([^,]*?)(\]|\))";
            Match match = Regex.Match(s, pattern);
            if (!match.Success || match.Groups.Count != 5)
            {
                throw new FormatException($"文字列: {s} は、 {nameof(Interval<T>)} 型の文字列表現になっていません。区間表現（例: [0.0, 1.0)）のような文字列である必要があります。");
            }

            string leftStr = match.Groups[2].Value.Trim();
            string rightStr = match.Groups[3].Value.Trim();

            bool leftOpen = match.Groups[1].Value == "(";
            bool rightOpen = match.Groups[4].Value == ")";

            // 空文字は、左側なら MinValue、右側なら MaxValue として扱う。
            T leftValue = string.IsNullOrEmpty(leftStr) ? ReadStaticField("MinValue") : (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, leftStr);
            T rightValue = string.IsNullOrEmpty(rightStr) ? ReadStaticField("MaxValue") : (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, rightStr);

            return new Interval<T>(leftValue, rightValue, leftOpen, rightOpen);
        }

        private static T ReadStaticField(string name)
        {
            FieldInfo field = typeof(T).GetField(name, BindingFlags.Public | BindingFlags.Static);

            if (field == null)
            {
                throw new NotSupportedException(
                    $"Typeparameter's type of Interval<{typeof(T).Name}> (equals to 'typeof({typeof(T).Name})') does not support static '{name}' property.");
            }

            return (T)field.GetValue(null);
        }
    }

    [Flags]
    public enum IntervalType
    {
        // 閉区間: [a, b]
        Closed = 0x00,
        // 半開区間1: (a, b]
        LeftOpen = 0x01,
        // 半開区間2: [a, b)
        RightOpen = 0x02,
        // 開区間: (a, b)
        Open = LeftOpen | RightOpen,
    }

    public static class IntervalFactory<T> where T : IComparable<T>
    {
        /// <summary>
        /// 閉区間: [a, b] のインスタンスを作成します。
        /// </summary>
        /// <param name="endpointA">端点a</param>
        /// <param name="endpointB">端点b</param>
        /// <returns>区間</returns>
        public static Interval<T> CreateClosedInterval(T endpointA, T endpointB)
        {
            return new Interval<T>(endpointA, endpointB, false, false);
        }

        /// <summary>
        /// 半開区間: (a, b] のインスタンスを作成します。
        /// </summary>
        /// <param name="endpointA">端点a</param>
        /// <param name="endpointB">端点b</param>
        /// <returns>区間</returns>
        public static Interval<T> CreateLeftOpenInterval(T endpointA, T endpointB)
        {
            return new Interval<T>(endpointA, endpointB, true, false);
        }

        /// <summary>
        /// 半開区間: [a, b) のインスタンスを作成します。
        /// </summary>
        /// <param name="endpointA">端点a</param>
        /// <param name="endpointB">端点b</param>
        /// <returns>区間</returns>
        public static Interval<T> CreateRightOpenInterval(T endpointA, T endpointB)
        {
            return new Interval<T>(endpointA, endpointB, false, true);
        }

        /// <summary>
        /// 開区間: (a, b) のインスタンスを作成します。
        /// </summary>
        /// <param name="endpointA">端点a</param>
        /// <param name="endpointB">端点b</param>
        /// <returns>区間</returns>
        public static Interval<T> CreateOpenInterval(T endpointA, T endpointB)
        {
            return new Interval<T>(endpointA, endpointB, true, true);
        }
    }
}
