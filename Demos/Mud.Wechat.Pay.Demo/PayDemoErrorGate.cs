// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Demo.Shared;
using Mud.Wechat.Pay.Abstractions.Exceptions;

namespace Mud.Wechat.Pay.Demo;

/// <summary>
/// 支付线错误映射闸：把 <see cref="WechatPayException"/> 的<b>字符串</b>官方码映射成 502 JSON。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么不复用 <see cref="DemoApiErrorGate"/> 就够了</b>：公用层判错契约的错误码槽位是
/// <c>int</c>，而 APIv3 只有 <c>{"code":"ORDER_NOT_EXIST"}</c> 这类字符串码，
/// 基类只能给出 <see cref="WechatPayException.NumericFailureSentinel"/>（-1）。
/// 只看 <c>errorCode</c> 的宿主会把所有支付失败看成同一个码 —— 排查时必须去翻日志，
/// 而 Demo 的用途恰恰是「把失败原因直接摆在响应里」。
/// </para>
/// <para>
/// 因此本闸<b>必须注册在通用闸之后</b>（<c>app.Use</c> 的语义是「越晚注册越靠内，越靠内越先捕获异常」，
/// 与「先注册的先处理请求」正好相反 —— 顺序写反不会报错，只会让支付失败退化成 <c>errorCode:-1</c>，
/// 是实测踩到的静默失效）。本闸把 <c>payErrorCode</c> 原样回显，
/// 其余 <c>WechatApiException</c>（公众号/小程序/企微那些整数码）继续由外层通用闸兜底。
/// </para>
/// <para><c>RequestUri</c> 在异常构造期已脱敏（查询串与 userinfo 已剥离），可安全回显。</para>
/// </remarks>
internal static class PayDemoErrorGate
{
    /// <summary>挂接支付线错误映射。</summary>
    /// <param name="app">应用。</param>
    /// <returns>应用（链式）。</returns>
    public static WebApplication UseWechatPayErrorGate(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next().ConfigureAwait(false);
            }
            catch (WechatPayException ex) when (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status502BadGateway;
                await context.Response.WriteAsJsonAsync(new
                {
                    payErrorCode = ex.PayErrorCode,
                    errorMessage = ex.Message,
                    requestUri = ex.RequestUri,
                    hint = "微信支付 APIv3 的业务失败（HTTP 4xx/5xx + code/message）。"
                        + "权威码见 payErrorCode 原文（WechatPayErrorCodes 常量表），"
                        + "errorCode 槽位对支付线恒为 -1（APIv3 无整数错误码）。",
                }).ConfigureAwait(false);
            }
        });

        return app;
    }
}
