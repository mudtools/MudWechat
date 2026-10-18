// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.Abstractions;
using Mud.Wechat.Pay.Abstractions.Exceptions;

namespace Mud.Wechat.AllInOne.Demo;

/// <summary>
/// 线专属异常映射闸：兜住<b>不被公用错误闸覆盖</b>的那两族异常。
/// </summary>
/// <remarks>
/// <para>
/// <b>同宿主里共存着三种异常族、两套错误码口径</b>，这是单线 Demo 看不到的图景：
/// </para>
/// <list type="table">
/// <item><term>企微 / 公众号 / 小店</term><description><c>WechatApiException</c>（小店线的 <c>WechatChannelsException</c> 派生自它）⇒ 公用闸 <c>UseWechatApiErrorGate()</c> 足够，错误码是<b>整数</b> errcode。</description></item>
/// <item><term>微信支付 APIv3</term><description><c>WechatPayException</c> ⇒ 权威码是<b>字符串</b>（<c>ORDER_NOT_EXIST</c>…）；它在公用闸的 <c>errorCode</c> 槽位恒为 <c>-1</c>，只看那个槽会把所有支付失败看成同一个码。</description></item>
/// <item><term>开放平台</term><description><c>WechatOpenPlatformException</c> <b>直接继承 <see cref="Exception"/></b>（凭证链自成一套令牌体系，未并入公用层异常族）⇒ 公用闸根本不接，不挂本闸就是 500 + SDK 内部堆栈写进响应体。</description></item>
/// </list>
/// <para>
/// <b>顺序纪律（反直觉，写反不报错只静默失效）</b>：<c>app.Use</c> 越<b>晚</b>注册越<b>靠内</b>，
/// 越靠内越<b>先</b>捕获异常。故本闸必须注册在公用闸<b>之后</b>；写反时支付失败退化成 <c>errorCode:-1</c>、
/// 开放平台失败退化成开发者异常页。
/// </para>
/// <para>
/// 消息可回显的依据：两族异常的消息侧均已约束不得含 appsecret / ticket / 令牌原文，
/// <c>WechatWorkException.RequestUri</c> 在构造期剥离 query 与 userinfo（AGENTS §8）⇒ 本闸不追加任何凭据字段。
/// </para>
/// </remarks>
internal static class AllInOneErrorGates
{
    /// <summary>挂接支付线与开放平台线的错误映射（须晚于 <c>UseWechatApiErrorGate</c>）。</summary>
    public static WebApplication UseAllInOneLineErrorGates(this WebApplication app)
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
                    hint = "微信支付 APIv3 的业务失败：权威码是字符串（payErrorCode），errorCode 槽位对本线恒为 -1。",
                }).ConfigureAwait(false);
            }
            catch (WechatOpenPlatformException ex) when (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status502BadGateway;
                await context.Response.WriteAsJsonAsync(new
                {
                    errorCode = ex.ErrorCode,
                    errorMessage = ex.Message,
                    hint = "开放平台凭证链失败（票据未到达 / 官方报错 / 应答不含令牌）。"
                        + "本线异常不派生自 WechatApiException ⇒ 只有本闸能接住。",
                }).ConfigureAwait(false);
            }
        });

        return app;
    }
}
