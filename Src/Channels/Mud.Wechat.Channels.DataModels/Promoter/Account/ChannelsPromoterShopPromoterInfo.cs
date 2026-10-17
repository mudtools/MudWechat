// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>合作小店的关联账号信息（官方 <c>promoter_list</c> 数组项）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterShopPromoterInfo
{
    /// <summary>获取或设置关联账号的 id（官方 <c>promoter_id</c>）。</summary>
    [JsonPropertyName("promoter_id")]
    public string? PromoterId { get; set; }

    /// <summary>获取或设置关联账号头像（官方 <c>avatar_image_url</c>）。</summary>
    [JsonPropertyName("avatar_image_url")]
    public string? AvatarImageUrl { get; set; }

    /// <summary>获取或设置关联账号类型（官方 <c>promoter_type</c>，1：视频号；4：公众号；5：服务号；仅视频号类型有直播和短视频内容，见 <see cref="ChannelsPromoterShopPromoterTypes"/>）。</summary>
    [JsonPropertyName("promoter_type")]
    public int? PromoterType { get; set; }

    /// <summary>获取或设置关联账号名称（官方 <c>promoter_name</c>）。</summary>
    [JsonPropertyName("promoter_name")]
    public string? PromoterName { get; set; }
}
