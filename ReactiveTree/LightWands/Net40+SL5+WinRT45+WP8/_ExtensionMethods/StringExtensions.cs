using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Kirinji.LightWands
{
    public static class StringExtensions
    {
        /// <summary>
        /// 改行も Trim する
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string TrimBreak(this string s)
        {
            if (s == null)
                return null;
            else if (s == "")
                return "";
            else
                return s.Trim(' ', '\r', '\n');
        }

        /// <summary>
        /// 改行も削除する
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string DeleteSpaces(this string s)
        {
            return s.Replace(" ", "").Replace("\r", "").Replace("\n", "");
        }


        private static IDictionary<string, string> WidthDicExcludingRegexSymbols = new Dictionary<string, string>
            {

                {"Ａ","A"},
                {"Ｂ","B"},
                {"Ｃ","C"},
                {"Ｄ","D"},
                {"Ｅ","E"},
                {"Ｆ","F"},
                {"Ｇ","G"},
                {"Ｈ","H"},
                {"Ｉ","I"},
                {"Ｊ","J"},
                {"Ｋ","K"},
                {"Ｌ","L"},
                {"Ｍ","M"},
                {"Ｎ","N"},
                {"Ｏ","O"},
                {"Ｐ","P"},
                {"Ｑ","Q"},
                {"Ｒ","R"},
                {"Ｓ","S"},
                {"Ｔ","T"},
                {"Ｕ","U"},
                {"Ｖ","V"},
                {"Ｗ","W"},
                {"Ｘ","X"},
                {"Ｙ","Y"},
                {"Ｚ","Z"},
                {"ａ","a"},
                {"ｂ","b"},
                {"ｃ","c"},
                {"ｄ","d"},
                {"ｅ","e"},
                {"ｆ","f"},
                {"ｇ","g"},
                {"ｈ","h"},
                {"ｉ","i"},
                {"ｊ","j"},
                {"ｋ","k"},
                {"ｌ","l"},
                {"ｍ","m"},
                {"ｎ","n"},
                {"ｏ","o"},
                {"ｐ","p"},
                {"ｑ","q"},
                {"ｒ","r"},
                {"ｓ","s"},
                {"ｔ","t"},
                {"ｕ","u"},
                {"ｖ","v"},
                {"ｗ","w"},
                {"ｘ","x"},
                {"ｙ","y"},
                {"ｚ","z"},
                {"ｶﾞ", "ガ"},
                {"ｷﾞ", "ギ"},
                {"ｸﾞ", "グ"},
                {"ｹﾞ", "ゲ"},
                {"ｺﾞ", "ゴ"},
                {"ｻﾞ", "ザ"},
                {"ｼﾞ", "ジ"},
                {"ｽﾞ", "ズ"},
                {"ｾﾞ", "ゼ"},
                {"ｿﾞ", "ゾ"},
                {"ﾀﾞ", "ダ"},
                {"ﾁﾞ", "ヂ"},
                {"ﾂﾞ", "ヅ"},
                {"ﾃﾞ", "デ"},
                {"ﾄﾞ", "ド"},
                {"ﾊﾞ", "バ"},
                {"ﾋﾞ", "ビ"},
                {"ﾌﾞ", "ブ"},
                {"ﾍﾞ", "ベ"},
                {"ﾎﾞ", "ボ"},
                {"ﾊﾟ", "パ"},
                {"ﾋﾟ", "ピ"},
                {"ﾌﾟ", "プ"},
                {"ﾍﾟ", "ペ"},
                {"ﾎﾟ", "ポ"},
                {"ｳﾞ", "ヴ"},
                {"ｱ", "ア"},
                {"ｲ", "イ"},
                {"ｳ", "ウ"},
                {"ｴ", "エ"},
                {"ｵ", "オ"},
                {"ｶ", "カ"},
                {"ｷ", "キ"},
                {"ｸ", "ク"},
                {"ｹ", "ケ"},
                {"ｺ", "コ"},
                {"ｻ", "サ"},
                {"ｼ", "シ"},
                {"ｽ", "ス"},
                {"ｾ", "セ"},
                {"ｿ", "ソ"},
                {"ﾀ", "タ"},
                {"ﾁ", "チ"},
                {"ﾂ", "ツ"},
                {"ﾃ", "テ"},
                {"ﾄ", "ト"},
                {"ﾅ", "ナ"},
                {"ﾆ", "ニ"},
                {"ﾇ", "ヌ"},
                {"ﾈ", "ネ"},
                {"ﾉ", "ノ"},
                {"ﾊ", "ハ"},
                {"ﾋ", "ヒ"},
                {"ﾌ", "フ"},
                {"ﾍ", "ヘ"},
                {"ﾎ", "ホ"},
                {"ﾏ", "マ"},
                {"ﾐ", "ミ"},
                {"ﾑ", "ム"},
                {"ﾒ", "メ"},
                {"ﾓ", "モ"},
                {"ﾔ", "ヤ"},
                {"ﾕ", "ユ"},
                {"ﾖ", "ヨ"},
                {"ﾗ", "ラ"},
                {"ﾘ", "リ"},
                {"ﾙ", "ル"},
                {"ﾚ", "レ"},
                {"ﾛ", "ロ"},
                {"ﾜ", "ワ"},
                {"ｦ", "ヲ"},
                {"ﾝ", "ン"},
                {"ｧ", "ァ"},
                {"ｨ", "ィ"},
                {"ｩ", "ゥ"},
                {"ｪ", "ェ"},
                {"ｫ", "ォ"},
                {"ｬ", "ャ"},
                {"ｭ", "ュ"},
                {"ｮ", "ョ"},
                {"ｯ", "ッ"},
                {"ﾞ", "゛"},
                {"ﾟ", "゜"},
                {"｢", "「"},
                {"｣", "」"},
                {"､", "、"},
                {"｡", "。"},
                {"ｰ", "ー"}
            };

        // ToDo: 不完全
        static IDictionary<string, string> WidthDicRegexSymbolsToRegexSymbols = new Dictionary<string, string>
        {
            {"￥",@"\\"},
            {"（",@"\("},
            {"）",@"\)"}
        };

        // ToDo: 不完全
        static IDictionary<string, string> WidthDicRegexSymbolsToString = new Dictionary<string, string>
        {
            {"￥",@"\"},
            {"（","("},
            {"）",")"}
        };

        /// <summary>
        /// 全角文字、半角文字を統一する（正規表現の文字列に対して）
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RegexIgnoreWidth(this string s)
        {
            var str = s;

            foreach (var set in WidthDicExcludingRegexSymbols)
            {
                str = str.Replace(set.Key, set.Value);
            }

            foreach (var set in WidthDicRegexSymbolsToRegexSymbols)
            {
                str = str.Replace(set.Key, set.Value);
            }

            return str;
        }

        /// <summary>
        /// 全角文字、半角文字を統一する（正規表現でない文字列に対して）
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string StringIgnoreWidth(this string s)
        {
            var str = s;

            foreach (var set in WidthDicExcludingRegexSymbols)
            {
                str = str.Replace(set.Key, set.Value);
            }

            foreach (var set in WidthDicRegexSymbolsToString)
            {
                str = str.Replace(set.Key, set.Value);
            }

            return str;
        }


        static IDictionary<string, string> KanaDic = new Dictionary<string, string>
        {
            {"あ", "ア"},
            {"い", "イ"},
            {"う", "ウ"},
            {"え", "エ"},
            {"お", "オ"},
            {"か", "カ"},
            {"き", "キ"},
            {"く", "ク"},
            {"け", "ケ"},
            {"こ", "コ"},
            {"が", "ガ"},
            {"ぎ", "ギ"},
            {"ぐ", "グ"},
            {"げ", "ゲ"},
            {"ご", "ゴ"},
            {"さ", "サ"},
            {"し", "シ"},
            {"す", "ス"},
            {"せ", "セ"},
            {"そ", "ソ"},
            {"ざ", "ザ"},
            {"じ", "ジ"},
            {"ず", "ズ"},
            {"ぜ", "ゼ"},
            {"ぞ", "ゾ"},
            {"た", "タ"},
            {"ち", "チ"},
            {"つ", "ツ"},
            {"て", "テ"},
            {"と", "ト"},
            {"だ", "ダ"},
            {"ぢ", "ヂ"},
            {"づ", "ヅ"},
            {"で", "デ"},
            {"ど", "ド"},
            {"な", "ナ"},
            {"に", "ニ"},
            {"ぬ", "ヌ"},
            {"ね", "ネ"},
            {"の", "ノ"},
            {"は", "ハ"},
            {"ひ", "ヒ"},
            {"ふ", "フ"},
            {"へ", "ヘ"},
            {"ほ", "ホ"},
            {"ば", "バ"},
            {"び", "ビ"},
            {"ぶ", "ブ"},
            {"べ", "ベ"},
            {"ぼ", "ボ"},
            {"ぱ", "パ"},
            {"ぴ", "ピ"},
            {"ぷ", "プ"},
            {"ぺ", "ペ"},
            {"ぽ", "ポ"},
            {"ま", "マ"},
            {"み", "ミ"},
            {"む", "ム"},
            {"め", "メ"},
            {"も", "モ"},
            {"や", "ヤ"},
            {"ゆ", "ユ"},
            {"よ", "ヨ"},
            {"ら", "ラ"},
            {"り", "リ"},
            {"る", "ル"},
            {"れ", "レ"},
            {"ろ", "ロ"},
            {"わ", "ワ"},
            {"ゐ", "ヰ"},
            {"ゑ", "ヱ"},
            {"を", "ヲ"},
            {"ん", "ン"},
            {"ぁ", "ァ"},
            {"ぃ", "ィ"},
            {"ぅ", "ゥ"},
            {"ぇ", "ェ"},
            {"ぉ", "ォ"},
            {"ゕ", "ヵ"},
            {"ゖ", "ヶ"},
            {"ゔ", "ヴ"},
            {"ゝ", "ヽ"},
            //{"ゞ", "ヾ"} なぜかバグる
        };

        /// <summary>
        /// 全角文字、半角文字を統一する。対象文字列が通常の文字列でも Regex コンストラクタに渡す文字列でもどちらでも構わない
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string IgnoreKana(this string s)
        {
            var str = s;

            foreach (var set in KanaDic)
            {
                str = str.Replace(set.Key, set.Value);
            }

            return str;
        }

        public static byte? ByteParse(this string s)
        {
            byte outByte;
            if (byte.TryParse(s, out outByte))
                return outByte;
            else
                return null;
        }

        public static byte? ByteParse(this string s, NumberStyles numberStyles, IFormatProvider formatProvider)
        {
            byte outByte;
            if (byte.TryParse(s, numberStyles, formatProvider, out outByte))
                return outByte;
            else
                return null;
        }

        public static int? IntParse(this string s)
        {
            int outInt;
            if (int.TryParse(s, out outInt))
                return outInt;
            else
                return null;
        }

        public static int? IntParse(this string s, NumberStyles numberStyles, IFormatProvider formatProvider)
        {
            int outInt;
            if (int.TryParse(s, numberStyles, formatProvider, out outInt))
                return outInt;
            else
                return null;
        }

        public static long? LongParse(this string s)
        {
            long outLong;
            if (long.TryParse(s, out outLong))
                return outLong;
            else
                return null;
        }

        public static long? LongParse(this string s, NumberStyles numberStyles, IFormatProvider formatProvider)
        {
            long outLong;
            if (long.TryParse(s, numberStyles, formatProvider, out outLong))
                return outLong;
            else
                return null;
        }

        public static DateTime? DateTimeParse(this string s)
        {
            DateTime outDate;
            if (DateTime.TryParse(s, out outDate))
                return outDate;
            else
                return null;
        }

        public static DateTime? DateTimeParse(this string s, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
        {
            DateTime outDate;
            if (DateTime.TryParse(s, formatProvider, dateTimeStyles, out outDate))
                return outDate;
            else
                return null;
        }

        public static DateTime? DateTimeParseExact(this string s, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles, params string[] formats)
        {
            DateTime outDate;
            if (DateTime.TryParseExact(s, formats, formatProvider, dateTimeStyles, out outDate))
                return outDate;
            else
                return null;
        }

        public static bool? BoolParse(this string s)
        {
            bool outBool;
            if (bool.TryParse(s, out outBool))
                return outBool;
            else
                return null;
        }
    }
}
