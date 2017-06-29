namespace MobileCenterDemoApp.Helpers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public static class Extensions
    {
        /// <summary>
        /// Remove last elements from range
        /// </summary>
        /// <typeparam name="T">IEnumerable elements type</typeparam>
        /// <param name="collection">IEnumerable</param>
        /// <param name="removeCount">Elements count for remove</param>
        /// <returns>IEnumerable without last elements</returns>
        public static IEnumerable<T> RemoveLastElements<T>(this IEnumerable<T> collection, int removeCount = 1)
        {
            if (removeCount < 0)
            {
                throw new ArgumentException("Remove count must be more than 0");
            }
            int count = collection.Count();
            if (removeCount > count)
            {
                throw new ArgumentException("Remove count must be less than collection count");
            }
            return collection.Take(count - removeCount);
        }
    }
}
