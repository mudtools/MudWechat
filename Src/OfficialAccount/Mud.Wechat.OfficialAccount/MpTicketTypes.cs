// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions;

/// <summary>
/// 公众号 / 服务号临时票据类型常量（官方 <c>GET /cgi-bin/ticket/getticket</c> 的 <c>type</c> 参数）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与 <see cref="MpTokenTypes"/> 的区别</b>：<c>access_token</c> 是<b>请求注入型</b>凭据（由
/// <c>[Token]</c> 注入到每个业务请求，参与 errcode 恢复链路）；<c>ticket</c> 是<b>宿主自用型</b>凭据
/// （由宿主取走后在前端计算 JS-SDK 签名，<b>不</b>参与注入与恢复链路）。
/// </para>
/// <para>
/// <b>官方数值锁定</b>：仅两个合法取值（<c>jsapi</c> / <c>wx_card</c>）；有效期 <b>7200 秒</b>。
/// </para>
/// </remarks>
public static class MpTicketTypes
{
    /// <summary>官方返回的票据有效期（秒）。</summary>
    public const int ExpireSeconds = 7200;

    /// <summary>JS-SDK 凭证（<c>type=jsapi</c>）。</summary>
    public const string JsApi = "jsapi";

    /// <summary>微信卡券凭证（<c>type=wx_card</c>）。</summary>
    public const string WxCard = "wx_card";

    /// <summary>持久化键前缀：JS-SDK 票据（与卡券票据互不覆盖）。</summary>
    internal const string JsApiChannelKeyPrefix = "Wechat.Mp.JsApiTicket";

    /// <summary>持久化键前缀：卡券票据。</summary>
    internal const string WxCardChannelKeyPrefix = "Wechat.Mp.WxCardTicket";
}
