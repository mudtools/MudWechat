// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 服务订单条目（官方 <c>service_order_list[]</c>）。
/// </summary>
/// <remarks>
/// <para>官方字段名拼作 <c>craete_time</c>（原文拼写，照抄勿改）；订单状态 1 生效中 / 2 已过期 / 3 已停用 / 4 未付款。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductServiceOrderItem
{
    /// <summary>服务订单 ID。</summary>
    [JsonPropertyName("service_order_id")]
    public long? ServiceOrderId { get; set; }

    /// <summary>服务 ID。</summary>
    [JsonPropertyName("service_id")]
    public long? ServiceId { get; set; }

    /// <summary>服务名称。</summary>
    [JsonPropertyName("service_name")]
    public string? ServiceName { get; set; }

    /// <summary>创建时间（官方原文拼写 craete_time）。</summary>
    [JsonPropertyName("craete_time")]
    public string? CraeteTime { get; set; }

    /// <summary>过期时间。</summary>
    [JsonPropertyName("expire_time")]
    public string? ExpireTime { get; set; }

    /// <summary>服务类型（2 商品 / 3 订单）。</summary>
    [JsonPropertyName("service_type")]
    public int? ServiceType { get; set; }

    /// <summary>规格英文名称。</summary>
    [JsonPropertyName("specification_id")]
    public string? SpecificationId { get; set; }

    /// <summary>订单总价格（分）。</summary>
    [JsonPropertyName("total_price")]
    public long? TotalPrice { get; set; }

    /// <summary>订单状态（1~4）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}
