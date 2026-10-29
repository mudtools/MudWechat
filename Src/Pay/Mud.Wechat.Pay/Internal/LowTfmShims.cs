// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NETSTANDARD2_0

using System.IO;
using System.Threading;
using System.Threading.Tasks;

// netstandard2.0 缺失的 BCL 便利成员（均为「同名同语义」的本地补齐，不改任何算法口径）。
namespace System.Security.Cryptography
{
    /// <summary>ns2.0 缺失的 <c>CryptographicOperations</c> 等价物（BCL 自 netcoreapp3.0 起提供）。</summary>
    internal static class CryptographicOperations
    {
        /// <summary>清零缓冲区（对应 BCL 的 <c>ZeroMemory</c>）。</summary>
        /// <param name="buffer">目标缓冲区。</param>
        internal static void ZeroMemory(byte[] buffer)
        {
            if (buffer is null)
            {
                return;
            }

            Array.Clear(buffer, 0, buffer.Length);
        }

        /// <summary>定长比较（对应 BCL 的 <c>FixedTimeEquals</c>，不因首个不等字节提前返回）。</summary>
        /// <param name="left">左侧数据。</param>
        /// <param name="right">右侧数据。</param>
        /// <returns>长度相同且逐字节相等时为 <see langword="true"/>。</returns>
        internal static bool FixedTimeEquals(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
        {
            if (left.Length != right.Length)
            {
                return false;
            }

            var diff = 0;
            for (var i = 0; i < left.Length; i++)
            {
                diff |= left[i] ^ right[i];
            }

            return diff == 0;
        }
    }
}

namespace System
{
    /// <summary>ns2.0 缺失的 <c>HashCode</c>（BCL 自 netcoreapp2.1 起提供）。</summary>
    internal struct HashCode
    {
        /// <summary>组合两个值的哈希（语义与 BCL 一致：稳定且与参数顺序相关）。</summary>
        /// <typeparam name="T1">第一个值类型。</typeparam>
        /// <typeparam name="T2">第二个值类型。</typeparam>
        /// <param name="value1">第一个值。</param>
        /// <param name="value2">第二个值。</param>
        /// <returns>组合后的哈希码。</returns>
        internal static int Combine<T1, T2>(T1 value1, T2 value2)
        {
            var hash = 17;
            hash = (hash * 31) + (value1 is null ? 0 : value1.GetHashCode());
            hash = (hash * 31) + (value2 is null ? 0 : value2.GetHashCode());
            return hash;
        }
    }
}

namespace System.Net.Http
{
    /// <summary>ns2.0 缺失的 <see cref="HttpContent"/> 取消令牌重载（BCL 自 net5.0 起提供）。</summary>
    internal static class LowTfmHttpContentExtensions
    {
        /// <summary>读取为字符串（忽略取消令牌，与 ns2.0 的同步语义一致）。</summary>
        /// <param name="content">HTTP 内容。</param>
        /// <param name="cancellationToken">取消令牌（ns2.0 无对应重载，仅保持调用点写法统一）。</param>
        /// <returns>内容字符串。</returns>
        internal static Task<string> ReadAsStringAsync(this HttpContent content, CancellationToken cancellationToken)
        {
            _ = cancellationToken;
            return content.ReadAsStringAsync();
        }

        /// <summary>拷贝到流（忽略取消令牌，同上）。</summary>
        /// <param name="content">HTTP 内容。</param>
        /// <param name="stream">目标流。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>任务。</returns>
        internal static Task CopyToAsync(this HttpContent content, Stream stream, CancellationToken cancellationToken)
        {
            _ = cancellationToken;
            return content.CopyToAsync(stream);
        }
    }
}

#endif
