// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Abstractions.Contracts;
using Mud.Wechat.Abstractions.Exceptions;

namespace Mud.Wechat.MiniProgram.Abstractions;

/// <summary>
/// 微信小程序业务异常：HTTP 200 + <c>errcode != 0</c>（且非令牌失效码）时抛出。
/// </summary>
/// <remarks>
/// <para>
/// 形态与公众号线 <c>MpException</c> 一致（同平台、同错误信封）；<b>类型身份独立</b>是为了让宿主的
/// <c>catch</c> 分支能区分「小程序调用失败」与「公众号调用失败」——
/// 两条线可同宿主共存（<c>AddMpApp</c> 注册的同一批应用同时被两条线消费）。
/// </para>
/// <para>令牌失效码（<c>40001</c> 等）由<b>复用</b>的公众号线失效判定器识别、交恢复链路自动重试，不经本异常。</para>
/// <para>错误码与请求地址脱敏由公用基类 <see cref="WechatApiException"/> 承担
/// （小程序登录端点的 <c>secret</c> 以 Query 承载，脱敏词表已覆盖）。</para>
/// </remarks>
public sealed class WxaException : WechatApiException
{
    /// <summary>使用错误码与消息构造异常。</summary>
    /// <param name="errorCode">微信小程序业务错误码。</param>
    /// <param name="message">错误消息。</param>
    /// <param name="requestUri">请求地址（可选，诊断用；构造期自动脱敏）。</param>
    public WxaException(int errorCode, string message, string? requestUri = null)
        : base(errorCode, message, requestUri)
    {
    }

    /// <summary>使用错误码、消息与内部异常构造异常。</summary>
    /// <param name="errorCode">微信小程序业务错误码。</param>
    /// <param name="message">错误消息。</param>
    /// <param name="innerException">内部异常。</param>
    public WxaException(int errorCode, string message, Exception innerException)
        : base(errorCode, message, innerException)
    {
    }

    /// <summary>本产品线异常工厂（静态缓存，零每调用分配）。</summary>
    private static readonly Func<int, string, string?, WechatApiException> ExceptionFactory =
        static (errorCode, message, requestUri) => new WxaException(errorCode, message, requestUri);

    /// <summary>统一判错出口：响应为 <c>null</c> 或 <c>errcode != 0</c> 时抛出本异常。</summary>
    /// <typeparam name="TResponse">响应类型（须实现判错契约）。</typeparam>
    /// <param name="response">响应实例。</param>
    /// <param name="requestUri">请求地址（可选，诊断用）。</param>
    public static void ThrowIfFailed<TResponse>(TResponse? response, string? requestUri = null)
        where TResponse : class, IWechatApiResponse
        => WechatApiResponseGuard.ThrowIfFailed(response, ExceptionFactory, requestUri);
}
