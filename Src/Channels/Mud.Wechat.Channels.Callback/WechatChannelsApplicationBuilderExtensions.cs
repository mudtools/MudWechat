// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.Callback;

/// <summary>微信小店回调中间件接入扩展。</summary>
public static class WechatChannelsApplicationBuilderExtensions
{
    /// <summary>
    /// 接入微信小店回调中间件（路由 <c>/{GlobalRoutePrefix}/{AppKey}</c>；前缀与凭据请求期热更）。
    /// </summary>
    /// <param name="app">应用构建器。</param>
    /// <returns>应用构建器（链式）。</returns>
    /// <remarks>
    /// 经典约定式 <c>RequestDelegate</c> 中间件（不进 DI 泛型派发）；须在
    /// <c>AddWechatChannelsCallback</c> 之后调用（依赖接收器与分发器单例）。
    /// <b>组合根末端封闭注册表</b>（v1 方案 F15①/D12）：处理器 / 拦截器 / 结构化应答三张注册表
    /// <c>Seal()</c> 冻结，运行期再注册即抛。
    /// </remarks>
    public static IApplicationBuilder UseWechatChannelsWebhook(this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        app.ApplicationServices.GetRequiredService<ChannelsCallbackHandlerRegistry>().Seal();
        app.ApplicationServices.GetRequiredService<ChannelsCallbackInterceptorRegistry>().Seal();
        app.ApplicationServices.GetRequiredService<ChannelsCallbackReplyRegistry>().Seal();

        return app.UseMiddleware<WechatChannelsCallbackMiddleware>();
    }
}
