// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>
/// 添加团购优惠（<c>addlocalproduct</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：<c>product_type</c> 与 <c>code_source_type</c> 创建后不支持修改；本接口不支持直接设置库存，
/// 创建完成后须调用库存管理接口。<c>attr_kv_map</c> 为字符串键值 map，值为 JSON 字符串化属性，
/// 键集参照官方「代金券属性KV / 兑换券属性KV」小节（如 use_date_rule、use_time、detail_imgs、set_meals、
/// exclusive_date、voucher_amount、bring_out_type、appointment、need_phone_num、additional_rule、
/// available_store_desc、settle_type、refund_policy）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsAddLocallifeProductRequest
{
    /// <summary>获取或设置外部系统 ID（官方 <c>out_product_id</c>，选填；仅作存储字段，商家需自行维护与平台 product_id 的对应关系）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>获取或设置券类型（官方 <c>product_type</c>，必填，见 <see cref="ChannelsLocallifeProductTypes"/>；创建后不支持修改）。</summary>
    [JsonPropertyName("product_type")]
    public int ProductType { get; set; }

    /// <summary>获取或设置名称（官方 <c>product_name</c>，必填；3–60 字符，中/英文/数字各算 1 字符，不得仅为数字或英文，不得含非法字符）。</summary>
    [JsonPropertyName("product_name")]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>获取或设置三级类目 ID（官方 <c>category_id</c>，string(uint64)，必填；经「获取所有的类目」接口获取）。</summary>
    [JsonPropertyName("category_id")]
    public string CategoryId { get; set; } = string.Empty;

    /// <summary>获取或设置品牌 ID（官方 <c>brand_id</c>，string(uint64)，必填；经「获取生效中的品牌资质列表」接口获取）。</summary>
    [JsonPropertyName("brand_id")]
    public string BrandId { get; set; } = string.Empty;

    /// <summary>获取或设置主图列表（官方 <c>head_imgs</c>，必填；须通过「上传图片」接口获取）。</summary>
    [JsonPropertyName("head_imgs")]
    public List<string> HeadImgs { get; set; } = new();

    /// <summary>获取或设置商品资质列表（官方 <c>product_qua_infos</c>，选填；参考类目信息中的 product_qua_list）。</summary>
    [JsonPropertyName("product_qua_infos")]
    public List<ChannelsLocallifeProductQuaInfo>? ProductQuaInfos { get; set; }

    /// <summary>获取或设置核销跳转出口（官方 <c>verify_page</c>，选填；verify_at_store=0 时必填，不满足体验规范的商家将被断播处罚）。</summary>
    [JsonPropertyName("verify_page")]
    public ChannelsLocallifeWxAppPath? VerifyPage { get; set; }

    /// <summary>获取或设置是否支持到店核销（官方 <c>verify_at_store</c>，选填；1=支持（默认），0=不支持。填 0 时订单页不展示核销二维码，且必须填 verify_page）。</summary>
    [JsonPropertyName("verify_at_store")]
    public int? VerifyAtStore { get; set; }

    /// <summary>获取或设置券码来源（官方 <c>code_source_type</c>，必填，见 <see cref="ChannelsLocallifeCodeSourceTypes"/>；创建后不支持修改）。</summary>
    [JsonPropertyName("code_source_type")]
    public int CodeSourceType { get; set; }

    /// <summary>获取或设置商品属性 KV（官方 <c>attr_kv_map</c>，必填；字符串键值 map，值为 JSON 字符串化属性）。</summary>
    [JsonPropertyName("attr_kv_map")]
    public Dictionary<string, string> AttrKvMap { get; set; } = new();

    /// <summary>获取或设置售卖单元列表（官方 <c>skus</c>，必填；本接口不支持直接设库存，需另调库存接口）。</summary>
    [JsonPropertyName("skus")]
    public List<ChannelsLocallifeSkuRequest> Skus { get; set; } = new();
}

/// <summary>添加团购优惠（<c>addlocalproduct</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsAddLocallifeProductResponse : ChannelsResponse
{
    /// <summary>获取或设置创建结果（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsAddLocallifeProductData? Data { get; set; }
}

/// <summary>添加团购优惠结果（官方 <c>data</c> 对象，<c>addlocalproduct</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsAddLocallifeProductData
{
    /// <summary>获取或设置平台生成的商品 ID（官方 <c>product_id</c>，string(uint64)）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置 SKU ID 列表（官方 <c>sku_ids</c>；与请求中 sku 列表顺序一致）。</summary>
    [JsonPropertyName("sku_ids")]
    public List<string>? SkuIds { get; set; }
}

/// <summary>查询团购优惠列表（<c>getlocalproductlist</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsGetLocallifeProductListRequest
{
    /// <summary>获取或设置商品状态（官方 <c>status</c>，选填；不填默认拉全部商品，不包含回收站）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，选填，默认 10，不超过 30）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }

    /// <summary>
    /// 获取或设置翻页上下文（官方 <c>next_key</c>，选填；由上次请求返回，传入时会从上次返回的结果往后翻一页，不传默认获取第一页数据）。
    /// </summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>查询团购优惠列表（<c>getlocalproductlist</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsGetLocallifeProductListResponse : ChannelsResponse
{
    /// <summary>获取或设置商品 ID 列表（官方 <c>product_ids</c>）。</summary>
    [JsonPropertyName("product_ids")]
    public List<string>? ProductIds { get; set; }

    /// <summary>获取或设置本次翻页的上下文（官方 <c>next_key</c>，用于请求下一页）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置商品总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }
}

/// <summary>查询团购优惠详情（<c>getlocalproduct</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsGetLocallifeProductRequest
{
    /// <summary>获取或设置平台生成的商品 ID（官方 <c>product_id</c>，string(uint64)，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置数据类型（官方 <c>data_type</c>，选填，默认取 1：1=获取线上数据，2=获取草稿数据，3=同时获取线上和草稿数据。上架过的商品才有线上数据）。</summary>
    [JsonPropertyName("data_type")]
    public int? DataType { get; set; }
}

/// <summary>查询团购优惠详情（<c>getlocalproduct</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsGetLocallifeProductResponse : ChannelsResponse
{
    /// <summary>获取或设置商品线上数据（官方 <c>online_data</c>；data_type=2 时不返回；data_type=3 且从未上架过时不返回）。</summary>
    [JsonPropertyName("online_data")]
    public ChannelsLocallifeProductData? OnlineData { get; set; }

    /// <summary>获取或设置商品草稿数据（官方 <c>draft_data</c>；data_type=1 时不返回）。</summary>
    [JsonPropertyName("draft_data")]
    public ChannelsLocallifeProductData? DraftData { get; set; }
}

/// <summary>
/// 团购优惠商品数据（官方 <c>online_data</c>/<c>draft_data</c> 对象，<c>getlocalproduct</c>——官方文档明示两者结构相同 ⇒ 共用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档矛盾照录</b>：响应参数表把 <c>product_id</c>/<c>category_id</c>/<c>brand_id</c> 标注为 number、
/// 返回示例为字符串（如 "15000000134007"）——SDK 按返回示例与同域 add/update 请求参数表 string(uint64) 建模为字符串。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsLocallifeProductData
{
    /// <summary>获取或设置平台生成的商品 ID（官方 <c>product_id</c>；官方参数表 number、返回示例为字符串——矛盾照录，建模为字符串）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置外部系统 ID（官方 <c>out_product_id</c>；只作为商品字段存储，不用做索引，商家需维护其与 product_id 的对应关系）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>获取或设置券类型（官方 <c>product_type</c>，见 <see cref="ChannelsLocallifeProductTypes"/>）。</summary>
    [JsonPropertyName("product_type")]
    public int? ProductType { get; set; }

    /// <summary>获取或设置名称（官方 <c>product_name</c>）。</summary>
    [JsonPropertyName("product_name")]
    public string? ProductName { get; set; }

    /// <summary>获取或设置三级类目 ID（官方 <c>category_id</c>；官方参数表 number、返回示例为字符串——矛盾照录，建模为字符串）。</summary>
    [JsonPropertyName("category_id")]
    public string? CategoryId { get; set; }

    /// <summary>获取或设置品牌 ID（官方 <c>brand_id</c>；官方参数表 number、返回示例为字符串——矛盾照录，建模为字符串）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>获取或设置主图列表（官方 <c>head_imgs</c>）。</summary>
    [JsonPropertyName("head_imgs")]
    public List<string>? HeadImgs { get; set; }

    /// <summary>获取或设置核销跳转出口（官方 <c>verify_page</c>）。</summary>
    [JsonPropertyName("verify_page")]
    public ChannelsLocallifeWxAppPath? VerifyPage { get; set; }

    /// <summary>获取或设置券码来源（官方 <c>code_source_type</c>，见 <see cref="ChannelsLocallifeCodeSourceTypes"/>）。</summary>
    [JsonPropertyName("code_source_type")]
    public int? CodeSourceType { get; set; }

    /// <summary>
    /// 获取或设置商品属性 KV（官方 <c>attr_kv_map</c>）。官方响应参数表列出已知键：<c>use_date_rule</c> 有效期规则、
    /// <c>total_price</c> 兑换券门市价（分）、<c>available_store_desc</c> 可用门店说明（≤100 字）、<c>use_time</c> 可用时段、
    /// <c>exclusive_date</c> 不可使用日期、<c>voucher_amount</c> 代金券面额、<c>bring_out_type</c> 堂食外带、
    /// <c>appointment</c> 是否需要预约、<c>limit_buy_rule</c> 单用户购买限制、<c>order_limit_num</c> 单笔订单限购数量（0–25）、
    /// <c>additional_rule</c> 其他文本规则（≤1000 字）、<c>need_phone_num</c> 是否需要用户手机号、<c>settle_type</c> 结算对象、
    /// <c>refund_policy</c> 退款策略、<c>detail_imgs</c> 详情图片（≤10 张）、<c>set_meals</c> 套餐内容（≤10 个子套餐分组）；
    /// 值为 JSON 字符串化属性。
    /// </summary>
    [JsonPropertyName("attr_kv_map")]
    public Dictionary<string, string>? AttrKvMap { get; set; }

    /// <summary>获取或设置售卖单元列表（官方 <c>skus</c>；仅返回示例列出 sku_id/sale_price/stock_num 三字段）。</summary>
    [JsonPropertyName("skus")]
    public List<ChannelsLocallifeSku>? Skus { get; set; }

    /// <summary>获取或设置商品草稿状态（官方 <c>draft_status</c>，见 <see cref="ChannelsLocallifeDraftStatuses"/>）。</summary>
    [JsonPropertyName("draft_status")]
    public int? DraftStatus { get; set; }

    /// <summary>获取或设置商品线上状态（官方 <c>online_status</c>，见 <see cref="ChannelsLocallifeOnlineStatuses"/>）。</summary>
    [JsonPropertyName("online_status")]
    public int? OnlineStatus { get; set; }

    /// <summary>获取或设置商品审核结果（官方 <c>audit_result</c>）。</summary>
    [JsonPropertyName("audit_result")]
    public ChannelsLocallifeAuditResult? AuditResult { get; set; }

    /// <summary>获取或设置是否支持到店核销（官方 <c>verify_at_store</c>；1=支持，0=不支持。选 0 时订单页不展示核销二维码且必须填 verify_page）。</summary>
    [JsonPropertyName("verify_at_store")]
    public int? VerifyAtStore { get; set; }

    /// <summary>获取或设置商品资质列表（官方 <c>product_qua_infos</c>）。</summary>
    [JsonPropertyName("product_qua_infos")]
    public List<ChannelsLocallifeProductQuaInfo>? ProductQuaInfos { get; set; }
}

/// <summary>团购优惠审核结果（官方 <c>audit_result</c> 对象，<c>getlocalproduct</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsLocallifeAuditResult
{
    /// <summary>获取或设置审核状态（官方 <c>audit_status</c>，见 <see cref="ChannelsLocallifeAuditStatuses"/>；官方返回示例亦出现 0——照录）。</summary>
    [JsonPropertyName("audit_status")]
    public int? AuditStatus { get; set; }

    /// <summary>获取或设置审核失败原因（官方 <c>audit_reason</c>；审核失败时有值）。</summary>
    [JsonPropertyName("audit_reason")]
    public string? AuditReason { get; set; }
}

/// <summary>
/// 团购优惠商品资质（官方 <c>product_qua_infos[]</c> 元素；add/update 请求与 get 响应共用——字段集一致。
/// 官方 add 页参数表 qua_id 为 number、update 页为 string——矛盾照录，按 add 页与 get 响应表建模为数字）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsLocallifeProductQuaInfo
{
    /// <summary>获取或设置商品资质 ID（官方 <c>qua_id</c>；对应类目信息中 product_qua_list[].qua_id）。</summary>
    [JsonPropertyName("qua_id")]
    public long? QuaId { get; set; }

    /// <summary>获取或设置商品资质图片列表（官方 <c>qua_url</c>；单个资质 ID 下最多 10 张，须使用「上传图片」接口）。</summary>
    [JsonPropertyName("qua_url")]
    public List<string>? QuaUrl { get; set; }
}

/// <summary>
/// 核销跳转出口小程序路径（官方 <c>verify_page</c>（WxAppPath）对象；add/update 请求与 get 响应共用——字段集一致）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsLocallifeWxAppPath
{
    /// <summary>获取或设置小程序 appid（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>获取或设置小程序页面路径（官方 <c>path</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }
}

/// <summary>
/// 团购优惠售卖单元请求形态（官方 <c>skus[]</c> 元素；add/update/auditfree 三端点共用——字段集一致：
/// <c>sku_id</c> 更新与免审更新时必填（add 由平台生成、官方示例仅传 sale_price），<c>sale_price</c> 必填）。
/// </summary>
/// <remarks>官方契约：本组接口均不支持直接设置库存数量，须调用修改库存接口。</remarks>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsLocallifeSkuRequest
{
    /// <summary>获取或设置平台生成的 SKU ID（官方 <c>sku_id</c>；更新/免审更新时必填）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置实际售卖价格（官方 <c>sale_price</c>，必填，单位：分）。</summary>
    [JsonPropertyName("sale_price")]
    public long SalePrice { get; set; }
}

/// <summary>团购优惠售卖单元响应形态（官方 <c>skus[]</c> 元素，<c>getlocalproduct</c> 返回示例形态）。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsLocallifeSku
{
    /// <summary>获取或设置平台生成的 SKU ID（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置实际售卖价格（官方 <c>sale_price</c>，单位：分）。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>获取或设置库存数量（官方 <c>stock_num</c>）。</summary>
    [JsonPropertyName("stock_num")]
    public long? StockNum { get; set; }
}

/// <summary>免审更新团购优惠（<c>updatelocalauditfree</c>）请求体。</summary>
/// <remarks>
/// 官方契约：仅针对<b>已上线</b>的本地团购优惠；更新时 <c>sku_id</c> 必填；
/// 本接口不支持直接设置库存数量，须调用修改库存接口。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsAuditFreeUpdateLocallifeProductRequest
{
    /// <summary>获取或设置平台生成的商品 ID（官方 <c>product_id</c>，string(uint64)，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置售卖单元列表（官方 <c>skus</c>，必填；更新时 sku_id 必填）。</summary>
    [JsonPropertyName("skus")]
    public List<ChannelsLocallifeSkuRequest> Skus { get; set; } = new();
}

/// <summary>
/// 更新团购优惠（<c>updatelocalproduct</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：全量更新形态；<c>product_type</c> 与 <c>code_source_type</c> 创建后不支持修改；
/// 更新时 <c>sku_id</c> 必填；本接口不支持直接设置库存数量，须调用修改库存接口。
/// 官方参数表未列 <c>out_product_id</c>（与 add 不同），照官方原文建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsUpdateLocallifeProductRequest
{
    /// <summary>获取或设置平台生成的商品 ID（官方 <c>product_id</c>，string(uint64)，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置券类型（官方 <c>product_type</c>，必填，见 <see cref="ChannelsLocallifeProductTypes"/>；创建后不支持修改）。</summary>
    [JsonPropertyName("product_type")]
    public int ProductType { get; set; }

    /// <summary>获取或设置名称（官方 <c>product_name</c>，必填；最少 3 字符，最多 60 字符，不得仅为数字或英文，不得含非法字符）。</summary>
    [JsonPropertyName("product_name")]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>获取或设置三级类目 ID（官方 <c>category_id</c>，string(uint64)，必填；经「获取所有的类目」接口获取）。</summary>
    [JsonPropertyName("category_id")]
    public string CategoryId { get; set; } = string.Empty;

    /// <summary>获取或设置品牌 ID（官方 <c>brand_id</c>，string(uint64)，必填；可通过「获取生效中的品牌资质列表」获取）。</summary>
    [JsonPropertyName("brand_id")]
    public string BrandId { get; set; } = string.Empty;

    /// <summary>获取或设置主图列表（官方 <c>head_imgs</c>，必填；须使用「上传图片」接口上传）。</summary>
    [JsonPropertyName("head_imgs")]
    public List<string> HeadImgs { get; set; } = new();

    /// <summary>获取或设置商品资质列表（官方 <c>product_qua_infos</c>，选填；参考「获取类目信息」的 product_qua_list）。</summary>
    [JsonPropertyName("product_qua_infos")]
    public List<ChannelsLocallifeProductQuaInfo>? ProductQuaInfos { get; set; }

    /// <summary>获取或设置核销跳转出口（官方 <c>verify_page</c>，选填；订单详情页可跳转至商家小程序核销，verify_at_store=0 时必填）。</summary>
    [JsonPropertyName("verify_page")]
    public ChannelsLocallifeWxAppPath? VerifyPage { get; set; }

    /// <summary>获取或设置是否支持到店核销（官方 <c>verify_at_store</c>，选填；1=支持（默认），0=不支持。选 0 时订单页不展示核销二维码，且必须填 verify_page）。</summary>
    [JsonPropertyName("verify_at_store")]
    public int? VerifyAtStore { get; set; }

    /// <summary>获取或设置券码来源（官方 <c>code_source_type</c>，必填，见 <see cref="ChannelsLocallifeCodeSourceTypes"/>；创建后不支持修改）。</summary>
    [JsonPropertyName("code_source_type")]
    public int CodeSourceType { get; set; }

    /// <summary>获取或设置商品属性 KV（官方 <c>attr_kv_map</c>，必填；字符串键值 map，值为 JSON 字符串化属性）。</summary>
    [JsonPropertyName("attr_kv_map")]
    public Dictionary<string, string> AttrKvMap { get; set; } = new();

    /// <summary>获取或设置售卖单元列表（官方 <c>skus</c>，必填；更新时 sku_id 必填，不支持直接设置库存数量）。</summary>
    [JsonPropertyName("skus")]
    public List<ChannelsLocallifeSkuRequest> Skus { get; set; } = new();
}

/// <summary>团购优惠券类型常量（官方 <c>product_type</c> 枚举值；创建后不支持修改）。</summary>
public static class ChannelsLocallifeProductTypes
{
    /// <summary>代金券。</summary>
    public const int Voucher = 1;

    /// <summary>兑换券。</summary>
    public const int Exchange = 2;

    /// <summary>次卡。</summary>
    public const int TimeCard = 3;
}

/// <summary>团购优惠券码来源常量（官方 <c>code_source_type</c> 枚举值；创建后不支持修改）。</summary>
public static class ChannelsLocallifeCodeSourceTypes
{
    /// <summary>平台生成。</summary>
    public const int Platform = 1;

    /// <summary>商家预存 code（官方 add 页原文「商家预存code，用户支付成功后平台发放」；update/get 页作「商家自定义」）。</summary>
    public const int Merchant = 2;

    /// <summary>发券实时传 code（官方注明「实时发券模式暂不开放」）。</summary>
    public const int Realtime = 3;
}

/// <summary>团购优惠草稿状态常量（官方 <c>draft_status</c> 枚举值，<c>getlocalproduct</c>）。</summary>
public static class ChannelsLocallifeDraftStatuses
{
    /// <summary>初始化。</summary>
    public const int Init = 0;

    /// <summary>编辑中。</summary>
    public const int Editing = 1;
}

/// <summary>团购优惠线上状态常量（官方 <c>online_status</c> 枚举值，<c>getlocalproduct</c>）。</summary>
public static class ChannelsLocallifeOnlineStatuses
{
    /// <summary>上架售卖中。</summary>
    public const int OnSale = 5;

    /// <summary>自主下架。</summary>
    public const int OffSaleByMerchant = 11;

    /// <summary>违规下架/风控系统下架。</summary>
    public const int OffSaleByRiskControl = 13;

    /// <summary>保证金不足下架。</summary>
    public const int OffSaleByDepositInsufficient = 14;

    /// <summary>品牌过期下架。</summary>
    public const int OffSaleByBrandExpired = 15;

    /// <summary>商品被封禁。</summary>
    public const int Banned = 20;
}

/// <summary>团购优惠审核状态常量（官方 <c>audit_status</c> 枚举值，<c>getlocalproduct</c> audit_result）。</summary>
public static class ChannelsLocallifeAuditStatuses
{
    /// <summary>等待审核中。</summary>
    public const int Waiting = 1;

    /// <summary>审核失败。</summary>
    public const int Failed = 2;

    /// <summary>审核成功。</summary>
    public const int Success = 3;
}
