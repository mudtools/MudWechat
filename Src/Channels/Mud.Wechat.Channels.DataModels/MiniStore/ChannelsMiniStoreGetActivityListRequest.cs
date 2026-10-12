// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.MiniStore;

/// <summary>查询小店礼物活动列表（<c>activity/list/promoter/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreGetActivityListRequest
{
    /// <summary>获取或设置偏移量（官方 <c>offset</c>，必填）。</summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>获取或设置单次请求个数（官方 <c>limit</c>，必填，最大为 100）。</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    /// <summary>获取或设置小店 appid（官方 <c>shop_appid</c>，选填）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppId { get; set; }

    /// <summary>获取或设置活动状态（官方 <c>status</c>，选填，见 <see cref="ChannelsMiniStoreActivityStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置类型（官方 <c>type</c>，选填，见 <see cref="ChannelsMiniStoreActivityTypes"/>；默认为 0）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }
}
