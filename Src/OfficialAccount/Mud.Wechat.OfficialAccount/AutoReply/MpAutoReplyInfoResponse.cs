// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.AutoReply;

/// <summary>
/// 获取当前自动回复规则（<c>get_current_autoreply_info</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<b>GET</b>、无请求体；仅能获取公众平台官网自动回复功能中设置的规则
/// （公众号自行开发或第三方实现的自动回复无法获取）。
/// </para>
/// <para>
/// <b>素材时效（官方注意事项原文）</b>：返回的图片/语音/视频为<b>临时素材</b>（每次获取不同，
/// 3 天内有效，需通过「获取临时素材」接口获取）；图文消息为<b>永久素材</b>（需通过「获取永久素材」接口获取）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AutoReply")]
public class MpAutoReplyInfoResponse : MpResponse
{
    /// <summary>获取或设置关注后自动回复是否开启（官方 <c>is_add_friend_reply_open</c>：0 未开启 / 1 开启）。</summary>
    [JsonPropertyName("is_add_friend_reply_open")]
    public int IsAddFriendReplyOpen { get; set; }

    /// <summary>获取或设置消息自动回复是否开启（官方 <c>is_autoreply_open</c>：0 未开启 / 1 开启）。</summary>
    [JsonPropertyName("is_autoreply_open")]
    public int IsAutoReplyOpen { get; set; }

    /// <summary>获取或设置关注后自动回复的信息（官方 <c>add_friend_autoreply_info</c>）。</summary>
    [JsonPropertyName("add_friend_autoreply_info")]
    public MpSimpleAutoReplyInfo? AddFriendAutoReplyInfo { get; set; }

    /// <summary>获取或设置消息自动回复的信息（官方 <c>message_default_autoreply_info</c>）。</summary>
    [JsonPropertyName("message_default_autoreply_info")]
    public MpSimpleAutoReplyInfo? MessageDefaultAutoReplyInfo { get; set; }

    /// <summary>获取或设置关键词自动回复的信息（官方 <c>keyword_autoreply_info</c>）。</summary>
    [JsonPropertyName("keyword_autoreply_info")]
    public MpKeywordAutoReplyInfo? KeywordAutoReplyInfo { get; set; }
}
