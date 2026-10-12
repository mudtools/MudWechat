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
