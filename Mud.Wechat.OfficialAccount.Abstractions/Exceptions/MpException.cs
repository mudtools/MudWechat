// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions.Exceptions;

/// <summary>
/// 微信公众号 / 服务号业务异常：HTTP 200 + <c>errcode != 0</c>（且非令牌失效码）时抛出。
/// </summary>
/// <remarks>
/// <para>
/// 令牌失效码（<c>{40001, 40014, 42001}</c>）由 <c>MpTokenInvalidationDetector</c> 识别后交
/// 恢复链路自动「失效缓存 → 刷新 → 重试」，不抛业务异常；其余非零 errcode 由令牌管理器 / 业务层经
/// <see cref="ThrowIfFailed{TResponse}"/> 抛出本异常。
/// </para>
/// <para>
/// 错误码与请求地址脱敏由公用基类 <see cref="WechatApiException"/> 统一承担
/// （公众号的 <c>secret</c> 以 Query 承载于 <c>getAccessToken</c>，<c>access_token</c> 以 Query 承载于业务端点）。
/// </para>
/// </remarks>
public sealed class MpException : WechatApiException
{
    /// <summary>
    /// 使用错误码与消息构造异常。
    /// </summary>
    /// <param name="errorCode">微信公众号业务错误码。</param>
    /// <param name="message">错误消息。</param>
    /// <param name="requestUri">请求地址（可选，诊断用；构造期自动脱敏）。</param>
    public MpException(int errorCode, string message, string? requestUri = null)
        : base(errorCode, message, requestUri)
    {
    }

    /// <summary>
    /// 使用错误码、消息与内部异常构造异常。
    /// </summary>
    /// <param name="errorCode">微信公众号业务错误码。</param>
    /// <param name="message">错误消息。</param>
    /// <param name="innerException">内部异常。</param>
    public MpException(int errorCode, string message, Exception innerException)
        : base(errorCode, message, innerException)
    {
    }

    /// <summary>本产品线异常工厂（静态缓存，零每调用分配）。</summary>
    private static readonly Func<int, string, string?, WechatApiException> ExceptionFactory =
        static (errorCode, message, requestUri) => new MpException(errorCode, message, requestUri);

    /// <summary>
    /// 统一判错出口：响应为 null 或 <c>errcode != 0</c> 时抛出本异常。
    /// </summary>
    /// <typeparam name="TResponse">响应类型（须实现判错契约）。</typeparam>
    /// <param name="response">响应实例。</param>
    /// <param name="requestUri">请求地址（可选，诊断用）。</param>
    public static void ThrowIfFailed<TResponse>(TResponse? response, string? requestUri = null)
        where TResponse : class, IWechatApiResponse
        => WechatApiResponseGuard.ThrowIfFailed(response, ExceptionFactory, requestUri);
}
