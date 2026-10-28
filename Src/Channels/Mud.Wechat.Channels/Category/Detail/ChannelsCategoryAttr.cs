// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

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
