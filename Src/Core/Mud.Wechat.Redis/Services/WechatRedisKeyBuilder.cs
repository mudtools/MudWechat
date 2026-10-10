// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text;

namespace Mud.Wechat.Redis.Services;

/// <summary>
/// 统一键构造器——四类 Redis 键（token / corpa / ticket / replay）的唯一权威构造入口
/// （对齐 Mud.Feishu.Redis 的 <c>RedisKeyBuilder</c>，R-01/R-20/R2-02 护栏同源）。
/// </summary>
/// <remarks>
/// <para>
/// <b>RD2（与飞书的有意偏离）——令牌 <c>storeKey</c> 作为不透明叶子、不做段转义</b>：
/// 令牌键 <c>{prefix}:token:{storeKey}</c> 中的 <c>storeKey</c>（三段式 <c>{tokenType}:{appKey}:{scopeKey}</c>）
/// 是 SDK 内部构造的完整键，回读口径必须与 <c>InMemoryWechatTokenStore</c> 的键空间逐字节一致
/// （<c>WechatAppManager.PurgeAppTokensAsync</c> 依赖三段解析）。因此令牌域经
/// <see cref="TokenKey"/> 原样拼接、<see cref="TryStripTokenKeyPrefix"/> 按已知字面量前缀长度剥离，
/// 不做 <c>:</c> 转义/反转义；<c>corpa</c> / <c>ticket</c> / <c>replay</c> 域的组合段照常经
/// <see cref="Combine"/> 转义。
/// </para>
/// <para>
/// <b>SCAN 模式唯一出口</b>：<see cref="Pattern"/> 在 <see cref="Combine"/> 产物之上做 glob 字面量转义
/// （<c>\</c> <c>*</c> <c>?</c> <c>[</c> <c>]</c>，对齐飞书 <c>RedisGlobPattern</c>）——键中真实的
/// <c>\:</c> 转义序列在 pattern 中须写作 <c>\\:</c>，否则 pattern 与键互不匹配（飞书 D10 静默失效缺陷同源）。
/// 任何 SCAN/Keys 的模式都不得绕过本方法裸拼。
/// </para>
/// </remarks>
internal static class WechatRedisKeyBuilder
{
    /// <summary>分隔符——所有键段之间使用 <c>:</c> 连接。</summary>
    private const string Separator = ":";

    /// <summary>转义分隔符——段内的 <c>:</c> 替换为 <c>\:</c>，杜绝跨段碰撞（R-20）。</summary>
    private const string EscapedSeparator = @"\:";

    /// <summary>单段最大长度，防止超长键 DoS。</summary>
    private const int MaxSegmentLength = 256;

    /// <summary>转义键段中的分隔符，使段内 <c>:</c> 不会与段间分隔符混淆。</summary>
    /// <param name="segment">原始段值。</param>
    /// <returns>转义后的段值（空段返回空串）。</returns>
    public static string Escape(string segment)
    {
        if (string.IsNullOrEmpty(segment))
        {
            return string.Empty;
        }

        return segment.Replace(Separator, EscapedSeparator);
    }

    /// <summary>
    /// 组合键——强制非空前缀 + 转义各段 + <c>:</c> 分隔。
    /// </summary>
    /// <param name="prefix">键前缀（必须非空且不以 <c>*</c> 开头）。</param>
    /// <param name="segments">零或多个键段（空段跳过）。</param>
    /// <returns>组合后的 Redis 键。</returns>
    /// <exception cref="InvalidOperationException">前缀为空或以 <c>*</c> 开头（R-01 护栏）。</exception>
    /// <exception cref="WechatRedisException">键段超长（<see cref="WechatRedisFailureKind.InvalidArgument"/>：
    /// 调用方输入非法不应重试，与「服务端/连接故障可降级」区分）。</exception>
    public static string Combine(string prefix, params string?[] segments)
    {
        if (string.IsNullOrEmpty(prefix))
        {
            throw new InvalidOperationException(
                "键前缀不能为空——空前缀会导致 pattern={prefix}* 退化为 * 并清空整个 Redis 库（R-01 护栏）。");
        }

        if (prefix.StartsWith("*"))
        {
            throw new InvalidOperationException(
                "键前缀不能以 '*' 开头——通配符前缀会导致 SCAN 匹配所有键（R-01 护栏）。");
        }

        var parts = new List<string> { prefix };

        foreach (var segment in segments)
        {
            if (string.IsNullOrEmpty(segment))
            {
                continue;
            }

            if (segment!.Length > MaxSegmentLength)
            {
                throw new WechatRedisException(
                    WechatRedisFailureKind.InvalidArgument,
                    $"键段长度 {segment.Length} 超过上限 {MaxSegmentLength}。");
            }

            parts.Add(Escape(segment));
        }

        return string.Join(Separator, parts);
    }

    /// <summary>
    /// 构造 SCAN 模式：glob 字面量转义后的 <c>Combine(prefix, segments…)</c> + <c>:*</c>（R2-02 单一出口）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>不变量</b>：任何用于 SCAN/Keys 的模式都必须经本方法产出（与键同源，段级转义一致）。
    /// 以 <c>:*</c> 结尾（而非裸 <c>*</c>）保证段级精确匹配，防止前缀兄弟键越界。
    /// </para>
    /// </remarks>
    /// <param name="prefix">键前缀（必须非空且不以 <c>*</c> 开头）。</param>
    /// <param name="segments">零或多个键段。</param>
    /// <returns>SCAN 模式（以 <c>:*</c> 结尾）。</returns>
    /// <exception cref="InvalidOperationException">前缀为空或以 <c>*</c> 开头。</exception>
    /// <exception cref="WechatRedisException">键段超长。</exception>
    public static string Pattern(string prefix, params string?[] segments)
        => EscapeGlobLiteral(Combine(prefix, segments)) + Separator + "*";

    /// <summary>
    /// 把字面量片段转为 Redis glob 字面量（通配元字符与转义符全部转义）。
    /// </summary>
    /// <remarks>
    /// Redis 的 <c>SCAN MATCH</c>（<c>Keys(pattern:)</c>）使用 <c>stringmatchlen</c> 语义：
    /// <c>\</c> 转义后继字符；<c>*</c> / <c>?</c> / <c>[</c> / <c>]</c> 为通配元字符。
    /// <see cref="Combine"/> 产出的字面量（其中 <c>\</c> 已用于转义 <c>:</c>）必须再转义一次，
    /// 否则 pattern 与键互不匹配、SCAN 零命中（飞书 D10 静默失效缺陷）。
    /// </remarks>
    /// <param name="literal">字面量片段。</param>
    /// <returns>可直接拼接通配符的 glob 字面量。</returns>
    public static string EscapeGlobLiteral(string? literal)
    {
        if (string.IsNullOrEmpty(literal))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(literal!.Length + 8);
        foreach (var c in literal)
        {
            switch (c)
            {
                // '\' 必须自转义；'*'/'?'/'['/']' 是 stringmatchlen 的通配元字符。
                case '\\':
                case '*':
                case '?':
                case '[':
                case ']':
                    sb.Append('\\');
                    break;
            }

            sb.Append(c);
        }

        return sb.ToString();
    }

    /// <summary>
    /// 构造令牌域键 <c>{prefix}:token:{storeKey}</c>（RD2：<c>storeKey</c> 为不透明叶子，原样拼接不转义）。
    /// </summary>
    /// <param name="prefix">键前缀（已规范化）。</param>
    /// <param name="storeKey">三段式持久层键 <c>{tokenType}:{appKey}:{scopeKey}</c>（与 InMemory 键空间逐字节一致）。</param>
    /// <returns>Redis 键。</returns>
    public static string TokenKey(string prefix, string? storeKey)
        => Combine(prefix, "token") + Separator + (storeKey ?? string.Empty);

    /// <summary>
    /// 剥离令牌域键的已知字面量前缀 <c>{prefix}:token:</c>，还原 <c>storeKey</c>（RD2 按长度剥离，unambiguous）。
    /// </summary>
    /// <param name="key">Redis 键。</param>
    /// <param name="prefix">键前缀（已规范化）。</param>
    /// <param name="storeKey">剥离后的三段式持久层键。</param>
    /// <returns>前缀匹配成功返回 <c>true</c>；否则 <c>false</c>（非令牌域键，如其它模块共存同库）。</returns>
    public static bool TryStripTokenKeyPrefix(string? key, string prefix, out string storeKey)
    {
        storeKey = string.Empty;
        var domainPrefix = Combine(prefix, "token") + Separator;
        if (key is null || key.Length == 0 || !key.StartsWith(domainPrefix, StringComparison.Ordinal))
        {
            return false;
        }

        storeKey = key.Substring(domainPrefix.Length);
        return true;
    }
}
