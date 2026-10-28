// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Shop;

/// <summary>店铺基本信息（官方 <c>info</c> 对象，<c>mmecapi_basicinfo</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Shop")]
public class ChannelsShopBasicInfo
{
    /// <summary>获取或设置店铺名称（官方 <c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>获取或设置店铺头像 URL（官方 <c>headimg_url</c>）。</summary>
    [JsonPropertyName("headimg_url")]
    public string? HeadimgUrl { get; set; }

    /// <summary>获取或设置店铺类型（官方 <c>subject_type</c>，见 <see cref="ChannelsShopSubjectTypes"/>）。</summary>
    [JsonPropertyName("subject_type")]
    public string? SubjectType { get; set; }

    /// <summary>获取或设置店铺状态（官方 <c>status</c>，见 <see cref="ChannelsShopStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>获取或设置店铺原始 ID（官方 <c>username</c>）。</summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    /// <summary>获取或设置是否本地生活小店（官方 <c>is_local_life</c>：0 否，1 是）。</summary>
    [JsonPropertyName("is_local_life")]
    public int? IsLocalLife { get; set; }

    /// <summary>获取或设置开店时间戳（官方 <c>open_timestamp</c>；店铺状态为 open_finished 时返回）。</summary>
    [JsonPropertyName("open_timestamp")]
    public long? OpenTimestamp { get; set; }
}
