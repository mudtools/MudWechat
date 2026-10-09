// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// <b>Capital（设计方案里的「资金应用 / 提现」）的处置裁决</b>守卫。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何要有一组用例</b>：设计方案 §2.5 的域表把 <c>Capital</c> 与其余域并列（记
/// 「普通商户文档中心无入口；服务商侧 <c>…/partner/4012476670</c>」），若不把「为什么不实现」
/// 写成可执行的裁决，后来者会照那行去补一个<b>本仓商户模型根本调不动</b>的接口。
/// </para>
/// <para>
/// <b>本轮核验到的事实（2026-10-09）</b>：该入口页的标题是「<b>平台预约提现</b>」，
/// 产品归属为「<b>平台收付通 - 电商交易解决方案 → 账户资金管理 → 商户提现</b>」，
/// 官方标注<b>支持商户：【平台商户】</b>（更新时间 2024.12.19），
/// 接口为 <c>POST /v3/merchant/fund/withdraw</c>。
/// </para>
/// <para>
/// <b>裁决：不在本仓支付线增量范围内</b>，理由三条：
/// </para>
/// <list type="number">
/// <item>
/// <b>商户形态不匹配</b>：本仓商户基座（<see cref="Mud.Wechat.Pay.Abstractions.Configuration.WechatPayMerchantConfig"/>）
/// 精确覆盖「普通商户」与「服务商（<c>sp_mchid</c> + <c>sub_mchid</c>）」两形态，
/// 而该产品的调用方是<b>平台商户</b>（平台收付通体系）—— 该形态在本仓<b>不存在</b>。
/// </item>
/// <item>
/// <b>它不是「一个端点」，而是一整套产品面</b>：资金/提现依赖平台收付通的账户与结算语义
/// （二级商户、账本、结算账户…）。只补 <c>/v3/merchant/fund/withdraw</c> 会给出一个
/// <b>必填参数无从填写</b>的接口 —— 属「看起来可用实则不可用」，比不实现更糟。
/// </item>
/// <item>
/// <b>归属应是独立产品线</b>：与 P3 的第三方平台同理（见小程序线 P3 裁决守卫），
/// 引入平台收付通须先有<b>该体系的商户形态与令牌/签名归属</b>，不能在既有支付线里顺手加。
/// </item>
/// </list>
/// <para>
/// <b>解除条件</b>：若将来确定要做平台收付通，须先引入「平台商户」形态（配置 + 分槽 + 守卫），
/// 再按域增量落地，并<b>同批</b>解除本裁决。
/// </para>
/// </remarks>
public class WechatPayCapitalRulingContractGuards
{
    /// <summary>Capital 族的路径前缀（官方原文：<c>/v3/merchant/fund/…</c>）。</summary>
    private const string CapitalRoutePrefix = "/v3/merchant/fund";

    /// <summary>
    /// C1：支付线<b>全部</b>声明式接口都<b>不得</b>出现 Capital 族路由（防「照域表补一个调不动的接口」）。
    /// </summary>
    [Fact]
    public void CapitalEndpoints_ShouldStayUnmodeled()
    {
        var routes = typeof(IWechatPayTransactionsService).Assembly.GetTypes()
            .Where(static t => t.IsInterface && t.IsPublic)
            .SelectMany(static t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            .SelectMany(static m => m.GetCustomAttributes(false))
            .Select(static a => a.GetType().GetProperty("RequestUri")?.GetValue(a) as string)
            .Where(static uri => !string.IsNullOrWhiteSpace(uri))
            .ToArray();

        // 防「发现机制失效导致白名单真空」的静默空跑。
        routes.Should().NotBeEmpty("支付线声明式路由的收集口径必须有效");

        routes.Where(r => r!.StartsWith(CapitalRoutePrefix, StringComparison.Ordinal))
            .Should().BeEmpty(
                "Capital（资金应用/提现）的调用方是**平台商户**（平台收付通体系），"
                + "本仓商户基座只覆盖普通商户/服务商 ⇒ 不得在本支付线内建模；"
                + "若将来引入平台收付通，须先落「平台商户」形态再同批解除本裁决");
    }

    /// <summary>
    /// C2：<b>本仓商户形态确实只有「普通商户 / 服务商」两种</b>（裁决的事实前提，机械化锁定）。
    /// </summary>
    /// <remarks>
    /// 判据取「配置类型上是否存在表述平台商户身份的成员」这一<b>可反射的近似</b>：
    /// 平台收付通必然需要一个平台商户号/子商户体系，而本仓配置里与之最接近的
    /// <c>AuthorizationMchId</c>（服务商的 <c>sp_mchid</c> 用于签名归属）语义完全不同。
    /// <b>刻意不断言「将来也不会有」</b>：本用例锁的是「当前形态」，随形态扩展同批改写。
    /// </remarks>
    [Fact]
    public void MerchantModel_ShouldOnlyCoverRegularAndServiceProvider()
    {
        var configType = typeof(Mud.Wechat.Pay.Abstractions.Configuration.WechatPayMerchantConfig);

        configType.GetProperty("MchId").Should().NotBeNull("普通商户形态");
        configType.GetProperty("AuthorizationMchId").Should().NotBeNull(
            "服务商形态（签名归属 mchid = sp_mchid）");

        foreach (var name in new[] { "PlatformMchId", "MerchantCategory", "AccountType" })
        {
            configType.GetProperty(name).Should().BeNull(
                "本仓未引入「平台商户」形态 ⇒ Capital 族无调用身份可承载");
        }
    }
}
