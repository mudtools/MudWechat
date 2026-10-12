// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.MiniStore;

/// <summary>查询收礼者订单列表（<c>receiverorderlist/get</c>）请求体。</summary>
/// <remarks>时间范围跨度不可超过 7 天；page_size 不超过 100；游标翻页（next_key）。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreGetReceiverOrderListRequest
{
    /// <summary>获取或设置订单创建时间范围（官方 <c>create_time_range</c>，选填）。</summary>
    [JsonPropertyName("create_time_range")]
    public ChannelsMiniStoreTimeRange? CreateTimeRange { get; set; }

    /// <summary>获取或设置订单更新时间范围（官方 <c>update_time_range</c>，选填）。</summary>
    [JsonPropertyName("update_time_range")]
    public ChannelsMiniStoreTimeRange? UpdateTimeRange { get; set; }

    /// <summary>获取或设置小店 appid（官方 <c>shop_appid</c>，必填）。</summary>
    [JsonPropertyName("shop_appid")]
    public string ShopAppId { get; set; } = string.Empty;

    /// <summary>获取或设置收礼者 openid（官方 <c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填，不超过 100）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>next_key</c>，选填，上一页请求返回，首次置空即可）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}
