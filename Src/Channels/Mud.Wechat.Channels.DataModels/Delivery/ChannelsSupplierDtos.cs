// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>获取供货商列表（<c>get_supplier_list</c>）请求体。</summary>
/// <remarks>官方契约：<b>POST</b> + JSON 请求体；<c>offset</c>/<c>limit</c> 均选填。所属权限集 id：131。</remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsGetSupplierListRequest
{
    /// <summary>获取或设置偏移量（官方 <c>offset</c>，选填）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>获取或设置单页数量（官方 <c>limit</c>，选填）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}

/// <summary>获取供货商列表（<c>get_supplier_list</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsGetSupplierListResponse : ChannelsResponse
{
    /// <summary>获取或设置供货商列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsSupplier>? List { get; set; }

    /// <summary>获取或设置列表总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public int? TotalCount { get; set; }
}

/// <summary>供货商信息（官方 <c>list</c> 数组元素，<c>get_supplier_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplier
{
    /// <summary>获取或设置供货商名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置供货商 appid（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>获取或设置状态（官方 <c>status</c>，见 <see cref="ChannelsSupplierStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置绑定审核状态（官方 <c>bind_audit_status</c>，见 <see cref="ChannelsSupplierBindAuditStatuses"/>）。</summary>
    [JsonPropertyName("bind_audit_status")]
    public int? BindAuditStatus { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }
}

/// <summary>申请关联供货商（<c>invite_supplier</c>）请求体。</summary>
/// <remarks>
/// 官方契约：<b>POST</b> + JSON 请求体；<c>service_id</c> 在第三方平台代商家调用时<b>必填</b>
/// （平台将校验该 ServiceID 与实际调用方的一致性）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsInviteSupplierRequest
{
    /// <summary>获取或设置申请关联的供货商的 appid（官方 <c>supplier_appid</c>，必填）。</summary>
    [JsonPropertyName("supplier_appid")]
    public string SupplierAppid { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置第三方服务商 ServiceID（官方 <c>service_id</c>，选填）——
    /// 第三方平台代商家调用时必填，且平台将校验该 ServiceID 与实际调用方的一致性。
    /// </summary>
    [JsonPropertyName("service_id")]
    public long? ServiceId { get; set; }

    /// <summary>
    /// 获取或设置第三方服务商备注信息（官方 <c>remark</c>，选填）——应清晰说明具体的调用场景
    /// （官方建议示例：「由店铺成员 xxx 手动发起确认」「XX 货源平台『一件代发』供货商 xxx」）。
    /// </summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }
}

/// <summary>获取分配方式（<c>get_distribute</c>）响应。</summary>
/// <remarks>
/// 官方契约：<b>POST</b> + 空请求体（官方参数表无 Body 参数）；
/// 分配类型枚举见 <see cref="ChannelsDistributeTypes"/>。所属权限集 id：131。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsGetDistributeResponse : ChannelsResponse
{
    /// <summary>获取或设置分配类型（官方 <c>distribute_type</c>，见 <see cref="ChannelsDistributeTypes"/>）。</summary>
    [JsonPropertyName("distribute_type")]
    public int? DistributeType { get; set; }

    /// <summary>获取或设置全店订单自动分配的供货商 appid（官方 <c>all_supplier_appid</c>）。</summary>
    [JsonPropertyName("all_supplier_appid")]
    public string? AllSupplierAppid { get; set; }

    /// <summary>获取或设置全店订单自动分配的供货商名称（官方 <c>all_distribute_supplier_name</c>）。</summary>
    [JsonPropertyName("all_distribute_supplier_name")]
    public string? AllDistributeSupplierName { get; set; }
}

/// <summary>设置全店订单自动分配（<c>set_all_distribution</c>）请求体。</summary>
/// <remarks>官方契约：<b>POST</b> + JSON 请求体；<c>supplier_appid</c> 必填。</remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSetAllDistributionRequest
{
    /// <summary>获取或设置全店订单自动分配给该供货商的 appid（官方 <c>supplier_appid</c>，必填）。</summary>
    [JsonPropertyName("supplier_appid")]
    public string SupplierAppid { get; set; } = string.Empty;
}

/// <summary>按商品自动分配设置项（官方 <c>list</c> 数组元素，<c>set_product_distribute</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsProductDistributeItem
{
    /// <summary>获取或设置供货商 appid（官方 <c>supplier_appid</c>，必填）。</summary>
    [JsonPropertyName("supplier_appid")]
    public string SupplierAppid { get; set; } = string.Empty;

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>
    /// 获取或设置该商品是否自动分配给该供货商（官方 <c>is_distribute</c>，必填）——
    /// 官方原文：当 <c>is_distribute = false</c> 时取消分配给供货商。
    /// </summary>
    [JsonPropertyName("is_distribute")]
    public bool IsDistribute { get; set; }

    /// <summary>
    /// 获取或设置主赠品同步模式（官方 <c>apply_to_gift</c>，选填，默认同步）——
    /// 官方原文：当 <c>apply_to_gift = true</c> 时强制同步主品自动分配关系给赠品；
    /// 此参数不填时不变更主赠品同步模式。单独为赠品设置代发关系时须设为 <c>false</c>。
    /// </summary>
    [JsonPropertyName("apply_to_gift")]
    public bool? ApplyToGift { get; set; }
}

/// <summary>设置按商品自动分配（<c>set_product_distribute</c>）请求体。</summary>
/// <remarks>官方契约：<b>POST</b> + JSON 请求体；<c>list</c> 必填且数量不超过 200。</remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSetProductDistributeRequest
{
    /// <summary>获取或设置设置列表（官方 <c>list</c>，必填，数量不超过 200）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsProductDistributeItem> List { get; set; } = new();
}

/// <summary>获取商品对应的自动分配供货商（<c>get_product_default_distribute</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsGetProductDefaultDistributeRequest
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }
}

/// <summary>获取商品对应的自动分配供货商（<c>get_product_default_distribute</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsGetProductDefaultDistributeResponse : ChannelsResponse
{
    /// <summary>获取或设置是否存在已设置自动分配的供货商（官方 <c>exist_default_supplier</c>）。</summary>
    [JsonPropertyName("exist_default_supplier")]
    public bool? ExistDefaultSupplier { get; set; }

    /// <summary>
    /// 获取或设置自动分配的供货商 appid（官方 <c>supplier_appid</c>）——
    /// 当且仅当 <c>exist_default_supplier = true</c> 时有意义。
    /// </summary>
    [JsonPropertyName("supplier_appid")]
    public string? SupplierAppid { get; set; }
}

/// <summary>获取按商品自动分配的商品列表（<c>get_product_list</c>）请求体。</summary>
/// <remarks>
/// <para>
/// <b>官方文档矛盾照录</b>：官方请求示例中出现 <c>is_filter_product_status</c> / <c>product_status</c>
/// 两字段，但参数表未列出——SDK 按参数表建模，不登记表外字段。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsGetProductListRequest
{
    /// <summary>获取或设置是否过滤 <c>distribute_status</c>（官方 <c>is_filter_distribute_status</c>，选填）。</summary>
    [JsonPropertyName("is_filter_distribute_status")]
    public bool? IsFilterDistributeStatus { get; set; }

    /// <summary>
    /// 获取或设置分配状态（官方 <c>distribute_status</c>，选填，见 <see cref="ChannelsProductDistributeStatuses"/>）——
    /// 当且仅当 <c>is_filter_distribute_status = true</c> 时生效。
    /// </summary>
    [JsonPropertyName("distribute_status")]
    public int? DistributeStatus { get; set; }

    /// <summary>获取或设置是否过滤 <c>supplier_appid</c>（官方 <c>is_filter_supplier_appid</c>，选填）。</summary>
    [JsonPropertyName("is_filter_supplier_appid")]
    public bool? IsFilterSupplierAppid { get; set; }

    /// <summary>
    /// 获取或设置供货商 appid（官方 <c>supplier_appid</c>，选填）——
    /// 当且仅当 <c>is_filter_supplier_appid = true</c> 时有效。
    /// </summary>
    [JsonPropertyName("supplier_appid")]
    public string? SupplierAppid { get; set; }

    /// <summary>获取或设置单页数量（官方 <c>limit</c>，选填）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>获取或设置偏移量（官方 <c>page_context</c>，选填，分页用）——首次请求传空串，翻页回传上一页返回值，直到返回空串结束。</summary>
    [JsonPropertyName("page_context")]
    public string? PageContext { get; set; }
}

/// <summary>按商品自动分配的商品条目（官方 <c>list</c> 数组元素，<c>get_product_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsProductDistributeStatus
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置分配状态（官方 <c>distribute_status</c>，见 <see cref="ChannelsProductDistributeStatuses"/>）。</summary>
    [JsonPropertyName("distribute_status")]
    public int? DistributeStatus { get; set; }

    /// <summary>
    /// 获取或设置供货商 appid（官方 <c>supplier_appid</c>）——
    /// 当且仅当 <c>distribute_status = 0</c>（已分配）时有意义。
    /// </summary>
    [JsonPropertyName("supplier_appid")]
    public string? SupplierAppid { get; set; }

    /// <summary>
    /// 获取或设置供货商名称（官方 <c>supplier_name</c>）——
    /// 当且仅当 <c>distribute_status = 0</c>（已分配）时有意义。
    /// </summary>
    [JsonPropertyName("supplier_name")]
    public string? SupplierName { get; set; }
}

/// <summary>获取按商品自动分配的商品列表（<c>get_product_list</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsGetProductListResponse : ChannelsResponse
{
    /// <summary>获取或设置偏移量（官方 <c>page_context</c>，分页用；返回空串表示结束）。</summary>
    [JsonPropertyName("page_context")]
    public string? PageContext { get; set; }

    /// <summary>获取或设置总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public int? TotalCount { get; set; }

    /// <summary>获取或设置商品列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsProductDistributeStatus>? List { get; set; }
}

/// <summary>供货商状态常量（官方 <c>status</c> 枚举值，<c>get_supplier_list</c>）。</summary>
public static class ChannelsSupplierStatuses
{
    /// <summary>初始状态。</summary>
    public const int Init = 0;

    /// <summary>已绑定。</summary>
    public const int Bound = 1;

    /// <summary>已解绑。</summary>
    public const int Unbound = 2;
}

/// <summary>供货商绑定审核状态常量（官方 <c>bind_audit_status</c> 枚举值，<c>get_supplier_list</c>）。</summary>
public static class ChannelsSupplierBindAuditStatuses
{
    /// <summary>待审核。</summary>
    public const int Pending = 0;

    /// <summary>已接受。</summary>
    public const int Accepted = 1;

    /// <summary>已拒绝。</summary>
    public const int Rejected = 2;

    /// <summary>超时自动拒绝。</summary>
    public const int TimeoutRejected = 3;

    /// <summary>小店自行取消。</summary>
    public const int CanceledByShop = 4;

    /// <summary>初始状态。</summary>
    public const int Init = 5;
}

/// <summary>分配类型常量（官方 <c>distribute_type</c> 枚举值，<c>get_distribute</c>）。</summary>
public static class ChannelsDistributeTypes
{
    /// <summary>手动分配。</summary>
    public const int Manual = 1;

    /// <summary>全店自动分配。</summary>
    public const int AllAuto = 2;

    /// <summary>按商品分配。</summary>
    public const int ProductAuto = 3;
}

/// <summary>商品分配状态常量（官方 <c>distribute_status</c> 枚举值，<c>get_product_list</c>）。</summary>
public static class ChannelsProductDistributeStatuses
{
    /// <summary>已分配。</summary>
    public const int Distributed = 0;

    /// <summary>未分配。</summary>
    public const int NotDistributed = 1;
}
