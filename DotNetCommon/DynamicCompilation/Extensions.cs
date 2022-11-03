namespace DotNetCommon.DynamicCompilation
{
    public static class Extensions
    {
        public static string ToValidNamespace(this string source)
        {
            return source.ToValidClassName();
        }

        public static string ToValidClassName(this string source, string spaceReplaceChar = "")
        {
            if (String.IsNullOrEmpty(source)) return source;
            source = source.Replace(" ", spaceReplaceChar);
            string keepChars = new String(source.Where(x => Char.IsLetterOrDigit(x) || x == '_').ToArray());
            if (Char.IsDigit(keepChars[0])) keepChars = "_" + keepChars;
            return keepChars;
        }

        public static string ToValidMethodName(this string source)
        {
            return source.ToValidClassName();
        }

        public static string ToValidVariableName(this string source)
        {
            return source.ToValidClassName();
        }
    }
}