// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;
using Mud.Wechat.OfficialAccount.Abstractions.Configuration;

namespace Mud.Wechat.OfficialAccount.Web;

/// <summary>
/// JS-SDK 前端注入签名结果（供 <c>wx.config</c> 使用）。
/// </summary>
/// <remarks>
/// <b>字段集与官方样例一致</b>：四份官方样例（java/node/python/php）的签名返回对象均含
/// <c>appId</c>/<c>timestamp</c>/<c>nonceStr</c>/<c>url</c>/<c>signature</c>（前端 <c>wx.config</c> 需要这五项）。
/// <b>刻意不返回 <c>jsapi_ticket</c> 与 <c>rawString</c></b>：二者都含票据原文，
/// 一旦随响应外泄即等于公开票据（官方 PHP 样例同样不把 ticket 放进 <c>signPackage</c>；
/// node 样例把 ticket 放进返回对象仅供其自身调试，不可照搬到服务端响应）。
/// </remarks>
public sealed class MpJsApiSignatureResult
{
    /// <summary>创建签名结果。</summary>
    /// <param name="appId">公众号 AppID（前端 <c>wx.config</c> 的 <c>appId</c>）。</param>
    /// <param name="url">参与签名的 URL（已去除 <c>#</c> 片段）。</param>
    /// <param name="nonceStr">随机字符串。</param>
    /// <param name="timeStamp">时间戳（Unix 秒）。</param>
    /// <param name="signature">签名值（sha1 小写十六进制）。</param>
    public MpJsApiSignatureResult(string appId, string url, string nonceStr, long timeStamp, string signature)
    {
        AppId = appId;
        Url = url;
        NonceStr = nonceStr;
        TimeStamp = timeStamp;
        Signature = signature;
    }

    /// <summary>公众号 AppID（前端 <c>wx.config</c> 的 <c>appId</c>）。</summary>
    public string AppId { get; }

    /// <summary>参与签名的 URL（**不含 <c>#</c> 及其后片段**）。</summary>
    public string Url { get; }

    /// <summary>随机字符串（对应前端 <c>nonceStr</c>）。</summary>
    public string NonceStr { get; }

    /// <summary>时间戳（秒；对应前端 <c>timestamp</c>）。</summary>
    public long TimeStamp { get; }

    /// <summary>签名值。</summary>
    public string Signature { get; }
}

/// <summary>
/// JS-SDK 签名服务：取 <c>type=jsapi</c> 票据 → 按官方算法生成 <c>wx.config</c> 所需签名。
/// </summary>
/// <remarks>
/// <para>
/// <b>票据侧不重复实现</b>：票据获取与缓存（7200 秒有效期、频次限制保护、多实例写穿）由既有
/// <see cref="IMpJsApiTicketManager"/> 承担（F21/F22 已核验），本服务只做<b>签名组装</b>
/// —— 与卡券票据严格分键（<c>jsapi</c> 与 <c>wx_card</c> 不可互用）。
/// </para>
/// <para><b>AOT</b>：纯字符串运算与 SHA1，无反射、无 JSON，天然 AOT 安全。</para>
/// </remarks>
public interface IMpJsApiSignatureService
{
    /// <summary>为指定页面 URL 生成 JS-SDK 签名。</summary>
    /// <param name="url">页面 URL（通常为 <c>location.href</c>，允许携带 <c>#</c> 片段 —— 内部会去除）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>签名结果（URL / nonceStr / timestamp / signature）。</returns>
    Task<MpJsApiSignatureResult> SignAsync(string url, CancellationToken cancellationToken = default);
}

/// <inheritdoc cref="IMpJsApiSignatureService" />
public sealed class MpJsApiSignatureService : IMpJsApiSignatureService
{
    private readonly IMpJsApiTicketManager _ticketManager;
    private readonly string _appId;
    private readonly Func<DateTimeOffset> _utcNow;

    /// <summary>创建签名服务。</summary>
    /// <param name="ticketManager">JS-SDK 票据管理器（<c>type=jsapi</c>）。</param>
    /// <param name="options">当前应用配置（取 <see cref="MpAppConfig.AppId"/> 回包给前端 <c>wx.config</c>；与票据管理器同源）。</param>
    /// <param name="utcNow">当前时间提供器（测试可注入）。</param>
    public MpJsApiSignatureService(
        IMpJsApiTicketManager ticketManager,
        IOptions<MpAppConfig> options,
        Func<DateTimeOffset>? utcNow = null)
    {
        _ticketManager = ticketManager ?? throw new ArgumentNullException(nameof(ticketManager));
        var appId = (options ?? throw new ArgumentNullException(nameof(options))).Value?.AppId;
        if (string.IsNullOrEmpty(appId))
        {
            // 前端 wx.config 必须带 appId；缺失即无法产出可用签名包 ⇒ fail-fast 而非回包半成品。
            throw new ArgumentException("应用配置缺少 AppId（前端 wx.config 需要该字段）。", nameof(options));
        }

        _appId = appId!;
        _utcNow = utcNow ?? (() => DateTimeOffset.UtcNow);
    }

    /// <inheritdoc />
    public async Task<MpJsApiSignatureResult> SignAsync(string url, CancellationToken cancellationToken = default)
    {
        var normalizedUrl = MpJsApiSignature.NormalizeUrl(url);
        var ticket = await _ticketManager.GetTicketAsync(cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrEmpty(ticket))
        {
            throw new InvalidOperationException(
                "未取得 jsapi_ticket，无法生成 JS-SDK 签名（请检查公众号令牌配置与网络）。");
        }

        var nonceStr = MpJsApiSignature.CreateNonceStr();
        var timestamp = _utcNow().ToUnixTimeSeconds();
        var signature = MpJsApiSignature.Compute(ticket, nonceStr, timestamp, normalizedUrl);

        return new MpJsApiSignatureResult(_appId, normalizedUrl, nonceStr, timestamp, signature);
    }
}
