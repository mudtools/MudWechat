// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Tests.Credential;

/// <summary>
/// <see cref="WechatPayMerchantManager"/> 单元测试（P0-c 多商户基座）。
/// </summary>
public class WechatPayMerchantManagerTests
{
    /// <summary>注册顺序必须保留（诊断输出与「哪个商户先注册」的可追溯性）。</summary>
    [Fact]
    public void Merchants_ShouldPreserveRegistrationOrder_WhenMultipleRegistered()
    {
        var manager = new WechatPayMerchantManager(new[]
        {
            CreateOrdinary("1900000000"),
            CreateOrdinary("1900000001"),
            CreateServicePartner("1900000002", "1600000001"),
        });

        manager.Merchants.Select(m => m.MerchantKey)
            .Should().Equal(new[] { "1900000000", "1900000001", "1900000002:1600000001" },
                "because 注册顺序是诊断信息的一部分，不得因内部字典实现而丢失");
    }

    /// <summary>空集合即配置错误 —— 支付无默认商户可回落，必须启动期即失败。</summary>
    [Fact]
    public void Constructor_ShouldRejectEmptyCollection_WhenNoMerchantRegistered()
    {
        var act = () => new WechatPayMerchantManager(Array.Empty<WechatPayMerchantConfig>());

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*至少需要注册一个微信支付商户*");
    }

    /// <summary>
    /// 重复商户键必须启动期失败：两份凭据争同一槽位会让后注册者<b>静默覆盖</b>，
    /// 症状是「收款落到错误商户」—— 资金侧错误，不可留到运行期。
    /// </summary>
    [Fact]
    public void Constructor_ShouldRejectDuplicateMerchantKey_WhenTwoConfigsShareKey()
    {
        var act = () => new WechatPayMerchantManager(new[]
        {
            CreateOrdinary("1900000000"),
            CreateOrdinary("1900000000"),
        });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*重复注册*");
    }

    /// <summary>配置校验在构造期执行（不得把坏配置留到首次真实交易）。</summary>
    [Fact]
    public void Constructor_ShouldValidateEachConfig_WhenAnyIsInvalid()
    {
        var invalid = CreateOrdinary("1900000000");
        invalid.SerialNumber = string.Empty;

        var act = () => new WechatPayMerchantManager(new[] { invalid });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*SerialNumber*");
    }

    /// <summary>按商户键查询：命中返回同一实例，未命中返回 false（不得回落到默认商户）。</summary>
    [Fact]
    public void TryGetMerchant_ShouldReturnFalse_WhenKeyUnknown()
    {
        var manager = CreateManager();

        manager.TryGetMerchant("1900000000", out var hit).Should().BeTrue();
        hit.Should().NotBeNull();
        hit!.MchId.Should().Be("1900000000");

        manager.TryGetMerchant("9999999999", out var miss).Should().BeFalse(
            "未知商户必须不命中；静默回落会让请求挂到错误的收款主体上");
        miss.Should().BeNull();

        manager.TryGetMerchant(null, out _).Should().BeFalse("空键视为未命中，不得抛异常");
        manager.TryGetMerchant("   ", out _).Should().BeFalse();
    }

    /// <summary>按商户键查询（显式）：未命中抛出并列出已注册键，便于一次性定位配置遗漏。</summary>
    [Fact]
    public void GetMerchant_ShouldThrowWithRegisteredKeys_WhenKeyUnknown()
    {
        var manager = CreateManager();

        var act = () => manager.GetMerchant("9999999999");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*9999999999*")
            .And.Message.Should().Contain("1900000000",
                "异常消息必须列出已注册商户键，否则宿主要靠二分排查");
    }

    /// <summary>
    /// 按官方报文标识反查 —— 回调侧定位商户的唯一入口。
    /// </summary>
    /// <remarks>普通商户报文带 <c>mchid</c>；服务商带 <c>sp_mchid</c> + <c>sub_mchid</c>。</remarks>
    [Fact]
    public void TryGetByOfficialIds_ShouldRouteByForm_WhenNotificationArrives()
    {
        var manager = CreateManager();

        manager.TryGetByOfficialIds("1900000000", null, null, out var ordinary)
            .Should().BeTrue();
        ordinary!.MchId.Should().Be("1900000000");

        // 服务商通知：此时报文中的 mchid 字段即服务商商户号，仍须按 sp + sub 定位子商户。
        manager.TryGetByOfficialIds("1900000002", "1900000002", "1600000001", out var partner)
            .Should().BeTrue();
        partner!.MerchantKey.Should().Be("1900000002:1600000001");
    }

    /// <summary>
    /// 子商户号<b>跨服务商可重复</b> ⇒ 必须 <c>sp_mchid + sub_mchid</c> 复合才唯一。
    /// </summary>
    [Fact]
    public void TryGetByOfficialIds_ShouldRequireCompositeKey_WhenSubMchIdReused()
    {
        var manager = new WechatPayMerchantManager(new[]
        {
            CreateServicePartner("1900000002", "1600000001"),
            CreateServicePartner("1900000003", "1600000001"),
        });

        manager.TryGetByOfficialIds(null, "1900000003", "1600000001", out var hit)
            .Should().BeTrue();
        hit!.SpMchId.Should().Be("1900000003",
            "同样的 sub_mchid 在不同服务商下必须命中的不是同一个商户");
    }

    /// <summary>
    /// 服务商字段只给其一 = 畸形报文 ⇒ **不命中也不猜**。
    /// </summary>
    /// <remarks>
    /// 在此「猜一个商户」等于把通知算到错误的收款主体上，属资金侧错误，
    /// 因此宁可不命中并由调用方告警。
    /// </remarks>
    [Theory]
    [InlineData("1900000002", "")]
    [InlineData("", "1600000001")]
    [InlineData(null, "1600000001")]
    public void TryGetByOfficialIds_ShouldNotGuess_WhenServicePartnerIdentityIncomplete(
        string spMchId, string subMchId)
    {
        var manager = CreateManager();

        manager.TryGetByOfficialIds(null, spMchId, subMchId, out var config)
            .Should().BeFalse("服务商标识不完整时不得猜测商户");
        config.Should().BeNull();
    }

    /// <summary>未命中的反查必须返回 false（不得抛异常，回调侧靠它区分「未知商户」与「畸形报文」）。</summary>
    [Fact]
    public void TryGetByOfficialIds_ShouldReturnFalse_WhenNoMerchantMatches()
    {
        var manager = CreateManager();

        manager.TryGetByOfficialIds("8888888888", null, null, out _).Should().BeFalse();
        manager.TryGetByOfficialIds(null, null, null, out _).Should().BeFalse(
            "全部为空 = 畸形，不得命中任何商户");
    }

    /// <summary>辅助：合法普通商户。</summary>
    private static WechatPayMerchantConfig CreateOrdinary(string mchId) => new()
    {
        MchId = mchId,
        SerialNumber = "SERIAL-" + mchId,
        PrivateKeySecretName = "pay:merchant:" + mchId + ":key",
        ApiKeySecretName = "pay:merchant:" + mchId + ":apikey",
    };

    /// <summary>辅助：合法服务商商户。</summary>
    private static WechatPayMerchantConfig CreateServicePartner(string spMchId, string subMchId) => new()
    {
        SpMchId = spMchId,
        SubMchId = subMchId,
        SerialNumber = "SERIAL-" + spMchId,
        PrivateKeySecretName = "pay:sp:" + spMchId + ":key",
        ApiKeySecretName = "pay:sp:" + spMchId + ":apikey",
    };

    /// <summary>辅助：两个普通商户 + 一个服务商的管理器。</summary>
    private static WechatPayMerchantManager CreateManager() => new(new[]
    {
        CreateOrdinary("1900000000"),
        CreateOrdinary("1900000001"),
        CreateServicePartner("1900000002", "1600000001"),
    });
}
