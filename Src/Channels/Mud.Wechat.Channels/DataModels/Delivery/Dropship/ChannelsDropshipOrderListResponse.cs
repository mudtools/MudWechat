// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>代发单号列表响应（<c>dropship/list</c> 与 <c>dropship/search</c> 共用）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：拉取代发单列表与搜索代发单两响应字段集完全一致
/// （<c>ds_order_id_list</c>/<c>next_page</c>/<c>total</c>/<c>has_more</c>）⇒ 共用本 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDropshipOrderListResponse : ChannelsResponse
{
    /// <summary>获取或设置代发单号列表（官方 <c>ds_order_id_list</c>）。</summary>
    [JsonPropertyName("ds_order_id_list")]
    public List<string>? DsOrderIdList { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>next_page</c>，下一页请求返回）。</summary>
    [JsonPropertyName("next_page")]
    public ChannelsDeliveryPageInfo? NextPage { get; set; }

    /// <summary>获取或设置总数（官方 <c>total</c>）。</summary>
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    /// <summary>获取或设置是否还有下一页（官方 <c>has_more</c>，true 表示有下一页，false 表示已经结束）。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }
}
