// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Exceptions;

/// <summary>
/// 企业微信业务异常：HTTP 200 + errcode != 0（且非令牌失效码）时抛出。
/// </summary>
/// <remarks>
/// 职责分工（与 Mud.HttpUtils v2.0.9 errcode 恢复判定器协同）：
/// <para>
/// 令牌失效码（<c>{40014, 42001, 42007, 42009, 42011}</c>）由
/// <c>WechatTokenInvalidationDetector</c> 识别后交给恢复链路自动
/// 「失效缓存 → 刷新 → 重试」，不抛业务异常；其余非零 errcode 由令牌管理器 / 业务层经
/// <see cref="ThrowIfFailed{TResponse}"/> 抛出本异常。
/// </para>
/// </remarks>
public sealed class WechatWorkException : Exception
{
    /// <summary>企业微信业务错误码。</summary>
    public int ErrorCode { get; }

    /// <summary>发生错误的请求地址（已脱敏：剥离 query 与 userinfo，用于诊断）。</summary>
    /// <remarks>
    /// <b>P2-4</b>：传入的原始 URI <b>不得</b>直接暴露——企业微信的 <c>corpsecret</c> /
    /// <c>suite_access_token</c> / <c>provider_access_token</c> 均以 Query 承载，随异常日志
    /// /APM 上报外泄。构造期统一剥离 query 与 userinfo（本地实现，不依赖组件 internal 类型）。
    /// </remarks>
    public string? RequestUri { get; }

    /// <summary>
    /// 使用错误码与消息构造异常。
    /// </summary>
    /// <param name="errorCode">企业微信业务错误码。</param>
    /// <param name="message">错误消息。</param>
    /// <param name="requestUri">请求地址（可选，诊断用；构造期自动脱敏）。</param>
    public WechatWorkException(int errorCode, string message, string? requestUri = null)
        : base(message)
    {
        ErrorCode = errorCode;
        RequestUri = RedactUri(requestUri);
    }

    /// <summary>
    /// 使用错误码、消息与内部异常构造异常。
    /// </summary>
    /// <param name="errorCode">企业微信业务错误码。</param>
    /// <param name="message">错误消息。</param>
    /// <param name="innerException">内部异常。</param>
    public WechatWorkException(int errorCode, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// 脱敏请求地址：剥离 <c>userinfo</c>（<c>scheme://user:pass@host</c>）与 query（<c>?…</c>）。
    /// </summary>
    private static string? RedactUri(string? uri)
    {
        if (string.IsNullOrEmpty(uri))
        {
            return uri;
        }

        var value = uri!;

        var schemeIndex = value.IndexOf("://", StringComparison.Ordinal);
        if (schemeIndex >= 0)
        {
            var authorityStart = schemeIndex + 3;
            var authorityEnd = value.Length;
            for (var i = authorityStart; i < value.Length; i++)
            {
                var c = value[i];
                if (c == '/' || c == '?' || c == '#')
                {
                    authorityEnd = i;
                    break;
                }
            }

            var userInfoEnd = value.IndexOf('@', authorityStart);
            if (userInfoEnd >= 0 && userInfoEnd < authorityEnd)
            {
                value = value.Substring(0, authorityStart) + "***@" + value.Substring(userInfoEnd + 1);
            }
        }

        var queryIndex = value.IndexOf('?');
        return queryIndex >= 0 ? value.Substring(0, queryIndex) : value;
    }

    /// <summary>
    /// 令牌管理器统一出口：响应为 null 或 errcode != 0 时抛出
    /// （令牌失效码由恢复链路处理，不会走到本方法——签发接口自身不带 [Token]，
    /// 其 errcode 失效码也按业务异常抛出）。
    /// </summary>
    /// <typeparam name="TResponse">响应类型。</typeparam>
    /// <param name="response">响应实例。</param>
    /// <param name="requestUri">请求地址（可选，诊断用）。</param>
    public static void ThrowIfFailed<TResponse>(TResponse? response, string? requestUri = null)
        where TResponse : WechatWorkResponse
    {
        if (response == null)
        {
            throw new WechatWorkException(-1, "企业微信 API 返回结果为 null。", requestUri);
        }

        if (!response.IsSuccess)
        {
            throw new WechatWorkException(
                response.ErrorCode,
                $"企业微信 API 调用失败：errcode={response.ErrorCode}, errmsg={response.ErrorMessage}。",
                requestUri);
        }
    }
}
