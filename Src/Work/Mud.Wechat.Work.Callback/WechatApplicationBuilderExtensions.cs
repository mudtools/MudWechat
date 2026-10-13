// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Callback;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 应用程序管道扩展：企业微信回调中间件接入（v1 方案 §5.8，宿主一行接入）。
/// </summary>
public static class WechatApplicationBuilderExtensions
{
    /// <summary>
    /// 启用企业微信回调中间件（默认路由 <c>/{GlobalRoutePrefix}/{AppKey}</c>，默认前缀 <c>wechat</c>）。
    /// </summary>
    /// <param name="app">应用程序构建器。</param>
    /// <returns>应用程序构建器（链式）。</returns>
    /// <remarks>
    /// <para>
    /// 需先经 <c>AddWechatCallback(...)</c> 完成服务装配；路由前缀经
    /// <see cref="WechatCallbackOptions.GlobalRoutePrefix"/> 配置（支持热更新）。
    /// </para>
    /// <para>
    /// <b>组合根末端封闭注册表</b>（v1 方案 F15①/D12）：宿主注册在此全部完成 ⇒ 处理器 / 拦截器 /
    /// bot 处理器三张注册表 <c>Seal()</c> 冻结，运行期再注册即抛（漏注册必须在组装期暴露，
    /// 而非线上首个回调才静默 unhandled）。
    /// </para>
    /// <para>
    /// 宿主需保证 ASP.NET Core 运行环境（本扩展仅在 ASP.NET Core 应用管道可用）。
    /// </para>
    /// </remarks>
    public static IApplicationBuilder UseWechatWebhook(this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        // 冻结注册表（幂等）：先于 UseMiddleware —— 管道构建期即完成封闭。
        app.ApplicationServices.GetRequiredService<WechatCallbackHandlerRegistry>().Seal();
        app.ApplicationServices.GetRequiredService<WechatCallbackInterceptorRegistry>().Seal();
        app.ApplicationServices.GetRequiredService<WechatBotHandlerRegistry>().Seal();

        return app.UseMiddleware<WechatCallbackMiddleware>();
    }
}
