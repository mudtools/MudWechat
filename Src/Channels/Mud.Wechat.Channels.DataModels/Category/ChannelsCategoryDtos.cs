// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>
/// 获取所有类目（<c>getallcategory</c>）响应。
/// </summary>
/// <remarks>
/// 官方契约：<b>POST</b> + 空请求体（官方变更日志：2026-09-15 HTTPS 调用由 GET 改为 POST）；
/// <c>cats</c> 为旧三级类目树、<c>cats_v2</c> 为新多级类目树，两版本嵌套结构差异见各嵌套类 XML 注释。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsGetAllCategoryResponse : ChannelsResponse
{
    /// <summary>获取或设置旧类目信息列表（官方 <c>cats</c>，旧三级类目树）。</summary>
    [JsonPropertyName("cats")]
    public List<ChannelsCategoryAndQua>? Cats { get; set; }

    /// <summary>获取或设置新类目信息列表（官方 <c>cats_v2</c>，新多级类目树；新旧目录树差异参阅官方「新旧类目差异说明」）。</summary>
    [JsonPropertyName("cats_v2")]
    public List<ChannelsCategoryAndQua>? CatsV2 { get; set; }
}

/// <summary>类目与资质信息（官方 <c>cats</c> / <c>cats_v2</c> 数组元素，结构一致）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryAndQua
{
    /// <summary>获取或设置类目详情（官方 <c>cat</c>；<c>cats_v2</c> 版本额外携带 <c>leaf</c>）。</summary>
    [JsonPropertyName("cat")]
    public ChannelsCategoryCat? Cat { get; set; }

    /// <summary>获取或设置类目资质详情（官方 <c>qua</c>；<c>cert_group_list</c> 仅 <c>cats</c> 旧树携带）。</summary>
    [JsonPropertyName("qua")]
    public ChannelsCategoryQua? Qua { get; set; }

    /// <summary>获取或设置商品资质信息（官方 <c>product_qua</c>，官方已注明「将废弃，使用 product_qua_list 代替」）。</summary>
    [JsonPropertyName("product_qua")]
    public ChannelsCategoryQuaInfo? ProductQua { get; set; }

    /// <summary>获取或设置品牌资质信息（官方 <c>brand_qua</c>）。</summary>
    [JsonPropertyName("brand_qua")]
    public ChannelsCategoryQuaInfo? BrandQua { get; set; }

    /// <summary>获取或设置商品资质列表（官方 <c>product_qua_list</c>，替代 <c>product_qua</c>）。</summary>
    [JsonPropertyName("product_qua_list")]
    public List<ChannelsCategoryQuaInfo>? ProductQuaList { get; set; }

    /// <summary>获取或设置是否放心买必须打开坏损包赔（官方 <c>is_confidence_require_bad_must_pay</c>）。</summary>
    [JsonPropertyName("is_confidence_require_bad_must_pay")]
    public bool? IsConfidenceRequireBadMustPay { get; set; }
}

/// <summary>类目详情（官方 <c>cat</c> 对象；<c>leaf</c> 仅 <c>cats_v2</c> 新树版本出现）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryCat
{
    /// <summary>获取或设置类目 ID（官方 <c>cat_id</c>）。</summary>
    [JsonPropertyName("cat_id")]
    public long? CatId { get; set; }

    /// <summary>获取或设置类目名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置父类目 ID（官方 <c>f_cat_id</c>）。</summary>
    [JsonPropertyName("f_cat_id")]
    public long? FCatId { get; set; }

    /// <summary>获取或设置类目等级（官方 <c>level</c>）。</summary>
    [JsonPropertyName("level")]
    public int? Level { get; set; }

    /// <summary>获取或设置是否为叶子类目（品类）（官方 <c>leaf</c>，仅 <c>cats_v2</c> 新树版本携带）。</summary>
    [JsonPropertyName("leaf")]
    public bool? Leaf { get; set; }
}

/// <summary>类目资质详情（官方 <c>qua</c> 对象；<c>cert_group_list</c> 仅 <c>cats</c> 旧树版本携带）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryQua
{
    /// <summary>获取或设置资质 ID（官方 <c>qua_id</c>）。</summary>
    [JsonPropertyName("qua_id")]
    public long? QuaId { get; set; }

    /// <summary>获取或设置该类目是否需要申请才能用（官方 <c>need_to_apply</c>）。</summary>
    [JsonPropertyName("need_to_apply")]
    public bool? NeedToApply { get; set; }

    /// <summary>获取或设置资质描述信息（官方 <c>tips</c>）。</summary>
    [JsonPropertyName("tips")]
    public string? Tips { get; set; }

    /// <summary>获取或设置申请此类目时是否必须提交资质证明（官方 <c>mandatory</c>）。</summary>
    [JsonPropertyName("mandatory")]
    public bool? Mandatory { get; set; }

    /// <summary>
    /// 获取或设置证照信息列表（官方 <c>cert_group_list</c>，仅 <c>cats</c> 旧树版本携带）——
    /// 每个类目有多个证照组要求，须提交全部证照组；证照组内的证照选一个填写提交即可。
    /// </summary>
    [JsonPropertyName("cert_group_list")]
    public List<ChannelsCategoryCertGroup>? CertGroupList { get; set; }
}

/// <summary>证照信息（官方 <c>cert_group_list</c> 数组元素，<c>getallcategory</c> 旧树 <c>qua</c> 内）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryCertGroup
{
    /// <summary>获取或设置证照组 id（官方 <c>license_group_id</c>，证照组内的证照选一个填写提交即可）。</summary>
    [JsonPropertyName("license_group_id")]
    public long? LicenseGroupId { get; set; }

    /// <summary>获取或设置是否必填（官方 <c>is_necessary</c>）。</summary>
    [JsonPropertyName("is_necessary")]
    public int? IsNecessary { get; set; }

    /// <summary>获取或设置证照组说明（官方 <c>busi_license_desc</c>）。</summary>
    [JsonPropertyName("busi_license_desc")]
    public string? BusiLicenseDesc { get; set; }

    /// <summary>获取或设置证照列表（官方 <c>license_list</c>）。</summary>
    [JsonPropertyName("license_list")]
    public List<ChannelsCategoryLicenseInfo>? LicenseList { get; set; }
}

/// <summary>证照列表项（官方 <c>license_list</c> 数组元素，<c>getallcategory</c> 证照组内）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryLicenseInfo
{
    /// <summary>获取或设置证照 id（官方 <c>id</c>）。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>获取或设置证照名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置是否必填（官方 <c>is_necessary</c>）。</summary>
    [JsonPropertyName("is_necessary")]
    public int? IsNecessary { get; set; }

    /// <summary>获取或设置填写字段列表（官方 <c>license_field_list</c>）。</summary>
    [JsonPropertyName("license_field_list")]
    public List<ChannelsCategoryLicenseField>? LicenseFieldList { get; set; }
}

/// <summary>证照填写字段（官方 <c>license_field_list</c> 数组元素，<c>getallcategory</c> 证照内）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryLicenseField
{
    /// <summary>
    /// 获取或设置填写字段类型（官方 <c>field_key</c>）——官方枚举：<c>audit_license_no</c>（许可证编号）/
    /// <c>audit_license_registration_no</c>（备案编号）/ <c>audit_license_validty_period</c>（有效期，官方历史拼写照录）。
    /// </summary>
    [JsonPropertyName("field_key")]
    public string? FieldKey { get; set; }

    /// <summary>获取或设置填写字段名称（官方 <c>field_name</c>）。</summary>
    [JsonPropertyName("field_name")]
    public string? FieldName { get; set; }

    /// <summary>获取或设置字段正则要求（官方 <c>info</c>）。</summary>
    [JsonPropertyName("info")]
    public string? Info { get; set; }

    /// <summary>获取或设置是否必填（官方 <c>is_necessary</c>）。</summary>
    [JsonPropertyName("is_necessary")]
    public int? IsNecessary { get; set; }
}

/// <summary>资质条目（官方 <c>product_qua</c> / <c>brand_qua</c> 对象与 <c>product_qua_list</c> 数组元素共用形态——
/// 官方 <c>product_qua</c>/<c>brand_qua</c> 未携带 <c>name</c> 字段，可空超集覆盖）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryQuaInfo
{
    /// <summary>获取或设置资质 ID（官方 <c>qua_id</c>）。</summary>
    [JsonPropertyName("qua_id")]
    public long? QuaId { get; set; }

    /// <summary>获取或设置该类目是否需要申请才能用（官方 <c>need_to_apply</c>）。</summary>
    [JsonPropertyName("need_to_apply")]
    public bool? NeedToApply { get; set; }

    /// <summary>获取或设置资质的描述信息（官方 <c>tips</c>）。</summary>
    [JsonPropertyName("tips")]
    public string? Tips { get; set; }

    /// <summary>获取或设置申请此类目时是否必须提交资质证明（官方 <c>mandatory</c>）。</summary>
    [JsonPropertyName("mandatory")]
    public bool? Mandatory { get; set; }

    /// <summary>获取或设置资质名称（官方 <c>name</c>，仅 <c>product_qua_list</c> 元素携带）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

/// <summary>获取类目信息（<c>getcategorydetail</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsGetCategoryDetailRequest
{
    /// <summary>获取或设置品类 ID（官方 <c>cat_id</c>，必填，叶子类目 ID）。</summary>
    [JsonPropertyName("cat_id")]
    public long CatId { get; set; }
}

/// <summary>获取类目信息（<c>getcategorydetail</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsGetCategoryDetailResponse : ChannelsResponse
{
    /// <summary>获取或设置类目信息（官方 <c>info</c>：类目 id / 名称）。</summary>
    [JsonPropertyName("info")]
    public ChannelsCategoryInfo? Info { get; set; }

    /// <summary>获取或设置类目属性信息（官方 <c>attr</c>）。</summary>
    [JsonPropertyName("attr")]
    public ChannelsCategoryAttr? Attr { get; set; }

    /// <summary>获取或设置资质信息（官方 <c>product_qua_list</c>）。</summary>
    [JsonPropertyName("product_qua_list")]
    public List<ChannelsCategoryQuaInfo>? ProductQuaList { get; set; }
}

/// <summary>类目基础信息（官方 <c>info</c> 对象，<c>getcategorydetail</c> 与 <c>getcategoryproductrule</c> 字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryInfo
{
    /// <summary>获取或设置类目 ID（官方 <c>cat_id</c>）。</summary>
    [JsonPropertyName("cat_id")]
    public long? CatId { get; set; }

    /// <summary>获取或设置类目名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

/// <summary>类目属性信息（官方 <c>attr</c> 对象，<c>getcategorydetail</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryAttr
{
    /// <summary>获取或设置是否支持虚拟发货（官方 <c>shop_no_shipment</c>）。</summary>
    [JsonPropertyName("shop_no_shipment")]
    public bool? ShopNoShipment { get; set; }

    /// <summary>获取或设置是否定向准入（官方 <c>access_permit_required</c>）。</summary>
    [JsonPropertyName("access_permit_required")]
    public bool? AccessPermitRequired { get; set; }

    /// <summary>获取或设置是否支持预售（官方 <c>pre_sale</c>）。</summary>
    [JsonPropertyName("pre_sale")]
    public bool? PreSale { get; set; }

    /// <summary>获取或设置是否必须支持 7 天无理由退货（官方 <c>seven_day_return</c>）。</summary>
    [JsonPropertyName("seven_day_return")]
    public bool? SevenDayReturn { get; set; }

    /// <summary>获取或设置定准类目品牌列表（官方 <c>brand_list</c>）。</summary>
    [JsonPropertyName("brand_list")]
    public List<ChannelsCategoryBrandRef>? BrandList { get; set; }

    /// <summary>获取或设置类目关联的保证金（官方 <c>deposit</c>，单位：分）。</summary>
    [JsonPropertyName("deposit")]
    public long? Deposit { get; set; }

    /// <summary>获取或设置产品属性（官方 <c>product_attr_list</c>；官方建议改用「获取类目下商品发布规则」接口）。</summary>
    [JsonPropertyName("product_attr_list")]
    public List<ChannelsCategoryAttrItem>? ProductAttrList { get; set; }

    /// <summary>获取或设置销售属性（官方 <c>sale_attr_list</c>；官方建议改用「获取类目下商品发布规则」接口）。</summary>
    [JsonPropertyName("sale_attr_list")]
    public List<ChannelsCategoryAttrItem>? SaleAttrList { get; set; }

    /// <summary>获取或设置交易费信息（官方 <c>transactionfee_info</c>）。</summary>
    [JsonPropertyName("transactionfee_info")]
    public ChannelsCategoryTransactionFeeInfo? TransactionFeeInfo { get; set; }

    /// <summary>获取或设置折扣规则（官方 <c>coupon_rule</c>）。</summary>
    [JsonPropertyName("coupon_rule")]
    public ChannelsCategoryCouponRule? CouponRule { get; set; }

    /// <summary>获取或设置价格下限（官方 <c>floor_price</c>，单位：分）。</summary>
    [JsonPropertyName("floor_price")]
    public long? FloorPrice { get; set; }

    /// <summary>
    /// 获取或设置收货时间选项（官方 <c>confirm_receipt_days</c>）——官方参数表仅标 array、未标注元素类型，SDK 按数值列表建模。
    /// </summary>
    [JsonPropertyName("confirm_receipt_days")]
    public List<long>? ConfirmReceiptDays { get; set; }

    /// <summary>获取或设置是否品牌定向准入（官方 <c>is_limit_brand</c>）。</summary>
    [JsonPropertyName("is_limit_brand")]
    public bool? IsLimitBrand { get; set; }

    /// <summary>获取或设置商品编辑要求（官方 <c>product_requirement</c>）。</summary>
    [JsonPropertyName("product_requirement")]
    public ChannelsCategoryProductRequirement? ProductRequirement { get; set; }

    /// <summary>获取或设置尺码表（官方 <c>size_chart</c>）。</summary>
    [JsonPropertyName("size_chart")]
    public ChannelsCategorySizeChart? SizeChart { get; set; }

    /// <summary>获取或设置是否放心买必须打开坏损包赔（官方 <c>is_confidence_require_bad_must_pay</c>）。</summary>
    [JsonPropertyName("is_confidence_require_bad_must_pay")]
    public bool? IsConfidenceRequireBadMustPay { get; set; }
}

/// <summary>类目属性项（官方 <c>product_attr_list</c> / <c>sale_attr_list</c> 数组元素，<c>getcategorydetail</c> 形态——
/// 与 <c>getcategoryproductrule</c> 的属性形态不同（那边无 <c>type</c>/<c>is_required</c>、多 <c>required_rule</c>）⇒ 分建）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryAttrItem
{
    /// <summary>获取或设置类目必填项名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置属性类型（官方 <c>type</c>）：<c>string</c> 为自定义，<c>select_one</c> 为多选一；
    /// 短期保留，将废弃，用 <c>type_v2</c> 替代。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置可选项列表（官方 <c>value</c>）——<c>type</c> 为 <c>select_one</c>/<c>select_many</c> 时为选项列表，
    /// <c>integer_unit</c>/<c>decimal4_unit</c> 时为单位列表。
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    /// <summary>获取或设置是否类目必填项（官方 <c>is_required</c>）。</summary>
    [JsonPropertyName("is_required")]
    public bool? IsRequired { get; set; }

    /// <summary>获取或设置输入提示（官方 <c>hint</c>）。</summary>
    [JsonPropertyName("hint")]
    public string? Hint { get; set; }

    /// <summary>获取或设置允许添加选项（官方 <c>append_allowed</c>）。</summary>
    [JsonPropertyName("append_allowed")]
    public bool? AppendAllowed { get; set; }

    /// <summary>获取或设置属性类型 v2（官方 <c>type_v2</c>，7 种取值，见 <see cref="ChannelsCategoryAttrTypes"/>）。</summary>
    [JsonPropertyName("type_v2")]
    public string? TypeV2 { get; set; }
}

/// <summary>交易费信息（官方 <c>transactionfee_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryTransactionFeeInfo
{
    /// <summary>获取或设置类目实收交易佣金比例（官方 <c>basis_point</c>，单位：万分比）。</summary>
    [JsonPropertyName("basis_point")]
    public long? BasisPoint { get; set; }

    /// <summary>获取或设置类目原始佣金比例（官方 <c>original_basis_point</c>，单位：万分比）。</summary>
    [JsonPropertyName("original_basis_point")]
    public long? OriginalBasisPoint { get; set; }

    /// <summary>获取或设置佣金激励类型（官方 <c>incentive_type</c>，见 <see cref="ChannelsCategoryIncentiveTypes"/>）。</summary>
    [JsonPropertyName("incentive_type")]
    public int? IncentiveType { get; set; }
}

/// <summary>折扣规则（官方 <c>coupon_rule</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryCouponRule
{
    /// <summary>获取或设置最高折扣比例（官方 <c>discount_ratio_limit</c>，百分比，0 表示无限制）。</summary>
    [JsonPropertyName("discount_ratio_limit")]
    public long? DiscountRatioLimit { get; set; }

    /// <summary>获取或设置最高折扣金额（官方 <c>discount_limit</c>，单位：分，0 表示无限制）。</summary>
    [JsonPropertyName("discount_limit")]
    public long? DiscountLimit { get; set; }
}

/// <summary>商品编辑要求（官方 <c>product_requirement</c> 对象，<c>getcategorydetail</c> 与 <c>getcategoryproductrule</c> 字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryProductRequirement
{
    /// <summary>获取或设置商品标题的编辑要求（官方 <c>product_title_requirement</c>）。</summary>
    [JsonPropertyName("product_title_requirement")]
    public string? ProductTitleRequirement { get; set; }

    /// <summary>获取或设置商品主图的编辑要求（官方 <c>product_img_requirement</c>）。</summary>
    [JsonPropertyName("product_img_requirement")]
    public string? ProductImgRequirement { get; set; }

    /// <summary>获取或设置商品描述的编辑要求（官方 <c>product_desc_requirement</c>）。</summary>
    [JsonPropertyName("product_desc_requirement")]
    public string? ProductDescRequirement { get; set; }
}

/// <summary>尺码表（官方 <c>size_chart</c> 对象，<c>getcategorydetail</c> 与 <c>getcategoryproductrule</c> 字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategorySizeChart
{
    /// <summary>获取或设置是否支持尺码表（官方 <c>is_support</c>）。</summary>
    [JsonPropertyName("is_support")]
    public bool? IsSupport { get; set; }

    /// <summary>获取或设置尺码属性列表（官方 <c>item_list</c>）。</summary>
    [JsonPropertyName("item_list")]
    public List<ChannelsCategorySizeChartItem>? ItemList { get; set; }
}

/// <summary>尺码属性项（官方 <c>size_chart.item_list</c> 数组元素；官方 <c>type</c>/<c>format</c>/<c>limit</c> 标注为 string，照抄）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategorySizeChartItem
{
    /// <summary>获取或设置尺码属性名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置单位（官方 <c>unit</c>）。</summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    /// <summary>获取或设置尺码属性值的类型（官方 <c>type</c>，官方标注 string：「1：字符型，2：整数型，3：小数型」）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置尺码属性值的填写格式（官方 <c>format</c>，官方标注 string：「1：单值填写，2：区间值填写，3：支持单值或区间值」）。</summary>
    [JsonPropertyName("format")]
    public string? Format { get; set; }

    /// <summary>获取或设置尺码属性值的限制（官方 <c>limit</c>，官方标注 string）。</summary>
    [JsonPropertyName("limit")]
    public string? Limit { get; set; }

    /// <summary>获取或设置是否必填（官方 <c>is_required</c>）。</summary>
    [JsonPropertyName("is_required")]
    public bool? IsRequired { get; set; }
}

/// <summary>定准类目品牌引用（官方 <c>attr.brand_list</c> 数组元素与「申请类目」<c>brand_list</c> 元素字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryBrandRef
{
    /// <summary>获取或设置品牌 ID（官方 <c>brand_id</c>；申请类目时须为店铺已审核通过的品牌 ID）。</summary>
    [JsonPropertyName("brand_id")]
    public long? BrandId { get; set; }
}

/// <summary>申请类目（<c>addcategory</c>）请求体。</summary>
/// <remarks>
/// 官方契约：<b>POST</b> + 请求体 <c>{"category_info":{…}}</c>；成功后创建审核单并返回 <c>audit_id</c>。
/// 图片必须用「上传资质图片」接口获取的 file_id；资质材料最多 10 张。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsAddCategoryRequest
{
    /// <summary>获取或设置申请的类目信息（官方 <c>category_info</c>，必填）。</summary>
    [JsonPropertyName("category_info")]
    public ChannelsCategoryInfoApply CategoryInfo { get; set; } = new();
}

/// <summary>申请类目信息（官方 <c>category_info</c> 对象，<c>addcategory</c>）。</summary>
/// <remarks>
/// <b>类目树双轨</b>：设了 <c>cats_v2</c> 则优先读取（多级结构）；未设则用 <c>level1/level2/level3</c>（旧三级结构）；
/// <c>cats_v2</c> 数组下标顺序与一至 N 级类目严格一致。
/// <b>官方参数表标「certificate 必填」但同时注明已废弃</b>，须改用证照组 <c>license_group_list</c>
/// （<c>is_new_apply_cat</c> 须设为 true）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryInfoApply
{
    /// <summary>获取或设置一级类目 ID（官方 <c>level1</c>，必填；未设 <c>cats_v2</c> 时生效）。</summary>
    [JsonPropertyName("level1")]
    public long Level1 { get; set; }

    /// <summary>获取或设置二级类目 ID（官方 <c>level2</c>，必填；未设 <c>cats_v2</c> 时生效）。</summary>
    [JsonPropertyName("level2")]
    public long Level2 { get; set; }

    /// <summary>获取或设置三级类目 ID（官方 <c>level3</c>，必填；未设 <c>cats_v2</c> 时生效）。</summary>
    [JsonPropertyName("level3")]
    public long Level3 { get; set; }

    /// <summary>
    /// 获取或设置类目树（官方 <c>cats_v2</c>，新多级类目）——官方参数表标必填，但官方注意事项写明
    /// 「未设则用 level1/level2/level3（旧三级结构）」（文档矛盾照录，SDK 按注意事项建模为可空）。
    /// </summary>
    [JsonPropertyName("cats_v2")]
    public List<ChannelsCategoryTreeNode>? CatsV2 { get; set; }

    /// <summary>
    /// 获取或设置资质材料（官方 <c>certificate</c>，官方标必填但注明「已废弃，须改用证照组」；
    /// 官方参数表仅标 array、描述为「图片fileid+类型」，SDK 按 file_id 字符串列表建模）。
    /// </summary>
    [JsonPropertyName("certificate")]
    public List<string>? Certificate { get; set; }

    /// <summary>获取或设置报备函（官方 <c>baobeihan</c>，选填，最多 10 张）。</summary>
    [JsonPropertyName("baobeihan")]
    public List<string>? Baobeihan { get; set; }

    /// <summary>获取或设置经营证明（官方 <c>jingyingzhengming</c>，选填，最多 10 张）。</summary>
    [JsonPropertyName("jingyingzhengming")]
    public List<string>? Jingyingzhengming { get; set; }

    /// <summary>获取或设置带货口碑（官方 <c>daihuokoubei</c>，选填，最多 10 张）。</summary>
    [JsonPropertyName("daihuokoubei")]
    public List<string>? Daihuokoubei { get; set; }

    /// <summary>获取或设置入驻资质（官方 <c>ruzhuzhizhi</c>，选填，最多 10 张）。</summary>
    [JsonPropertyName("ruzhuzhizhi")]
    public List<string>? Ruzhuzhizhi { get; set; }

    /// <summary>获取或设置经营流水（官方 <c>jingyingliushui</c>，选填，最多 10 张）。</summary>
    [JsonPropertyName("jingyingliushui")]
    public List<string>? Jingyingliushui { get; set; }

    /// <summary>获取或设置补充材料（官方 <c>buchongcailiao</c>，选填，最多 10 张）。</summary>
    [JsonPropertyName("buchongcailiao")]
    public List<string>? Buchongcailiao { get; set; }

    /// <summary>
    /// 获取或设置经营平台（官方 <c>jingyingpintai</c>——官方历史拼写照录，少一个 g，
    /// 审核单详情响应中同义字段拼作 <c>jingyingpingtai</c>，两者勿统一）；
    /// 仅支持 taobao/jd/douyin/kuaishou/pdd/other。
    /// </summary>
    [JsonPropertyName("jingyingpintai")]
    public string? Jingyingpintai { get; set; }

    /// <summary>获取或设置账号名称（官方 <c>zhanghaomingcheng</c>，选填）。</summary>
    [JsonPropertyName("zhanghaomingcheng")]
    public string? Zhanghaomingcheng { get; set; }

    /// <summary>获取或设置品牌列表（官方 <c>brand_list</c>，选填；当类目 <c>attr.is_limit_brand</c> 为 true 时必传）。</summary>
    [JsonPropertyName("brand_list")]
    public List<ChannelsCategoryBrandRef>? BrandList { get; set; }

    /// <summary>获取或设置证照组列表（官方 <c>license_group_list</c>，必填）。</summary>
    [JsonPropertyName("license_group_list")]
    public List<ChannelsCategoryLicenseGroup> LicenseGroupList { get; set; } = new();

    /// <summary>获取或设置是否为新版证照组申请（官方 <c>is_new_apply_cat</c>，必填；官方要求须设为 true 并用证照组提交）。</summary>
    [JsonPropertyName("is_new_apply_cat")]
    public bool IsNewApplyCat { get; set; }
}

/// <summary>新类目树节点（官方 <c>cats_v2</c> 数组元素，<c>addcategory</c>；下标顺序与一至 N 级类目严格一致）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryTreeNode
{
    /// <summary>获取或设置新类目树类目 ID（官方 <c>cat_id</c>，必填）。</summary>
    [JsonPropertyName("cat_id")]
    public long CatId { get; set; }
}

/// <summary>证照组（官方 <c>license_group_list</c> 数组元素——「申请类目」请求体与「审核单详情」响应 <c>audit_info</c> 字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryLicenseGroup
{
    /// <summary>获取或设置证照组 id（官方 <c>license_group_id</c>，必填；经「获取所有类目」获取）。</summary>
    [JsonPropertyName("license_group_id")]
    public long LicenseGroupId { get; set; }

    /// <summary>获取或设置证照信息（官方 <c>license</c>，必填；组内多个证照只需选一个提交）。</summary>
    [JsonPropertyName("license")]
    public ChannelsCategoryLicense License { get; set; } = new();
}

/// <summary>证照信息（官方 <c>license</c> 对象——「申请类目」请求体与「审核单详情」响应字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryLicense
{
    /// <summary>获取或设置证照 id（官方 <c>license_id</c>，必填；经「获取所有类目」获取）。</summary>
    [JsonPropertyName("license_id")]
    public long LicenseId { get; set; }

    /// <summary>获取或设置证照图片列表（官方 <c>file_id_list</c>，必填；必须用「上传资质图片」接口获取的 file_id）。</summary>
    [JsonPropertyName("file_id_list")]
    public List<string> FileIdList { get; set; } = new();

    /// <summary>获取或设置证照填写字段列表（官方 <c>license_field_list</c>，必填）。</summary>
    [JsonPropertyName("license_field_list")]
    public List<ChannelsCategoryLicenseFieldPair> LicenseFieldList { get; set; } = new();
}

/// <summary>证照填写字段键值对（官方 <c>license_field_list</c> 数组元素 <c>{key, value}</c>，证照提交与审核材料形态一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryLicenseFieldPair
{
    /// <summary>获取或设置字段 key（官方 <c>key</c>，必填；取值为「获取所有类目」返回的 <c>field_key</c>）。</summary>
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    /// <summary>获取或设置字段 value（官方 <c>value</c>，必填）。</summary>
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}

/// <summary>申请类目（<c>addcategory</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsAddCategoryResponse : ChannelsResponse
{
    /// <summary>获取或设置审核单 id（官方 <c>audit_id</c>；审核完成后通过「类目审核结果通知」回调通知）。</summary>
    [JsonPropertyName("audit_id")]
    public long? AuditId { get; set; }
}

/// <summary>获取店铺的类目审核单列表（<c>getbizcatflowlist</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsGetBizCatFlowListRequest
{
    /// <summary>获取或设置是否过滤状态（官方 <c>is_filter_status</c>，必填）。</summary>
    [JsonPropertyName("is_filter_status")]
    public bool IsFilterStatus { get; set; }

    /// <summary>获取或设置过滤的状态（官方 <c>status</c>，选填；当 is_filter_status = true 时生效，见 <see cref="ChannelsCategoryAuditStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置偏移量（官方 <c>offset</c>，必填）。</summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>limit</c>，必填）。</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}

/// <summary>获取店铺的类目审核单列表（<c>getbizcatflowlist</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsGetBizCatFlowListResponse : ChannelsResponse
{
    /// <summary>获取或设置审核单列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsCategoryAuditFlow>? List { get; set; }
}

/// <summary>
/// 类目审核单（官方 <c>list</c> 数组元素与 <c>getbizcatflowdetail</c> 响应 <c>info</c> 对象字段集一致 ⇒ 共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryAuditFlow
{
    /// <summary>获取或设置类目 id（官方 <c>cat_id</c>）。</summary>
    [JsonPropertyName("cat_id")]
    public long? CatId { get; set; }

    /// <summary>获取或设置审核单状态（官方 <c>status</c>，见 <see cref="ChannelsCategoryAuditStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置审核单 id（官方 <c>audit_id</c>）。</summary>
    [JsonPropertyName("audit_id")]
    public long? AuditId { get; set; }

    /// <summary>获取或设置资质 id（官方 <c>qua_id</c>）。</summary>
    [JsonPropertyName("qua_id")]
    public long? QuaId { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置审核材料（官方 <c>audit_info</c>）。</summary>
    [JsonPropertyName("audit_info")]
    public ChannelsCategoryAuditInfo? AuditInfo { get; set; }

    /// <summary>获取或设置审核时间（官方 <c>audit_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("audit_time")]
    public long? AuditTime { get; set; }

    /// <summary>获取或设置驳回原因（官方 <c>audit_reason</c>）。</summary>
    [JsonPropertyName("audit_reason")]
    public string? AuditReason { get; set; }
}

/// <summary>
/// 审核材料（官方 <c>audit_info</c> 对象——<c>getbizcatflowlist</c>/<c>getbizcatflowdetail</c>/，
/// <c>get_category_relation_detail</c> 三处字段集一致 ⇒ 共用）。
/// </summary>
/// <remarks>
/// <b>官方历史拼写照录</b>：本响应对象中经营平台字段拼作 <c>jingyingpingtai</c>（多一个 g），
/// 与「申请类目」请求体中的 <c>jingyingpintai</c> 拼写不一致，照抄官方原文、勿统一。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryAuditInfo
{
    /// <summary>获取或设置类目 id（官方 <c>level3</c>）。</summary>
    [JsonPropertyName("level3")]
    public long? Level3 { get; set; }

    /// <summary>
    /// 获取或设置资质资料（官方 <c>certificate</c>）——官方参数表仅标 array、未标注元素结构，
    /// SDK 按 file_id 字符串列表建模。
    /// </summary>
    [JsonPropertyName("certificate")]
    public List<string>? Certificate { get; set; }

    /// <summary>获取或设置品牌列表（官方 <c>brand_list</c>）。</summary>
    [JsonPropertyName("brand_list")]
    public List<ChannelsCategoryBrandRef>? BrandList { get; set; }

    /// <summary>获取或设置报备函（官方 <c>baobeihan</c>）。</summary>
    [JsonPropertyName("baobeihan")]
    public List<string>? Baobeihan { get; set; }

    /// <summary>获取或设置经营证明（官方 <c>jingyingzhengming</c>）。</summary>
    [JsonPropertyName("jingyingzhengming")]
    public List<string>? Jingyingzhengming { get; set; }

    /// <summary>获取或设置经营平台（官方 <c>jingyingpingtai</c>——官方响应侧历史拼写照录，与请求侧 <c>jingyingpintai</c> 差一个 g，勿统一）。</summary>
    [JsonPropertyName("jingyingpingtai")]
    public string? Jingyingpingtai { get; set; }

    /// <summary>获取或设置帐号名称（官方 <c>zhanghaomingcheng</c>）。</summary>
    [JsonPropertyName("zhanghaomingcheng")]
    public string? Zhanghaomingcheng { get; set; }

    /// <summary>获取或设置入驻证明（官方 <c>ruzhuzhizhi</c>）。</summary>
    [JsonPropertyName("ruzhuzhizhi")]
    public List<string>? Ruzhuzhizhi { get; set; }

    /// <summary>获取或设置带货口碑（官方 <c>daihuokoubei</c>）。</summary>
    [JsonPropertyName("daihuokoubei")]
    public List<string>? Daihuokoubei { get; set; }

    /// <summary>获取或设置经营流水（官方 <c>jingyingliushui</c>）。</summary>
    [JsonPropertyName("jingyingliushui")]
    public List<string>? Jingyingliushui { get; set; }

    /// <summary>获取或设置补充材料（官方 <c>buchongcailiao</c>）。</summary>
    [JsonPropertyName("buchongcailiao")]
    public List<string>? Buchongcailiao { get; set; }

    /// <summary>获取或设置证照信息（官方 <c>license_group_list</c>）。</summary>
    [JsonPropertyName("license_group_list")]
    public List<ChannelsCategoryLicenseGroup>? LicenseGroupList { get; set; }

    /// <summary>获取或设置承诺函（官方 <c>commitment_letter_list</c>）。</summary>
    [JsonPropertyName("commitment_letter_list")]
    public List<string>? CommitmentLetterList { get; set; }

    /// <summary>获取或设置账号链接（官方 <c>account_link</c>）。</summary>
    [JsonPropertyName("account_link")]
    public string? AccountLink { get; set; }

    /// <summary>获取或设置是否为新版证照组申请流程（官方 <c>is_new_apply_cat</c>）。</summary>
    [JsonPropertyName("is_new_apply_cat")]
    public bool? IsNewApplyCat { get; set; }
}

/// <summary>获取店铺的类目审核单详情（<c>getbizcatflowdetail</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsGetBizCatFlowDetailRequest
{
    /// <summary>获取或设置审核单 id（官方 <c>audit_id</c>，必填；本端点官方类型为 number，与「撤销类目审核」的 string 型不一致，照抄官方）。</summary>
    [JsonPropertyName("audit_id")]
    public long AuditId { get; set; }
}

/// <summary>获取店铺的类目审核单详情（<c>getbizcatflowdetail</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsGetBizCatFlowDetailResponse : ChannelsResponse
{
    /// <summary>获取或设置类目审核单详情（官方 <c>info</c>；与列表接口 <c>list</c> 元素字段集一致 ⇒ 共用 <see cref="ChannelsCategoryAuditFlow"/>）。</summary>
    [JsonPropertyName("info")]
    public ChannelsCategoryAuditFlow? Info { get; set; }
}

/// <summary>获取店铺的类目权限列表（<c>get_category_relation_list</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsGetCategoryRelationListRequest
{
    /// <summary>获取或设置是否过滤状态（官方 <c>is_filter_status</c>，必填）。</summary>
    [JsonPropertyName("is_filter_status")]
    public bool IsFilterStatus { get; set; }

    /// <summary>获取或设置过滤的状态（官方 <c>status</c>，选填；当 is_filter_status = true 时生效，见 <see cref="ChannelsCategoryRelationStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}

/// <summary>获取店铺的类目权限列表（<c>get_category_relation_list</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsGetCategoryRelationListResponse : ChannelsResponse
{
    /// <summary>获取或设置店铺类目权限列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsCategoryRelation>? List { get; set; }
}

/// <summary>店铺类目权限（官方 <c>list</c> 数组元素，<c>get_category_relation_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryRelation
{
    /// <summary>获取或设置类目 id（官方 <c>id</c>）。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>获取或设置类目状态（官方 <c>status</c>，见 <see cref="ChannelsCategoryRelationStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置失效原因（官方 <c>uneffective_reason</c>）。</summary>
    [JsonPropertyName("uneffective_reason")]
    public string? UneffectiveReason { get; set; }

    /// <summary>获取或设置生效时间（官方 <c>effective_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("effective_time")]
    public long? EffectiveTime { get; set; }

    /// <summary>获取或设置失效时间（官方 <c>uneffective_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("uneffective_time")]
    public long? UneffectiveTime { get; set; }

    /// <summary>获取或设置类目资质 id（官方 <c>qua_id</c>）。</summary>
    [JsonPropertyName("qua_id")]
    public long? QuaId { get; set; }
}

/// <summary>获取店铺的类目权限详情（<c>get_category_relation_detail</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsGetCategoryRelationDetailRequest
{
    /// <summary>获取或设置类目 ID（官方 <c>category_id</c>，必填）。</summary>
    [JsonPropertyName("category_id")]
    public long CategoryId { get; set; }
}

/// <summary>获取店铺的类目权限详情（<c>get_category_relation_detail</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsGetCategoryRelationDetailResponse : ChannelsResponse
{
    /// <summary>获取或设置类目 ID（官方 <c>category_id</c>）。</summary>
    [JsonPropertyName("category_id")]
    public long? CategoryId { get; set; }

    /// <summary>获取或设置类目状态（官方 <c>status</c>，见 <see cref="ChannelsCategoryRelationStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置失效原因（官方 <c>uneffective_reason</c>）。</summary>
    [JsonPropertyName("uneffective_reason")]
    public string? UneffectiveReason { get; set; }

    /// <summary>获取或设置审核材料（官方 <c>audit_info</c>；与审核单两接口字段集一致 ⇒ 共用 <see cref="ChannelsCategoryAuditInfo"/>）。</summary>
    [JsonPropertyName("audit_info")]
    public ChannelsCategoryAuditInfo? AuditInfo { get; set; }

    /// <summary>获取或设置生效时间（官方 <c>effective_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("effective_time")]
    public long? EffectiveTime { get; set; }

    /// <summary>获取或设置失效时间（官方 <c>uneffective_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("uneffective_time")]
    public long? UneffectiveTime { get; set; }

    /// <summary>获取或设置类目资质 ID（官方 <c>qua_id</c>）。</summary>
    [JsonPropertyName("qua_id")]
    public long? QuaId { get; set; }
}

/// <summary>撤销类目审核（<c>cancelauditcategory</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCancelCategoryAuditRequest
{
    /// <summary>
    /// 获取或设置审核单 id（官方 <c>audit_id</c>，必填；提交审核时返回）——
    /// 本端点官方类型为 string，与「获取审核单详情」的 number 型不一致，照抄官方。
    /// </summary>
    [JsonPropertyName("audit_id")]
    public string AuditId { get; set; } = string.Empty;
}

/// <summary>获取类目规则（<c>getcategoryrule</c>）请求体——官方双语义（保证金 / 发货方式）请求体形态一致 ⇒ 共用。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsGetCategoryRuleRequest
{
    /// <summary>获取或设置类目 id（官方 <c>category_id</c>，必填）。</summary>
    [JsonPropertyName("category_id")]
    public long CategoryId { get; set; }
}

/// <summary>
/// 获取类目规则（<c>getcategoryrule</c>）响应——官方双语义同路由（<c>rule_id=1</c> 保证金 / <c>rule_id=2</c> 发货方式）
/// 响应形态不同，以可空超集合并承载：按请求 <c>rule_id</c> 取对应字段。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsGetCategoryRuleResponse : ChannelsResponse
{
    /// <summary>获取或设置保证金（官方 <c>deposit</c>，单位：分；仅 <c>rule_id=1</c> 保证金语义返回）。</summary>
    [JsonPropertyName("deposit")]
    public long? Deposit { get; set; }

    /// <summary>获取或设置发货方式规则列表（官方 <c>list</c>；仅 <c>rule_id=2</c> 发货方式语义返回）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsCategoryDeliverRule>? List { get; set; }
}

/// <summary>发货方式类目规则（官方 <c>list</c> 数组元素，<c>getcategoryrule</c> 发货方式语义）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryDeliverRule
{
    /// <summary>获取或设置发货方式（官方 <c>deliver_type</c>，见 <see cref="ChannelsCategoryDeliverTypes"/>）。</summary>
    [JsonPropertyName("deliver_type")]
    public int? DeliverType { get; set; }

    /// <summary>获取或设置支持的账号类型列表（官方 <c>acct_type</c>，官方类型 numarray，见 <see cref="ChannelsCategoryAcctTypes"/>）。</summary>
    [JsonPropertyName("acct_type")]
    public List<int>? AcctType { get; set; }
}

/// <summary>获取类目下商品发布规则（<c>getcategoryproductrule</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsGetCategoryProductRuleRequest
{
    /// <summary>获取或设置类目 ID（官方 <c>cat_id</c>，必填）。</summary>
    [JsonPropertyName("cat_id")]
    public long CatId { get; set; }

    /// <summary>获取或设置发布模式（官方 <c>release_mode</c>，必填，见 <see cref="ChannelsCategoryReleaseModes"/>）。</summary>
    [JsonPropertyName("release_mode")]
    public int ReleaseMode { get; set; }

    /// <summary>获取或设置品牌 ID（官方 <c>brand_id</c>，选填；传入时返回 <c>brand_info</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public long? BrandId { get; set; }
}

/// <summary>获取类目下商品发布规则（<c>getcategoryproductrule</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsGetCategoryProductRuleResponse : ChannelsResponse
{
    /// <summary>获取或设置类目信息（官方 <c>info</c>：cat_id / name；与「获取类目信息」<c>info</c> 字段集一致 ⇒ 共用 <see cref="ChannelsCategoryInfo"/>）。</summary>
    [JsonPropertyName("info")]
    public ChannelsCategoryInfo? Info { get; set; }

    /// <summary>获取或设置产品属性（官方 <c>product_attr_list</c>）。</summary>
    [JsonPropertyName("product_attr_list")]
    public List<ChannelsCategoryPublishAttrItem>? ProductAttrList { get; set; }

    /// <summary>获取或设置销售属性（官方 <c>sale_attr_list</c>）。</summary>
    [JsonPropertyName("sale_attr_list")]
    public List<ChannelsCategoryPublishAttrItem>? SaleAttrList { get; set; }

    /// <summary>获取或设置资质信息（官方 <c>product_qua_list</c>）。</summary>
    [JsonPropertyName("product_qua_list")]
    public List<ChannelsCategoryPublishQua>? ProductQuaList { get; set; }

    /// <summary>获取或设置价格下限（官方 <c>floor_price</c>，单位：分）。</summary>
    [JsonPropertyName("floor_price")]
    public long? FloorPrice { get; set; }

    /// <summary>获取或设置尺码表（官方 <c>size_chart</c>；与「获取类目信息」字段集一致 ⇒ 共用 <see cref="ChannelsCategorySizeChart"/>）。</summary>
    [JsonPropertyName("size_chart")]
    public ChannelsCategorySizeChart? SizeChart { get; set; }

    /// <summary>获取或设置商品编辑要求（官方 <c>product_requirement</c>；与「获取类目信息」字段集一致 ⇒ 共用 <see cref="ChannelsCategoryProductRequirement"/>）。</summary>
    [JsonPropertyName("product_requirement")]
    public ChannelsCategoryProductRequirement? ProductRequirement { get; set; }

    /// <summary>获取或设置额外服务列表（官方 <c>extra_service_list</c>，对应添加商品 extra_service 参数）。</summary>
    [JsonPropertyName("extra_service_list")]
    public List<ChannelsCategoryExtraService>? ExtraServiceList { get; set; }

    /// <summary>获取或设置是否必填 sku 条形码（官方 <c>is_need_bar_code</c>）。</summary>
    [JsonPropertyName("is_need_bar_code")]
    public bool? IsNeedBarCode { get; set; }

    /// <summary>获取或设置预售规则（官方 <c>presale_rule</c>）。</summary>
    [JsonPropertyName("presale_rule")]
    public ChannelsCategoryPresaleRule? PresaleRule { get; set; }

    /// <summary>获取或设置品牌信息（官方 <c>brand_info</c>，传入 brand_id 时返回）。</summary>
    [JsonPropertyName("brand_info")]
    public ChannelsCategoryBrandInfo? BrandInfo { get; set; }

    /// <summary>获取或设置当前类目是否为定向类目（官方 <c>is_directed_category</c>）。</summary>
    [JsonPropertyName("is_directed_category")]
    public bool? IsDirectedCategory { get; set; }

    /// <summary>获取或设置类目可售品牌范围（官方 <c>restricted_brand_list</c>）。</summary>
    [JsonPropertyName("restricted_brand_list")]
    public List<ChannelsCategoryRestrictedBrand>? RestrictedBrandList { get; set; }
}

/// <summary>发品属性项（官方 <c>product_attr_list</c> / <c>sale_attr_list</c> 数组元素，<c>getcategoryproductrule</c> 形态——
/// 与 <c>getcategorydetail</c> 的属性形态不同（无 <c>type</c>/<c>is_required</c>、多 <c>required_rule</c>）⇒ 分建）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryPublishAttrItem
{
    /// <summary>获取或设置类目必填项名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置选项或单位列表（官方 <c>value</c>）——<c>type_v2</c> 为 <c>select_one</c>/<c>select_many</c> 时为选项列表，
    /// <c>integer_unit</c>/<c>decimal4_unit</c> 时为单位列表。
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    /// <summary>获取或设置输入提示（官方 <c>hint</c>）。</summary>
    [JsonPropertyName("hint")]
    public string? Hint { get; set; }

    /// <summary>获取或设置是否允许添加新选项（官方 <c>append_allowed</c>）。</summary>
    [JsonPropertyName("append_allowed")]
    public bool? AppendAllowed { get; set; }

    /// <summary>获取或设置属性类型 v2（官方 <c>type_v2</c>，7 种取值，见 <see cref="ChannelsCategoryAttrTypes"/>）。</summary>
    [JsonPropertyName("type_v2")]
    public string? TypeV2 { get; set; }

    /// <summary>获取或设置填写规则（官方 <c>required_rule</c>）。</summary>
    [JsonPropertyName("required_rule")]
    public ChannelsCategoryRequiredRule? RequiredRule { get; set; }
}

/// <summary>属性填写规则（官方 <c>required_rule</c> 对象，<c>getcategoryproductrule</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryRequiredRule
{
    /// <summary>获取或设置规则类型（官方 <c>rule_type</c>，见 <see cref="ChannelsCategoryRequiredRuleTypes"/>）。</summary>
    [JsonPropertyName("rule_type")]
    public int? RuleType { get; set; }

    /// <summary>获取或设置规则组合列表（官方 <c>or_combinators</c>，组合间关系固定为 or，仅 rule_type == 3 时生效）。</summary>
    [JsonPropertyName("or_combinators")]
    public List<ChannelsCategoryRuleOrCombinator>? OrCombinators { get; set; }
}

/// <summary>规则 or 组合（官方 <c>or_combinators</c> 数组元素，组合间关系固定为 or）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryRuleOrCombinator
{
    /// <summary>获取或设置 and 组合（官方 <c>and_combinators</c>，关系固定为 and）。</summary>
    [JsonPropertyName("and_combinators")]
    public ChannelsCategoryRuleAndCombinator? AndCombinators { get; set; }
}

/// <summary>规则 and 组合（官方 <c>and_combinators</c> 对象，关系固定为 and）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryRuleAndCombinator
{
    /// <summary>获取或设置组合类型（官方 <c>combine_type</c>）。</summary>
    [JsonPropertyName("combine_type")]
    public int? CombineType { get; set; }

    /// <summary>获取或设置检查参数列表（官方 <c>text_conditions</c>）。</summary>
    [JsonPropertyName("text_conditions")]
    public List<ChannelsCategoryRuleTextCondition>? TextConditions { get; set; }
}

/// <summary>规则文本检查条件（官方 <c>text_conditions</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryRuleTextCondition
{
    /// <summary>获取或设置检查的字段（官方 <c>check_field</c>，见 <see cref="ChannelsCategoryCheckFields"/>）。</summary>
    [JsonPropertyName("check_field")]
    public int? CheckField { get; set; }

    /// <summary>获取或设置操作类型（官方 <c>oper_type</c>，见 <see cref="ChannelsCategoryOperTypes"/>）。</summary>
    [JsonPropertyName("oper_type")]
    public int? OperType { get; set; }

    /// <summary>获取或设置关键字列表（官方 <c>keywords</c>）。</summary>
    [JsonPropertyName("keywords")]
    public List<string>? Keywords { get; set; }
}

/// <summary>发品资质项（官方 <c>product_qua_list</c> 数组元素，<c>getcategoryproductrule</c> 形态——
/// 与 <c>getallcategory</c>/<c>getcategorydetail</c> 的资质形态不同（本形态无 <c>qua_id</c>/<c>need_to_apply</c>/<c>mandatory</c>、
/// 以 <c>id</c> 为标识且多 <c>required_rule</c>）⇒ 分建）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryPublishQua
{
    /// <summary>获取或设置资质 ID（官方 <c>id</c>）。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>获取或设置资质描述信息（官方 <c>tips</c>）。</summary>
    [JsonPropertyName("tips")]
    public string? Tips { get; set; }

    /// <summary>获取或设置资质名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置填写规则（官方 <c>required_rule</c>）。</summary>
    [JsonPropertyName("required_rule")]
    public ChannelsCategoryRequiredRule? RequiredRule { get; set; }
}

/// <summary>额外服务项（官方 <c>extra_service_list</c> 数组元素，对应添加商品 extra_service 参数）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryExtraService
{
    /// <summary>获取或设置参数名（官方 <c>extra_service_name</c>）。</summary>
    [JsonPropertyName("extra_service_name")]
    public string? ExtraServiceName { get; set; }

    /// <summary>获取或设置参数类型（官方 <c>extra_service_type</c>：<c>select_one</c>（单选）或 <c>integer</c>）。</summary>
    [JsonPropertyName("extra_service_type")]
    public string? ExtraServiceType { get; set; }

    /// <summary>获取或设置可选值（官方 <c>extra_service_value</c>）。</summary>
    [JsonPropertyName("extra_service_value")]
    public string? ExtraServiceValue { get; set; }
}

/// <summary>预售规则（官方 <c>presale_rule</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryPresaleRule
{
    /// <summary>获取或设置是否支持预售（官方 <c>presale_switch</c>）。</summary>
    [JsonPropertyName("presale_switch")]
    public bool? PresaleSwitch { get; set; }

    /// <summary>获取或设置预售发货规则（官方 <c>delivery_after_presale</c>）。</summary>
    [JsonPropertyName("delivery_after_presale")]
    public ChannelsCategoryPresaleSpan? DeliveryAfterPresale { get; set; }

    /// <summary>获取或设置支付后发货规则（官方 <c>delivery_after_pay</c>）。</summary>
    [JsonPropertyName("delivery_after_pay")]
    public ChannelsCategoryPresaleDelay? DeliveryAfterPay { get; set; }

    /// <summary>获取或设置商品价格最低要求（官方 <c>product_price_threshold</c>）。</summary>
    [JsonPropertyName("product_price_threshold")]
    public string? ProductPriceThreshold { get; set; }
}

/// <summary>预售发货周期规则（官方 <c>delivery_after_presale</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryPresaleSpan
{
    /// <summary>获取或设置预售周期最大可设置天数（官方 <c>presale_cycle_span</c>）。</summary>
    [JsonPropertyName("presale_cycle_span")]
    public int? PresaleCycleSpan { get; set; }

    /// <summary>获取或设置最短发货延迟天数（官方 <c>min_delay_day</c>）。</summary>
    [JsonPropertyName("min_delay_day")]
    public int? MinDelayDay { get; set; }

    /// <summary>获取或设置最长发货延迟天数（官方 <c>max_delay_day</c>）。</summary>
    [JsonPropertyName("max_delay_day")]
    public int? MaxDelayDay { get; set; }
}

/// <summary>支付后发货规则（官方 <c>delivery_after_pay</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryPresaleDelay
{
    /// <summary>获取或设置最短发货延迟天数（官方 <c>min_delay_day</c>）。</summary>
    [JsonPropertyName("min_delay_day")]
    public int? MinDelayDay { get; set; }

    /// <summary>获取或设置最长发货延迟天数（官方 <c>max_delay_day</c>）。</summary>
    [JsonPropertyName("max_delay_day")]
    public int? MaxDelayDay { get; set; }
}

/// <summary>类目品牌信息（官方 <c>brand_info</c> 对象，传入 brand_id 时返回）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryBrandInfo
{
    /// <summary>获取或设置品牌 ID（官方 <c>brand_id</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public long? BrandId { get; set; }

    /// <summary>获取或设置品牌商标中文名（官方 <c>ch_name</c>）。</summary>
    [JsonPropertyName("ch_name")]
    public string? ChName { get; set; }

    /// <summary>获取或设置品牌商标英文名（官方 <c>en_name</c>）。</summary>
    [JsonPropertyName("en_name")]
    public string? EnName { get; set; }

    /// <summary>获取或设置定向类目时品牌是否在品牌池内（官方 <c>is_in_category_brand_pool</c>）。</summary>
    [JsonPropertyName("is_in_category_brand_pool")]
    public bool? IsInCategoryBrandPool { get; set; }

    /// <summary>获取或设置店铺是否具备该品牌资质（官方 <c>is_brand_authorized</c>）。</summary>
    [JsonPropertyName("is_brand_authorized")]
    public bool? IsBrandAuthorized { get; set; }
}

/// <summary>类目可售品牌（官方 <c>restricted_brand_list</c> 数组元素，仅品牌三元信息、与 <c>brand_info</c> 形态不同 ⇒ 分建）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryRestrictedBrand
{
    /// <summary>获取或设置品牌 ID（官方 <c>brand_id</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public long? BrandId { get; set; }

    /// <summary>获取或设置品牌商标中文名（官方 <c>ch_name</c>）。</summary>
    [JsonPropertyName("ch_name")]
    public string? ChName { get; set; }

    /// <summary>获取或设置品牌商标英文名（官方 <c>en_name</c>）。</summary>
    [JsonPropertyName("en_name")]
    public string? EnName { get; set; }
}

/// <summary>类目规则 id 常量（官方 <c>rule_id</c> Query 参数取值，<c>getcategoryrule</c> 双语义分流）。</summary>
public static class ChannelsCategoryRuleIds
{
    /// <summary>保证金类目规则。</summary>
    public const int Deposit = 1;

    /// <summary>发货方式类目规则。</summary>
    public const int DeliveryMethod = 2;
}

/// <summary>类目审核单状态常量（官方 <c>status</c> 枚举值，<c>getbizcatflowlist</c>/<c>getbizcatflowdetail</c>）。</summary>
public static class ChannelsCategoryAuditStatuses
{
    /// <summary>审核中。</summary>
    public const int Auditing = 1;

    /// <summary>审核驳回。</summary>
    public const int Rejected = 2;

    /// <summary>审核成功。</summary>
    public const int Approved = 3;

    /// <summary>取消审核单。</summary>
    public const int Canceled = 4;
}

/// <summary>店铺类目权限状态常量（官方 <c>status</c> 枚举值，<c>get_category_relation_list</c>/<c>get_category_relation_detail</c>）。</summary>
public static class ChannelsCategoryRelationStatuses
{
    /// <summary>生效中。</summary>
    public const int Effective = 1;

    /// <summary>已失效。</summary>
    public const int Uneffective = 2;
}

/// <summary>发货方式常量（官方 <c>deliver_type</c> 枚举值，<c>getcategoryrule</c> 发货方式语义）。</summary>
public static class ChannelsCategoryDeliverTypes
{
    /// <summary>快递。</summary>
    public const int Express = 0;

    /// <summary>教培类。</summary>
    public const int Education = 1;

    /// <summary>卡券类。</summary>
    public const int CardCoupon = 3;

    /// <summary>游戏角色。</summary>
    public const int GameRole = 4;

    /// <summary>游戏帐号。</summary>
    public const int GameAccount = 5;

    /// <summary>同城配送。</summary>
    public const int IntraCityDelivery = 6;
}

/// <summary>账号类型常量（官方 <c>acct_type</c> 枚举值，<c>getcategoryrule</c> 发货方式语义）。</summary>
public static class ChannelsCategoryAcctTypes
{
    /// <summary>微信号。</summary>
    public const int Wechat = 1;

    /// <summary>QQ。</summary>
    public const int QQ = 2;

    /// <summary>手机号。</summary>
    public const int Phone = 3;

    /// <summary>邮箱。</summary>
    public const int Email = 4;
}

/// <summary>发布模式常量（官方 <c>release_mode</c> 枚举值，<c>getcategoryproductrule</c>）。</summary>
public static class ChannelsCategoryReleaseModes
{
    /// <summary>普通模式。</summary>
    public const int Normal = 0;

    /// <summary>极简模式。</summary>
    public const int Simple = 1;
}

/// <summary>属性类型 v2 常量（官方 <c>type_v2</c> 枚举值，7 种）。</summary>
public static class ChannelsCategoryAttrTypes
{
    /// <summary>文本（自定义输入）。</summary>
    public const string String = "string";

    /// <summary>多选一。</summary>
    public const string SelectOne = "select_one";

    /// <summary>多选。</summary>
    public const string SelectMany = "select_many";

    /// <summary>整数。</summary>
    public const string Integer = "integer";

    /// <summary>4 位精度小数。</summary>
    public const string Decimal4 = "decimal4";

    /// <summary>整数 + 单位。</summary>
    public const string IntegerUnit = "integer_unit";

    /// <summary>4 位精度小数 + 单位。</summary>
    public const string Decimal4Unit = "decimal4_unit";
}

/// <summary>佣金激励类型常量（官方 <c>incentive_type</c> 枚举值，<c>transactionfee_info</c>）。</summary>
public static class ChannelsCategoryIncentiveTypes
{
    /// <summary>无激励。</summary>
    public const int None = 0;

    /// <summary>新店佣金减免。</summary>
    public const int NewShopCommissionReduction = 1;
}

/// <summary>属性填写规则类型常量（官方 <c>rule_type</c> 枚举值，<c>required_rule</c>）。</summary>
public static class ChannelsCategoryRequiredRuleTypes
{
    /// <summary>必填。</summary>
    public const int Required = 1;

    /// <summary>选填。</summary>
    public const int Optional = 2;

    /// <summary>条件必填。</summary>
    public const int ConditionalRequired = 3;
}

/// <summary>规则检查字段常量（官方 <c>check_field</c> 枚举值，<c>text_conditions</c>）。</summary>
public static class ChannelsCategoryCheckFields
{
    /// <summary>检查商品参数 key。</summary>
    public const int ProductParamKey = 1;

    /// <summary>检查商品参数 value。</summary>
    public const int ProductParamValue = 2;

    /// <summary>检查商品标题。</summary>
    public const int ProductTitle = 3;
}

/// <summary>规则操作类型常量（官方 <c>oper_type</c> 枚举值，<c>text_conditions</c>）。</summary>
public static class ChannelsCategoryOperTypes
{
    /// <summary>包含。</summary>
    public const int Contains = 1;

    /// <summary>不含。</summary>
    public const int NotContains = 2;

    /// <summary>正则。</summary>
    public const int Regex = 3;
}
