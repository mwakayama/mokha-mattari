using System;

namespace MokhaMattari.RuntimeSupport
{
    /// <summary>
    /// DateTime および DateTimeOffset の拡張メソッドを提供します。
    /// 
    /// FIXME: 同機能を持つ Nuget パッケージが存在するなら、それを使ったほうが良い。
    /// すべてのプロジェクトで参照するようなユーティリティは、パッケージで提供されるべきで、
    /// プロジェクト間依存性を高めるようなこの実装は良くない。
    /// https://www.nuget.org/packages/Exceptionless.DateTimeExtensions/
    /// が見つかったが、実装が古い。.NET 4.6.1 の System.Net.Http.Formatting.Extension 5.2.3 を参照している。
    /// </summary>
    public static class DateTimeExtension
    {
        private static readonly int daysOfWeek = 7;

        /// <summary>
        /// この日時から時刻情報を削除した新しい DateTime インスタンスを取得します。
        /// </summary>
        /// <param name="dateTime">日時</param>
        /// <returns>同日の 00:00:00.000 を示す DateTime のインスタンス</returns>
        public static DateTime DropTime(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        /// <summary>
        /// 時刻情報を 23:59:59.999 に書き換えた新しい DateTime インスタンスを取得します。
        /// </summary>
        /// <param name="dateTime">日時</param>
        /// <returns>同日の 23:59.59.999 を示す DateTinme のインスタンス</returns>
        public static DateTime FillTime(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, 999, DateTimeKind.Local);
        }

        /// <summary>
        /// この DateTime の月初を取得します。
        /// </summary>
        /// <param name="dateTime">同月の日時</param>
        /// <returns>月初。時刻は 00:00:00.000</returns>
        public static DateTime FirstDayOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        /// <summary>
        /// この DateTime の月末を取得します。
        /// </summary>
        /// <param name="dateTime">同月の日時</param>
        /// <returns>月末。時刻は 00:00:00.000（23:59 ではない）</returns>
        public static DateTime LastDayOfMonth(this DateTime dateTime)
        {
            int days = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            return new DateTime(dateTime.Year, dateTime.Month, days);
        }

        /// <summary>
        /// この DateTime 以前の直近の日曜日（週初の日曜日）を取得します。
        /// <paramref name="dateTime"/> が日曜日なら、同日が返却されます。
        /// </summary>
        /// <returns>週初の日曜日。時刻は 00:00:00.000（23:59 ではない）</returns>
        public static DateTime Sunday(this DateTime dateTime)
        {
            return dateTime.AddDays(DayOfWeek.Sunday - dateTime.DayOfWeek).DropTime();
        }

        /// <summary>
        /// この DateTime 以前の直近の月曜日（週初の月曜日）を取得します。
        /// <paramref name="dateTime"/> が月曜日なら、同日が返却されます。
        /// </summary>
        /// <param name="dateTime">日時</param>
        /// <returns>週初の月曜日。時刻は 00:00:00.000（23:59 ではない）</returns>
        public static DateTime Monday(this DateTime dateTime)
        {
            return dateTime.AddDays(DayOfWeek.Monday - dateTime.DayOfWeek).DropTime();
        }

        /// <summary>
        /// この DateTime の次の日曜日を取得します。
        /// <paramref name="dateTime"/> が日曜日なら、次の週の日曜日が返却されます。
        /// </summary>
        /// <param name="dateTime">日時</param>
        /// <returns>次の日曜日。時刻は 00:00:00.000（23:59 ではない）</returns>
        public static DateTime NextSunday(this DateTime dateTime)
        {
            return dateTime.AddDays(daysOfWeek - (int)dateTime.DayOfWeek).DropTime();
        }

        /// <summary>
        /// この日時から時刻情報を削除した新しい DateTimeOffset インスタンスを取得します。
        /// Offset（UTC からの時差）情報は保持されます。
        /// </summary>
        /// <param name="dateTime">日時</param>
        /// <returns>同日の AM 0:00 を示す DateTimeOffset のインスタンス。Offset は <paramref name="dateTime"/> と同じ</returns>
        public static DateTimeOffset DropTime(this DateTimeOffset dateTime)
        {
            return new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0, dateTime.Offset);
        }

        /// <summary>
        /// 時刻情報を 23:59:59.999 に書き換えた新しい DateTimeOffset インスタンスを取得します。
        /// Offset（UTC からの時差）情報は保持されます。
        /// </summary>
        /// <param name="dateTime">日時</param>
        /// <returns>同日の 23:59.59.999 を示す DateTinme のインスタンス。Offset は <paramref name="dateTime"/> と同じ</returns>
        public static DateTimeOffset FillTime(this DateTimeOffset dateTime)
        {
            return new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, 999, dateTime.Offset);
        }

        /// <summary>
        /// この DateTimeOffset の月初を取得します。
        /// </summary>
        /// <param name="dateTime">同月の日時</param>
        /// <returns>月初。時刻は 00:00:00.000、Offset は <paramref name="dateTime"/> と同じ</returns>
        public static DateTimeOffset FirstDayOfMonth(this DateTimeOffset dateTime)
        {
            return new DateTimeOffset(dateTime.Year, dateTime.Month, 1, 0, 0, 0, 0, dateTime.Offset);
        }

        /// <summary>
        /// この DateTimeOffset の月末を取得します。
        /// </summary>
        /// <param name="dateTime">同月の日時</param>
        /// <returns>月末。時刻は 00:00:00.000（23:59 ではない）、Offset は <paramref name="dateTime"/> と同じ</returns>
        public static DateTimeOffset LastDayOfMonth(this DateTimeOffset dateTime)
        {
            int days = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            return new DateTimeOffset(dateTime.Year, dateTime.Month, days,
                                      0, 0, 0, 0, dateTime.Offset);
        }

        /// <summary>
        /// この DateTimeOffset 以前の直近の日曜日（週初の日曜日）を取得します。
        /// <paramref name="dateTime"/> が日曜日なら、同日が返却されます。
        /// </summary>
        /// <returns>週初の日曜日。時刻は 00:00:00.000（23:59 ではない）、Offset は <paramref name="dateTime"> と同じ</returns>
        public static DateTimeOffset Sunday(this DateTimeOffset dateTime)
        {
            return dateTime.AddDays(DayOfWeek.Sunday - dateTime.DayOfWeek).DropTime();
        }

        /// <summary>
        /// この DateTimeOffset 以前の直近の月曜日（週初の月曜日）を取得します。
        /// <paramref name="dateTime"/> が月曜日なら、同日が返却されます。
        /// </summary>
        /// <param name="dateTime">日時</param>
        /// <returns>週初の月曜日。時刻は 00:00:00.000（23:59 ではない）、Offset は <paramref name="dateTime"> と同じ</returns>
        public static DateTimeOffset Monday(this DateTimeOffset dateTime)
        {
            return dateTime.AddDays(DayOfWeek.Monday - dateTime.DayOfWeek).DropTime();
        }

        /// <summary>
        /// この DateTimeOffset の次の日曜日を取得します。
        /// <paramref name="dateTime"/> が日曜日なら、次の週の日曜日が返却されます。
        /// </summary>
        /// <param name="dateTime">日時</param>
        /// <returns>次の日曜日。時刻は 00:00:00.000（23:59 ではない）</returns>
        public static DateTimeOffset NextSunday(this DateTimeOffset dateTime)
        {
            return dateTime.AddDays(daysOfWeek - (int)dateTime.DayOfWeek).DropTime();
        }
    }
}