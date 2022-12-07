namespace DotNetCommon.Extensions
{
    public static class ArrayExtensions
    {
        public static void ForEach(this Array array, Action<Array, int[]> action)
        {
            if (array.LongLength == 0) return;
            ArrayTraverse walker = new ArrayTraverse(array);
            do action(array, walker.Position);
            while (walker.Step());
        }

        public static int LastIndex<T>(this T[] array)
        {
            if (array == null) return -1;
            return array.Length - 1;
        }

        public static string ToDelimitedList(this IEnumerable<string> list, char delimiter = ',')
        {
            string delimitedList = string.Empty;
            foreach (string s in list)
            {
                delimitedList = delimitedList + s + delimiter;
            }
            if (delimitedList.Length > 0)
            {
                delimitedList = delimitedList.TrimEnd(delimiter);
            }

            return delimitedList;
        }

        public static string ToDelimitedList(this IEnumerable<string> list, string delimiter = ",")
        {
            string delimitedList = string.Empty;
            foreach (string s in list)
            {
                delimitedList = delimitedList + s + delimiter;
            }
            if (delimitedList.Length > 0)
            {
                delimitedList = delimitedList.TrimEnd(delimiter);
            }

            return delimitedList;
        }

        public static string ToDelimitedList(this IEnumerable<int> list, string delimiter = ",")
        {
            string delimitedList = string.Empty;
            foreach (int i in list)
            {
                delimitedList = delimitedList + i + delimiter;
            }
            if (delimitedList.Length > 0)
            {
                delimitedList = delimitedList.TrimEnd(delimiter);
            }

            return delimitedList;
        }
    }

    internal class ArrayTraverse
    {
        public int[] Position;
        private int[] maxLengths;

        public ArrayTraverse(Array array)
        {
            maxLengths = new int[array.Rank];
            for (int i = 0; i < array.Rank; ++i)
            {
                maxLengths[i] = array.GetLength(i) - 1;
            }
            Position = new int[array.Rank];
        }

        public bool Step()
        {
            for (int i = 0; i < Position.Length; ++i)
            {
                if (Position[i] < maxLengths[i])
                {
                    Position[i]++;
                    for (int j = 0; j < i; j++)
                    {
                        Position[j] = 0;
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
