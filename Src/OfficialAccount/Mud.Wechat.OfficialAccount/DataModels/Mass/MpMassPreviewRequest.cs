// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Mass;

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
