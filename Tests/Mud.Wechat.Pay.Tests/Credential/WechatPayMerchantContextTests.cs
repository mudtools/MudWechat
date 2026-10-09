// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Tests.Credential;

/// <summary>
/// <see cref="WechatPayMerchantContext"/> 单元测试（P1-a 基石：决定每笔请求用哪把商户私钥签名）。
/// </summary>
public class WechatPayMerchantContextTests
{
    /// <summary>单商户即默认：最常见的宿主形态零样板，不得要求显式开作用域。</summary>
    [Fact]
    public void ResolveCurrent_ShouldReturnSoleMerchant_WhenOnlyOneRegistered()
    {
        var context = new WechatPayMerchantContext(
            new WechatPayMerchantManager(new[] { CreateOrdinary("1900000000") }));

        context.ResolveCurrent().MerchantKey.Should().Be("1900000000");
    }

    /// <summary>
    /// 多商户且未开作用域 ⇒ fail-fast。
    /// 静默挑第一个会让请求<b>签到错误商户</b>（签名有效、落在错误商户号），属最难排查的一类故障。
    /// </summary>
    [Fact]
    public void ResolveCurrent_ShouldFailFast_WhenMultipleRegisteredAndNoScopeActive()
    {
        var context = new WechatPayMerchantContext(CreateManager());

        var act = () => context.ResolveCurrent();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*UseMerchant*", "错误必须点名处置方式，否则宿主只能靠猜");
    }

    /// <summary>作用域内解析到指定商户。</summary>
    [Fact]
    public void ResolveCurrent_ShouldReturnScopedMerchant_WhenInsideUseMerchant()
    {
        var context = new WechatPayMerchantContext(CreateManager());

        using (context.UseMerchant("1900000001"))
        {
            context.ResolveCurrent().MerchantKey.Should().Be("1900000001");
        }
    }

    /// <summary>作用域释放后<b>还原上一层</b>（而非无条件清空），回到「无作用态」的默认判定。</summary>
    [Fact]
    public void ResolveCurrent_ShouldFailFast_AfterScopeDisposed_WhenMultipleMerchantsRegistered()
    {
        var context = new WechatPayMerchantContext(CreateManager());

        using (context.UseMerchant("1900000001"))
        {
        }

        var act = () => context.ResolveCurrent();

        act.Should().Throw<InvalidOperationException>().WithMessage("*UseMerchant*");
    }

    /// <summary>嵌套作用域释放时还原<b>外层</b>商户，不得把外层覆盖成 null。</summary>
    [Fact]
    public void UseMerchant_ShouldRestoreOuterMerchant_WhenNestedScopeDisposed()
    {
        var context = new WechatPayMerchantContext(CreateManager());

        using (context.UseMerchant("1900000001"))
        {
            using (context.UseMerchant("1900000002:1600000001"))
            {
                context.ResolveCurrent().MerchantKey.Should().Be("1900000002:1600000001");
            }

            context.ResolveCurrent().MerchantKey.Should().Be(
                "1900000001", "because 内层释放必须还原外层，而非清空成「无作用态」");
        }
    }

    /// <summary>重复释放幂等：第二次释放不得再覆盖一次，否则会把外层值清掉。</summary>
    [Fact]
    public void Dispose_ShouldBeIdempotent_WhenScopeDisposedTwice()
    {
        var context = new WechatPayMerchantContext(CreateManager());

        using (context.UseMerchant("1900000001"))
        {
            var inner = context.UseMerchant("1900000002:1600000001");
            inner.Dispose();
            inner.Dispose();

            context.ResolveCurrent().MerchantKey.Should().Be(
                "1900000001", "because 重复 Dispose 幂等，外层商户不得被二次清空");
        }
    }

    /// <summary>未注册的商户键在<b>进入作用域时</b>即失败，不留到真正签名才暴露。</summary>
    [Fact]
    public void UseMerchant_ShouldThrow_WhenMerchantKeyNotRegistered()
    {
        var context = new WechatPayMerchantContext(CreateManager());

        var act = () => context.UseMerchant("0000000000");

        act.Should().Throw<InvalidOperationException>().WithMessage("*0000000000*");
    }

    /// <summary>空白键直接拒绝（与「未注册」区分开）。</summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UseMerchant_ShouldThrow_WhenMerchantKeyIsBlank(string merchantKey)
    {
        var context = new WechatPayMerchantContext(CreateManager());

        var act = () => context.UseMerchant(merchantKey);

        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// 环境值须<b>流入 await 后的子异步流</b>：签名发生在传输层（已 await 过请求体序列化），
    /// 若上下文不随执行上下文流动，签名时会退回默认判定而拿到错误商户。
    /// </summary>
    [Fact]
    public async Task ResolveCurrent_ShouldFlowIntoAwaitedChild_WhenInsideUseMerchant()
    {
        var context = new WechatPayMerchantContext(CreateManager());

        using (context.UseMerchant("1900000001"))
        {
            await Task.Yield();

            var fromChild = await Task.Run(() => context.ResolveCurrent().MerchantKey);

            fromChild.Should().Be("1900000001");
        }
    }

    /// <summary>
    /// 管理器若暴露空商户表（自定义实现）也必须给出可读错误，而非默认判定静默越界。
    /// </summary>
    [Fact]
    public void ResolveCurrent_ShouldThrow_WhenManagerExposesNoMerchant()
    {
        var empty = new Mock<IWechatPayMerchantManager>();
        empty.SetupGet(m => m.Merchants).Returns(new List<WechatPayMerchantConfig>());

        var context = new WechatPayMerchantContext(empty.Object);

        var act = () => context.ResolveCurrent();

        act.Should().Throw<InvalidOperationException>().WithMessage("*AddPayApp*");
    }

    /// <summary>辅助：构造合法的普通商户。</summary>
    private static WechatPayMerchantConfig CreateOrdinary(string mchId) => new()
    {
        MchId = mchId,
        SerialNumber = "SERIAL-" + mchId,
        PrivateKeySecretName = "pay:merchant:" + mchId + ":key",
        ApiKeySecretName = "pay:merchant:" + mchId + ":apikey",
    };

    /// <summary>辅助：普通商户 + 服务商各一，构成「必须显式选商户」的多商户形态。</summary>
    private static WechatPayMerchantManager CreateManager() => new(new[]
    {
        CreateOrdinary("1900000000"),
        CreateOrdinary("1900000001"),
        new WechatPayMerchantConfig
        {
            SpMchId = "1900000002",
            SubMchId = "1600000001",
            SerialNumber = "SERIAL-1900000002",
            PrivateKeySecretName = "pay:sp:1900000002:key",
            ApiKeySecretName = "pay:sp:1900000002:apikey",
        },
    });
}
