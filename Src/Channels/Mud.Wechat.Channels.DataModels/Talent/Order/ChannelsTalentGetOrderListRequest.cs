// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------
namespace Mud.Wechat.Channels.DataModels.Talent;

/// <summary>获取佣金单列表请求（官方 <c>talent/get_order_list</c> 请求体）。</summary>
[HttpJsonSerializable(SerializerClassName = "Talent")]
public class ChannelsTalentGetOrderListRequest
{
    /// <summary>获取或设置佣金单创建时间范围之开始时间（官方 <c>create_time_gt</c>，与更新时间范围二选一）。</summary>
    [JsonPropertyName("create_time_gt")]
    public long? CreateTimeGt { get; set; }

    /// <summary>获取或设置佣金单创建时间范围之结束时间（官方 <c>create_time_lt</c>，与更新时间范围二选一）。</summary>
    [JsonPropertyName("create_time_lt")]
    public long? CreateTimeLt { get; set; }

    /// <summary>获取或设置佣金单更新时间范围之开始时间（官方 <c>update_time_gt</c>，与创建时间范围二选一）。</summary>
    [JsonPropertyName("update_time_gt")]
    public long? UpdateTimeGt { get; set; }

    /// <summary>获取或设置佣金单更新时间范围之结束时间（官方 <c>update_time_lt</c>，与创建时间范围二选一）。</summary>
    [JsonPropertyName("update_time_lt")]
    public long? UpdateTimeLt { get; set; }

    /// <summary>获取或设置订单 id 过滤（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置商品 id 过滤（官方 <c>spu_id</c>）。</summary>
    [JsonPropertyName("spu_id")]
    public string? SpuId { get; set; }

    /// <summary>获取或设置单页佣金单数（官方 <c>page_size</c>，必填，不超过 10）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>next_key</c>，由上次请求返回，顺序翻页时传入）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}
