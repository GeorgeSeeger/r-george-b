namespace RGeorgeB {
    public static class EnumerableExtensions {
        public static int IndexOf<T>(this T[] array, Func<T, bool> predicate) {
            for (var i = 0; i < array.Length; i++) {
                if (predicate(array[i])) return i;
            }

            return -1;
        }
    }
}