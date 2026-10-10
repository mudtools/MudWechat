// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Ads.DataModels.Adgroups;
using Mud.Wechat.Ads.DataModels.Advertiser;
using Mud.Wechat.Ads.DataModels.Common;
using Mud.Wechat.Ads.DataModels.OAuth;
using Mud.Wechat.Ads.DataModels.Reports;

namespace Mud.Wechat.Ads.Abstractions.Extensions;

/// <summary>
/// 广告线 AOT JsonContext 合并器（形态照 <c>PayJsonResolverExtensions</c> / <c>WechatJsonResolverExtensions</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须有它</b>：生成的实现类构造期取的是 <c>IOptions&lt;JsonSerializerOptions&gt;.TypeInfoResolver</c>，
/// 未登记时回退组件默认序列化器，而后者在 AOT 分支<b>只</b>合并库内置的 <c>MudHttpJsonContext.Default</c>、
/// <b>绝不</b>回退反射 ⇒ 广告 DTO 在 Native AOT 下没有元数据。
/// 失败点在<b>第一次真实应答反序列化</b>（JIT 下靠反射侥幸可用，最易漏测），故登记必须在注册入口内完成。
/// </para>
/// <para>
/// <b>为什么落在 Abstractions 而不是主包</b>：OAuth 两支端点是<b>手写</b>传输（令牌注入与业务客户端分面，
/// 见 <see cref="Transport.AdsHttpClientNames"/>），只调 <c>AddAdsApp</c> 而不装任何业务模块的宿主
/// 也要能换码 —— 把 <c>OAuthJsonContext</c> 登记在主包里会让这条最小路径在 AOT 下静默失败。
/// 因此本类登记<b>全部</b>广告线上下文，由 <c>AddAdsApp</c> 单点调用。
/// </para>
/// <para>
/// <b>新增域上下文必须同批追加到这里</b>（守卫 ADS-B6 逐个数 <c>DataModels/Generated/*JsonContext.g.cs</c>
/// 的文件名并要求每个都出现在本方法体内 —— 漏一个即编译期假绿、运行期 AOT 失败，所以钉成硬断言）。
/// </para>
/// <para>组件该方法仅在 <c>net8.0</c> / <c>net10.0</c> 存在 ⇒ 以 <c>NET8_0_OR_GREATER</c> 裁剪；
/// 低 TFM 走 JIT 反射路径（不参与本仓 AOT 门禁）。上下文键空间与库内置不重叠，合并顺序无关。</para>
/// </remarks>
internal static class AdsJsonResolverExtensions
{
#if NET8_0_OR_GREATER
    /// <summary>把广告线全部 AOT JsonContext 登记进组件的序列化选项管线。</summary>
    /// <param name="services">服务集合。</param>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 为 <c>null</c>。</exception>
    internal static void ConfigureDataModelsResolver(IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var resolver = JsonTypeInfoResolver.Combine(
            CommonJsonContext.Default,
            OAuthJsonContext.Default,
            AdvertiserJsonContext.Default,
            AdgroupsJsonContext.Default,
            ReportsJsonContext.Default);

        services.AddMudHttpClientJsonContext(resolver);
    }
#endif
}
