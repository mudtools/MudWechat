// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Compass;

/// <summary>获取电商概览数据（<c>getfinderoverall</c>，罗盘达人版）响应。</summary>
/// <remarks>官方类型均为 number（与商家版 string 口径不同）；权限集 id：176；金额单位为<b>分</b>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassFinderOverallResponse : ChannelsResponse
{
    /// <summary>获取或设置电商概览数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsCompassFinderOverallData? Data { get; set; }
}

/// <summary>电商概览数据（官方 <c>data</c> 对象，<c>getfinderoverall</c>，官方类型 number）。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassFinderOverallData
{
    /// <summary>获取或设置成交金额（官方 <c>pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_gmv")]
    public long? PayGmv { get; set; }

    /// <summary>获取或设置直播成交金额（官方 <c>live_pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("live_pay_gmv")]
    public long? LivePayGmv { get; set; }

    /// <summary>获取或设置短视频成交金额（官方 <c>feed_pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("feed_pay_gmv")]
    public long? FeedPayGmv { get; set; }

    /// <summary>获取或设置橱窗成交金额（官方 <c>window_pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("window_pay_gmv")]
    public long? WindowPayGmv { get; set; }

    /// <summary>获取或设置商品分享支付金额（官方 <c>product_pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("product_pay_gmv")]
    public long? ProductPayGmv { get; set; }

    /// <summary>获取或设置其他渠道成交金额（官方 <c>other_pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("other_pay_gmv")]
    public long? OtherPayGmv { get; set; }
}

/// <summary>获取带货商品数据（<c>getfinderproductdata</c>，罗盘达人版）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassFinderProductDataResponse : ChannelsResponse
{
    /// <summary>获取或设置带货商品数据（官方 <c>product_info</c>）。</summary>
    [JsonPropertyName("product_info")]
    public ChannelsCompassFinderProductInfo? ProductInfo { get; set; }
}

/// <summary>
/// 罗盘达人版带货商品（官方 <c>product_info</c> 对象，<c>getfinderproductdata</c>；
/// 与 <c>getfinderproductlist</c> 的 <c>product_list</c> 元素字段集一致 ⇒ 共用）。
/// </summary>
/// <remarks>官方类型均为 number（与商家版商品档案 string 口径不同 ⇒ 不共用 DTO）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassFinderProductInfo
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品头图（官方 <c>head_img_url</c>）。</summary>
    [JsonPropertyName("head_img_url")]
    public string? HeadImgUrl { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置商品价格（官方 <c>price</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>获取或设置 1 级类目（官方 <c>first_category_id</c>）。</summary>
    [JsonPropertyName("first_category_id")]
    public long? FirstCategoryId { get; set; }

    /// <summary>获取或设置 2 级类目（官方 <c>second_category_id</c>）。</summary>
    [JsonPropertyName("second_category_id")]
    public long? SecondCategoryId { get; set; }

    /// <summary>获取或设置 3 级类目（官方 <c>third_category_id</c>）。</summary>
    [JsonPropertyName("third_category_id")]
    public long? ThirdCategoryId { get; set; }

    /// <summary>获取或设置商品明细数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsCompassFinderProductData? Data { get; set; }
}

/// <summary>罗盘达人版商品明细数据（官方 <c>data</c> 对象，官方类型均为 number）。</summary>
/// <remarks>金额字段单位为<b>分</b>；比率字段为转化率口径。</remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassFinderProductData
{
    /// <summary>获取或设置成交金额（官方 <c>pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_gmv")]
    public long? PayGmv { get; set; }

    /// <summary>获取或设置下单金额（官方 <c>create_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("create_gmv")]
    public long? CreateGmv { get; set; }

    /// <summary>获取或设置下单订单数（官方 <c>create_cnt</c>）。</summary>
    [JsonPropertyName("create_cnt")]
    public int? CreateCnt { get; set; }

    /// <summary>获取或设置下单人数（官方 <c>create_uv</c>）。</summary>
    [JsonPropertyName("create_uv")]
    public int? CreateUv { get; set; }

    /// <summary>获取或设置下单件数（官方 <c>create_product_cnt</c>）。</summary>
    [JsonPropertyName("create_product_cnt")]
    public int? CreateProductCnt { get; set; }

    /// <summary>获取或设置成交订单数（官方 <c>pay_cnt</c>）。</summary>
    [JsonPropertyName("pay_cnt")]
    public int? PayCnt { get; set; }

    /// <summary>获取或设置成交人数（官方 <c>pay_uv</c>）。</summary>
    [JsonPropertyName("pay_uv")]
    public int? PayUv { get; set; }

    /// <summary>获取或设置成交件数（官方 <c>pay_product_cnt</c>）。</summary>
    [JsonPropertyName("pay_product_cnt")]
    public int? PayProductCnt { get; set; }

    /// <summary>获取或设置成交金额（剔除退款）（官方 <c>pure_pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pure_pay_gmv")]
    public long? PurePayGmv { get; set; }

    /// <summary>获取或设置成交客单价（官方 <c>pay_gmv_per_uv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_gmv_per_uv")]
    public long? PayGmvPerUv { get; set; }

    /// <summary>获取或设置实际结算金额（官方 <c>actual_commission</c>，单位：分）。</summary>
    [JsonPropertyName("actual_commission")]
    public long? ActualCommission { get; set; }

    /// <summary>获取或设置预估佣金金额（官方 <c>predict_commission</c>，单位：分）。</summary>
    [JsonPropertyName("predict_commission")]
    public long? PredictCommission { get; set; }

    /// <summary>获取或设置商品点击人数（官方 <c>product_click_uv</c>）。</summary>
    [JsonPropertyName("product_click_uv")]
    public int? ProductClickUv { get; set; }

    /// <summary>获取或设置商品点击次数（官方 <c>product_click_cnt</c>）。</summary>
    [JsonPropertyName("product_click_cnt")]
    public int? ProductClickCnt { get; set; }

    /// <summary>获取或设置成交退款金额（官方 <c>pay_refund_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_refund_gmv")]
    public long? PayRefundGmv { get; set; }

    /// <summary>获取或设置成交退款人数（官方 <c>pay_refund_uv</c>）。</summary>
    [JsonPropertyName("pay_refund_uv")]
    public int? PayRefundUv { get; set; }

    /// <summary>获取或设置成交退款率（官方 <c>pay_refund_ratio</c>）。</summary>
    [JsonPropertyName("pay_refund_ratio")]
    public double? PayRefundRatio { get; set; }

    /// <summary>获取或设置发货后成交退款率（官方 <c>pay_refund_after_send_ratio</c>）。</summary>
    [JsonPropertyName("pay_refund_after_send_ratio")]
    public double? PayRefundAfterSendRatio { get; set; }

    /// <summary>获取或设置成交退款订单数（官方 <c>pay_refund_cnt</c>）。</summary>
    [JsonPropertyName("pay_refund_cnt")]
    public int? PayRefundCnt { get; set; }

    /// <summary>获取或设置成交退款件数（官方 <c>pay_refund_product_cnt</c>）。</summary>
    [JsonPropertyName("pay_refund_product_cnt")]
    public int? PayRefundProductCnt { get; set; }

    /// <summary>获取或设置发货前成交退款率（官方 <c>pay_refund_before_send_ratio</c>）。</summary>
    [JsonPropertyName("pay_refund_before_send_ratio")]
    public double? PayRefundBeforeSendRatio { get; set; }

    /// <summary>获取或设置退款金额（官方 <c>refund_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("refund_gmv")]
    public long? RefundGmv { get; set; }

    /// <summary>获取或设置退款件数（官方 <c>refund_product_cnt</c>）。</summary>
    [JsonPropertyName("refund_product_cnt")]
    public int? RefundProductCnt { get; set; }

    /// <summary>获取或设置退款订单数（官方 <c>refund_cnt</c>）。</summary>
    [JsonPropertyName("refund_cnt")]
    public int? RefundCnt { get; set; }

    /// <summary>获取或设置退款人数（官方 <c>refund_uv</c>）。</summary>
    [JsonPropertyName("refund_uv")]
    public int? RefundUv { get; set; }
}

/// <summary>获取带货商品列表（<c>getfinderproductlist</c>，罗盘达人版）响应。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：<c>product_list</c> 参数表标注为 objarray，官方返回示例为单对象——按参数表（列表）建模；
/// 元素与 <c>getfinderproductdata</c> 的 <c>product_info</c> 字段集一致 ⇒ 共用 <see cref="ChannelsCompassFinderProductInfo"/>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassFinderProductListResponse : ChannelsResponse
{
    /// <summary>获取或设置带货商品列表（官方 <c>product_list</c>）。</summary>
    [JsonPropertyName("product_list")]
    public List<ChannelsCompassFinderProductInfo>? ProductList { get; set; }
}

/// <summary>获取带货人群数据（<c>getfindersaleprofiledata</c>，罗盘达人版）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassFinderSaleProfileResponse : ChannelsResponse
{
    /// <summary>获取或设置带货人群数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsCompassFinderSaleProfile? Data { get; set; }
}

/// <summary>带货人群数据（官方 <c>data</c> 对象，<c>getfindersaleprofiledata</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassFinderSaleProfile
{
    /// <summary>获取或设置维度列表（官方 <c>field_list</c>）。</summary>
    [JsonPropertyName("field_list")]
    public List<ChannelsCompassFinderSaleProfileField>? FieldList { get; set; }
}

/// <summary>人群维度类别（官方 <c>field_list</c> 数组元素，<c>getfindersaleprofiledata</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassFinderSaleProfileField
{
    /// <summary>获取或设置维度类别名（官方 <c>field_name</c>）。</summary>
    [JsonPropertyName("field_name")]
    public string? FieldName { get; set; }

    /// <summary>获取或设置指标列表（官方 <c>data_list</c>）。</summary>
    [JsonPropertyName("data_list")]
    public List<ChannelsCompassFinderSaleProfileDim>? DataList { get; set; }
}

/// <summary>人群维度指标（官方 <c>data_list</c> 数组元素，<c>getfindersaleprofiledata</c>）。</summary>
/// <remarks><b>官方文档矛盾照录</b>：<c>dim_value</c> 参数表标注为 number、示例为字符串——按参数表（number）建模；与商家版（string）口径不同 ⇒ 不共用 DTO。</remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassFinderSaleProfileDim
{
    /// <summary>获取或设置维度指标名（官方 <c>dim_key</c>）。</summary>
    [JsonPropertyName("dim_key")]
    public string? DimKey { get; set; }

    /// <summary>获取或设置维度指标值（官方 <c>dim_value</c>，参数表类型 number）。</summary>
    [JsonPropertyName("dim_value")]
    public long? DimValue { get; set; }
}

/// <summary>罗盘达人版人群用户类型常量（官方 <c>type</c> 枚举值，<c>getfindersaleprofiledata</c>）。</summary>
public static class ChannelsCompassFinderSaleProfileUserTypes
{
    /// <summary>商品曝光用户。</summary>
    public const int ProductExposureUser = 1;

    /// <summary>商品点击用户。</summary>
    public const int ProductClickUser = 2;

    /// <summary>购买用户。</summary>
    public const int PurchaseUser = 3;

    /// <summary>首购用户。</summary>
    public const int FirstPurchaseUser = 4;

    /// <summary>复购用户。</summary>
    public const int RepeatPurchaseUser = 5;

    /// <summary>直播观看用户。</summary>
    public const int LiveWatchUser = 6;
}
