// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 应用管线扩展：接入公众号回调中间件。
/// </summary>
/// <remarks>
/// <b>为何不提供 <c>MapXxx</c> 端点扩展</b>：回调需要「原始字节 + 立即回写」，
/// 走 MVC/Minimal API 的模型绑定会丢失原始请求体并引入无谓反序列化开销（与企微侧一致）。
/// </remarks>
public static class MpApplicationBuilderExtensions
{
    /// <summary>
    /// 接入公众号回调中间件（路由 <c>/{GlobalRoutePrefix}/{AppKey}</c>，默认前缀 <c>mp</c>）。
    /// </summary>
    /// <param name="app">应用建造者。</param>
    /// <returns>应用建造者（链式）。</returns>
    public static IApplicationBuilder UseMpCallback(this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        return app.UseMiddleware<Mud.Wechat.OfficialAccount.Callback.MpCallbackMiddleware>();
    }
}
