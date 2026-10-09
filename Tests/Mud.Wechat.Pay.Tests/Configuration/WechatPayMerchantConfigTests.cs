// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Tests.Configuration;

/// <summary>
/// <see cref="WechatPayMerchantConfig"/> 单元测试（P0-c 凭据基座配置面）。
/// </summary>
public class WechatPayMerchantConfigTests
{
    /// <summary>普通商户形态：商户键即商户号。</summary>
    [Fact]
    public void MerchantKey_ShouldBeMchId_WhenOrdinaryMerchant()
    {
        var config = CreateOrdinary();

        config.Invoking(c => c.Validate()).Should().NotThrow("合法普通商户配置必须通过启动期校验");
        config.IsServicePartner.Should().BeFalse();
        config.MerchantKey.Should().Be("1900000000");
    }

    /// <summary>服务商形态：商户键为 <c>{sp_mchid}:{sub_mchid}</c> 复合（子商户号跨服务商可重复）。</summary>
    [Fact]
    public void MerchantKey_ShouldCompositeSpAndSub_WhenServicePartner()
    {
        var config = new WechatPayMerchantConfig
        {
            SpMchId = "1900000001",
            SubMchId = "1600000001",
            SerialNumber = "SERIAL",
            PrivateKeySecretName = "pay:key",
            ApiKeySecretName = "pay:apikey",
        };

        config.Invoking(c => c.Validate()).Should().NotThrow();
        config.IsServicePartner.Should().BeTrue();
        config.MerchantKey.Should().Be("1900000001:1600000001");
    }

    /// <summary>两形态互斥：官方服务商请求体不发送 <c>mchid</c>，共存会让 DTO 映射无从择一。</summary>
    [Fact]
    public void Validate_ShouldRejectBothFormsCoexisting_WhenMchIdSetForServicePartner()
    {
        var config = new WechatPayMerchantConfig
        {
            MchId = "1900000000",
            SpMchId = "1900000001",
            SubMchId = "1600000001",
            SerialNumber = "SERIAL",
            PrivateKeySecretName = "pay:key",
            ApiKeySecretName = "pay:apikey",
        };

        config.Invoking(c => c.Validate())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*不得同时填写 MchId*");
    }

    /// <summary>服务商两字段必须成对（只给其一即畸形，不得静默当作普通商户）。</summary>
    [Theory]
    [InlineData("1900000001", "")]
    [InlineData("", "1600000001")]
    public void Validate_ShouldRejectIncompleteServicePartner_WhenOnlyOneOfSpSubSet(
        string spMchId, string subMchId)
    {
        var config = new WechatPayMerchantConfig
        {
            SpMchId = spMchId,
            SubMchId = subMchId,
            SerialNumber = "SERIAL",
            PrivateKeySecretName = "pay:key",
            ApiKeySecretName = "pay:apikey",
        };

        config.Invoking(c => c.Validate())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*必须同时提供 SpMchId 与 SubMchId*");
    }

    /// <summary>普通商户缺商户号即非法。</summary>
    [Fact]
    public void Validate_ShouldRejectMissingMchId_WhenOrdinaryMerchant()
    {
        var config = new WechatPayMerchantConfig
        {
            SerialNumber = "SERIAL",
            PrivateKeySecretName = "pay:key",
            ApiKeySecretName = "pay:apikey",
        };

        config.Invoking(c => c.Validate())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*MchId*");
    }

    /// <summary>证书序列号必填（官方靠它反查公钥，缺失即全量验签失败）。</summary>
    [Fact]
    public void Validate_ShouldRejectMissingSerialNumber_WhenAbsent()
    {
        var config = CreateOrdinary();
        config.SerialNumber = "   ";

        config.Invoking(c => c.Validate())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*SerialNumber*");
    }

    /// <summary>
    /// 关键安全用例：<c>*SecretName</c> 被填成<b>密钥原文</b>时必须启动期点名拒绝。
    /// </summary>
    /// <remarks>
    /// 这是最常见的配置事故，且后果隐蔽：不拦的话，症状是运行期 <c>ISecretProvider</c> 查无此名，
    /// 抛出「找不到名字 -----BEGIN ...」这类离奇错误，<b>并把私钥头带进异常消息</b>。
    /// </remarks>
    [Fact]
    public void Validate_ShouldRejectKeyMaterialPastedIntoSecretName_WhenPemMarkerDetected()
    {
        var config = CreateOrdinary();
        config.PrivateKeySecretName = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBg";

        config.Invoking(c => c.Validate())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*密钥原文*");

        // 且异常消息不得回显密钥本身。
        var exception = Record.Exception(() => config.Validate());
        exception!.Message.Should().NotContain("MIIEvQIBADANBg",
            "拒绝理由本身也不能把疑似密钥内容带进日志链路");
    }

    /// <summary>
    /// 反向用例：名字里**恰好含** <c>MII</c> 子串的正常密钥名不得被误判为密钥材料。
    /// </summary>
    /// <remarks>
    /// <c>Validate()</c> 在启动期硬失败 ⇒ 误判即部署阻断。判定必须保守到「只抓住真密钥体」。
    /// </remarks>
    [Fact]
    public void Validate_ShouldAcceptNormalSecretName_WhenItMerelyContainsMiiSubstring()
    {
        var config = CreateOrdinary();
        config.PrivateKeySecretName = "vault:secret/mii-service:merchant-key";

        config.Invoking(c => c.Validate()).Should().NotThrow(
            "MII 若只是子串且不构成 Base64 密钥体，不得误判为密钥原文");
    }

    /// <summary>商户号含 <c>:</c> 会被拼进商户键 ⇒ 键别义 ⇒ 跨商户凭据串号，必须拒绝。</summary>
    [Theory]
    [InlineData("1900000000:evil")]
    [InlineData("19 00000000")]
    [InlineData("1900000000\n")]
    public void Validate_ShouldRejectSeparatorOrWhitespaceInMchId_WhenAliasable(string mchId)
    {
        var config = CreateOrdinary();
        config.MchId = mchId;

        config.Invoking(c => c.Validate())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*非法字符*");
    }

    /// <summary>
    /// 结构性红线：配置面<b>不得</b>出现任何可写「密钥值」属性。
    /// </summary>
    /// <remarks>
    /// 一旦有人图方便加 <c>public string ApiKey { get; set; }</c>（存密钥本身而非密钥名），
    /// 配置文件即成为私钥载体并随配置中心/版本库扩散，且不可撤回。
    /// 本断言把「只存名字、值走 <c>ISecretProvider</c>」锁成不可静默退化的契约。
    /// </remarks>
    [Fact]
    public void ConfigSurface_ShouldContainNoWritableKeyMaterialProperty_WhenInspected()
    {
        var writable = typeof(WechatPayMerchantConfig)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .Select(p => p.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();

        writable.Should().Equal(
            new[]
            {
                "ApiKeySecretName", "MchId", "PrivateKeySecretName",
                "SerialNumber", "SpMchId", "SubMchId",
            },
            "because 可写属性恰为这 6 个；多出任何一项都可能是把密钥值引进配置面");

        writable.Should().NotContain(n =>
            n.Contains("PrivateKey", StringComparison.OrdinalIgnoreCase) &&
            !n.EndsWith("SecretName", StringComparison.Ordinal),
            "私钥只能以「名字」出现");
    }

    /// <summary>辅助：构造一份合法的普通商户配置。</summary>
    private static WechatPayMerchantConfig CreateOrdinary() => new()
    {
        MchId = "1900000000",
        SerialNumber = "SERIAL",
        PrivateKeySecretName = "pay:merchant:1900000000:key",
        ApiKeySecretName = "pay:merchant:1900000000:apikey",
    };
}
