// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.Exceptions;

/// <summary>
/// 微信系 API 业务异常基类：HTTP 200 + 业务错误码非 0 时抛出。
/// </summary>
/// <remarks>
/// <para>
/// 各产品线派生自己的异常类型（企业微信 <c>WechatWorkException</c>、公众号 <c>MpException</c>），
/// 使既有 <c>catch</c> 面与类型身份不变；本基类统一承担「错误码 + 请求地址脱敏」两件事，
/// 避免每个产品线重复实现 query/userinfo 剥离。
/// </para>
/// <para>
/// <b>为何脱敏在构造期完成</b>：微信系凭据（<c>corpsecret</c> / <c>secret</c> /
/// <c>access_token</c> / <c>suite_access_token</c> 等）均以 Query 承载，异常对象一旦进入日志 / APM
/// 即外泄。构造期统一剥离 query 与 userinfo，是本层唯一的正确落点（本地实现，不依赖组件 internal 类型）。
/// </para>
/// </remarks>
public abstract class WechatApiException : Exception
{
    /// <summary>API 业务错误码。</summary>
    public int ErrorCode { get; }

    /// <summary>发生错误的请求地址（已脱敏：剥离 query 与 userinfo，用于诊断）。</summary>
    public string? RequestUri { get; }

    /// <summary>
    /// 使用错误码与消息构造异常。
    /// </summary>
    /// <param name="errorCode">API 业务错误码。</param>
    /// <param name="message">错误消息。</param>
    /// <param name="requestUri">请求地址（可选，诊断用；构造期自动脱敏）。</param>
    protected WechatApiException(int errorCode, string message, string? requestUri = null)
        : base(message)
    {
        ErrorCode = errorCode;
        RequestUri = RedactUri(requestUri);
    }

    /// <summary>
    /// 使用错误码、消息与内部异常构造异常。
    /// </summary>
    /// <param name="errorCode">API 业务错误码。</param>
    /// <param name="message">错误消息。</param>
    /// <param name="innerException">内部异常。</param>
    protected WechatApiException(int errorCode, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// 脱敏请求地址：剥离 <c>userinfo</c>（<c>scheme://user:pass@host</c>）与 query（<c>?…</c>）。
    /// </summary>
    /// <param name="uri">原始请求地址。</param>
    /// <returns>脱敏后的请求地址（输入为空时原样返回）。</returns>
    protected static string? RedactUri(string? uri)
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
}
