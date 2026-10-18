// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mud.Wechat.Abstractions.Exceptions;

namespace Mud.Wechat.Demo.Shared;

/// <summary>
/// Demo 统一业务异常出口：把各线的 <see cref="WechatApiException"/>（公众号 <c>MpException</c>、
/// 小程序 <c>WxaException</c>、企微 <c>WechatWorkException</c> 等均派生自它）映射为
/// <c>502</c> + <c>{errorCode, errorMessage, requestUri}</c>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何需要本闸</b>：SDK 的令牌链与手工二进制通道是「抛异常」语义（区别于生成客户端的
/// 「HTTP 200 + errcode 非零、由调用方判错」）。无本闸时，一次 <c>invalid appid</c> 会以未处理异常
/// 形态返回 500 并带完整堆栈 —— Demo 的使用者看不到错误码，只会看到一段 SDK 内部栈。
/// </para>
/// <para>
/// <b>可回显的依据</b>：<see cref="WechatApiException"/> 的 <c>Message</c> 是官方 errmsg，
/// <see cref="WechatApiException.RequestUri"/> 已在<b>构造期</b>剥离 query 与 userinfo
/// （凭据以 Query 承载 ⇒ 落点在异常基类，不在 Demo 侧二次过滤）。本闸不写堆栈、不写内部类型名。
/// </para>
/// <para>
/// <b>第二段 catch 打的是「包装形态」，不是「吞异常」</b>：组件（<c>Mud.HttpUtils</c> 的
/// <c>TokenManagerBase</c>）在<b>负缓存窗口</b>内抑制刷新时抛 <see cref="InvalidOperationException"/>，
/// 把真正的 <see cref="WechatApiException"/> 放在 <c>InnerException</c>（实测消息
/// 「令牌刷新在负缓存窗口内被抑制（ScopeKey=…）：微信 API 调用失败：errcode=40013 …」）。
/// 该包装不派生自 <see cref="WechatApiException"/>，故第一段 catch 接不住 ⇒ 未处理异常直接穿透，
/// Development 下由开发者异常页把<b>整段 SDK 堆栈</b>写进响应体（各线同病，与小店线无关，
/// 只是「先用假凭据连打两次」才撞到：第一次 40013 建立负缓存，第二次进抑制窗口）。
/// 判据取「内层必须是 <see cref="WechatApiException"/>」这一条，普通 <c>InvalidOperationException</c>
/// （配置错误、DI 解析失败等）仍原样上抛 —— 那是编程错误，映射成 502 等于把它藏起来。
/// </para>
/// </remarks>
public static class DemoApiErrorGate
{
    /// <summary>挂上业务异常映射闸，须在 <c>app.Run()</c> 之前调用。</summary>
    /// <param name="app">应用构建器。</param>
    public static WebApplication UseWechatApiErrorGate(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next().ConfigureAwait(false);
            }
            catch (WechatApiException ex) when (!context.Response.HasStarted)
            {
                await WriteApiErrorAsync(context, ex, suppression: null).ConfigureAwait(false);
            }
            catch (InvalidOperationException ex) when (
                !context.Response.HasStarted && ex.InnerException is WechatApiException apiException)
            {
                await WriteApiErrorAsync(context, apiException, ex.Message).ConfigureAwait(false);
            }
        });

        return app;
    }

    private static async Task WriteApiErrorAsync(HttpContext context, WechatApiException ex, string? suppression)
    {
        context.Response.StatusCode = StatusCodes.Status502BadGateway;
        await context.Response.WriteAsJsonAsync(new
        {
            errorCode = ex.ErrorCode,
            errorMessage = suppression ?? ex.Message,
            requestUri = ex.RequestUri,
            hint = "官方返回的业务错误码（令牌链或手工二进制通道以异常承载）；"
                + "声明式 JSON 端点的 errcode 不走本闸，须由调用方判 IsSuccess。"
                + (suppression == null
                    ? string.Empty
                    : " 本条是组件的令牌刷新负缓存（短期内重复取令牌会被直接抑制，不再打官方接口）。"),
        }).ConfigureAwait(false);
    }
}
