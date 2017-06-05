namespace MobileCenterDemoApp.Helpers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public static class Extentions
    {
        /// <summary>
        /// Remove last elements from range
        /// </summary>
        /// <typeparam name="T">INumerable elements type</typeparam>
        /// <param name="collection">INumerable</param>
        /// <param name="removeCount">Elements count for remove</param>
        /// <returns>IEnumerable without last elements</returns>
        public static IEnumerable<T> RemoveLastElements<T>(this IEnumerable<T> collection, int removeCount = 1)
        {
            if (removeCount < 0)
            {
                throw new ArgumentException("Remove count must be more than 0");
            }

            T[] array = collection.ToArray();

            if (removeCount > array.Length)
            {
                throw new ArgumentException("Remove count must be less than collection count");
            }

            return array.Reverse().Skip(removeCount).Reverse();
        }
    }
}
