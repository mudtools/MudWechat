// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;
using Mud.Wechat.OfficialAccount.Web;

namespace Mud.Wechat.OfficialAccount.Tests.Web;

/// <summary>
/// 与**官方样例代码逐行对齐**的一致性用例（样例目录：<c>.docs/公众号服务号/sample</c> 的 java / node / python / php）。
/// </summary>
/// <remarks>
/// <para>
/// 做法：把四份官方样例各自的签名算法**原样翻译为参考实现**（Reference* 方法），
/// 再对本 SDK 的 <see cref="MpJsApiSignature"/> 逐组参数比对 —— 任一差异都会红。
/// </para>
/// <para>
/// 参考实现与本 SDK 的**刻意差异**（已核对为安全/正确方向，不参与等价比对）：
/// node 样例用 <c>jsSHA</c> 的 <c>HEX</c> 输出（大写），本 SDK 与文档向量/Java/Python/PHP 一致取**小写**；
/// 四份样例均<b>不</b>剥离 <c>#</c> 片段（要求调用方传入页面真实 URL），本 SDK 主动剥离（文档明确要求，见 V7）。
/// </para>
/// </remarks>
public class MpJsApiSignatureSampleParityTests
{
    // —— 官方 java/sign.java 的核心（硬编码顺序 + 「参数名必须全部小写，且必须有序」+ SHA-1 %02x）——
    private static string ReferenceJava(string ticket, string nonce, long timestamp, string url)
        => Sha1Hex("jsapi_ticket=" + ticket + "&noncestr=" + nonce +
                   "&timestamp=" + timestamp.ToString(System.Globalization.CultureInfo.InvariantCulture) +
                   "&url=" + url);

    // —— 官方 php/jssdk.php 的核心（同一硬编码顺序 + sha1 小写）——
    private static string ReferencePhp(string ticket, string nonce, long timestamp, string url)
        => Sha1Hex("jsapi_ticket=$ticket&noncestr=$nonce&timestamp=$timestamp&url=$url"
            .Replace("$ticket", ticket).Replace("$nonce", nonce)
            .Replace("$timestamp", timestamp.ToString(System.Globalization.CultureInfo.InvariantCulture))
            .Replace("$url", url));

    // —— 官方 node/check_sign 的 raw()：keys.sort() + toLowerCase + '&k=v' 拼接（jsSHA 结果转小写以对齐文档向量）——
    private static string ReferenceNode(string ticket, string nonce, long timestamp, string url)
    {
        var args = new Dictionary<string, string>
        {
            ["jsapi_ticket"] = ticket,
            ["nonceStr"] = nonce, // 官方 node 样例以驼峰 nonceStr 参与，靠 toLowerCase 归一
            ["timestamp"] = timestamp.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["url"] = url,
        };

        var keys = args.Keys.OrderBy(k => k, StringComparer.Ordinal).ToList();
        var builder = new StringBuilder();
        for (var i = 0; i < keys.Count; i++)
        {
            if (i > 0)
            {
                builder.Append('&');
            }

            builder.Append(keys[i].ToLowerInvariant()).Append('=').Append(args[keys[i]]);
        }

        return Sha1Hex(builder.ToString());
    }

    // —— 官方 python/sign.py 的 sign()：sorted(原键) + key.lower() 拼接 + sha1 hexdigest ——
    private static string ReferencePython(string ticket, string nonce, long timestamp, string url)
    {
        var ret = new Dictionary<string, string>
        {
            ["nonceStr"] = nonce,
            ["jsapi_ticket"] = ticket,
            ["timestamp"] = timestamp.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["url"] = url,
        };

        var parts = ret.Keys
            .OrderBy(k => k, StringComparer.Ordinal)
            .Select(k => k.ToLowerInvariant() + "=" + ret[k]);
        return Sha1Hex(string.Join("&", parts));
    }

    private static string Sha1Hex(string text)
    {
        using var sha1 = SHA1.Create();
        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(text));
        var builder = new StringBuilder(hash.Length * 2);
        foreach (var b in hash)
        {
            builder.Append(b.ToString("x2", System.Globalization.CultureInfo.InvariantCulture));
        }

        return builder.ToString();
    }

    /// <summary>参数矩阵：覆盖四份样例各自的典型入参形态（含各样例的 nonce 长度风格）。</summary>
    public static TheoryData<string, string, long, string> Cases => new()
    {
        // 官方文档附录1 的样例向量
        {
            "sM4AOVdWfPE4DxkXGEs8VMCPGGVi4C3VM0P37wVUCFvkVAy_90u5h9nbSlYy3-Sl-HhTdfl2fzFy1AOcHKP7qg",
            "Wm3WZYTPz0wzccnW", 1414587457, "http://mp.weixin.qq.com?params=value"
        },
        // java/node/python 样例的占位入参
        { "jsapi_ticket", "Wm3WZYTPz0wzccnW", 1414587457, "http://example.com" },
        // node 样例 15 位 nonce
        { "ticket-abc", "n0kqgvi2ml0mji5r", 1700000000, "https://a.example.com/p" },
        // python 样例 15 位混合大小写
        { "TICKET", "aB3dEfGhIjKlMnO", 1700000001, "https://a.example.com/p?x=1&y=2" },
        // java 样例 UUID 形态（36 位含连字符，验证 nonce 长度不受算法约束）
        {
            "ticket-with-_and=chars", "f81d4fae-7dec-11d0-a765-00a0c91e6bf6", 1700000002,
            "http://wx.example.com/a/b?c=d#e"
        },
        // 大时间戳 + 空查询串
        { "t", "abcdefghijklmnop", 2147483647, "https://b.example.com" },
    };

    /// <summary>本 SDK 与 **java / php** 两份样例（硬编码顺序）逐例一致。</summary>
    [Theory]
    [MemberData(nameof(Cases))]
    public void ShouldMatchJavaAndPhpSamples(string ticket, string nonce, long timestamp, string url)
    {
        var actual = MpJsApiSignature.Compute(ticket, nonce, timestamp, url);

        actual.Should().Be(ReferenceJava(ticket, nonce, timestamp, MpJsApiSignature.NormalizeUrl(url)));
        actual.Should().Be(ReferencePhp(ticket, nonce, timestamp, MpJsApiSignature.NormalizeUrl(url)));
    }

    /// <summary>本 SDK 与 **node** 样例（排序 + toLowerCase）逐例一致。</summary>
    [Theory]
    [MemberData(nameof(Cases))]
    public void ShouldMatchNodeSample(string ticket, string nonce, long timestamp, string url)
    {
        var normalized = MpJsApiSignature.NormalizeUrl(url);

        MpJsApiSignature.Compute(ticket, nonce, timestamp, normalized)
            .Should().Be(ReferenceNode(ticket, nonce, timestamp, normalized));
    }

    /// <summary>本 SDK 与 **python** 样例（sorted + key.lower）逐例一致。</summary>
    [Theory]
    [MemberData(nameof(Cases))]
    public void ShouldMatchPythonSample(string ticket, string nonce, long timestamp, string url)
    {
        var normalized = MpJsApiSignature.NormalizeUrl(url);

        MpJsApiSignature.Compute(ticket, nonce, timestamp, normalized)
            .Should().Be(ReferencePython(ticket, nonce, timestamp, normalized));
    }

    /// <summary>签名输出形态：小写 40 位十六进制（与文档向量/java/python/php 一致；node 的 jsSHA HEX 为大写，属其输出形态差异）。</summary>
    [Theory]
    [MemberData(nameof(Cases))]
    public void SignatureShape_ShouldBeLowercaseHex40(string ticket, string nonce, long timestamp, string url)
    {
        MpJsApiSignature.Compute(ticket, nonce, timestamp, url)
            .Should().MatchRegex("^[0-9a-f]{40}$");
    }

    /// <summary>nonce 长度不受算法约束（样例分别为 15/16/36 位），本 SDK 取 16 位（与 PHP 样例一致）。</summary>
    [Fact]
    public void NonceLength_ShouldMatchPhpSample()
    {
        MpJsApiSignature.CreateNonceStr().Should().HaveLength(16);
    }
}