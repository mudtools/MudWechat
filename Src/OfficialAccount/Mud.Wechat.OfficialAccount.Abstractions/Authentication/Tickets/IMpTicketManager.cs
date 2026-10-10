// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions.Authentication;

/// <summary>
/// 公众号临时票据管理器（per-app；<c>jsapi</c> / <c>wx_card</c> 两类票据的共同契约）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须经管理器取票据</b>：官方「注意事项」原文明确「由于获取 api_ticket 的 api 调用次数非常有限，
/// 频繁刷新 api_ticket 会导致 api 调用受限，影响自身业务，开发者需在自己的服务存储与更新 api_ticket」
/// ⇒ 管理器承担<b>存储 + 更新</b>责任（进程内单一飞行 + 到期前阈值刷新 + 可选持久化仓储），
/// 宿主<b>不得</b>绕过本接口逐次直调官方端点。
/// </para>
/// <para>
/// <b>与 <c>IMpAccessTokenManager</c> 的定位差异</b>：<c>access_token</c> 由框架注入到每个业务请求并参与
/// errcode 恢复链路；票据由宿主取走后<b>自行使用</b>（前端签名 / 卡券），不参与注入与恢复链路。
/// </para>
/// <para>
/// <b>多实例部署</b>：装配了 <c>IWechatTokenStore</c> 时，票据与令牌共用该持久化仓储（键空间分别以
/// <c>Wechat.Mp.JsApiTicket:{AppKey}</c> / <c>Wechat.Mp.WxCardTicket:{AppKey}</c> 隔离），
/// 冷启动经组件读穿透直接命中其它实例写入的票据，避免多实例各自刷新触发官方频次限制。
/// </para>
/// </remarks>
public interface IMpTicketManager
{
    /// <summary>票据类型（<see cref="MpTicketTypes"/> 常量）。</summary>
    string TicketType { get; }

    /// <summary>获取临时票据（缓存命中直接返回；到期前按阈值提前刷新）。</summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>临时票据字符串。</returns>
    Task<string> GetTicketAsync(CancellationToken cancellationToken = default);
}

/// <summary>JS-SDK 票据管理器（<c>type=jsapi</c>；用于前端 <c>wx.config</c> 签名）。</summary>
public interface IMpJsApiTicketManager : IMpTicketManager
{
}

/// <summary>微信卡券票据管理器（<c>type=wx_card</c>；用于卡券 <c>chooseCard</c> / <c>addCard</c> 签名）。</summary>
public interface IMpWxCardTicketManager : IMpTicketManager
{
}
