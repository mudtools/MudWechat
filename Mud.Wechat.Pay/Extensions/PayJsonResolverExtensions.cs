// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Bill;
using Mud.Wechat.Pay.DataModels.Certificates;
using Mud.Wechat.Pay.DataModels.Common;
using Mud.Wechat.Pay.DataModels.Refund;
using Mud.Wechat.Pay.DataModels.Transactions;
using System.Text.Json.Serialization.Metadata;

namespace Mud.Wechat.Pay.Extensions;

/// <summary>
/// 微信支付线 AOT JsonContext 合并器（对齐 <c>WechatJsonResolverExtensions.ConfigureDataModelsResolver</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须有它</b>：生成的实现类构造期是
/// <c>contentSerializer ?? HttpContentSerializerFactory.CreateDefault()</c>。
/// 组件的 DI 注册会把 <c>IOptions&lt;JsonSerializerOptions&gt;</c> 传给该工厂，
/// 而工厂的 <b>AOT 分支只组合源生成上下文、绝不回退反射</b> ——
/// 若此处不登记 <c>TransactionsJsonContext</c>，支付 DTO 在 Native AOT 下<b>没有元数据</b>，
/// 且失败发生在<b>第一笔真实交易</b>（JIT 下因有反射兜底而一路正常，最易漏测）。
/// </para>
/// <para>
/// <b>组件该方法仅在 net8.0 / net10.0 存在</b>（实测 <c>PublicAPI.Shipped.txt</c> 无 net6.0 / netstandard2.0 条目），
/// 故此处与企微线同样以 <c>NET8_0_OR_GREATER</c> 裁剪；net6.0 走 JIT 反射路径（该 TFM 不参与本仓 AOT 门禁）。
/// </para>
/// <para>上下文键空间与库内置 <c>MudHttpJsonContext.Default</c> 不重叠，合并顺序无关（按序命中）。</para>
/// </remarks>
public static class PayJsonResolverExtensions
{
#if NET8_0_OR_GREATER
    /// <summary>把支付线全部 AOT JsonContext 登记进组件的序列化选项管线。</summary>
    /// <param name="services">服务集合。</param>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 为 <c>null</c>。</exception>
    public static void ConfigureDataModelsResolver(IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var resolver = JsonTypeInfoResolver.Combine(
            CommonJsonContext.Default,
            TransactionsJsonContext.Default,
            RefundJsonContext.Default,
            BillJsonContext.Default,
            CertificatesJsonContext.Default);

        services.AddMudHttpClientJsonContext(resolver);
    }
#endif
}
