// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Compass;

/// <summary>店铺开播条目（官方 <c>live_list</c> 数组元素，<c>getshoplivelist</c>）。</summary>
/// <remarks><c>live_time</c>/<c>live_duration</c> 官方类型为 string（Unix 时间戳/时长以字符串返回），照抄建模。</remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopLiveItem
{
    /// <summary>获取或设置直播 id（官方 <c>live_id</c>）。</summary>
    [JsonPropertyName("live_id")]
    public string? LiveId { get; set; }

    /// <summary>获取或设置直播标题（官方 <c>live_title</c>）。</summary>
    [JsonPropertyName("live_title")]
    public string? LiveTitle { get; set; }

    /// <summary>获取或设置开播时间（官方 <c>live_time</c>，官方类型 string 的 Unix 时间戳）。</summary>
    [JsonPropertyName("live_time")]
    public string? LiveTime { get; set; }

    /// <summary>获取或设置直播时长（官方 <c>live_duration</c>，官方类型 string）。</summary>
    [JsonPropertyName("live_duration")]
    public string? LiveDuration { get; set; }

    /// <summary>获取或设置直播封面（官方 <c>live_cover_img_url</c>）。</summary>
    [JsonPropertyName("live_cover_img_url")]
    public string? LiveCoverImgUrl { get; set; }
}
