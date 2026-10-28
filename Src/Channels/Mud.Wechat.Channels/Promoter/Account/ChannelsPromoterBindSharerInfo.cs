// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>推客绑定信息（官方 <c>sharer_info_list</c> 数组项）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterBindSharerInfo
{
    /// <summary>获取或设置推客的 appid（官方 <c>sharer_appid</c>）。</summary>
    [JsonPropertyName("sharer_appid")]
    public string? SharerAppId { get; set; }

    /// <summary>获取或设置绑定时间戳（官方 <c>bind_time</c>）。</summary>
    [JsonPropertyName("bind_time")]
    public long? BindTime { get; set; }

    /// <summary>获取或设置分佣比例（官方 <c>commission_ratio</c>）。</summary>
    [JsonPropertyName("commission_ratio")]
    public long? CommissionRatio { get; set; }

    /// <summary>获取或设置分佣类型（官方 <c>commission_type</c>，0：平台分佣 1：机构分佣，见 <see cref="ChannelsPromoterCommissionTypes"/>）。</summary>
    [JsonPropertyName("commission_type")]
    public int? CommissionType { get; set; }

    /// <summary>获取或设置推客的昵称（官方 <c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>获取或设置推客的头像（官方 <c>head_img_url</c>）。</summary>
    [JsonPropertyName("head_img_url")]
    public string? HeadImgUrl { get; set; }
}
