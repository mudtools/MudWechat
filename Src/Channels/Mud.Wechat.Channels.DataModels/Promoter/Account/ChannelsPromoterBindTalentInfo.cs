// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>合作带货者信息（官方 <c>talent_list</c> 数组项）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterBindTalentInfo
{
    /// <summary>获取或设置带货者的 id（官方 <c>talent_appid</c>）。</summary>
    [JsonPropertyName("talent_appid")]
    public string? TalentAppId { get; set; }

    /// <summary>获取或设置绑定时间（官方 <c>bind_time</c>）。</summary>
    [JsonPropertyName("bind_time")]
    public long? BindTime { get; set; }

    /// <summary>获取或设置带货者的昵称（官方 <c>talent_nickname</c>）。</summary>
    [JsonPropertyName("talent_nickname")]
    public string? TalentNickname { get; set; }

    /// <summary>获取或设置带货者的头像（官方 <c>talent_head_img</c>）。</summary>
    [JsonPropertyName("talent_head_img")]
    public string? TalentHeadImg { get; set; }

    /// <summary>获取或设置关联的视频号 username（官方 <c>related_finder_exportname_list</c>）。</summary>
    [JsonPropertyName("related_finder_exportname_list")]
    public List<string>? RelatedFinderExportNameList { get; set; }

    /// <summary>获取或设置关联的公众号 appid（官方 <c>related_mp_biz_appid_list</c>）。</summary>
    [JsonPropertyName("related_mp_biz_appid_list")]
    public List<string>? RelatedMpBizAppIdList { get; set; }
}
