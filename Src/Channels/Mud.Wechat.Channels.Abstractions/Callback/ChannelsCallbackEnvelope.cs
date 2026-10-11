// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.Abstractions.Callback;

/// <summary>
/// 微信小店 / 视频号回调事件信封（JSON 报文形态）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与企微 / 公众号信封的差异</b>：小店回调官方<b>强制 JSON + 安全模式</b>（不可选明文 / 兼容），
/// 解密明文是 JSON 而非 XML —— 故 <see cref="DecryptedXml"/> 承载的是<b>解密后明文 JSON</b>
/// （接口属性名为跨线统一契约命名，XML 文档以本注释为准；<b>敏感载体</b>：不得整体写入日志 / 遥测 / 异常消息）。
/// </para>
/// <para>
/// <see cref="EventTypeKey"/> = 明文顶层 <c>Event</c> 字段（如 <c>channels_ec_order_pay</c>）；
/// 官方事件报文恒为 <c>MsgType=event</c>，缺失 <c>Event</c> 的明文在接收器即拒收（fail-closed）。
/// </para>
/// </remarks>
public sealed class WechatChannelsCallbackEvent : Mud.Wechat.Abstractions.Callback.IWechatCallbackEnvelope
{
    /// <inheritdoc />
    public string? ToUserName { get; init; }

    /// <inheritdoc />
    public string? FromUserName { get; init; }

    /// <inheritdoc />
    public string? CreateTime { get; init; }

    /// <inheritdoc />
    public string? MsgType { get; init; }

    /// <inheritdoc />
    public string? Event { get; init; }

    /// <inheritdoc />
    public string? DecryptedXml { get; init; }

    /// <inheritdoc />
    public string? TimeStamp { get; init; }

    /// <inheritdoc />
    public string? Nonce { get; init; }

    /// <inheritdoc />
    public string? AppKey { get; init; }

    /// <inheritdoc />
    public string EventTypeKey => Event ?? string.Empty;
}
