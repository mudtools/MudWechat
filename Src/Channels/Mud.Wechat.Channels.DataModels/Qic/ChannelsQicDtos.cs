// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Qic;

/// <summary>查询质检仓配置（<c>getinspectconfig</c>）响应。</summary>
/// <remarks>官方契约：<b>GET</b> + 空请求体。</remarks>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsGetInspectConfigResponse : ChannelsResponse
{
    /// <summary>获取或设置质检仓配置信息（官方 <c>inspect_config</c>）。</summary>
    [JsonPropertyName("inspect_config")]
    public ChannelsInspectConfig? InspectConfig { get; set; }
}

/// <summary>质检仓配置（官方 <c>inspect_config</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsInspectConfig
{
    /// <summary>获取或设置质检仓 ID（官方 <c>warehouse_id</c>）。</summary>
    [JsonPropertyName("warehouse_id")]
    public string? WarehouseId { get; set; }

    /// <summary>获取或设置发货地址（官方 <c>delivery_address</c>）。</summary>
    [JsonPropertyName("delivery_address")]
    public ChannelsInspectAddress? DeliveryAddress { get; set; }

    /// <summary>获取或设置退货地址（官方 <c>return_address</c>）。</summary>
    [JsonPropertyName("return_address")]
    public ChannelsInspectAddress? ReturnAddress { get; set; }

    /// <summary>获取或设置质检仓名称（官方 <c>warehouse_name</c>）。</summary>
    [JsonPropertyName("warehouse_name")]
    public string? WarehouseName { get; set; }

    /// <summary>获取或设置质检仓地址（官方 <c>warehouse_addr</c>）。</summary>
    [JsonPropertyName("warehouse_addr")]
    public string? WarehouseAddr { get; set; }
}

/// <summary>质检收发货地址（官方 <c>delivery_address</c> / <c>return_address</c> 对象，字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsInspectAddress
{
    /// <summary>获取或设置联系人（官方 <c>contact_name</c>）。</summary>
    [JsonPropertyName("contact_name")]
    public string? ContactName { get; set; }

    /// <summary>获取或设置联系电话（官方 <c>contact_phone</c>）。</summary>
    [JsonPropertyName("contact_phone")]
    public string? ContactPhone { get; set; }

    /// <summary>获取或设置省（官方 <c>province</c>）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>获取或设置市（官方 <c>city</c>）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>获取或设置区（官方 <c>county</c>）。</summary>
    [JsonPropertyName("county")]
    public string? County { get; set; }

    /// <summary>获取或设置详细地址（官方 <c>detail</c>）。</summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }
}

/// <summary>查询送检配置模板信息（<c>getinspectsubmitconfig</c>）响应。</summary>
/// <remarks>官方契约：<b>GET</b> + 空请求体（<c>order_id</c> 走 URL 参数）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsGetInspectSubmitConfigResponse : ChannelsResponse
{
    /// <summary>获取或设置送检配置模板信息（官方 <c>submit_config</c>）。</summary>
    [JsonPropertyName("submit_config")]
    public ChannelsInspectSubmitConfig? SubmitConfig { get; set; }
}

/// <summary>送检配置模板（官方 <c>submit_config</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsInspectSubmitConfig
{
    /// <summary>获取或设置快递公司列表（官方 <c>delivery_list</c>）。</summary>
    [JsonPropertyName("delivery_list")]
    public List<ChannelsInspectDelivery>? DeliveryList { get; set; }

    /// <summary>获取或设置质检机构列表（官方 <c>inspect_org_list</c>）。</summary>
    [JsonPropertyName("inspect_org_list")]
    public List<ChannelsInspectOrg>? InspectOrgList { get; set; }

    /// <summary>获取或设置资费标准（官方 <c>charge_url</c>）。</summary>
    [JsonPropertyName("charge_url")]
    public string? ChargeUrl { get; set; }
}

/// <summary>送检快递公司（官方 <c>delivery_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsInspectDelivery
{
    /// <summary>获取或设置快递公司编码（官方 <c>id</c>，绑定送检信息 <c>delivery_id</c> 的取值来源）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>获取或设置快递公司名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置快递产品列表（官方 <c>delivery_products</c>）。</summary>
    [JsonPropertyName("delivery_products")]
    public List<ChannelsInspectDeliveryProduct>? DeliveryProducts { get; set; }
}

/// <summary>送检快递产品（官方 <c>delivery_products</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsInspectDeliveryProduct
{
    /// <summary>获取或设置快递公司产品 id（官方 <c>id</c>，绑定送检信息 <c>delivery_product_id</c> 的取值来源）。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>获取或设置快递公司产品名（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置是否支持保价（官方 <c>enable_insure</c>，见 <see cref="ChannelsInspectInsureSupports"/>）。</summary>
    [JsonPropertyName("enable_insure")]
    public int? EnableInsure { get; set; }

    /// <summary>获取或设置保价类型列表（官方 <c>insure_type_list</c>）。</summary>
    [JsonPropertyName("insure_type_list")]
    public List<ChannelsInspectInsureType>? InsureTypeList { get; set; }
}

/// <summary>保价类型（官方 <c>insure_type_list</c> 数组元素）。</summary>
/// <remarks>保价金额单位为<b>分</b>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsInspectInsureType
{
    /// <summary>获取或设置保价类型 id（官方 <c>id</c>，绑定送检信息 <c>delivery_insure_id</c> 的取值来源）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>获取或设置保价名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置上限类型（官方 <c>upper_limit_type</c>，见 <see cref="ChannelsInspectUpperLimitTypes"/>）。</summary>
    [JsonPropertyName("upper_limit_type")]
    public int? UpperLimitType { get; set; }

    /// <summary>获取或设置上限金额（官方 <c>upper_limit_amount</c>，单位：分）。</summary>
    [JsonPropertyName("upper_limit_amount")]
    public long? UpperLimitAmount { get; set; }
}

/// <summary>质检机构（官方 <c>inspect_org_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsInspectOrg
{
    /// <summary>获取或设置质检机构 id（官方 <c>id</c>，绑定送检信息机构 id 的取值来源）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>获取或设置质检机构名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置质检机构类别（官方 <c>org_category</c>，见 <see cref="ChannelsInspectOrgCategories"/>）。</summary>
    [JsonPropertyName("org_category")]
    public int? OrgCategory { get; set; }
}

/// <summary>打印质检码（<c>printinspectcode</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsPrintInspectCodeRequest
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填，官方示例 "37423523451235145"）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;
}

/// <summary>打印质检码（<c>printinspectcode</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsPrintInspectCodeResponse : ChannelsResponse
{
    /// <summary>获取或设置质检码详情（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsInspectCodeDetail? Data { get; set; }
}

/// <summary>
/// 质检码详情（官方 <c>data</c> 对象）。
/// </summary>
/// <remarks><b>官方本对象字段为 camelCase 命名</b>（如 backupDeliveryId、boxDTOList），与小店其余端点 snake_case 不同，字段名照抄官方。</remarks>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsInspectCodeDetail
{
    /// <summary>获取或设置备用发货快递编码（官方 <c>backupDeliveryId</c>）。</summary>
    [JsonPropertyName("backupDeliveryId")]
    public string? BackupDeliveryId { get; set; }

    /// <summary>获取或设置备用发货快递名称（官方 <c>backupDeliveryName</c>，官方说明原文「用发货快递名称」照录）。</summary>
    [JsonPropertyName("backupDeliveryName")]
    public string? BackupDeliveryName { get; set; }

    /// <summary>获取或设置包装盒列表（官方 <c>boxDTOList</c>）。</summary>
    [JsonPropertyName("boxDTOList")]
    public List<ChannelsInspectBox>? BoxDtoList { get; set; }

    /// <summary>获取或设置视频号店铺 appid（官方 <c>channelAppId</c>）。</summary>
    [JsonPropertyName("channelAppId")]
    public string? ChannelAppId { get; set; }

    /// <summary>获取或设置发货快递公司编码（官方 <c>deliveryId</c>，同视频号小店电子面单支持快递公司编码）。</summary>
    [JsonPropertyName("deliveryId")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置发货快递公司名称（官方 <c>deliveryName</c>）。</summary>
    [JsonPropertyName("deliveryName")]
    public string? DeliveryName { get; set; }

    /// <summary>获取或设置商品镶嵌材质（官方 <c>embedGoodsMaterial</c>）。</summary>
    [JsonPropertyName("embedGoodsMaterial")]
    public string? EmbedGoodsMaterial { get; set; }

    /// <summary>获取或设置商品描述（官方 <c>goodsDesc</c>）。</summary>
    [JsonPropertyName("goodsDesc")]
    public string? GoodsDesc { get; set; }

    /// <summary>获取或设置是否合并发货（官方 <c>expressMerge</c>，true：合并，false：不合并）。</summary>
    [JsonPropertyName("expressMerge")]
    public bool? ExpressMerge { get; set; }

    /// <summary>获取或设置商品主材质（官方 <c>goodsMainMaterial</c>）。</summary>
    [JsonPropertyName("goodsMainMaterial")]
    public string? GoodsMainMaterial { get; set; }

    /// <summary>获取或设置商品名称（官方 <c>goodsName</c>）。</summary>
    [JsonPropertyName("goodsName")]
    public string? GoodsName { get; set; }

    /// <summary>获取或设置商品件数（官方 <c>goodsNum</c>）。</summary>
    [JsonPropertyName("goodsNum")]
    public int? GoodsNum { get; set; }

    /// <summary>获取或设置商品配件材质（官方 <c>goodsPartsMaterial</c>）。</summary>
    [JsonPropertyName("goodsPartsMaterial")]
    public string? GoodsPartsMaterial { get; set; }

    /// <summary>获取或设置质检仓 id（官方 <c>inspectBaseId</c>）。</summary>
    [JsonPropertyName("inspectBaseId")]
    public string? InspectBaseId { get; set; }

    /// <summary>获取或设置质检仓名称（官方 <c>inspectBaseName</c>）。</summary>
    [JsonPropertyName("inspectBaseName")]
    public string? InspectBaseName { get; set; }

    /// <summary>获取或设置质检码（官方 <c>inspectCode</c>）。</summary>
    [JsonPropertyName("inspectCode")]
    public string? InspectCode { get; set; }

    /// <summary>获取或设置质检机构 id（官方 <c>inspectOrgId</c>）。</summary>
    [JsonPropertyName("inspectOrgId")]
    public string? InspectOrgId { get; set; }

    /// <summary>获取或设置质检机构名称（官方 <c>inspectOrgName</c>）。</summary>
    [JsonPropertyName("inspectOrgName")]
    public string? InspectOrgName { get; set; }

    /// <summary>获取或设置质检机构名称简称（官方 <c>inspectOrgShortName</c>）。</summary>
    [JsonPropertyName("inspectOrgShortName")]
    public string? InspectOrgShortName { get; set; }

    /// <summary>获取或设置商家名称（官方 <c>merchantName</c>）。</summary>
    [JsonPropertyName("merchantName")]
    public string? MerchantName { get; set; }

    /// <summary>获取或设置订单号（官方 <c>orderId</c>）。</summary>
    [JsonPropertyName("orderId")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置是否加急订单（官方 <c>urgentOrder</c>，true-加急，false-非加急）。</summary>
    [JsonPropertyName("urgentOrder")]
    public bool? UrgentOrder { get; set; }

    /// <summary>
    /// 获取或设置打印模板数据和模板 html 地址（官方 <c>printInfo</c>，base64 编码）——
    /// 官方质检面单打印新方案：只需把本字段传入打印控件即可进行打印。
    /// </summary>
    [JsonPropertyName("printInfo")]
    public string? PrintInfo { get; set; }

    /// <summary>获取或设置是否需要打标（官方 <c>needLabel</c>，官方变更日志 2026-03-24 新增；true：需要打标）。</summary>
    [JsonPropertyName("needLabel")]
    public bool? NeedLabel { get; set; }
}

/// <summary>质检包装盒（官方 <c>boxDTOList</c> 数组元素，camelCase 照抄官方）。</summary>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsInspectBox
{
    /// <summary>获取或设置包装盒 id（官方 <c>boxId</c>）。</summary>
    [JsonPropertyName("boxId")]
    public long? BoxId { get; set; }

    /// <summary>获取或设置包装盒名称（官方 <c>boxName</c>）。</summary>
    [JsonPropertyName("boxName")]
    public string? BoxName { get; set; }

    /// <summary>获取或设置包装盒数量（官方 <c>boxNum</c>）。</summary>
    [JsonPropertyName("boxNum")]
    public int? BoxNum { get; set; }
}

/// <summary>绑定送检信息（<c>submitinspectinfo</c>）请求体。</summary>
/// <remarks>官方契约：<b>POST</b> + 请求体 <c>{"order_id":…,"inspect_info":{…}}</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsSubmitInspectRequest
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置送检信息（官方 <c>inspect_info</c>，必填）。</summary>
    [JsonPropertyName("inspect_info")]
    public ChannelsInspectInfo InspectInfo { get; set; } = new();
}

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

/// <summary>自寄快递送检（<c>registerlogistics</c>）请求体。</summary>
/// <remarks>官方契约：<b>POST</b> + 请求体；最多可修改三次自寄快递信息。</remarks>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsRegisterInspectLogisticsRequest
{
    /// <summary>获取或设置订单号列表（官方 <c>order_id_list</c>，必填）。</summary>
    [JsonPropertyName("order_id_list")]
    public List<string> OrderIdList { get; set; } = new();

    /// <summary>获取或设置快递物流信息（官方 <c>logistics_info</c>，必填）。</summary>
    [JsonPropertyName("logistics_info")]
    public ChannelsInspectLogisticsInfo LogisticsInfo { get; set; } = new();
}

/// <summary>自寄快递物流信息（官方 <c>logistics_info</c> 对象，<c>registerlogistics</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsInspectLogisticsInfo
{
    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string WaybillId { get; set; } = string.Empty;

    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>，必填，通过获取快递公司列表接口获得）。</summary>
    [JsonPropertyName("delivery_id")]
    public string DeliveryId { get; set; } = string.Empty;

    /// <summary>获取或设置快递公司名称（官方 <c>delivery_name</c>，必填，通过获取快递公司列表接口获得）。</summary>
    [JsonPropertyName("delivery_name")]
    public string DeliveryName { get; set; } = string.Empty;

    /// <summary>获取或设置发货方式（官方 <c>delivery_type</c>，必填，本接口固定 1 自寄快递发货，见 <see cref="ChannelsInspectDeliveryTypes"/>）。</summary>
    [JsonPropertyName("delivery_type")]
    public int DeliveryType { get; set; }
}

/// <summary>质检机构类别常量（官方 <c>org_category</c> 枚举值，<c>getinspectsubmitconfig</c>）。</summary>
public static class ChannelsInspectOrgCategories
{
    /// <summary>珠宝质检机构。</summary>
    public const int Jewelry = 0;

    /// <summary>沉香质检机构。</summary>
    public const int Agarwood = 1;
}

/// <summary>保价支持常量（官方 <c>enable_insure</c> 枚举值，快递产品）。</summary>
public static class ChannelsInspectInsureSupports
{
    /// <summary>支持保价。</summary>
    public const int Supported = 1;

    /// <summary>不支持保价。</summary>
    public const int Unsupported = 2;
}

/// <summary>保价上限类型常量（官方 <c>upper_limit_type</c> 枚举值，保价类型）。</summary>
public static class ChannelsInspectUpperLimitTypes
{
    /// <summary>有上限。</summary>
    public const int Limited = 1;

    /// <summary>无上限。</summary>
    public const int Unlimited = 2;
}

/// <summary>售后拦截策略常量（官方 <c>refund_intercept</c> 枚举值，<c>submitinspectinfo</c>）。</summary>
public static class ChannelsInspectRefundIntercepts
{
    /// <summary>发生售后即拦截。</summary>
    public const int InterceptOnAfterSale = 0;

    /// <summary>部分退款不拦截。</summary>
    public const int IgnorePartialRefund = 1;
}

/// <summary>送检发货方式常量（官方 <c>delivery_type</c> 枚举值，<c>registerlogistics</c>）。</summary>
public static class ChannelsInspectDeliveryTypes
{
    /// <summary>自寄快递发货。</summary>
    public const int SelfExpress = 1;
}
