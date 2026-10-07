// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Mass;

/// <summary>
/// 根据标签群发（<c>message/mass/sendall</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<c>filter</c>/<c>msgtype</c> 必填；六个消息分支对象按 msgtype
/// 携带其一（mpnews / text / voice / images / mpvideo / wxcard）。<b>不做运行时多态</b>（AOT 源生成
/// 按声明类型序列化）——分支对象可空、按 msgtype 携带其一，由调用方保证一致性（官方 40008 表达错配）。
/// </para>
/// <para>
/// <b>官方文档矛盾（照录）</b>：<c>send_ignore_reprint</c> 只出现在「原创校验」说明文字与图文示例
/// JSON 中，<b>未列入请求体参数表</b>——SDK 按说明与示例建模（默认 0：被判为转载即停止群发）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassSendAllRequest
{
    /// <summary>获取或设置接收者过滤条件（官方 <c>filter</c>，必填）。</summary>
    [JsonPropertyName("filter")]
    public MpMassFilter Filter { get; set; } = new MpMassFilter();

    /// <summary>获取或设置群发的消息类型（官方 <c>msgtype</c>：mpnews/text/voice/image/mpvideo/wxcard）。</summary>
    [JsonPropertyName("msgtype")]
    public string MsgType { get; set; } = string.Empty;

    /// <summary>获取或设置图文消息（官方 <c>mpnews</c>；media_id 来自草稿箱/临时素材）。</summary>
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

    /// <summary>获取或设置视频消息（官方 <c>mpvideo</c>）。</summary>
    [JsonPropertyName("mpvideo")]
    public MpMassMediaMessage? MpVideo { get; set; }

    /// <summary>获取或设置卡券消息（官方 <c>wxcard</c>）。</summary>
    [JsonPropertyName("wxcard")]
    public MpMassWxCard? WxCard { get; set; }

    /// <summary>获取或设置被判为转载时是否继续群发（官方 <c>send_ignore_reprint</c>：1 继续 / 0 停止，默认 0；官方参数表未列该行，按说明与示例建模）。</summary>
    [JsonPropertyName("send_ignore_reprint")]
    public int? SendIgnoreReprint { get; set; }

    /// <summary>获取或设置开发者侧群发 msgid（官方 <c>clientmsgid</c>，≤32 字节；24 小时内防重，45065 返回已存在任务的 msgid）。</summary>
    [JsonPropertyName("clientmsgid")]
    public string? ClientMsgId { get; set; }
}

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

/// <summary>
/// 预览消息（<c>message/mass/preview</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<c>touser</c>/<c>towxname</c> <b>二选一</b>（openid 预览 / 微信号预览）；
/// 分支对象比群发页多 music 与单图 image（均 <see cref="MpMassMediaMessage"/> 形态，共用）；
/// wxcard 带 card_ext。
/// </para>
/// <para>
/// <b>频次（官方注意事项原文）</b>：「该能力每日调用次数有限制（<b>100 次</b>），请勿滥用」
/// （针对 towxname 微信号预览能力；touser 预览本页未给次数）。
/// </para>
/// <para>官方文档缺陷（照录）：各分支子表「类型」列印作「-」而非 string。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassPreviewRequest
{
    /// <summary>获取或设置接收消息用户的 openid（官方 <c>touser</c>；与 towxname 二选一）。</summary>
    [JsonPropertyName("touser")]
    public string? ToUser { get; set; }

    /// <summary>获取或设置接收消息用户微信号（官方 <c>towxname</c>；每日限 100 次）。</summary>
    [JsonPropertyName("towxname")]
    public string? ToWxName { get; set; }

    /// <summary>获取或设置消息类型（官方 <c>msgtype</c>：mpnews/text/voice/music/image/mpvideo/wxcard）。</summary>
    [JsonPropertyName("msgtype")]
    public string MsgType { get; set; } = string.Empty;

    /// <summary>获取或设置文章（官方 <c>mpnews</c>）。</summary>
    [JsonPropertyName("mpnews")]
    public MpMassMediaMessage? MpNews { get; set; }

    /// <summary>获取或设置文本消息（官方 <c>text</c>）。</summary>
    [JsonPropertyName("text")]
    public MpMassText? Text { get; set; }

    /// <summary>获取或设置语音消息（官方 <c>voice</c>）。</summary>
    [JsonPropertyName("voice")]
    public MpMassMediaMessage? Voice { get; set; }

    /// <summary>获取或设置音乐消息（官方 <c>music</c>；<b>仅预览页出现</b>）。</summary>
    [JsonPropertyName("music")]
    public MpMassMediaMessage? Music { get; set; }

    /// <summary>获取或设置贴图消息（官方 <c>image</c>，单 media_id 形态；<b>与群发页多图 images 不同</b>）。</summary>
    [JsonPropertyName("image")]
    public MpMassMediaMessage? Image { get; set; }

    /// <summary>获取或设置视频消息（官方 <c>mpvideo</c>）。</summary>
    [JsonPropertyName("mpvideo")]
    public MpMassMediaMessage? MpVideo { get; set; }

    /// <summary>获取或设置卡券消息（官方 <c>wxcard</c>；预览页含 card_ext）。</summary>
    [JsonPropertyName("wxcard")]
    public MpMassWxCard? WxCard { get; set; }
}
