// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Qic;

/// <summary>送检信息（官方 <c>inspect_info</c> 对象，<c>submitinspectinfo</c>）。</summary>
/// <remarks>
/// 机构约束：当送检配置模板 <c>org_category</c> 出现 0（珠宝）时 <c>inspect_org_id</c> 必填、
/// 出现 1（沉香）时 <c>agarwood_inspect_org_id</c> 必填——两者至少有一个不为空；
/// 需保价时 <c>express_insure</c> 为 true，保价金额不能高于保价产品金额上限；备注限 32 个 utf8 字符且不可含材质信息。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsInspectInfo
{
    /// <summary>获取或设置发货快递公司编码（官方 <c>delivery_id</c>，必填，对应送检配置模板 delivery_list[].id）。</summary>
    [JsonPropertyName("delivery_id")]
    public string DeliveryId { get; set; } = string.Empty;

    /// <summary>获取或设置备用发货快递公司编码（官方 <c>backup_delivery_id</c>，必填）。</summary>
    [JsonPropertyName("backup_delivery_id")]
    public string BackupDeliveryId { get; set; } = string.Empty;

    /// <summary>获取或设置是否物流保价（官方 <c>express_insure</c>，必填，true：保价；false：不保价）。</summary>
    [JsonPropertyName("express_insure")]
    public bool ExpressInsure { get; set; }

    /// <summary>获取或设置保价金额（官方 <c>express_insure_amount</c>，单位：分；需保价时必填，不能高于保价产品金额上限）。</summary>
    [JsonPropertyName("express_insure_amount")]
    public long? ExpressInsureAmount { get; set; }

    /// <summary>获取或设置是否合并发货（官方 <c>express_merge</c>，必填，true：合并；false：不合并）。</summary>
    [JsonPropertyName("express_merge")]
    public bool ExpressMerge { get; set; }

    /// <summary>获取或设置珠宝质检机构 id（官方 <c>inspect_org_id</c>，选填；org_category 出现 0 时必填）。</summary>
    [JsonPropertyName("inspect_org_id")]
    public string? InspectOrgId { get; set; }

    /// <summary>获取或设置售后拦截策略（官方 <c>refund_intercept</c>，必填，见 <see cref="ChannelsInspectRefundIntercepts"/>）。</summary>
    [JsonPropertyName("refund_intercept")]
    public int RefundIntercept { get; set; }

    /// <summary>获取或设置珠宝质检机构名称（官方 <c>inspect_org_name</c>，选填）。</summary>
    [JsonPropertyName("inspect_org_name")]
    public string? InspectOrgName { get; set; }

    /// <summary>获取或设置质检仓名称（官方 <c>warehouse_name</c>，必填）。</summary>
    [JsonPropertyName("warehouse_name")]
    public string WarehouseName { get; set; } = string.Empty;

    /// <summary>获取或设置质检仓地址（官方 <c>warehouse_addr</c>，必填）。</summary>
    [JsonPropertyName("warehouse_addr")]
    public string WarehouseAddr { get; set; } = string.Empty;

    /// <summary>获取或设置发货快递公司产品 id（官方 <c>delivery_product_id</c>，必填，对应 delivery_products[].id）。</summary>
    [JsonPropertyName("delivery_product_id")]
    public long DeliveryProductId { get; set; }

    /// <summary>获取或设置保价类型 id（官方 <c>delivery_insure_id</c>，选填；需保价时必填）。</summary>
    [JsonPropertyName("delivery_insure_id")]
    public string? DeliveryInsureId { get; set; }

    /// <summary>获取或设置备用发货快递公司产品 id（官方 <c>backup_delivery_product_id</c>，必填）。</summary>
    [JsonPropertyName("backup_delivery_product_id")]
    public long BackupDeliveryProductId { get; set; }

    /// <summary>获取或设置备用发货保价类型 id（官方 <c>backup_delivery_insure_id</c>，选填；需保价时必填）。</summary>
    [JsonPropertyName("backup_delivery_insure_id")]
    public string? BackupDeliveryInsureId { get; set; }

    /// <summary>获取或设置备用发货是否物流保价（官方 <c>backup_express_insure</c>，选填）。</summary>
    [JsonPropertyName("backup_express_insure")]
    public bool? BackupExpressInsure { get; set; }

    /// <summary>获取或设置备用发货保价金额（官方 <c>backup_express_insure_amount</c>，单位：分；需保价时必填）。</summary>
    [JsonPropertyName("backup_express_insure_amount")]
    public long? BackupExpressInsureAmount { get; set; }

    /// <summary>获取或设置备注（官方 <c>remark</c>，选填，限制 32 个 utf8 字符，不可含材质信息）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>获取或设置沉香质检机构编号（官方 <c>agarwood_inspect_org_id</c>，选填；org_category 出现 1 时必填，与珠宝机构 id 至少有一个不为空）。</summary>
    [JsonPropertyName("agarwood_inspect_org_id")]
    public string? AgarwoodInspectOrgId { get; set; }

    /// <summary>获取或设置沉香质检机构名称（官方 <c>agarwood_inspect_org_name</c>，选填，与 agarwood_inspect_org_id 成对出现）。</summary>
    [JsonPropertyName("agarwood_inspect_org_name")]
    public string? AgarwoodInspectOrgName { get; set; }
}
