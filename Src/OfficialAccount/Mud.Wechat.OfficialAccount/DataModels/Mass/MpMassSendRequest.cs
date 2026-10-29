// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Mass;

/// <summary>
/// 根据 OpenID 群发（<c>message/mass/send</c>，<b>服务号专属</b>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<c>touser</c> 为 OpenID 列表，<b>最少 2 个、最多 10000 个</b>
/// （错误码 40130/40032 表达越界）；其余分支字段与 sendall 页同形
/// （mpvideo 本页多 title/description ⇒ <see cref="MpMassVideoMessage"/> 超集承载）。
/// </para>
/// <para>
/// <b>跨天群发注意（官方原文）</b>：「如果 media_id 是通过新建草稿接口来得到的，若涉及用同个 media_id
/// 进行跨天群发，则会生成不同的文章消息链接。如果不想生成不同的文章消息链接，那可以在用 media_id
/// 成功群发时拿到返回数据里的 msg_data_id，之后用 msg_data_id 而非 media_id 来进行群发」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassSendRequest
{
    /// <summary>获取或设置接收者 OpenID 列表（官方 <c>touser</c>，最少 2 个、最多 10000 个）。</summary>
    [JsonPropertyName("touser")]
    public List<string> ToUserList { get; set; } = new List<string>();

    /// <summary>获取或设置群发的消息类型（官方 <c>msgtype</c>；官方页说明「视频为 video」与对象名 mpvideo 矛盾，照录）。</summary>
    [JsonPropertyName("msgtype")]
    public string MsgType { get; set; } = string.Empty;

    /// <summary>获取或设置图文消息（官方 <c>mpnews</c>）。</summary>
    [JsonPropertyName("mpnews")]
    public MpMassMediaMessage? MpNews { get; set; }

    /// <summary>获取或设置文本消息（官方 <c>text</c>）。</summary>
    [JsonPropertyName("text")]
    public MpMassText? Text { get; set; }

    /// <summary>获取或设置语音消息（官方 <c>voice</c>）。</summary>
    [JsonPropertyName("voice")]
    public MpMassMediaMessage? Voice { get; set; }

    /// <summary>获取或设置图片消息（官方 <c>images</c>；多图形态）。</summary>
    [JsonPropertyName("images")]
    public MpMassImages? Images { get; set; }

    /// <summary>获取或设置视频消息（官方 <c>mpvideo</c>；本页含 title/description 子字段）。</summary>
    [JsonPropertyName("mpvideo")]
    public MpMassVideoMessage? MpVideo { get; set; }

    /// <summary>获取或设置卡券消息（官方 <c>wxcard</c>）。</summary>
    [JsonPropertyName("wxcard")]
    public MpMassWxCard? WxCard { get; set; }

    /// <summary>获取或设置被判为转载时是否继续群发（官方 <c>send_ignore_reprint</c>：1 继续 / 0 停止，默认 0；本页参数表有该行）。</summary>
    [JsonPropertyName("send_ignore_reprint")]
    public int? SendIgnoreReprint { get; set; }

    /// <summary>获取或设置开发者侧群发 msgid（官方 <c>clientmsgid</c>，≤32 字节）。</summary>
    [JsonPropertyName("clientmsgid")]
    public string? ClientMsgId { get; set; }
}
