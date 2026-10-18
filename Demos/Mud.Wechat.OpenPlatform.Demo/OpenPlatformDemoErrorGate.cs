// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.Abstractions;

namespace Mud.Wechat.OpenPlatform.Demo;

/// <summary>
/// 开放平台线错误映射闸：把凭证链异常 <see cref="WechatOpenPlatformException"/> 映射成 502 JSON。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么通用闸接不住</b>：<c>DemoApiErrorGate</c> 捕获的是 <c>WechatApiException</c>
/// （企微/公众号/小程序那些线异常的共同基类），而本线的 <see cref="WechatOpenPlatformException"/>
/// <b>直接继承 <see cref="Exception"/></b>（凭证链是本线自成一套的令牌体系，未并入公用层异常族）⇒
/// 「票据缺失」「官方报错」「应答不含令牌」这三类失败会以未处理异常穿透，浏览器看到 500 + SDK 内部堆栈。
/// </para>
/// <para>
/// 与支付线同款的顺序纪律：<b>通用闸先注册、本闸后注册</b>（<c>app.Use</c> 越晚注册越靠内、越先捕获）。
/// 顺序写反不报错，只是把 <c>errorCode</c>（官方 errcode 字符串）换成一段堆栈。
/// </para>
/// <para>
/// 消息可回显的依据：SDK 侧已约束该异常的消息<b>不得</b>含 appsecret / ticket / 令牌原文
/// （见 <c>WechatOpenPlatformException</c> 的 remarks），本闸也不再追加任何凭据字段。
/// </para>
/// </remarks>
internal static class OpenPlatformDemoErrorGate
{
    /// <summary>挂接开放平台线错误映射。</summary>
    /// <param name="app">应用。</param>
    /// <returns>应用（链式）。</returns>
    public static WebApplication UseWechatOpenPlatformErrorGate(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next().ConfigureAwait(false);
            }
            catch (WechatOpenPlatformException ex) when (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status502BadGateway;
                await context.Response.WriteAsJsonAsync(new
                {
                    errorCode = ex.ErrorCode,
                    errorMessage = ex.Message,
                    hint = "开放平台凭证链失败（票据未到达 / 官方报错 / 应答不含令牌）。"
                        + "本线异常不派生自 WechatApiException，故由本闸映射；"
                        + "声明式 JSON 端点的 errcode 不走本闸，须由调用方判 IsSuccess。",
                }).ConfigureAwait(false);
            }
        });

        return app;
    }
}
