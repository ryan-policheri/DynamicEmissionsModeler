using System.Text;

namespace DotNetCommon
{
    public static class StringBuilderExtensions
    { //https://stackoverflow.com/questions/17580150/net-stringbuilder-check-if-ends-with-string
        public static bool EndsWith(this StringBuilder sb, string test)
        {
            return EndsWith(sb, test, StringComparison.CurrentCulture);
        }

        public static bool EndsWith(this StringBuilder sb, string test,
            StringComparison comparison)
        {
            if (sb.Length < test.Length)
                return false;

            string end = sb.ToString(sb.Length - test.Length, test.Length);
            return end.Equals(test, comparison);
        }

        //https://stackoverflow.com/questions/24769701/trim-whitespace-from-the-end-of-a-stringbuilder-without-calling-tostring-trim
        public static StringBuilder TrimEnd(this StringBuilder builder)
        {
            if (builder == null || builder.Length == 0) return builder;

            int i = builder.Length - 1;

            for (; i >= 0; i--)
                if (!char.IsWhiteSpace(builder[i]))
                    break;

            if (i < builder.Length - 1)
                builder.Length = i + 1;

            return builder;
        }

        public static StringBuilder TrimEnd(this StringBuilder builder, char trimChar)
        {
            if (builder == null || builder.Length == 0) return builder;

            int i = builder.Length - 1;

            for (; i >= 0; i--)
                if (builder[i] != trimChar)
                    break;

            if (i < builder.Length - 1)
                builder.Length = i + 1;

            return builder;
        }

        public static StringBuilder TrimNewline(this StringBuilder builder)
        {
            char[] chars = Environment.NewLine.ToCharArray().Reverse().ToArray();
            foreach (char c in chars) builder.TrimEnd(c);
            return builder;
        }
    }
}
