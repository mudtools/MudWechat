// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;
using Mud.Wechat.Pay.V2.Security;

namespace Mud.Wechat.Pay.Tests.Security;

/// <summary>
/// APIv2 <c>workwx_sign</c>（企业红包 / 向员工付款两支的双签）判定器测试。
/// </summary>
/// <remarks>
/// <para>
/// 与 <see cref="Mud.Wechat.Pay.Tests.Transport.WechatPayV2TransportPipelineTests"/> 同一口径：
/// 期望值在<b>本文件内按官方规范独立重算</b>（排序 → 拼串 → 追加 <c>secret=</c> → MD5 大写十六进制），
/// <b>不复用</b>生产实现 —— 否则「实现把串尾写成 <c>key=</c>」这类错误会跟着断言一起绿。
/// </para>
/// <para>
/// 本族的两条红线也在这里锁死：① <c>workwx_sign</c> 只签<b>官方白名单</b>字段
/// （多签一个 <c>wishing</c> 即 SIGNERROR）；② <c>workwx_sign</c> <b>参与</b> <c>sign</c>
/// （官方「除 sign 外全部参与，包括 workwx_sign」）⇒ 顺序颠倒产出的 <c>sign</c> 必然错。
/// </para>
/// </remarks>
public class WechatPayV2WorkWxSignerTests
{
    /// <summary>企业微信应用 Secret（32 位；任何异常消息都不得回显它）。</summary>
    private static readonly string WxSecret = new string('s', 32);

    /// <summary>发放企业红包的官方白名单（照抄官方本页，与生产常量各自成文以便互相校验）。</summary>
    private static readonly string[] RedPackWhitelist =
    {
        "act_name", "mch_billno", "mch_id", "nonce_str", "re_openid", "total_amount", "wxappid",
    };

    /// <summary>
    /// 白名单外字段（<c>agentid</c> / <c>wishing</c> / <c>remark</c>）进了签名串即错。
    /// </summary>
    [Fact]
    public void Sign_ShouldIgnoreNonWhitelistFields_WhenRequestCarriesExtraFields()
    {
        var fields = RedPackFields();
        fields.Add(new KeyValuePair<string, string?>("agentid", "1000002"));
        fields.Add(new KeyValuePair<string, string?>("wishing", "辛苦了"));
        fields.Add(new KeyValuePair<string, string?>("remark", "季度奖金"));

        var actual = WechatPayV2WorkWxSigner.Sign(fields, RedPackWhitelist, WxSecret);

        actual.Should().Be(Expected(RedPackFields()));
    }

    /// <summary>官方串尾是 <c>secret=&lt;Secret&gt;</c>，写成 <c>sign</c> 用的 <c>key=</c> 即错。</summary>
    [Fact]
    public void Sign_ShouldAppendSecretKeyword_WhenCanonicalStringBuilt()
    {
        var fields = RedPackFields();

        var withKey = Md5Upper(Canonical(fields, RedPackWhitelist) + "key=" + WxSecret);
        var withSecret = Md5Upper(Canonical(fields, RedPackWhitelist) + "secret=" + WxSecret);

        withKey.Should().NotBe(withSecret, "because 两条串尾只差一个词，写错的症状是官方 SIGNERROR 而非明确报错");
        WechatPayV2WorkWxSigner.Sign(fields, RedPackWhitelist, WxSecret).Should().Be(withSecret);
    }

    /// <summary>算法恒为 MD5，不随 <c>sign_type=HMAC-SHA256</c> 变化（官方本页即如此规定）。</summary>
    [Fact]
    public void Sign_ShouldStayMd5_WhenRequestDeclaresHmacSha256ForMainSign()
    {
        var fields = RedPackFields();
        fields.Add(new KeyValuePair<string, string?>("sign_type", "HMAC-SHA256"));

        var expected = Md5Upper(Canonical(fields, RedPackWhitelist) + "secret=" + WxSecret);
        var hmac = HMACSHA256.HashData(Encoding.UTF8.GetBytes(WxSecret),
            Encoding.UTF8.GetBytes(Canonical(fields, RedPackWhitelist) + "secret=" + WxSecret));

        WechatPayV2WorkWxSigner.Sign(fields, RedPackWhitelist, WxSecret)
            .Should().Be(expected)
            .And.NotBe(ToUpperHex(hmac), "because workwx_sign 官方只给 MD5，跟着 sign_type 走会算出另一个值");
    }

    /// <summary>
    /// <b>顺序锁定</b>：<c>workwx_sign</c> 入表后必须参与 <c>sign</c> 的规范化串
    /// （除 <c>sign</c> 外全参与）⇒ 反过来先 <c>sign</c> 后 <c>workwx_sign</c> 必然对不上。
    /// </summary>
    [Fact]
    public void CanonicalString_ShouldIncludeWorkWxSign_ButExcludeSign_WhenBothPresent()
    {
        var fields = RedPackFields();
        fields.Add(new KeyValuePair<string, string?>(
            WechatPayV2SignatureBuilder.WorkWxSignFieldName, "A1B2C3D4E5F6A7B8C9D0E1F2A3B4C5D6"));
        fields.Add(new KeyValuePair<string, string?>(WechatPayV2SignatureBuilder.SignFieldName, "deadbeef"));

        var canonical = WechatPayV2SignatureBuilder.BuildCanonicalString(fields);

        canonical.Should().Contain("workwx_sign=");
        canonical.Should().NotContain("deadbeef", "because sign 自身不参与签名串");

        // ASCII 序里 workwx_sign 落在 total_amount 与 wxappid 之间（'o' < 'x'）——
        // 排序口径若被人改成「不区分大小写」或「按属性名」，这一节就会挪位，签名跟着变。
        canonical.Should().Contain("&workwx_sign=A1B2C3D4E5F6A7B8C9D0E1F2A3B4C5D6&wxappid=");
    }

    /// <summary>白名单字段一个都没序列化出来 ⇒ fail-fast，不把配置缺失发出去换一次 SIGNERROR。</summary>
    [Fact]
    public void Sign_ShouldThrow_WhenNoWhitelistFieldPresent()
    {
        var fields = new List<KeyValuePair<string, string?>>
        {
            new("agentid", "1000002"),
            new("wishing", "辛苦了"),
        };

        var error = Assert.Throws<ArgumentException>(
            () => WechatPayV2WorkWxSigner.Sign(fields, RedPackWhitelist, WxSecret));

        error.Message.Should().Contain(WechatPayV2WorkWxSigner.MissingWhitelistFieldMessage);
    }

    /// <summary>空 Secret 直接拒，且异常消息不回显 Secret 值（PAY-B7 同线红线）。</summary>
    [Fact]
    public void Sign_ShouldThrowWithoutEchoingSecret_WhenSecretIsEmpty()
    {
        var error = Assert.Throws<ArgumentException>(
            () => WechatPayV2WorkWxSigner.Sign(RedPackFields(), RedPackWhitelist, string.Empty));

        error.Message.Should().NotContain(WxSecret);
    }

    /// <summary>白名单常量本身也是契约面：少一个字段即全量商户的双签报文被拒。</summary>
    [Fact]
    public void Whitelist_ShouldMatchOfficialFieldSets()
    {
        WechatPayV2WorkWxSigner.SendRedPackWhitelist.Should().BeEquivalentTo(RedPackWhitelist);

        WechatPayV2WorkWxSigner.PayPocketWhitelist.Should().BeEquivalentTo(
            "amount", "appid", "desc", "mch_id", "nonce_str", "openid", "partner_trade_no", "ww_msg_type");
    }

    /// <summary>幂等判定：大小写不敏感地识别字段表里已有的 <c>workwx_sign</c>。</summary>
    [Fact]
    public void HasWorkWxSignature_ShouldMatchCaseInsensitively()
    {
        WechatPayV2WorkWxSigner
            .HasWorkWxSignature(new[] { new KeyValuePair<string, string?>("WorkWx_Sign", "X") })
            .Should().BeTrue();

        WechatPayV2WorkWxSigner
            .HasWorkWxSignature(new[] { new KeyValuePair<string, string?>("sign", "X") })
            .Should().BeFalse();
    }

    private static List<KeyValuePair<string, string?>> RedPackFields() => new()
    {
        new("wxappid", "ww1234567890abcdef"),
        new("mch_id", "1900000000"),
        new("mch_billno", "1900000000202610180001"),
        new("nonce_str", "ABCDEF1234567890"),
        new("re_openid", "oABCD0EXAMPLE0openid"),
        new("total_amount", "100"),
        new("act_name", "季度奖金"),
    };

    /// <summary>按官方规范独立重算：白名单内非空字段 ASCII 升序拼 <c>k=v&amp;</c>。</summary>
    private static string Canonical(
        IEnumerable<KeyValuePair<string, string?>> fields, IEnumerable<string> whitelist)
    {
        var set = new HashSet<string>(whitelist, StringComparer.OrdinalIgnoreCase);

        var builder = new StringBuilder();
        foreach (var field in fields
                     .Where(f => !string.Equals(f.Key, "sign", StringComparison.OrdinalIgnoreCase))
                     .Where(f => set.Contains(f.Key))
                     .Where(f => !string.IsNullOrEmpty(f.Value))
                     .OrderBy(f => f.Key, StringComparer.Ordinal))
        {
            builder.Append(field.Key).Append('=').Append(field.Value).Append('&');
        }

        return builder.ToString();
    }

    private static string Expected(List<KeyValuePair<string, string?>> fields)
        => Md5Upper(Canonical(fields, RedPackWhitelist) + "secret=" + WxSecret);

    private static string Md5Upper(string message)
        => ToUpperHex(MD5.HashData(Encoding.UTF8.GetBytes(message)));

    private static string ToUpperHex(byte[] hash)
    {
        var builder = new StringBuilder(hash.Length * 2);
        foreach (var b in hash)
        {
            builder.Append(b.ToString("X2"));
        }

        return builder.ToString();
    }
}
