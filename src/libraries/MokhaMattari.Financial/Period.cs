using System;
using System.ComponentModel.DataAnnotations;

namespace MokhaMattari.Financial
{
    /// <summary>
    /// 年数／月数／日数指定による期間表現です。
    /// "1" 年後や "1" ヶ月後など、状況によっては具体的な Duration 値が異なる（※）期間を表現します。
    /// ※ 例：2月の 1ヶ月は 28 日だが、3 月は 31日。それらを考慮せず、"次の月の同じ日" を計算する場合などに使用する。
    /// </summary>
    public class Period : IComparable<Period>
    {
        public static readonly Period Zero = new Period(0, 0, 0);

        public static readonly Period MinValue = Zero;

        public static readonly Period MaxValue = new Period(int.MaxValue, 11, 31);


        [Range(0, int.MaxValue)]
        public int Years { get; private set; }

        [Range(0, 11)]
        public int Months { get; private set; }

        [Range(0, 31)]
        public int Days { get; private set; }

        /// <summary>
        /// この期間の長さ（月数単位）を取得します。
        /// </summary>
        /// <value>この期間の長さ（月数）</value>
        public int DurationInMonths
        {
            get
            {
                return Years * 12 + Months;
            }
        }

        /// <summary>
        /// 年数／月数／日数を指定して、 Period の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="years">年数</param>
        /// <param name="months">月数</param>
        /// <param name="days">日数</param>
        public Period(int years, int months, int days)
        {
            this.Years = years;
            this.Months = months;
            this.Days = days;
        }

        /// <summary>
        /// 他の期間とこの期間の長さを比較します。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>このオブジェクトの方が短いなら負の値、長いなら正の値、同一なら 0</returns>
        public int CompareTo(Period obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (this.DurationInMonths == obj.DurationInMonths)
            {
                return this.Days - obj.Days;
            }

            return this.DurationInMonths - obj.DurationInMonths;
        }
    }
}