// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.Abstractions.Exceptions;

/// <summary>
/// 腾讯广告 Marketing API v3.0 业务异常：应答信封 <c>code != 0</c> 时抛出。
/// </summary>
/// <remarks>
/// <para>
/// <b>与微信系异常的两点差异</b>：
/// ① 错误码字段名是 <c>code</c>（整数）而非 <c>errcode</c> —— 由 <see cref="AdsResponse"/>
/// 映射进公用层判错契约后统一消费，异常侧无需感知字段名；
/// ② 官方同时返回 <c>message</c>（英文）与 <c>message_cn</c>（中文）。消息拼装规则是「英文支恒显示、
/// 中文支非空时追加」，但<b>中文支只在直接构造时可达</b>：<see cref="ThrowIfFailed{TResponse}"/> 的唯一
/// 输入是公用契约 <see cref="IWechatApiResponse"/>（只有 <c>code</c> / <c>message</c>），
/// 因此经守卫抛出的异常 <see cref="MessageCn"/> 恒为 <c>null</c>。
/// 需要中文描述时由调用方自行读应答的 <c>MessageCn</c> 字段（守卫 ADS-B2 锁定该字段名不驼峰化）。
/// </para>
/// <para>
/// <b>批量族的判错层次</b>：外层 <c>code == 0</c> 只代表「请求被受理」，
/// 逐条成败在 <c>data.list[]</c> 的每个元素里各带 <c>code</c>（见 <c>AdsBatchResultItem</c>）。
/// 本异常<b>只</b>覆盖外层失败；逐条失败不抛异常，由调用方按守卫 ADS-B2 锁定的形态自行检查。
/// </para>
/// <para>
/// 错误码与请求地址脱敏由公用基类 <see cref="WechatApiException"/> 统一承担（构造期剥离 query 与 userinfo）。
/// 广告线的凭据 <c>access_token</c> / <c>timestamp</c> / <c>nonce</c> / <c>client_secret</c> 均在 Query，
/// 因此这层脱敏是<b>必需</b>的（守卫 ADS-B5 同时核验参数名 ⊆ 组件脱敏词表）。
/// </para>
/// </remarks>
public class WechatAdsException : WechatApiException
{
    /// <summary>官方中文错误描述（<c>message_cn</c>，可为空）。</summary>
    public string? MessageCn { get; }

    /// <summary>用官方错误码、描述与请求地址构造异常。</summary>
    /// <param name="errorCode">官方 <c>code</c>。</param>
    /// <param name="message">官方 <c>message</c>（英文权威支）。</param>
    /// <param name="messageCn">官方 <c>message_cn</c>（中文支，多数场景为空串）。</param>
    /// <param name="requestUri">请求地址（诊断用，构造期自动脱敏）。</param>
    public WechatAdsException(int errorCode, string message, string? messageCn = null, string? requestUri = null)
        : base(errorCode, BuildMessage(errorCode, message, messageCn), requestUri)
    {
        MessageCn = messageCn;
    }

    /// <summary>用官方错误码、描述与内部异常构造异常。</summary>
    /// <param name="errorCode">官方 <c>code</c>。</param>
    /// <param name="message">错误消息。</param>
    /// <param name="innerException">内部异常。</param>
    public WechatAdsException(int errorCode, string message, Exception innerException)
        : base(errorCode, message, innerException)
    {
    }

    /// <summary>本产品线异常工厂（静态缓存，零每调用分配）。</summary>
    private static readonly Func<int, string, string?, WechatApiException> ExceptionFactory =
        static (errorCode, message, requestUri) => new WechatAdsException(errorCode, message, null, requestUri);

    /// <summary>
    /// 统一判错出口：响应为 <c>null</c> 或 <c>code != 0</c> 时抛出 <see cref="WechatAdsException"/>。
    /// </summary>
    /// <typeparam name="TResponse">响应类型（须实现公用层判错契约）。</typeparam>
    /// <param name="response">响应实例。</param>
    /// <param name="requestUri">请求地址（可选，诊断用）。</param>
    public static void ThrowIfFailed<TResponse>(TResponse? response, string? requestUri = null)
        where TResponse : class, IWechatApiResponse
        => WechatApiResponseGuard.ThrowIfFailed(response, ExceptionFactory, requestUri);

    /// <summary>拼装异常消息：英文支恒显示，中文支非空时追加（<b>不含</b>任何令牌值）。</summary>
    private static string BuildMessage(int errorCode, string? message, string? messageCn)
    {
        var text = $"腾讯广告 API 调用失败：code={errorCode}, message={message}";
        if (!string.IsNullOrEmpty(messageCn))
        {
            text += $", message_cn={messageCn}";
        }

        return text + "。";
    }
}

/// <summary>
/// 需要<b>重新授权</b>才能继续取令牌：refresh_token 缺失、已过期，或刷新被官方拒绝。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么要单独一个类型</b>：这一类失败<b>不可由 SDK 自愈</b>。官方原文
/// （<see href="https://developers.e.qq.com/v3.0/docs/api/oauth/refresh_token"/>，2026-10-10 核验）：
/// 「Refresh Token 在有效期内，调用此接口刷新 Refresh Token，获得新的 Refresh Token 及 Access Token，
/// 并更新有效期，<b>原 Access Token 及 Refresh Token 会失效</b>」—— 一次性语义意味着
/// 「重试」这个动作本身没有意义，必须由人重走一遍授权页。
/// </para>
/// <para>
/// <b>处置约定（守卫 ADS-B3）</b>：抛出本异常<b>之前</b>，存储里的授权状态必须已经删除。
/// 顺序反了（先抛后删）会让进程崩溃 / 宿主吞异常时把一份「官方已作废的 refresh_token」留在库里，
/// 此后每次请求都撞一次无效刷新。类型身份本身也是宿主的重试策略开关：
/// <c>catch (WechatAdsException)</c> 里做退避重试是合理的，
/// <c>catch (WechatAdsReauthorizationRequiredException)</c> 里做重试则是纯粹的量攻击。
/// </para>
/// </remarks>
public sealed class WechatAdsReauthorizationRequiredException : WechatAdsException
{
    /// <summary>本地判定（官方未给出 code）时填充 <see cref="WechatApiException.ErrorCode"/> 的哨兵值。</summary>
    public const int ReauthorizationRequiredSentinel = -1;

    /// <summary>构造异常。</summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="reason">失败原因（<b>不得</b>包含令牌值）。</param>
    /// <param name="underlyingCode">官方返回的 <c>code</c>；本地判定（如库里根本没有 refresh_token）时为 <c>null</c>。</param>
    public WechatAdsReauthorizationRequiredException(
        string appKey,
        string reason,
        int? underlyingCode = null)
        : base(underlyingCode ?? ReauthorizationRequiredSentinel, Build(appKey, reason, underlyingCode))
    {
        AppKey = appKey;
    }

    /// <summary>触发本异常的应用键（宿主据此定位需要重新授权的 <c>client_id</c>）。</summary>
    public string AppKey { get; }

    private static string Build(string appKey, string reason, int? underlyingCode)
    {
        var code = underlyingCode is null ? string.Empty : $"（官方 code={underlyingCode}）";
        return $"腾讯广告应用 {appKey} 需要重新授权：{reason}{code}。" +
               "Refresh Token 为一次性凭据，刷新失败后旧值已被作废，" +
               "请引导用户重新完成 OAuth 授权并用 authorization_code 换取新令牌对。";
    }
}
