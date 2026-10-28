// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>为某个推客生成单个直播预约的推广二维码请求（官方 <c>promoter/get_live_notice_record_qr_code</c> 请求体）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterGetLiveNoticeRecordQrCodeRequest
{
    /// <summary>获取或设置带货者的 appid（官方 <c>talent_appid</c>，必填）。</summary>
    [JsonPropertyName("talent_appid")]
    public string? TalentAppId { get; set; }

    /// <summary>获取或设置直播预约 id（官方 <c>notice_id</c>，必填）。</summary>
    [JsonPropertyName("notice_id")]
    public string? NoticeId { get; set; }

    /// <summary>获取或设置推客的 appid（官方 <c>sharer_appid</c>，必填）。</summary>
    [JsonPropertyName("sharer_appid")]
    public string? SharerAppId { get; set; }
}
