// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Tests.ContractGuards;

namespace Mud.Wechat.Pay.Tests.Credential;

/// <summary>
/// <see cref="WechatPayMerchantCredentialProvider"/> 单元测试（P0-c 凭据取用）。
/// </summary>
public class WechatPayMerchantCredentialProviderTests
{
    private const string PrivateKeyName = "pay:merchant:1900000000:key";
    private const string ApiKeyName = "pay:merchant:1900000000:apikey";

    /// <summary>
    /// 取回的私钥必须能<b>复现黄金签名</b> —— 端到端证明「密钥名 → ISecretProvider → PEM 解析 → RSA」整条链路无损。
    /// </summary>
    [Fact]
    public async Task LoadPrivateKeyAsync_ShouldProduceUsableKey_WhenPemRegistered()
    {
        var provider = CreateProvider(new Dictionary<string, string>
        {
            [PrivateKeyName] = WechatPayGoldenVectors.MerchantPrivateKeyPem,
        });

        using var rsa = await provider.LoadPrivateKeyAsync(CreateConfig());

        rsa.KeySize.Should().Be(2048, "商户 API 证书为 RSA-2048");

        var message = Encoding.UTF8.GetString(
            Convert.FromBase64String(WechatPayGoldenVectors.RequestMessageB64));
        Convert.ToBase64String(rsa.SignData(
                Encoding.UTF8.GetBytes(message), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
            .Should().Be(WechatPayGoldenVectors.RequestSignature,
                "because 经 ISecretProvider 取回并解析的私钥必须与黄金向量同钥，链路任一环节失真都会改变签名");
    }

    /// <summary>
    /// 私钥 PEM 不可解析时抛出，且<b>异常消息不得回显 PEM</b>（否则私钥随日志/APM 二次扩散、不可撤回）。
    /// </summary>
    [Fact]
    public async Task LoadPrivateKeyAsync_ShouldNotEchoPem_WhenPemUnparseable()
    {
        const string bogus = "-----BEGIN PRIVATE KEY-----\nNOT-A-REAL-KEY\n-----END PRIVATE KEY-----\n";
        var provider = CreateProvider(new Dictionary<string, string> { [PrivateKeyName] = bogus });

        var act = () => provider.LoadPrivateKeyAsync(CreateConfig());

        var exception = (await act.Should().ThrowAsync<InvalidOperationException>()).Which;
        exception.Message.Should().Contain("1900000000", "异常须指明是哪个商户的密钥有问题");
        exception.Message.Should().NotContain("NOT-A-REAL-KEY",
            "解密失败的原文不得进异常消息（PAY-B7）");
    }

    /// <summary>
    /// 密钥名在 <c>ISecretProvider</c> 中查无时抛出；异常消息中的名字必须<b>截断</b> ——
    /// 若宿主把密钥值本身填进了 <c>*SecretName</c>，整段回显等于把密钥洒进日志。
    /// </summary>
    [Fact]
    public async Task LoadPrivateKeyAsync_ShouldTruncateSecretName_WhenSecretNotFound()
    {
        const string name = "pay:merchant:1900000000:key:with-a-very-long-suffix-that-must-be-cut";
        var provider = CreateProvider(new Dictionary<string, string>());

        var config = CreateConfig();
        config.PrivateKeySecretName = name;

        var act = () => provider.LoadPrivateKeyAsync(config);

        var exception = (await act.Should().ThrowAsync<InvalidOperationException>()).Which;
        exception.Message.Should().NotContain(name,
            "密钥名可能就是被误填的密钥值，整段写进异常等于把它带进日志链路");
        exception.Message.Should().Contain("PrivateKeySecretName",
            "截断后仍须保留属性名，便于定位是哪一处配置");
        exception.Message.Should().Contain("密钥名",
            "并点明该字段存的是名字而非值 —— 这正是最常见的误配");
    }

    /// <summary>APIv3 密钥按 UTF-8 取 32 字节（官方固定长度）。</summary>
    [Fact]
    public async Task LoadApiKeyAsync_ShouldReturn32Bytes_WhenRegistered()
    {
        const string expected = "0123456789abcdef0123456789abcdef";
        var provider = CreateProvider(new Dictionary<string, string> { [ApiKeyName] = expected });

        var key = await provider.LoadApiKeyAsync(CreateConfig());

        key.Length.Should().Be(WechatPayMerchantCredentialProvider.ApiKeySizeBytes);
        Convert.ToBase64String(key).Should().Be(Convert.ToBase64String(Encoding.UTF8.GetBytes(expected)));
    }

    /// <summary>
    /// 密钥长度不符时必须点名说明「32 字节」——
    /// 下沉到 AES-GCM 侧只会抛 <c>CryptographicException</c>，那里的消息不指认配置语境。
    /// </summary>
    [Theory]
    [InlineData("too-short")]
    [InlineData("0123456789abcdef0123456789abcdefEXTRA")]
    public async Task LoadApiKeyAsync_ShouldExplainRequiredLength_WhenWrongLength(string value)
    {
        var provider = CreateProvider(new Dictionary<string, string> { [ApiKeyName] = value });

        var act = () => provider.LoadApiKeyAsync(CreateConfig());

        var exception = (await act.Should().ThrowAsync<InvalidOperationException>()).Which;
        exception.Message.Should().Contain("必须为 32 字节");
    }

    /// <summary>入参为 null 时按参数错误处理（不得 NRE）。</summary>
    [Fact]
    public async Task LoadAsync_ShouldThrowArgumentNull_WhenConfigIsNull()
    {
        var provider = CreateProvider(new Dictionary<string, string>());

        await FluentActions.Awaiting(() => provider.LoadPrivateKeyAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>();
        await FluentActions.Awaiting(() => provider.LoadApiKeyAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    /// <summary>构造器对 <c>ISecretProvider</c> 为 null 必须 fail-fast（静默 null 会在首次取密钥时 NRE）。</summary>
    [Fact]
    public void Constructor_ShouldThrow_WhenSecretProviderIsNull()
    {
        var act = () => new WechatPayMerchantCredentialProvider(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>辅助：以给定密钥表构造取用器与一份合法商户配置。</summary>
    private static WechatPayMerchantCredentialProvider CreateProvider(
        IDictionary<string, string> secrets)
        => new(new InMemorySecretProvider(secrets));

    /// <summary>辅助：合法普通商户配置（密钥名与本文件常量一致）。</summary>
    private static WechatPayMerchantConfig CreateConfig() => new()
    {
        MchId = "1900000000",
        SerialNumber = "SERIAL",
        PrivateKeySecretName = PrivateKeyName,
        ApiKeySecretName = ApiKeyName,
    };
}
