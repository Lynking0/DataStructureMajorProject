using System;
using System.Collections.Generic;
/// <summary>
///     提供IEnumerable<T>和ICollection<T>的扩展方法
/// </summary>
namespace Shared.Extensions.ICollectionExtensions
{
    public static class _ICollectionExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            for (IEnumerator<T> it = list.GetEnumerator(); it.MoveNext();)
                action(it.Current);
        }
        public static void ForEach<T>(this IEnumerable<T> list, Action<T, int> action)
        {
            int idx = 0;
            for (IEnumerator<T> it = list.GetEnumerator(); it.MoveNext(); ++idx)
                action(it.Current, idx);
        }
        public static void ForEach<T>(this IEnumerable<T> list, Action<T, int, IEnumerable<T>> action)
        {
            int idx = 0;
            for (IEnumerator<T> it = list.GetEnumerator(); it.MoveNext(); ++idx)
                action(it.Current, idx, list);
        }
        public static TResult[] Map<T, TResult>(this ICollection<T> list, Func<T, TResult> func)
        {
            TResult[] result = new TResult[list.Count];
            int idx = 0;
            for (IEnumerator<T> it = list.GetEnumerator(); it.MoveNext(); ++idx)
                result[idx] = func(it.Current);
            return result;
        }
        public static TResult[] Map<T, TResult>(this ICollection<T> list, Func<T, int, TResult> func)
        {
            TResult[] result = new TResult[list.Count];
            int idx = 0;
            for (IEnumerator<T> it = list.GetEnumerator(); it.MoveNext(); ++idx)
                result[idx] = func(it.Current, idx);
            return result;
        }
        public static TResult[] Map<T, TResult>(this ICollection<T> list, Func<T, int, ICollection<T>, TResult> func)
        {
            TResult[] result = new TResult[list.Count];
            int idx = 0;
            for (IEnumerator<T> it = list.GetEnumerator(); it.MoveNext(); ++idx)
                result[idx] = func(it.Current, idx, list);
            return result;
        }
        public static TResult[] Map<T, TResult>(this IReadOnlyCollection<T> list, Func<T, TResult> func)
        {
            TResult[] result = new TResult[list.Count];
            int idx = 0;
            for (IEnumerator<T> it = list.GetEnumerator(); it.MoveNext(); ++idx)
                result[idx] = func(it.Current);
            return result;
        }
        public static TResult[] Map<T, TResult>(this IReadOnlyCollection<T> list, Func<T, int, TResult> func)
        {
            TResult[] result = new TResult[list.Count];
            int idx = 0;
            for (IEnumerator<T> it = list.GetEnumerator(); it.MoveNext(); ++idx)
                result[idx] = func(it.Current, idx);
            return result;
        }
        public static TResult[] Map<T, TResult>(this IReadOnlyCollection<T> list, Func<T, int, IReadOnlyCollection<T>, TResult> func)
        {
            TResult[] result = new TResult[list.Count];
            int idx = 0;
            for (IEnumerator<T> it = list.GetEnumerator(); it.MoveNext(); ++idx)
                result[idx] = func(it.Current, idx, list);
            return result;
        }
        public static IEnumerable<(int index, T element)> Enumerate<T>(this IEnumerable<T> list)
        {
            int idx = 0;
            for (IEnumerator<T> it = list.GetEnumerator(); it.MoveNext(); ++idx)
                yield return (idx, it.Current);
        }
        /// <summary>
        ///     将当前可迭代对象与其他多个可迭代对象合并成新的可迭代对象并返回（不改变原可迭代对象）
        /// </summary>
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> thisEnum, params IEnumerable<T>[] enumerators)
        {
            for (IEnumerator<T> it = thisEnum.GetEnumerator(); it.MoveNext();)
                yield return it.Current;
            foreach (IEnumerable<T> enumerator in enumerators)
                for (IEnumerator<T> it = enumerator.GetEnumerator(); it.MoveNext();)
                    yield return it.Current;
        }
    }
}