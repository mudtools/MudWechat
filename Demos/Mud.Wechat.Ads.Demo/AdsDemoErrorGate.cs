// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.Abstractions.Exceptions;

namespace Mud.Wechat.Ads.Demo;

/// <summary>
/// 广告线错误映射闸：把「需要重新授权」与「官方业务失败」拆成两种**语义不同**的应答。
/// </summary>
/// <remarks>
/// <para>
/// 本线异常 <see cref="WechatAdsException"/> <b>派生自</b> <c>WechatApiException</c> ⇒ 通用闸
/// <c>UseWechatApiErrorGate()</c> 本来就能接住，因此本闸不是「补漏」而是**分流**：
/// </para>
/// <list type="bullet">
/// <item><description><see cref="WechatAdsReauthorizationRequiredException"/> ⇒ <b>401</b>。
/// 它的可处置性与「官方报错」完全不同：refresh_token 是一次性凭据，刷新失败后旧值<b>已被 SDK 删除</b>
/// （ADS-B3 锁定「先删库后抛」的顺序即契约），此时任何重试都无意义，唯一出路是引导用户重新走完 OAuth。
/// 让它和 502 混在一起，读者会去重试而不是去重新授权。</description></item>
/// <item><description>其余 <see cref="WechatAdsException"/> ⇒ <b>502</b> + <c>messageCn</c> 单列字段。
/// 官方信封是 <c>{code, message, message_cn, data}</c>，中文支在异常消息里已被基类追加进文本，
/// 但作为<b>独立字段</b>回显才好被脚本断言（守卫 ADS-B2 锁定 <c>message_cn</c> 字段名不驼峰化）。</description></item>
/// </list>
/// <para>
/// <b>顺序纪律</b>：通用闸先注册、本闸后注册（<c>app.Use</c> 越晚注册越靠内、越先捕获）。写反不报错，
/// 只会把 401 退化成 502、把 <c>messageCn</c> 吞进一段英文消息里。
/// </para>
/// <para>
/// <b>catch 顺序不可交换</b>：<see cref="WechatAdsReauthorizationRequiredException"/> 是
/// <see cref="WechatAdsException"/> 的派生类型，写在后面是 CS0160 编译错误（不是「运行期不生效」）。
/// </para>
/// <para>
/// 消息可回显的依据：异常的消息与 <c>RequestUri</c> 由公用基类在构造期剥离 query 与 userinfo，
/// 而本线的 <c>access_token</c> / <c>timestamp</c> / <c>nonce</c> / <c>client_secret</c> 全在 Query
/// （守卫 ADS-B5 核验参数名 ⊆ 组件脱敏词表）⇒ 本闸不追加任何凭据字段。
/// </para>
/// </remarks>
internal static class AdsDemoErrorGate
{
    /// <summary>挂接广告线错误映射（须晚于 <c>UseWechatApiErrorGate</c>）。</summary>
    /// <param name="app">应用。</param>
    /// <returns>应用（链式）。</returns>
    public static WebApplication UseWechatAdsErrorGate(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next().ConfigureAwait(false);
            }
            catch (WechatAdsReauthorizationRequiredException ex) when (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    appKey = ex.AppKey,
                    errorCode = ex.ErrorCode,
                    errorMessage = ex.Message,
                    hint = "401 而非 502：refresh_token 是一次性凭据，刷新不可继续时 SDK 已把库里的授权状态删除。"
                        + "重试同一个端点永远不会成功 —— 请回到 GET /api/authorize-url 重新引导用户授权，"
                        + "再用 POST /api/auth/exchange 换取新令牌对。",
                }).ConfigureAwait(false);
            }
            catch (WechatAdsException ex) when (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status502BadGateway;
                await context.Response.WriteAsJsonAsync(new
                {
                    errorCode = ex.ErrorCode,
                    errorMessage = ex.Message,
                    messageCn = ex.MessageCn,
                    hint = "腾讯广告官方业务失败（信封 {code, message, message_cn, data}，code != 0）。"
                        + "注意本线 code 是整数但语义与微信 errcode 完全不同，失效码集合与判定器**不得**跨线套用。",
                }).ConfigureAwait(false);
            }
        });

        return app;
    }
}
