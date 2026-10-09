// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Mud.Wechat.Pay.Abstractions.Configuration;
using Mud.Wechat.Pay.Abstractions.Credential;
using Mud.Wechat.Pay.DataModels.Transactions;
using Mud.Wechat.Pay.Transactions;

namespace Mud.Wechat.Pay.Tests.Transactions;

/// <summary>
/// 小程序调起支付签名用例（官方 P1-a 清单的「小程序调起支付」端点，docId <c>4012791898</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>采用「独立复算」断言</b>（与 <c>WechatPayAuthorizationHandlerTests</c> 同款技法）：
/// 从产出的参数里取出时间戳 / 随机串 / <c>package</c>，按官方规范<b>手工</b>重建签名串
/// （<b>不</b>调用被测的组装方法），再用<b>公钥</b>验签。
/// 这样任何一处错位（字段顺序、换行、<c>prepay_id=</c> 前缀、<c>appId</c> 被忽略）都会失败，
/// 而「生产代码自证生产代码」的循环论证不会。
/// </para>
/// </remarks>
public class WechatPayMiniProgramPaySignServiceTests
{
    private const string AppId = "wx1234567890abcdef";
    private const string PrepayId = "wx201410272009395522657a690389285100";

    /// <summary>独立复算 + 公钥验签：签名串必为 <c>appId\ntimeStamp\nnonceStr\npackage\n</c>。</summary>
    [Fact]
    public async Task CreatePaySignAsync_ShouldProduceVerifiableSignature()
    {
        var (service, publicKey, _) = CreateService();

        var result = await service.CreatePaySignAsync(AppId, PrepayId);

        // 手工重建（刻意不复用生产组装方法）。
        var expectedMessage = AppId + "\n" + result.TimeStamp + "\n" + result.NonceStr + "\n" + result.Package + "\n";

        var verified = publicKey.VerifyData(
            Encoding.UTF8.GetBytes(expectedMessage),
            Convert.FromBase64String(result.PaySign!),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        verified.Should().BeTrue(
            "paySign 必须是对 appId\\ntimeStamp\\nnonceStr\\npackage\\n 的 RSA-SHA256 签名（任一字节漂移即调起失败）");
    }

    /// <summary><c>package</c> 固定为 <c>prepay_id={prepay_id}</c>，<c>signType</c> 固定 <c>RSA</c>。</summary>
    [Fact]
    public async Task CreatePaySignAsync_ShouldUseOfficialPackageAndSignType()
    {
        var (service, _, _) = CreateService();

        var result = await service.CreatePaySignAsync(AppId, PrepayId);

        result.Package.Should().Be("prepay_id=" + PrepayId, "官方格式固定带 prepay_id= 前缀；传裸值会签名不匹配");
        result.SignType.Should().Be("RSA", "官方 signType 仅支持 RSA");
    }

    /// <summary>时间戳为<b>秒级</b> 10 位数字（毫秒会校验失败）。</summary>
    [Fact]
    public async Task CreatePaySignAsync_ShouldUseSecondPrecisionTimestamp()
    {
        var (service, _, _) = CreateService();

        var result = await service.CreatePaySignAsync(AppId, PrepayId);

        result.TimeStamp.Should().MatchRegex("^[0-9]{10}$", "官方要求 10 位秒级时间戳（毫秒级需转换为秒）");
        long.Parse(result.TimeStamp!, System.Globalization.CultureInfo.InvariantCulture)
            .Should().BeCloseTo(DateTimeOffset.UtcNow.ToUnixTimeSeconds(), 60);
    }

    /// <summary>随机串不超过 32 位（官方上限），且两次调用不同（密码学随机）。</summary>
    [Fact]
    public async Task CreatePaySignAsync_ShouldGenerateFreshNonceWithinOfficialLimit()
    {
        var (service, _, _) = CreateService();

        var first = await service.CreatePaySignAsync(AppId, PrepayId);
        var second = await service.CreatePaySignAsync(AppId, PrepayId);

        first.NonceStr!.Length.Should().BeLessThanOrEqualTo(32);
        first.NonceStr.Should().NotBe(second.NonceStr, "随机串必须逐次唯一（防重放的第一道前提）");
    }

    /// <summary><b>appId 参与签名</b>：换 appId 必换签名（防「忽略入参」的静默错位）。</summary>
    [Fact]
    public async Task CreatePaySignAsync_ShouldBindSignatureToAppId()
    {
        var (service, _, _) = CreateService();

        var result = await service.CreatePaySignAsync(AppId, PrepayId);
        var other = await service.CreatePaySignAsync("wx-other-appid", PrepayId);

        other.PaySign.Should().NotBe(result.PaySign, "appId 是签名串首项，必须真正参与签名");
    }

    /// <summary>官方字段名锁定（驼峰写法，不得「规范化」）。</summary>
    [Fact]
    public void PaySignDto_ShouldKeepOfficialCamelCaseFieldNames()
    {
        JsonNameShouldBe(nameof(WechatPayMiniProgramPaySign.TimeStamp), "timeStamp");
        JsonNameShouldBe(nameof(WechatPayMiniProgramPaySign.NonceStr), "nonceStr");
        JsonNameShouldBe(nameof(WechatPayMiniProgramPaySign.Package), "package");
        JsonNameShouldBe(nameof(WechatPayMiniProgramPaySign.SignType), "signType");
        JsonNameShouldBe(nameof(WechatPayMiniProgramPaySign.PaySign), "paySign");
    }

    /// <summary>空白入参 ⇒ 点名异常（编程错误，不得静默产出无效签名）。</summary>
    [Theory]
    [InlineData("", "wx-prepay")]
    [InlineData("  ", "wx-prepay")]
    [InlineData("wx-appid", "")]
    [InlineData("wx-appid", "   ")]
    public async Task CreatePaySignAsync_ShouldRejectBlankArguments(string appId, string prepayId)
    {
        var (service, _, _) = CreateService();

        var act = async () => await service.CreatePaySignAsync(appId, prepayId);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    // ---- helpers -------------------------------------------------------------

    private static void JsonNameShouldBe(string propertyName, string expectedJsonName)
    {
        var property = typeof(WechatPayMiniProgramPaySign).GetProperty(propertyName);
        property.Should().NotBeNull($"{propertyName} 必须存在（契约面漂移）");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(expectedJsonName);
    }

    /// <summary>真实 RSA 密钥对 + 商户上下文 / 签名工厂替身（只替身，密码学全真）。</summary>
    private static (WechatPayMiniProgramPaySignService Service, RSA PublicKey, RSA PrivateKey) CreateService()
    {
        var privateKey = RSA.Create(2048);
        var publicKey = RSA.Create();
        publicKey.ImportParameters(privateKey.ExportParameters(includePrivateParameters: false));

        var merchant = new WechatPayMerchantConfig
        {
            MchId = "1900000000",
            SerialNumber = "SERIAL-MERCHANT-0",
            PrivateKeySecretName = "pay:test:key",
            ApiKeySecretName = "pay:test:apikey",
        };

        var provider = new WechatPaySignatureProvider(
            merchant.SerialNumber, privateKey, new WechatPayPlatformCertificateCache());

        var service = new WechatPayMiniProgramPaySignService(
            new FakeMerchantContext(merchant), new FakeSignatureProviderFactory(provider));

        return (service, publicKey, privateKey);
    }

    private sealed class FakeMerchantContext : IWechatPayMerchantContext
    {
        private readonly WechatPayMerchantConfig _merchant;

        public FakeMerchantContext(WechatPayMerchantConfig merchant) => _merchant = merchant;

        public WechatPayMerchantConfig ResolveCurrent() => _merchant;

        public IDisposable UseMerchant(string merchantKey) => throw new NotSupportedException();
    }

    private sealed class FakeSignatureProviderFactory : IWechatPaySignatureProviderFactory
    {
        private readonly IWechatPaySignatureProvider _provider;

        public FakeSignatureProviderFactory(IWechatPaySignatureProvider provider) => _provider = provider;

        public Task<IWechatPaySignatureProvider> GetOrCreateAsync(
            WechatPayMerchantConfig merchant, CancellationToken cancellationToken = default)
            => Task.FromResult(_provider);
    }
}
