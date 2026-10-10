// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels;
using Mud.Wechat.MiniProgram.DataModels.Auth;
using Mud.Wechat.MiniProgram.DataModels.DataAnalysis;
using Mud.Wechat.MiniProgram.DataModels.QrCodeLink;
using Mud.Wechat.MiniProgram.DataModels.Security;
using System.Text.Json.Serialization.Metadata;

namespace Mud.Wechat.MiniProgram.Extensions;

/// <summary>
/// 小程序线 AOT JsonContext 合并器（对齐公众号 <c>MpJsonResolverExtensions</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须有它</b>：生成的实现类构造期是
/// <c>contentSerializer ?? HttpContentSerializerFactory.CreateDefault()</c>，
/// 而该工厂的 <b>AOT 分支只组合源生成上下文、绝不回退反射</b> —— 不登记本线上下文时，
/// DTO 在 Native AOT 下<b>没有元数据</b>，且失败发生在<b>第一次真实调用</b>
/// （JIT 下因有反射兜底而一路正常，最易漏测）。
/// </para>
/// <para>
/// <b>组件该方法仅在 net8.0 / net10.0 存在</b>，故以 <c>NET8_0_OR_GREATER</c> 裁剪；
/// 本线继承根 props 的 4 档 TFM，netstandard2.0 / net6.0 走 JIT 反射路径（不参与本仓 AOT 门禁）。
/// </para>
/// </remarks>
public static class MiniProgramJsonResolverExtensions
{
#if NET8_0_OR_GREATER
    /// <summary>把小程序线全部 AOT JsonContext 登记进组件的序列化选项管线。</summary>
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
            AuthJsonContext.Default,
            QrCodeLinkJsonContext.Default,
            SecurityJsonContext.Default,
            DataAnalysisJsonContext.Default);

        services.AddMudHttpClientJsonContext(resolver);
    }
#endif
}
