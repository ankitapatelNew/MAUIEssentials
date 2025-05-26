namespace MAUIEssentials.ExtensionMethods
{
    public static class IListExtensions
    {
        private static Random ran = new Random();
        /// <summary>
        /// Shuffles the element order of the specified list.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ran.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}