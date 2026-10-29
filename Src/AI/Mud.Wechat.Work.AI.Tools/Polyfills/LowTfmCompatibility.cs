// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NETSTANDARD2_0

using System.Collections.Generic;

// 低两档缺失的 BCL 便利成员：以扩展方法补齐，使业务代码在四档保持同一写法（net6.0+ 有实例方法，故只在 ns2.0 生效）。
namespace System.Collections.Generic
{
    /// <summary>ns2.0 的 <see cref="Dictionary{TKey,TValue}"/> 便利扩展。</summary>
    internal static class LowTfmDictionaryExtensions
    {
        /// <summary>键不存在时添加（对应 netstandard2.1+ 的实例方法 <c>TryAdd</c>）。</summary>
        /// <typeparam name="TKey">键类型。</typeparam>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="dictionary">目标字典。</param>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <returns>是否添加成功（键已存在返回 <see langword="false"/>）。</returns>
        internal static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                return false;
            }

            dictionary.Add(key, value);
            return true;
        }
    }

    /// <summary>ns2.0 的键值对解构扩展（对应 netstandard2.1+ 的 <c>Deconstruct</c>）。</summary>
    internal static class LowTfmKeyValuePairExtensions
    {
        /// <summary>解构为键与值，供 <c>foreach (var (key, value) in ...)</c> 使用。</summary>
        /// <typeparam name="TKey">键类型。</typeparam>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="pair">键值对。</param>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        internal static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
        {
            key = pair.Key;
            value = pair.Value;
        }
    }
}

#endif
