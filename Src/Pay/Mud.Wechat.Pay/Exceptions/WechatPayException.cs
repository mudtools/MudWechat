// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Abstractions.Exceptions;
using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.Abstractions.Exceptions;

/// <summary>
/// 微信支付 APIv3 业务异常：HTTP 4xx/5xx 且响应体含官方 <c>code</c> / <c>message</c> 时抛出。
/// </summary>
/// <remarks>
/// <para>
/// <b>字符串错误码 vs 基类 int 槽位</b>：APIv3 的官方错误码是<b>字符串</b>（<c>SIGN_ERROR</c> / <c>PARAM_ERROR</c> …），
/// 而公用层基类 <see cref="WechatApiException.ErrorCode"/> 是 <see cref="int"/>（为企微 / 公众号的 errcode 而设）。
/// 本类<b>不扭曲官方语义</b>：权威码落在 <see cref="PayErrorCode"/>（string），
/// 基类 int 槽位填 <see cref="NumericFailureSentinel"/>（-1，明确区别于「0 = 成功」）。
/// 调用方应按 <see cref="PayErrorCode"/> 做分类，<b>勿</b>按 <c>ErrorCode</c> 判断具体原因。
/// </para>
/// <para>
/// <b>请求地址脱敏</b>：由基类构造期剥离 query / userinfo，账单 <c>download_url</c> 携带的
/// <c>token</c> 因此不会进入异常消息（红线 PAY-B7）。
/// </para>
/// </remarks>
public sealed class WechatPayException : WechatApiException
{
    /// <summary>
    /// 基类 int 错误码槽位的失败哨兵值（APIv3 无整数错误码）。
    /// </summary>
    /// <remarks>取 <c>-1</c> 而非 <c>0</c>：<c>0</c> 在判错契约里表示成功，填 <c>0</c> 会让失败看起来像成功。</remarks>
    public const int NumericFailureSentinel = -1;

    /// <summary>官方 APIv3 错误码（<c>code</c> 字段原文；无该字段时为 <c>null</c>）。</summary>
    public string? PayErrorCode { get; }

    /// <summary>
    /// 使用官方错误码与消息构造异常。
    /// </summary>
    /// <param name="payErrorCode">官方 APIv3 错误码（<c>code</c> 原文）。</param>
    /// <param name="message">错误消息。</param>
    /// <param name="requestUri">请求地址（可选，诊断用；构造期自动脱敏）。</param>
    public WechatPayException(string? payErrorCode, string message, string? requestUri = null)
        : base(NumericFailureSentinel, message, requestUri)
    {
        PayErrorCode = payErrorCode;
    }

    /// <summary>
    /// 使用官方错误码、消息与内部异常构造异常。
    /// </summary>
    /// <param name="payErrorCode">官方 APIv3 错误码（<c>code</c> 原文）。</param>
    /// <param name="message">错误消息。</param>
    /// <param name="innerException">内部异常。</param>
    public WechatPayException(string? payErrorCode, string message, Exception innerException)
        : base(NumericFailureSentinel, message, innerException)
    {
        PayErrorCode = payErrorCode;
    }

    /// <summary>
    /// 统一判错出口：响应为 <c>null</c> 或 <see cref="WechatPayResponse.IsSuccess"/> 为 <c>false</c> 时抛出本异常。
    /// </summary>
    /// <param name="response">支付响应实例（可为 null）。</param>
    /// <param name="requestUri">请求地址（可选，诊断用）。</param>
    /// <remarks>
    /// <para>
    /// <b>为何不走公用层 <c>WechatApiResponseGuard</c></b>：该守卫的异常工厂签名是
    /// <c>Func&lt;int, string, string?, WechatApiException&gt;</c>，只有 int 槽位，
    /// 会把 APIv3 的字符串错误码<b>丢成 -1</b> —— 调用方再也拿不到 <c>SIGN_ERROR</c> 这类分类依据。
    /// 故本产品线自持判错出口，把字符串码完整带出（语义不丢，形态与公用层一致）。
    /// </para>
    /// <para>
    /// <b>调用时机</b>：支付接口带 <c>[AllowAnyStatusCode]</c>，4xx/5xx 的错误体<b>原样反序列化</b>进
    /// <see cref="WechatPayResponse"/> 派生 DTO，故<b>由调用方</b>在拿到响应后调用本方法判错
    /// （与公众号线「响应体自带判错面，由调用方 <c>MpException.ThrowIfFailed</c>」的既有契约一致）。
    /// </para>
    /// </remarks>
    public static void ThrowIfFailed(WechatPayResponse? response, string? requestUri = null)
    {
        if (response == null)
        {
            throw new WechatPayException(null, "微信支付 API 返回结果为 null。", requestUri);
        }

        if (!response.IsSuccess)
        {
            throw new WechatPayException(
                response.Code,
                $"微信支付 API 调用失败：code={response.Code}, message={response.Message}。",
                requestUri);
        }
    }
}
