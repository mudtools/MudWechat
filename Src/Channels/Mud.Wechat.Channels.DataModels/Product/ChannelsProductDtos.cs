// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// 商品详情文本（官方 <c>desc_info</c> 对象：<c>imgs</c> + <c>desc</c>）。
/// </summary>
/// <remarks>
/// 官方契约见「添加商品 / 更新商品」的 <c>Body.desc_info Object Payload</c> 小节：
/// <c>imgs</c> 最少 1 张、最多 50 张（食品饮料和生鲜类目商品最少 3 张），不得有重复图片；
/// 图片 url 必须经「上传图片」接口（<c>resp_type=1</c>）返回（前缀 <c>mmecimage.cn/p/</c>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductDescInfo
{
    /// <summary>获取或设置商品详情图片列表（官方 <c>imgs</c>，必填）。</summary>
    [JsonPropertyName("imgs")]
    public List<string> Imgs { get; set; } = new();

    /// <summary>获取或设置商品详情文本（官方 <c>desc</c>，选填）。</summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; set; }
}

/// <summary>
/// 商品类目节点（官方 <c>cats</c> / <c>cats_v2</c> 数组元素：<c>cat_id</c>）。
/// </summary>
/// <remarks>
/// <para>旧类目树 <c>cats</c> 大小恒等于 3（一、二、三级类目）；新类目树 <c>cats_v2</c> 为多级类目，</para>
/// <para>两者均为「数组下标 0 = 一级类目」。商品上架后不可修改一级类目。</para>
/// <para>官方类型为 number，但官方代码示例同时出现数字（<c>6000</c>）与字符串（<c>"10000113"</c>）两种形态，</para>
/// <para>故建模为 string（反序列化时 number→string 由 System.Text.Json 原生支持）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductCategory
{
    /// <summary>获取或设置类目 ID（官方 <c>cat_id</c>，必填；经「获取类目接口」拿到可用的 cat_id）。</summary>
    [JsonPropertyName("cat_id")]
    public string CatId { get; set; } = string.Empty;
}

/// <summary>
/// 商品参数（官方 <c>attrs</c> 数组元素：<c>attr_key</c> + <c>attr_value</c>）。
/// </summary>
/// <remarks>
/// 部分类目有必填参数（参考「获取类目信息」的 <c>attr.product_attr_list[].is_required</c>）。
/// <c>attr_value</c> 按类目属性 type 有不同的格式约定（select_many 用 ; 分隔 / integer_unit
/// 为「数值 单位」/ integer 为字符串数字），SDK 不做二次加工，原样透传。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductAttr
{
    /// <summary>获取或设置属性键 key（官方 <c>attr_key</c>，属性自定义用）。</summary>
    [JsonPropertyName("attr_key")]
    public string AttrKey { get; set; } = string.Empty;

    /// <summary>获取或设置属性值 value（官方 <c>attr_value</c>，属性自定义用）。</summary>
    [JsonPropertyName("attr_value")]
    public string AttrValue { get; set; } = string.Empty;
}

/// <summary>
/// 商品资质（官方 <c>product_qua_infos</c> 数组元素：<c>qua_id</c> + <c>qua_url</c>）。
/// </summary>
/// <remarks>取代已废弃的 <c>qualifications</c> 字段；不同类目下必填的资质要求不同（参考「获取类目信息」的 <c>product_qua_list[]</c>）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductQuaInfo
{
    /// <summary>获取或设置商品资质 id（官方 <c>qua_id</c>，对应 <c>product_qua_list[].qua_id</c>）。</summary>
    [JsonPropertyName("qua_id")]
    public string? QuaId { get; set; }

    /// <summary>获取或设置商品资质图片列表（官方 <c>qua_url</c>，单个资质 id 下最多 10 张）。</summary>
    [JsonPropertyName("qua_url")]
    public List<string>? QuaUrl { get; set; }
}

/// <summary>
/// 运费信息（官方 <c>express_info</c> 对象：<c>template_id</c> / <c>weight</c> / <c>express_type</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductExpressInfo
{
    /// <summary>
    /// 获取或设置运费模板 ID（官方 <c>template_id</c>，先从「获取运费模板列表」拿到；
    /// <c>deliver_method</c> 为 1 或 3 时不用填写）。官方类型为 string，官方示例为数字 —— 建模为 string 兼容两种形态。
    /// </summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>获取或设置商品重量（官方 <c>weight</c>，单位：克；运费模版计价方式为「按重量」时必填）。</summary>
    [JsonPropertyName("weight")]
    public long? Weight { get; set; }

    /// <summary>获取或设置是否开启「偏远地区中转集运」（官方 <c>express_type</c>，见 <see cref="ChannelsProductExpressTypes"/>）。</summary>
    [JsonPropertyName("express_type")]
    public int? ExpressType { get; set; }
}

/// <summary>
/// 限购信息（官方 <c>limited_info</c> 对象：<c>period_type</c> + <c>limited_buy_num</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductLimitedInfo
{
    /// <summary>获取或设置限购周期类型（官方 <c>period_type</c>，见 <see cref="ChannelsProductLimitedPeriodTypes"/>）。</summary>
    [JsonPropertyName("period_type")]
    public int? PeriodType { get; set; }

    /// <summary>获取或设置限购数量（官方 <c>limited_buy_num</c>）。</summary>
    [JsonPropertyName("limited_buy_num")]
    public int? LimitedBuyNum { get; set; }
}

/// <summary>
/// 额外服务（官方 <c>extra_service</c> 对象：七天无理由 / 运费险 / 坏损包退 / 假一赔三 / 换货）。
/// </summary>
/// <remarks>
/// 官方说明：为提升商家发品成功率，若入参信息不符合平台规则，平台会将其优化至符合规则的参数值。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductExtraService
{
    /// <summary>获取或设置是否支持七天无理由退货（官方 <c>seven_day_return</c>，见 <see cref="ChannelsSevenDayReturnTypes"/>）。</summary>
    [JsonPropertyName("seven_day_return")]
    public int? SevenDayReturn { get; set; }

    /// <summary>获取或设置是否支持运费险（官方 <c>freight_insurance</c>：0 不支持，1 支持；需商户先开通运费险服务）。</summary>
    [JsonPropertyName("freight_insurance")]
    public int? FreightInsurance { get; set; }

    /// <summary>获取或设置是否支持坏损包退（官方 <c>damage_guarantee</c>：0 不支持，1 支持）。</summary>
    [JsonPropertyName("damage_guarantee")]
    public int? DamageGuarantee { get; set; }

    /// <summary>获取或设置是否支持假一赔三（官方 <c>fake_one_pay_three</c>：0 不支持，1 支持）。</summary>
    [JsonPropertyName("fake_one_pay_three")]
    public int? FakeOnePayThree { get; set; }

    /// <summary>获取或设置是否支持换货（官方 <c>exchange_support</c>：0 关闭支持换货，1 打开支持换货）。</summary>
    [JsonPropertyName("exchange_support")]
    public int? ExchangeSupport { get; set; }
}

/// <summary>
/// 售后 / 退货地址（官方 <c>after_sale_info</c> 对象：<c>after_sale_address_id</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductAfterSaleInfo
{
    /// <summary>
    /// 获取或设置售后 / 退货地址 id（官方 <c>after_sale_address_id</c>），
    /// 使用地址管理相关接口（物流发货域 <c>merchant/address/*</c>）进行添加或获取。
    /// </summary>
    [JsonPropertyName("after_sale_address_id")]
    public long? AfterSaleAddressId { get; set; }
}

/// <summary>
/// 商品头图视频（官方 <c>head_videos</c> 对象：<c>video_url</c>，最多 1 个）。
/// </summary>
/// <remarks>
/// 官方契约：不传递该字段不会删除商品原有视频；如需删除视频，请显式传入一个空的 <c>video_url</c> 字符串。
/// 官方变更日志：<c>video_url</c> 字段类型已从 array 更新为 string。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductHeadVideo
{
    /// <summary>获取或设置头图视频 url（官方 <c>video_url</c>；传空字符串表示删除视频）。</summary>
    [JsonPropertyName("video_url")]
    public string? VideoUrl { get; set; }
}

/// <summary>
/// 商品待开售信息（官方 <c>timing_onsale_info</c> 对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsTimingOnsaleInfo
{
    /// <summary>获取或设置状态（官方 <c>status</c>：0 没有待开售，1 待开售）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置开售时间（官方 <c>onsale_time</c>，秒级时间戳；0 为未配置时间）。</summary>
    [JsonPropertyName("onsale_time")]
    public long? OnsaleTime { get; set; }

    /// <summary>获取或设置是否隐藏价格（官方 <c>is_hide_price</c>：0 不隐藏，1 隐藏）。</summary>
    [JsonPropertyName("is_hide_price")]
    public int? IsHidePrice { get; set; }
}

/// <summary>
/// SKU 规格（官方 <c>sku_attrs</c> 数组元素：<c>attr_key</c> + <c>attr_value</c>）。
/// </summary>
/// <remarks>字段名与商品参数 <see cref="ChannelsProductAttr"/> 一致（官方同构），但语义为「销售规格」。</remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsSkuAttr
{
    /// <summary>获取或设置规格 key（官方 <c>attr_key</c>，最多 40 字符）。</summary>
    [JsonPropertyName("attr_key")]
    public string AttrKey { get; set; } = string.Empty;

    /// <summary>获取或设置规格 value（官方 <c>attr_value</c>，最多 40 字符）。</summary>
    [JsonPropertyName("attr_value")]
    public string AttrValue { get; set; } = string.Empty;
}

/// <summary>
/// SKU 预售信息（官方 <c>sku_deliver_info</c> 对象）。
/// </summary>
/// <remarks>
/// <c>stock_type = 1</c>（全款预售）时其余字段生效；预售时间区间 ≤ 15 天、
/// 预售结束时间距当前 ≤ 30 天（官方限制，见「更新商品免审」文档）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsSkuDeliverInfo
{
    /// <summary>获取或设置 sku 库存情况（官方 <c>stock_type</c>：0 现货（默认），1 全款预售；部分类目支持，参考类目信息 <c>attr.pre_sale</c>）。</summary>
    [JsonPropertyName("stock_type")]
    public int? StockType { get; set; }

    /// <summary>
    /// 获取或设置 sku 发货节点（官方 <c>full_payment_presale_delivery_type</c>，仅对 <c>stock_type=1</c> 有效）：
    /// 0 付款后 n 天发货，1 预售结束后 n 天发货。
    /// </summary>
    [JsonPropertyName("full_payment_presale_delivery_type")]
    public int? FullPaymentPresaleDeliveryType { get; set; }

    /// <summary>获取或设置预售周期开始时间（官方 <c>presale_begin_time</c>，秒级时间戳；仅对 <c>delivery_type=1</c> 有效）。</summary>
    [JsonPropertyName("presale_begin_time")]
    public long? PresaleBeginTime { get; set; }

    /// <summary>获取或设置预售周期结束时间（官方 <c>presale_end_time</c>，秒级时间戳；距现在 ≤ 30 天，预售区间 ≤ 15 天）。</summary>
    [JsonPropertyName("presale_end_time")]
    public long? PresaleEndTime { get; set; }

    /// <summary>
    /// 获取或设置 sku 发货时效（官方 <c>full_payment_presale_delivery_time</c>，仅对 <c>stock_type=1</c> 有效）：
    /// 发货节点 0 时范围 [4, 15]；发货节点 1 时范围 [1, 3]。
    /// </summary>
    [JsonPropertyName("full_payment_presale_delivery_time")]
    public int? FullPaymentPresaleDeliveryTime { get; set; }

    /// <summary>获取或设置是否在预售结束后自动转为现货（官方 <c>spot_after_presale_end</c>：0 否，1 是；仅对 <c>delivery_type=1</c> 有效）。</summary>
    [JsonPropertyName("spot_after_presale_end")]
    public int? SpotAfterPresaleEnd { get; set; }
}

/// <summary>
/// SPU 维度预售规则（官方 <c>spu_deliver_info</c> 对象：spu 维度配置全部 sku 预售规则）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsSpuDeliverInfo
{
    /// <summary>获取或设置是否生效（官方 <c>is_spu_range</c>：0 不生效，1 生效）。</summary>
    [JsonPropertyName("is_spu_range")]
    public int? IsSpuRange { get; set; }

    /// <summary>获取或设置 spu 维度下的 sku 预售配置（官方 <c>sku_deliver_info</c>）。</summary>
    [JsonPropertyName("sku_deliver_info")]
    public ChannelsSkuDeliverInfo? SkuDeliverInfo { get; set; }
}

/// <summary>
/// 商品 SKU（官方 <c>skus</c> 数组元素，添加商品形态：<c>out_sku_id</c> / <c>thumb_img</c> / <c>sale_price</c> / <c>stock_num</c> / …）。
/// </summary>
/// <remarks>SKU 长度最少 1、最多 500；价格单位：分，不超过 1000000000（1000 万元）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductSku
{
    /// <summary>获取或设置商家自定义 sku_id（官方 <c>out_sku_id</c>，最多 128 字符；添加成功后无法修改）。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>获取或设置 sku 小图（官方 <c>thumb_img</c>）。</summary>
    [JsonPropertyName("thumb_img")]
    public string? ThumbImg { get; set; }

    /// <summary>获取或设置售卖价格（官方 <c>sale_price</c>，单位：分，不超过 1000 万元）。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>获取或设置库存（官方 <c>stock_num</c>）。</summary>
    [JsonPropertyName("stock_num")]
    public int StockNum { get; set; }

    /// <summary>获取或设置 sku 编码（官方 <c>sku_code</c>，商家自定义编码，最多 100 字符）。</summary>
    [JsonPropertyName("sku_code")]
    public string? SkuCode { get; set; }

    /// <summary>获取或设置 sku 条形码（官方 <c>bar_code</c>）。</summary>
    [JsonPropertyName("bar_code")]
    public string? BarCode { get; set; }

    /// <summary>获取或设置规格列表（官方 <c>sku_attrs</c>）。</summary>
    [JsonPropertyName("sku_attrs")]
    public List<ChannelsSkuAttr>? SkuAttrs { get; set; }

    /// <summary>获取或设置 sku 预售信息（官方 <c>sku_deliver_info</c>）。</summary>
    [JsonPropertyName("sku_deliver_info")]
    public ChannelsSkuDeliverInfo? SkuDeliverInfo { get; set; }
}

/// <summary>
/// 库存信息（官方 <c>stock_info</c> 对象：<c>diff_type</c> + <c>num</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsStockInfo
{
    /// <summary>获取或设置修改类型（官方 <c>diff_type</c>：1 增加，2 减少，3 设置；建议使用 1 或 2）。</summary>
    [JsonPropertyName("diff_type")]
    public int? DiffType { get; set; }

    /// <summary>获取或设置增加、减少或者设置的库存值（官方 <c>num</c>）。</summary>
    [JsonPropertyName("num")]
    public long? Num { get; set; }
}

/// <summary>
/// SKU 更新条目（官方 <c>skus</c> 数组元素，更新商品免审形态：带 <c>sku_id</c> 与 <c>is_delete</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductSkuUpdate
{
    /// <summary>获取或设置 sku_id（官方 <c>sku_id</c>，更新时必填）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>获取或设置商品 sku 编码（官方 <c>sku_code</c>）。</summary>
    [JsonPropertyName("sku_code")]
    public string? SkuCode { get; set; }

    /// <summary>获取或设置库存信息（官方 <c>stock_info</c>；供货商铺货商品（<c>supply_source == 1</c>）不支持修改库存）。</summary>
    [JsonPropertyName("stock_info")]
    public ChannelsStockInfo? StockInfo { get; set; }

    /// <summary>获取或设置售卖价格（官方 <c>sale_price</c>，单位：分，不超过 1000 万元）。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>获取或设置 sku 预售信息（官方 <c>sku_deliver_info</c>）。</summary>
    [JsonPropertyName("sku_deliver_info")]
    public ChannelsSkuDeliverInfo? SkuDeliverInfo { get; set; }

    /// <summary>获取或设置是否删除当前 sku（官方 <c>is_delete</c>）。</summary>
    [JsonPropertyName("is_delete")]
    public int? IsDelete { get; set; }

    /// <summary>获取或设置更新 sku 状态（官方 <c>status</c>：0 默认值，5 上架，11 下架）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}

/// <summary>
/// 添加商品（<c>product/add</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<b>POST</b> + 请求体；草稿 / 线上双份数据语义——添加后只影响草稿，
/// 需调上架接口并审核通过后草稿才覆盖线上数据正式生效；sku 数量超过 25 个时接口会异步更新。
/// </para>
/// <para>
/// 图片相关参数（head_img / desc_info.imgs / product_qua_infos[].qua_url[] / skus[].thumb_img 等）
/// 请务必使用「上传图片」接口（resp_type=1）并回填返回的 img_url（前缀 mmecimage.cn/p/），
/// 不接受其他任何格式的图片 url。
/// </para>
/// <para>
/// <c>cats_v2</c> 新多级类目树与 <c>cats</c> 旧三级类目树兼容：设置了 <c>cats_v2</c> 优先读取 <c>cats_v2</c>。
/// </para>
/// <para>
/// 未建模的官方可选对象：<c>size_chart</c>（尺码表）、<c>supply_source</c>（供货货品来源）——
/// 官方参数表未列出其子字段，SDK 不发明字段；如需使用请按官方全量字段自定义提交。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsAddProductRequest
{
    /// <summary>获取或设置商家自定义商品 ID（官方 <c>out_product_id</c>，最多 128 字符；添加成功后无法修改）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>
    /// 获取或设置标题（官方 <c>title</c>，必填：至少 1 个有效字符、最多 60 字符；
    /// 不得仅为数字或英文，允许的特殊字符集见官方文档）。
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>获取或设置副标题（官方 <c>sub_title</c>，最多 18 字符；该字段已废弃）。</summary>
    [JsonPropertyName("sub_title")]
    public string? SubTitle { get; set; }

    /// <summary>获取或设置商品短标题（官方 <c>short_title</c>，最多 20 字符；用于礼物单等场景，缺省显示 title）。</summary>
    [JsonPropertyName("short_title")]
    public string? ShortTitle { get; set; }

    /// <summary>
    /// 获取或设置主图列表（官方 <c>head_imgs</c>，必填：最少 3 张（食品饮料和生鲜类目最少 4 张）、最多 9 张，不得重复）。
    /// </summary>
    [JsonPropertyName("head_imgs")]
    public List<string> HeadImgs { get; set; } = new();

    /// <summary>获取或设置发货方式（官方 <c>deliver_method</c>，必填：0 快递发货，1 无需快递-手机号发货，3 无需快递-可选发货账号类型）。</summary>
    [JsonPropertyName("deliver_method")]
    public int DeliverMethod { get; set; }

    /// <summary>获取或设置发货账号类型（官方 <c>deliver_acct_type</c>：1 微信 openid，2 QQ 号，3 手机号，4 邮箱；仅 <c>deliver_method=3</c> 时有意义）。</summary>
    [JsonPropertyName("deliver_acct_type")]
    public List<int>? DeliverAcctType { get; set; }

    /// <summary>获取或设置商品详情（官方 <c>desc_info</c>：<c>imgs</c> + <c>desc</c>）。</summary>
    [JsonPropertyName("desc_info")]
    public ChannelsProductDescInfo? DescInfo { get; set; }

    /// <summary>获取或设置商品类目（旧类目树，官方 <c>cats</c>，大小恒等于 3；商品上架后不可修改一级类目）。</summary>
    [JsonPropertyName("cats")]
    public List<ChannelsProductCategory>? Cats { get; set; }

    /// <summary>获取或设置商品类目（新类目树，官方 <c>cats_v2</c>；存在时优先于 <c>cats</c>）。</summary>
    [JsonPropertyName("cats_v2")]
    public List<ChannelsProductCategory>? CatsV2 { get; set; }

    /// <summary>获取或设置商品参数（官方 <c>attrs</c>；部分类目有必填参数）。</summary>
    [JsonPropertyName("attrs")]
    public List<ChannelsProductAttr>? Attrs { get; set; }

    /// <summary>获取或设置商家编码（官方 <c>spu_code</c>）。</summary>
    [JsonPropertyName("spu_code")]
    public string? SpuCode { get; set; }

    /// <summary>获取或设置品牌 id（官方 <c>brand_id</c>，无品牌为 <c>"2100000000"</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>获取或设置商品资质列表（官方 <c>product_qua_infos</c>，取代已废弃的 <c>qualifications</c>）。</summary>
    [JsonPropertyName("product_qua_infos")]
    public List<ChannelsProductQuaInfo>? ProductQuaInfos { get; set; }

    /// <summary>获取或设置运费信息（官方 <c>express_info</c>）。</summary>
    [JsonPropertyName("express_info")]
    public ChannelsProductExpressInfo? ExpressInfo { get; set; }

    /// <summary>获取或设置售后说明（官方 <c>aftersale_desc</c>，最多 200 UTF 字符）。</summary>
    [JsonPropertyName("aftersale_desc")]
    public string? AftersaleDesc { get; set; }

    /// <summary>获取或设置限购信息（官方 <c>limited_info</c>）。</summary>
    [JsonPropertyName("limited_info")]
    public ChannelsProductLimitedInfo? LimitedInfo { get; set; }

    /// <summary>获取或设置额外服务（官方 <c>extra_service</c>，必填；不符合平台规则会被平台优化至合规值）。</summary>
    [JsonPropertyName("extra_service")]
    public ChannelsProductExtraService? ExtraService { get; set; }

    /// <summary>获取或设置商品 SKU 列表（官方 <c>skus</c>，必填：长度最少 1、最大 500）。</summary>
    [JsonPropertyName("skus")]
    public List<ChannelsProductSku> Skus { get; set; } = new();

    /// <summary>获取或设置添加完成后是否立即上架（官方 <c>listing</c>：1 是，0 否，默认 0）。</summary>
    [JsonPropertyName("listing")]
    public int? Listing { get; set; }

    /// <summary>获取或设置售后 / 退货地址（官方 <c>after_sale_info</c>）。</summary>
    [JsonPropertyName("after_sale_info")]
    public ChannelsProductAfterSaleInfo? AfterSaleInfo { get; set; }

    /// <summary>获取或设置是否在店铺首页隐藏（官方 <c>hide_in_window</c>：0 不隐藏，1 隐藏）。</summary>
    [JsonPropertyName("hide_in_window")]
    public int? HideInWindow { get; set; }

    /// <summary>获取或设置发布模式（官方 <c>release_mode</c>：0 普通模式，1 极简模式）。</summary>
    [JsonPropertyName("release_mode")]
    public int? ReleaseMode { get; set; }

    /// <summary>获取或设置商品待开售信息（官方 <c>timing_onsale_info</c>）。</summary>
    [JsonPropertyName("timing_onsale_info")]
    public ChannelsTimingOnsaleInfo? TimingOnsaleInfo { get; set; }

    /// <summary>获取或设置商品头图视频（官方 <c>head_videos</c>，最多 1 个）。</summary>
    [JsonPropertyName("head_videos")]
    public ChannelsProductHeadVideo? HeadVideos { get; set; }

    /// <summary>获取或设置 spu 维度配置全部 sku 预售规则（官方 <c>spu_deliver_info</c>）。</summary>
    [JsonPropertyName("spu_deliver_info")]
    public ChannelsSpuDeliverInfo? SpuDeliverInfo { get; set; }

    /// <summary>获取或设置 3:4 主图列表（官方 <c>vertical_head_imgs</c>，最多 1 张，尺寸比例 3:4）。</summary>
    [JsonPropertyName("vertical_head_imgs")]
    public List<string>? VerticalHeadImgs { get; set; }

    /// <summary>获取或设置白底图列表（官方 <c>white_bg_imgs</c>，最多 1 张，尺寸比例 1:1）。</summary>
    [JsonPropertyName("white_bg_imgs")]
    public List<string>? WhiteBgImgs { get; set; }
}

/// <summary>
/// 添加商品（<c>product/add</c>）响应数据（官方 <c>data</c> 对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsAddProductData
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，格式 <c>YYYY-MM-DD hh:mm:ss</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }
}

/// <summary>添加商品（<c>product/add</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsAddProductResponse : ChannelsResponse
{
    /// <summary>获取或设置商品信息（官方 <c>data</c>：<c>product_id</c> + <c>create_time</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsAddProductData? Data { get; set; }
}

/// <summary>
/// 更新商品（<c>product/update</c>）请求体 —— 字段集与 <see cref="ChannelsAddProductRequest"/> 一致，额外携带 <c>product_id</c>。
/// </summary>
/// <remarks>官方契约：<b>POST</b> + 请求体；更新只影响草稿数据，需上架并审核通过后生效。</remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsUpdateProductRequest : ChannelsAddProductRequest
{
    /// <summary>获取或设置小店内部商品 ID（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }
}

/// <summary>更新商品（<c>product/update</c>）响应数据（官方 <c>data</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsUpdateProductData
{
    /// <summary>获取或设置小店内部商品 ID（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，格式 <c>YYYY-MM-DD hh:mm:ss</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }
}

/// <summary>更新商品（<c>product/update</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsUpdateProductResponse : ChannelsResponse
{
    /// <summary>获取或设置商品信息（官方 <c>data</c>：<c>product_id</c> + <c>update_time</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsUpdateProductData? Data { get; set; }
}

/// <summary>
/// 获取商品（<c>product/get</c>）请求体。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetProductRequest
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置数据类型（官方 <c>data_type</c>，默认 1）：1 线上数据，2 草稿数据，3 同时获取线上和草稿数据
    /// （注意：上架过的商品才有线上数据）。
    /// </summary>
    [JsonPropertyName("data_type")]
    public int? DataType { get; set; }
}

/// <summary>
/// 商品详情（官方 <c>product</c> / <c>edit_product</c> 对象，<c>product/get</c> 返回形态）。
/// </summary>
/// <remarks>线上数据（product）与草稿数据（edit_product）字段结构一致；字段来源：官方「获取商品」返回参数表。</remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductDetail
{
    /// <summary>获取或设置小店内部商品 ID（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置外部平台自定义商品 ID（官方 <c>out_product_id</c>；添加时没录入则回包可能不包含）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>获取或设置标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置副标题（官方 <c>sub_title</c>，已废弃）。</summary>
    [JsonPropertyName("sub_title")]
    public string? SubTitle { get; set; }

    /// <summary>获取或设置主图列表（官方 <c>head_imgs</c>，最多 9 张，每张不超过 2MB）。</summary>
    [JsonPropertyName("head_imgs")]
    public List<string>? HeadImgs { get; set; }

    /// <summary>获取或设置商品详情（官方 <c>desc_info</c>）。</summary>
    [JsonPropertyName("desc_info")]
    public ChannelsProductDescInfo? DescInfo { get; set; }

    /// <summary>获取或设置发货方式（官方 <c>deliver_method</c>）。</summary>
    [JsonPropertyName("deliver_method")]
    public int? DeliverMethod { get; set; }

    /// <summary>获取或设置发货账号类型（官方 <c>deliver_acct_type</c>）。</summary>
    [JsonPropertyName("deliver_acct_type")]
    public List<int>? DeliverAcctType { get; set; }

    /// <summary>获取或设置运费信息（官方 <c>express_info</c>）。</summary>
    [JsonPropertyName("express_info")]
    public ChannelsProductExpressInfo? ExpressInfo { get; set; }

    /// <summary>获取或设置售后说明（官方 <c>aftersale_desc</c>）。</summary>
    [JsonPropertyName("aftersale_desc")]
    public string? AftersaleDesc { get; set; }

    /// <summary>获取或设置限购信息（官方 <c>limited_info</c>）。</summary>
    [JsonPropertyName("limited_info")]
    public ChannelsProductLimitedInfo? LimitedInfo { get; set; }

    /// <summary>获取或设置额外服务（官方 <c>extra_service</c>）。</summary>
    [JsonPropertyName("extra_service")]
    public ChannelsProductExtraService? ExtraService { get; set; }

    /// <summary>获取或设置商品线上状态（官方 <c>status</c>，见 <see cref="ChannelsProductStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置商品草稿状态（官方 <c>edit_status</c>，以 edit_product 返回值为准）。</summary>
    [JsonPropertyName("edit_status")]
    public int? EditStatus { get; set; }

    /// <summary>获取或设置商品 SKU 最小价格（官方 <c>min_price</c>，单位：分）。</summary>
    [JsonPropertyName("min_price")]
    public long? MinPrice { get; set; }

    /// <summary>获取或设置商品类目（旧类目树，官方 <c>cats</c>）。</summary>
    [JsonPropertyName("cats")]
    public List<ChannelsProductCategory>? Cats { get; set; }

    /// <summary>获取或设置商品类目（新类目树，官方 <c>cats_v2</c>）。</summary>
    [JsonPropertyName("cats_v2")]
    public List<ChannelsProductCategory>? CatsV2 { get; set; }

    /// <summary>获取或设置商品参数（官方 <c>attrs</c>）。</summary>
    [JsonPropertyName("attrs")]
    public List<ChannelsProductAttr>? Attrs { get; set; }

    /// <summary>获取或设置商家自定义的商品编码（官方 <c>spu_code</c>）。</summary>
    [JsonPropertyName("spu_code")]
    public string? SpuCode { get; set; }

    /// <summary>获取或设置品牌 id（官方 <c>brand_id</c>，无品牌为 <c>"2100000000"</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>获取或设置 sku 信息（官方 <c>skus</c>）。</summary>
    [JsonPropertyName("skus")]
    public List<ChannelsProductSkuDetail>? Skus { get; set; }

    /// <summary>获取或设置商品类型（官方 <c>product_type</c>，见 <see cref="ChannelsProductTypes"/>）。</summary>
    [JsonPropertyName("product_type")]
    public int? ProductType { get; set; }

    /// <summary>获取或设置商品草稿最近一次修改时间（官方 <c>edit_time</c>，指定获取草稿数据时才返回）。</summary>
    [JsonPropertyName("edit_time")]
    public long? EditTime { get; set; }

    /// <summary>获取或设置商品售后信息（官方 <c>after_sale_info</c>）。</summary>
    [JsonPropertyName("after_sale_info")]
    public ChannelsProductAfterSaleInfo? AfterSaleInfo { get; set; }

    /// <summary>获取或设置来源商品 id（官方 <c>src_product_id</c>；商品类型为福袋抽奖且由橱窗自营商品导入时才有值）。</summary>
    [JsonPropertyName("src_product_id")]
    public long? SrcProductId { get; set; }

    /// <summary>获取或设置商品资质列表（官方 <c>product_qua_infos</c>）。</summary>
    [JsonPropertyName("product_qua_infos")]
    public List<ChannelsProductQuaInfo>? ProductQuaInfos { get; set; }

    /// <summary>获取或设置是否在店铺首页隐藏（官方 <c>hide_in_window</c>：0 不隐藏，1 隐藏）。</summary>
    [JsonPropertyName("hide_in_window")]
    public int? HideInWindow { get; set; }

    /// <summary>获取或设置商品待开售信息（官方 <c>timing_onsale_info</c>）。</summary>
    [JsonPropertyName("timing_onsale_info")]
    public ChannelsTimingOnsaleInfo? TimingOnsaleInfo { get; set; }

    /// <summary>获取或设置短标题（官方 <c>short_title</c>）。</summary>
    [JsonPropertyName("short_title")]
    public string? ShortTitle { get; set; }

    /// <summary>获取或设置销量（官方 <c>total_sold_num</c>）。</summary>
    [JsonPropertyName("total_sold_num")]
    public long? TotalSoldNum { get; set; }

    /// <summary>获取或设置发布模式（官方 <c>release_mode</c>：0 普通模式，1 极简模式）。</summary>
    [JsonPropertyName("release_mode")]
    public int? ReleaseMode { get; set; }

    /// <summary>获取或设置商品头图视频（官方 <c>head_videos</c>；没有上传则不返回）。</summary>
    [JsonPropertyName("head_videos")]
    public ChannelsProductHeadVideo? HeadVideos { get; set; }

    /// <summary>获取或设置 spu 维度 sku 预售配置（官方 <c>spu_deliver_info</c>）。</summary>
    [JsonPropertyName("spu_deliver_info")]
    public ChannelsSpuDeliverInfo? SpuDeliverInfo { get; set; }

    /// <summary>获取或设置商品子状态（官方 <c>sub_status</c>）。</summary>
    [JsonPropertyName("sub_status")]
    public int? SubStatus { get; set; }

    /// <summary>获取或设置 3:4 主图列表（官方 <c>vertical_head_imgs</c>，最多 1 张）。</summary>
    [JsonPropertyName("vertical_head_imgs")]
    public List<string>? VerticalHeadImgs { get; set; }

    /// <summary>获取或设置白底图列表（官方 <c>white_bg_imgs</c>，最多 1 张）。</summary>
    [JsonPropertyName("white_bg_imgs")]
    public List<string>? WhiteBgImgs { get; set; }
}

/// <summary>
/// 商品 SKU 详情（官方 <c>skus</c> 数组元素，<c>product/get</c> 返回形态）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductSkuDetail
{
    /// <summary>获取或设置 sku_id（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置外部平台自定义 sku_id（官方 <c>out_sku_id</c>）。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>获取或设置售卖价格（官方 <c>sale_price</c>，单位：分）。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>获取或设置 sku 库存（官方 <c>stock_num</c>）。</summary>
    [JsonPropertyName("stock_num")]
    public long? StockNum { get; set; }

    /// <summary>获取或设置 sku 编码（官方 <c>sku_code</c>）。</summary>
    [JsonPropertyName("sku_code")]
    public string? SkuCode { get; set; }

    /// <summary>获取或设置 sku 状态（官方 <c>status</c>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置规格（官方 <c>sku_attrs</c>）。</summary>
    [JsonPropertyName("sku_attrs")]
    public List<ChannelsSkuAttr>? SkuAttrs { get; set; }
}

/// <summary>
/// 当日售卖上限提醒（官方 <c>sale_limit_info</c> 对象；店铺受到售卖管控时返回，无本字段表示无额外限制）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsSaleLimitInfo
{
    /// <summary>获取或设置是否受到管控（官方 <c>is_limited</c>，存在售卖限制时固定返回 1）。</summary>
    [JsonPropertyName("is_limited")]
    public int? IsLimited { get; set; }

    /// <summary>获取或设置售卖限制标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置售卖限制描述（官方 <c>sub_title</c>）。</summary>
    [JsonPropertyName("sub_title")]
    public string? SubTitle { get; set; }
}

/// <summary>
/// 商品信息质量（官方 <c>info_score</c> 对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductInfoScore
{
    /// <summary>获取或设置商品信息质量总分等级（官方 <c>score_level</c>）。</summary>
    [JsonPropertyName("score_level")]
    public int? ScoreLevel { get; set; }
}

/// <summary>
/// 商品高价预警（官方 <c>cmp_price_info</c> 对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductCmpPriceInfo
{
    /// <summary>获取或设置检测状态（官方 <c>status</c>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}

/// <summary>
/// 审核信息（官方 <c>audit_info</c> 对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductAuditInfo
{
    /// <summary>获取或设置提审携带的策略开关列表（官方 <c>user_strategy_flag_list</c>，numarray）。</summary>
    [JsonPropertyName("user_strategy_flag_list")]
    public List<int>? UserStrategyFlagList { get; set; }
}

/// <summary>获取商品（<c>product/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetProductResponse : ChannelsResponse
{
    /// <summary>获取或设置商品线上数据（官方 <c>product</c>；<c>data_type==2</c> 时不返回）。</summary>
    [JsonPropertyName("product")]
    public ChannelsProductDetail? Product { get; set; }

    /// <summary>获取或设置商品草稿数据（官方 <c>edit_product</c>；<c>data_type==1</c> 时不返回）。</summary>
    [JsonPropertyName("edit_product")]
    public ChannelsProductDetail? EditProduct { get; set; }

    /// <summary>获取或设置当日售卖上限提醒（官方 <c>sale_limit_info</c>）。</summary>
    [JsonPropertyName("sale_limit_info")]
    public ChannelsSaleLimitInfo? SaleLimitInfo { get; set; }

    /// <summary>获取或设置商品信息质量（官方 <c>info_score</c>）。</summary>
    [JsonPropertyName("info_score")]
    public ChannelsProductInfoScore? InfoScore { get; set; }

    /// <summary>获取或设置商品高价预警（官方 <c>cmp_price_info</c>）。</summary>
    [JsonPropertyName("cmp_price_info")]
    public ChannelsProductCmpPriceInfo? CmpPriceInfo { get; set; }

    /// <summary>获取或设置审核信息（官方 <c>audit_info</c>）。</summary>
    [JsonPropertyName("audit_info")]
    public ChannelsProductAuditInfo? AuditInfo { get; set; }
}

/// <summary>
/// 获取商品列表（<c>product/list/get</c> / <c>product/gift/list/get</c>）请求体（两端点字段集一致 ⇒ 共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductListRequest
{
    /// <summary>
    /// 获取或设置商品状态（官方 <c>status</c>，不填默认拉全部商品（不包含从未上架的草稿和回收站商品）；
    /// gift/list 语义：0 初始值，5 上架，6 回收站（仅 status==6 才返回回收站），11 所有下架商品）。
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填：默认 10，不超过 30）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>next_key</c>，由上次请求返回；不传默认获取第一页）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>
/// 商品 id 列表响应（官方 <c>product/list/get</c> / <c>product/gift/list/get</c> /
/// <c>product/gift/onsale/set</c> 返回形态一致 ⇒ 共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductIdListResponse : ChannelsResponse
{
    /// <summary>获取或设置商品 id 列表（官方 <c>product_ids</c>）。</summary>
    [JsonPropertyName("product_ids")]
    public List<string>? ProductIds { get; set; }

    /// <summary>获取或设置本次翻页的上下文（官方 <c>next_key</c>，用于请求下一页）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置商品总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public long? TotalNum { get; set; }
}

/// <summary>
/// 商品 ID 请求体（官方 <c>listing</c> / <c>delisting</c> / <c>delete</c> / <c>audit/cancel</c> /
/// <c>canceltimingsale</c> 请求体字段集一致 ⇒ 共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductIdRequest
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填；赠品 ID 亦可共用）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;
}

/// <summary>
/// 更新商品免审（<c>product/auditfree</c>）请求体 —— 部分字段更新（sku / 库存 / 运费 / 限购等免审）。
/// </summary>
/// <remarks>
/// <para>官方契约：<b>POST</b> + 请求体；该接口仅对曾经上架成功过的商品适用（草稿状态非审核中，<c>edit_status != 2</c>）。</para>
/// <para>本地生活商品不受该规则约束。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsUpdateProductAuditFreeRequest
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，平台生成的 id）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置商品编码（官方 <c>spu_code</c>）。</summary>
    [JsonPropertyName("spu_code")]
    public string? SpuCode { get; set; }

    /// <summary>获取或设置需要进行更新的 sku 列表（官方 <c>skus</c>）。</summary>
    [JsonPropertyName("skus")]
    public List<ChannelsProductSkuUpdate> Skus { get; set; } = new();

    /// <summary>获取或设置限购信息（官方 <c>limited_info</c>）。</summary>
    [JsonPropertyName("limited_info")]
    public ChannelsProductLimitedInfo? LimitedInfo { get; set; }

    /// <summary>获取或设置运费信息（官方 <c>express_info</c>）。</summary>
    [JsonPropertyName("express_info")]
    public ChannelsProductExpressInfo? ExpressInfo { get; set; }

    /// <summary>获取或设置额外服务（官方 <c>extra_service</c>）。</summary>
    [JsonPropertyName("extra_service")]
    public ChannelsProductExtraService? ExtraService { get; set; }

    /// <summary>获取或设置发货方式（官方 <c>deliver_method</c>：0 快递发货（默认），1 无需快递；仅对部分类目开放）。</summary>
    [JsonPropertyName("deliver_method")]
    public int? DeliverMethod { get; set; }

    /// <summary>获取或设置是否在店铺首页隐藏（官方 <c>hide_in_window</c>：0 不隐藏，1 隐藏）。</summary>
    [JsonPropertyName("hide_in_window")]
    public int? HideInWindow { get; set; }

    /// <summary>获取或设置商品待开售信息（官方 <c>timing_onsale_info</c>）。</summary>
    [JsonPropertyName("timing_onsale_info")]
    public ChannelsTimingOnsaleInfo? TimingOnsaleInfo { get; set; }

    /// <summary>获取或设置 spu 维度配置全部 sku 预售规则（官方 <c>spu_deliver_info</c>）。</summary>
    [JsonPropertyName("spu_deliver_info")]
    public ChannelsSpuDeliverInfo? SpuDeliverInfo { get; set; }

    /// <summary>获取或设置售后 / 退货地址（官方 <c>after_sale_info</c>）。</summary>
    [JsonPropertyName("after_sale_info")]
    public ChannelsProductAfterSaleInfo? AfterSaleInfo { get; set; }
}

/// <summary>
/// 上架策略（官方 <c>audit_strategy</c> 对象：隐藏商品信息上架 / 可上架相似品 / 命中低风险规则可上架）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductAuditStrategy
{
    /// <summary>获取或设置隐藏商品信息上架（官方 <c>hide_err_field_flag</c>：1 打开，2 关闭；set 时 0 不修改）。</summary>
    [JsonPropertyName("hide_err_field_flag")]
    public int? HideErrFieldFlag { get; set; }

    /// <summary>获取或设置可上架相似品（官方 <c>hit_duplicated_flag</c>：1 打开，2 关闭；set 时 0 不修改）。</summary>
    [JsonPropertyName("hit_duplicated_flag")]
    public int? HitDuplicatedFlag { get; set; }

    /// <summary>获取或设置命中低风险规则可上架（官方 <c>hit_low_risk_rule_flag</c>：1 打开，2 关闭；set 时 0 不修改）。</summary>
    [JsonPropertyName("hit_low_risk_rule_flag")]
    public int? HitLowRiskRuleFlag { get; set; }
}

/// <summary>获取上架策略（<c>product/auditstrategy/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetProductAuditStrategyResponse : ChannelsResponse
{
    /// <summary>获取或设置上架策略（官方 <c>audit_strategy</c>）。</summary>
    [JsonPropertyName("audit_strategy")]
    public ChannelsProductAuditStrategy? AuditStrategy { get; set; }
}

/// <summary>设置上架策略（<c>product/auditstrategy/set</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsSetProductAuditStrategyRequest
{
    /// <summary>获取或设置上架策略（官方 <c>audit_strategy</c>，必填）。</summary>
    [JsonPropertyName("audit_strategy")]
    public ChannelsProductAuditStrategy AuditStrategy { get; set; } = new();
}

/// <summary>
/// 审核限额（官方 <c>audit_quota</c> 对象，<c>product/getauditquota</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductAuditQuota
{
    /// <summary>获取或设置是否已被限制（官方 <c>block_status</c>：1 时提审商品会变为 quota 不足状态）。</summary>
    [JsonPropertyName("block_status")]
    public int? BlockStatus { get; set; }

    /// <summary>获取或设置可用配额（官方 <c>avail_quota</c>，用尽时提审商品会变为 quota 不足状态）。</summary>
    [JsonPropertyName("avail_quota")]
    public long? AvailQuota { get; set; }

    /// <summary>获取或设置总配额（官方 <c>total_quota</c>）。</summary>
    [JsonPropertyName("total_quota")]
    public long? TotalQuota { get; set; }

    /// <summary>获取或设置是否不限制提审（官方 <c>unlimited_type</c>：1 时不受限、其他参数可忽略）。</summary>
    [JsonPropertyName("unlimited_type")]
    public int? UnlimitedType { get; set; }

    /// <summary>获取或设置审核总配额（官方 <c>audit_total_quota</c>）。</summary>
    [JsonPropertyName("audit_total_quota")]
    public long? AuditTotalQuota { get; set; }

    /// <summary>获取或设置审核剩余配额（官方 <c>audit_total_remaining</c>）。</summary>
    [JsonPropertyName("audit_total_remaining")]
    public long? AuditTotalRemaining { get; set; }

    /// <summary>获取或设置新增商品审核总配额（官方 <c>new_product_total_quota</c>）。</summary>
    [JsonPropertyName("new_product_total_quota")]
    public long? NewProductTotalQuota { get; set; }

    /// <summary>获取或设置新增商品审核剩余配额（官方 <c>new_product_remaining</c>）。</summary>
    [JsonPropertyName("new_product_remaining")]
    public long? NewProductRemaining { get; set; }
}

/// <summary>获取提审额度（<c>product/getauditquota</c>）响应。</summary>
/// <remarks>官方契约：<b>POST</b> + 空请求体（官方原文「调用接口时传空的json串即可」）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetProductAuditQuotaResponse : ChannelsResponse
{
    /// <summary>获取或设置审核限额（官方 <c>audit_quota</c>）。</summary>
    [JsonPropertyName("audit_quota")]
    public ChannelsProductAuditQuota? AuditQuota { get; set; }
}

/// <summary>获取商品受限信息（<c>product/getproductrestrictedinfo</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetProductRestrictedInfoRequest
{
    /// <summary>获取或设置商品 ID 列表（官方 <c>product_ids</c>，numarray，单次最多 50 个）。</summary>
    [JsonPropertyName("product_ids")]
    public List<long> ProductIds { get; set; } = new();
}

/// <summary>获取商品受限信息（<c>product/getproductrestrictedinfo</c>）响应数据（官方 <c>data</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductRestrictedData
{
    /// <summary>获取或设置商品受限信息列表（官方 <c>restricted_info_list</c>，按请求 product_ids 顺序对齐；缺失的 product_id 不在列表中）。</summary>
    [JsonPropertyName("restricted_info_list")]
    public List<ChannelsProductRestrictedInfo>? RestrictedInfoList { get; set; }
}

/// <summary>商品受限信息（官方 <c>restricted_info_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductRestrictedInfo
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置受限场景列表（官方 <c>scene_info</c>）。</summary>
    [JsonPropertyName("scene_info")]
    public List<ChannelsProductRestrictedScene>? SceneInfo { get; set; }
}

/// <summary>受限场景（官方 <c>scene_info</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductRestrictedScene
{
    /// <summary>获取或设置受限场景（官方 <c>scene</c>，见 <see cref="ChannelsProductRestrictedScenes"/>）。</summary>
    [JsonPropertyName("scene")]
    public int? Scene { get; set; }

    /// <summary>获取或设置策略来源（官方 <c>source</c>，见 <see cref="ChannelsProductRestrictedSources"/>）。</summary>
    [JsonPropertyName("source")]
    public int? Source { get; set; }
}

/// <summary>获取商品受限信息（<c>product/getproductrestrictedinfo</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetProductRestrictedInfoResponse : ChannelsResponse
{
    /// <summary>获取或设置响应数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsProductRestrictedData? Data { get; set; }
}

/// <summary>商品类目鉴定（<c>product/category/classify</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductClassifyRequest
{
    /// <summary>获取或设置请求类型（官方 <c>req_type</c>，必填）：1 基于标题和主图推断商品类目，2 基于商品内容判断是否类目错放。</summary>
    [JsonPropertyName("req_type")]
    public int ReqType { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置商品主图列表（官方 <c>head_imgs</c>，必填：至少传入一个有效的头图 url）。</summary>
    [JsonPropertyName("head_imgs")]
    public List<string> HeadImgs { get; set; } = new();

    /// <summary>获取或设置类目 id（官方 <c>cat_id</c>，请求类型为 2 时必填）。</summary>
    [JsonPropertyName("cat_id")]
    public string? CatId { get; set; }
}

/// <summary>类目信息（官方 <c>cat_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductClassifyCatInfo
{
    /// <summary>获取或设置类目 id（官方 <c>cat_id</c>）。</summary>
    [JsonPropertyName("cat_id")]
    public string? CatId { get; set; }
}

/// <summary>类目权限信息（官方 <c>cats</c> 对象：<c>cat_info</c> + <c>has_permission</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductClassifyCats
{
    /// <summary>获取或设置类目信息（官方 <c>cat_info</c>）。</summary>
    [JsonPropertyName("cat_info")]
    public ChannelsProductClassifyCatInfo? CatInfo { get; set; }

    /// <summary>获取或设置是否有该类目的权限（官方 <c>has_permission</c>）。</summary>
    [JsonPropertyName("has_permission")]
    public bool? HasPermission { get; set; }
}

/// <summary>推荐类目（官方 <c>categories</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductClassifyCategory
{
    /// <summary>获取或设置推荐的多个多级类目信息（官方 <c>cats</c>）。</summary>
    [JsonPropertyName("cats")]
    public ChannelsProductClassifyCats? Cats { get; set; }

    /// <summary>获取或设置是否有该类目的权限（官方 <c>has_permission</c>）。</summary>
    [JsonPropertyName("has_permission")]
    public bool? HasPermission { get; set; }

    /// <summary>获取或设置类目 id（官方 <c>cat_id</c>）。</summary>
    [JsonPropertyName("cat_id")]
    public string? CatId { get; set; }
}

/// <summary>商品类目鉴定（<c>product/category/classify</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductClassifyResponse : ChannelsResponse
{
    /// <summary>获取或设置推荐的多个多级类目信息（官方 <c>categories</c>）。</summary>
    [JsonPropertyName("categories")]
    public List<ChannelsProductClassifyCategory>? Categories { get; set; }

    /// <summary>获取或设置是否类目错放（官方 <c>wrong_cat</c>，仅请求类型为 2 时返回）。</summary>
    [JsonPropertyName("wrong_cat")]
    public bool? WrongCat { get; set; }
}

/// <summary>类目预检（<c>product/categoryprecheck</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductCategoryPrecheckRequest
{
    /// <summary>获取或设置叶子类目 id（官方 <c>cat_id</c>）。</summary>
    [JsonPropertyName("cat_id")]
    public long? CatId { get; set; }
}

/// <summary>类目预检（<c>product/categoryprecheck</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductCategoryPrecheckResponse : ChannelsResponse
{
    /// <summary>获取或设置类目是否可用（官方 <c>all_pass</c>：false 不可用请参考原因解决，true 正常）。</summary>
    [JsonPropertyName("all_pass")]
    public bool? AllPass { get; set; }

    /// <summary>获取或设置校验不通过的原因（官方 <c>fail_reasons</c>，可能为多个）。</summary>
    [JsonPropertyName("fail_reasons")]
    public List<string>? FailReasons { get; set; }
}

/// <summary>货主（官方 <c>supplier</c> 对象，<c>addproductthirdpartysource</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsThirdPartySourceSupplier
{
    /// <summary>获取或设置货主主体名称（官方 <c>supplier_name</c>，必填）。</summary>
    [JsonPropertyName("supplier_name")]
    public string SupplierName { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置货源平台类型（官方 <c>source_platform_type</c>）——
    /// <c>scene_value==1</c>（分销铺货）或 <c>scene_value==2</c>（商品搬家）时必填。
    /// </summary>
    [JsonPropertyName("source_platform_type")]
    public int? SourcePlatformType { get; set; }

    /// <summary>获取或设置货源平台名称（官方 <c>source_platform_name</c>；<c>scene_value==3</c> 其他场景时需入参平台名称）。</summary>
    [JsonPropertyName("source_platform_name")]
    public string? SourcePlatformName { get; set; }
}

/// <summary>货主店铺经营表现（官方 <c>supplier_shop_performance</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsSupplierShopPerformance
{
    /// <summary>获取或设置 24h 揽收率（官方 <c>accept_rate_24h</c>）。</summary>
    [JsonPropertyName("accept_rate_24h")]
    public long? AcceptRate24h { get; set; }

    /// <summary>获取或设置 48h 揽收率（官方 <c>accept_rate_48h</c>）。</summary>
    [JsonPropertyName("accept_rate_48h")]
    public long? AcceptRate48h { get; set; }

    /// <summary>获取或设置近 30 天代发数量（官方 <c>ship_count_30d</c>）。</summary>
    [JsonPropertyName("ship_count_30d")]
    public long? ShipCount30d { get; set; }

    /// <summary>获取或设置近 7 天代发数量（官方 <c>ship_count_7d</c>）。</summary>
    [JsonPropertyName("ship_count_7d")]
    public long? ShipCount7d { get; set; }

    /// <summary>获取或设置铺货分销商数量（官方 <c>distribute_product_count</c>）。</summary>
    [JsonPropertyName("distribute_product_count")]
    public long? DistributeProductCount { get; set; }

    /// <summary>获取或设置店铺准时发货率（官方 <c>on_time_ship_rate</c>）。</summary>
    [JsonPropertyName("on_time_ship_rate")]
    public long? OnTimeShipRate { get; set; }

    /// <summary>获取或设置商家身份（官方 <c>merchant_identity</c>）。</summary>
    [JsonPropertyName("merchant_identity")]
    public string? MerchantIdentity { get; set; }
}

/// <summary>第三方货源商品属性（官方 <c>attr_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsThirdPartyProductAttr
{
    /// <summary>获取或设置属性类型（官方 <c>attr_type</c>）。</summary>
    [JsonPropertyName("attr_type")]
    public int? AttrType { get; set; }

    /// <summary>获取或设置属性名称（官方 <c>attr_name</c>）。</summary>
    [JsonPropertyName("attr_name")]
    public string? AttrName { get; set; }

    /// <summary>获取或设置属性值（官方 <c>attr_value</c>）。</summary>
    [JsonPropertyName("attr_value")]
    public string? AttrValue { get; set; }
}

/// <summary>第三方货源商品 SKU（官方 <c>sku_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsThirdPartyProductSku
{
    /// <summary>获取或设置 sku ID（官方 <c>sku_id</c>，必填）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>获取或设置规格名称（官方 <c>spec_name</c>，必填）。</summary>
    [JsonPropertyName("spec_name")]
    public string SpecName { get; set; } = string.Empty;

    /// <summary>获取或设置规格值（官方 <c>spec_value</c>，必填）。</summary>
    [JsonPropertyName("spec_value")]
    public string SpecValue { get; set; } = string.Empty;

    /// <summary>获取或设置 sku 图（官方 <c>sku_img</c>，必填）。</summary>
    [JsonPropertyName("sku_img")]
    public string SkuImg { get; set; } = string.Empty;

    /// <summary>获取或设置价格（官方 <c>price</c>，必填）。</summary>
    [JsonPropertyName("price")]
    public long Price { get; set; }

    /// <summary>获取或设置库存（官方 <c>stock</c>，必填）。</summary>
    [JsonPropertyName("stock")]
    public long Stock { get; set; }

    /// <summary>获取或设置库存类型（官方 <c>stock_type</c>，必填）。</summary>
    [JsonPropertyName("stock_type")]
    public long StockType { get; set; }
}

/// <summary>第三方货源物流信息（官方 <c>logistics_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsThirdPartyLogisticsInfo
{
    /// <summary>获取或设置运费信息（官方 <c>freight_info</c>）。</summary>
    [JsonPropertyName("freight_info")]
    public string? FreightInfo { get; set; }

    /// <summary>获取或设置物流模版（官方 <c>logistics_template</c>）。</summary>
    [JsonPropertyName("logistics_template")]
    public string? LogisticsTemplate { get; set; }
}

/// <summary>商品在货源平台的信息（官方 <c>product_source_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsThirdPartyProductSourceInfo
{
    /// <summary>获取或设置可访问链接（官方 <c>visit_url</c>，必填）。</summary>
    [JsonPropertyName("visit_url")]
    public string VisitUrl { get; set; } = string.Empty;

    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置标题（官方 <c>title</c>，必填）。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>获取或设置主图 URL 列表（官方 <c>head_img_urls</c>，含视频 URL，必填）。</summary>
    [JsonPropertyName("head_img_urls")]
    public List<string> HeadImgUrls { get; set; } = new();

    /// <summary>获取或设置详情图 URL 列表（官方 <c>detail_img_urls</c>，必填）。</summary>
    [JsonPropertyName("detail_img_urls")]
    public List<string> DetailImgUrls { get; set; } = new();

    /// <summary>获取或设置商品资质列表（官方 <c>qualification_list</c>）。</summary>
    [JsonPropertyName("qualification_list")]
    public List<string>? QualificationList { get; set; }

    /// <summary>获取或设置品牌（官方 <c>brand</c>）。</summary>
    [JsonPropertyName("brand")]
    public string? Brand { get; set; }

    /// <summary>获取或设置属性列表（官方 <c>attr_list</c>）。</summary>
    [JsonPropertyName("attr_list")]
    public List<ChannelsThirdPartyProductAttr>? AttrList { get; set; }

    /// <summary>获取或设置规格列表（官方 <c>sku_list</c>，必填）。</summary>
    [JsonPropertyName("sku_list")]
    public List<ChannelsThirdPartyProductSku> SkuList { get; set; } = new();

    /// <summary>获取或设置详情描述（官方 <c>detail_description</c>）。</summary>
    [JsonPropertyName("detail_description")]
    public string? DetailDescription { get; set; }

    /// <summary>获取或设置物流信息（官方 <c>logistics_info</c>）。</summary>
    [JsonPropertyName("logistics_info")]
    public ChannelsThirdPartyLogisticsInfo? LogisticsInfo { get; set; }

    /// <summary>获取或设置支持的服务保障功能（官方 <c>service_guarantee</c>）。</summary>
    [JsonPropertyName("service_guarantee")]
    public string? ServiceGuarantee { get; set; }

    /// <summary>获取或设置发货时效 / 承诺发货时间（官方 <c>delivery_time</c>）。</summary>
    [JsonPropertyName("delivery_time")]
    public string? DeliveryTime { get; set; }
}

/// <summary>新增第三方货源商品（<c>addproductthirdpartysource</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsAddProductThirdPartySourceRequest
{
    /// <summary>获取或设置场景值（官方 <c>scene_value</c>，必填；见 <see cref="ChannelsThirdPartySourceScenes"/>）。</summary>
    [JsonPropertyName("scene_value")]
    public int SceneValue { get; set; }

    /// <summary>获取或设置商品发布方式（官方 <c>publish_method</c>，<c>scene_value==3</c> 其他场景时必填）。</summary>
    [JsonPropertyName("publish_method")]
    public int? PublishMethod { get; set; }

    /// <summary>获取或设置货主（官方 <c>supplier</c>，必填）。</summary>
    [JsonPropertyName("supplier")]
    public ChannelsThirdPartySourceSupplier Supplier { get; set; } = new();

    /// <summary>获取或设置货主店铺经营表现（官方 <c>supplier_shop_performance</c>）。</summary>
    [JsonPropertyName("supplier_shop_performance")]
    public ChannelsSupplierShopPerformance? SupplierShopPerformance { get; set; }

    /// <summary>获取或设置商品在货源平台的信息（官方 <c>product_source_info</c>，必填）。</summary>
    [JsonPropertyName("product_source_info")]
    public ChannelsThirdPartyProductSourceInfo ProductSourceInfo { get; set; } = new();
}

/// <summary>新增第三方货源商品（<c>addproductthirdpartysource</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsAddProductThirdPartySourceResponse : ChannelsResponse
{
    /// <summary>获取或设置货源 id（官方 <c>third_party_source_id</c>）。</summary>
    [JsonPropertyName("third_party_source_id")]
    public long? ThirdPartySourceId { get; set; }
}

/// <summary>品牌推荐（<c>product/productbrandrecommend</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductBrandRecommendRequest
{
    /// <summary>获取或设置叶子类目 id（官方 <c>cat_id</c>，必填）。</summary>
    [JsonPropertyName("cat_id")]
    public long CatId { get; set; }

    /// <summary>获取或设置主图列表（官方 <c>head_imgs</c>，必填：至少传一张）。</summary>
    [JsonPropertyName("head_imgs")]
    public List<string> HeadImgs { get; set; } = new();

    /// <summary>获取或设置详情图列表（官方 <c>detail_imgs</c>）。</summary>
    [JsonPropertyName("detail_imgs")]
    public List<string>? DetailImgs { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>title</c>，必填）。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}

/// <summary>品牌推荐（<c>product/productbrandrecommend</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductBrandRecommendResponse : ChannelsResponse
{
    /// <summary>获取或设置品牌 id（官方 <c>brand_id</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public long? BrandId { get; set; }

    /// <summary>获取或设置品牌中文名称（官方 <c>brand_name_chinese</c>）。</summary>
    [JsonPropertyName("brand_name_chinese")]
    public string? BrandNameChinese { get; set; }

    /// <summary>获取或设置品牌英文名称（官方 <c>brand_name_english</c>）。</summary>
    [JsonPropertyName("brand_name_english")]
    public string? BrandNameEnglish { get; set; }
}

/// <summary>外部商品属性映射（<c>externalproductmapping</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsExternalProductMappingRequest
{
    /// <summary>获取或设置叶子类目 id（官方 <c>cat_id</c>，必填）。</summary>
    [JsonPropertyName("cat_id")]
    public long CatId { get; set; }

    /// <summary>获取或设置外部商品属性 key（官方 <c>external_attribute_name</c>，必填）。</summary>
    [JsonPropertyName("external_attribute_name")]
    public string ExternalAttributeName { get; set; } = string.Empty;

    /// <summary>获取或设置外部商品属性值（官方 <c>external_attribute_value</c>）。</summary>
    [JsonPropertyName("external_attribute_value")]
    public string? ExternalAttributeValue { get; set; }

    /// <summary>获取或设置外部商品类目名称（官方 <c>external_category_name</c>）。</summary>
    [JsonPropertyName("external_category_name")]
    public string? ExternalCategoryName { get; set; }
}

/// <summary>外部商品属性映射（<c>externalproductmapping</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsExternalProductMappingResponse : ChannelsResponse
{
    /// <summary>获取或设置外部商品属性 key（官方 <c>external_attribute_name</c>）。</summary>
    [JsonPropertyName("external_attribute_name")]
    public string? ExternalAttributeName { get; set; }

    /// <summary>获取或设置外部商品属性值（官方 <c>external_attribute_value</c>）。</summary>
    [JsonPropertyName("external_attribute_value")]
    public string? ExternalAttributeValue { get; set; }

    /// <summary>获取或设置内部商品属性 key（官方 <c>internal_attribute_name</c>）。</summary>
    [JsonPropertyName("internal_attribute_name")]
    public string? InternalAttributeName { get; set; }

    /// <summary>获取或设置内部商品属性值（官方 <c>internal_attribute_value</c>，可能为多选）。</summary>
    [JsonPropertyName("internal_attribute_value")]
    public List<string>? InternalAttributeValue { get; set; }
}

/// <summary>外部商品属性（官方 <c>external_attributes</c> / 响应 <c>attributes</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsExternalProductAttr
{
    /// <summary>获取或设置外部属性 key / 属性 key（官方 <c>key</c>）。</summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>获取或设置外部属性值 / 属性值（官方 <c>value</c>）。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

/// <summary>外部商品属性映射（新版，<c>externalproductmappingnew</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsExternalProductMappingNewRequest
{
    /// <summary>获取或设置叶子类目 id（官方 <c>cat_id</c>，必填）。</summary>
    [JsonPropertyName("cat_id")]
    public long CatId { get; set; }

    /// <summary>获取或设置外部商品类目名称（官方 <c>external_category_name</c>）。</summary>
    [JsonPropertyName("external_category_name")]
    public string? ExternalCategoryName { get; set; }

    /// <summary>获取或设置主图列表（官方 <c>head_imgs</c>，必填：至少传一张）。</summary>
    [JsonPropertyName("head_imgs")]
    public List<string> HeadImgs { get; set; } = new();

    /// <summary>获取或设置详情图列表（官方 <c>detail_imgs</c>）。</summary>
    [JsonPropertyName("detail_imgs")]
    public List<string>? DetailImgs { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>title</c>，必填）。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>获取或设置属性列表（官方 <c>external_attributes</c>）。</summary>
    [JsonPropertyName("external_attributes")]
    public List<ChannelsExternalProductAttr>? ExternalAttributes { get; set; }
}

/// <summary>外部商品属性映射（新版，<c>externalproductmappingnew</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsExternalProductMappingNewResponse : ChannelsResponse
{
    /// <summary>获取或设置映射属性结果（官方 <c>attributes</c>）。</summary>
    [JsonPropertyName("attributes")]
    public List<ChannelsExternalProductAttr>? Attributes { get; set; }
}

/// <summary>开始定时开售（<c>begintimingsale</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsBeginTimingSaleRequest
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置定时开售任务 ID（官方 <c>task_id</c>，必填；来自商品里的字段）。</summary>
    [JsonPropertyName("task_id")]
    public long TaskId { get; set; }
}

/// <summary>商品微信口令（<c>taglink/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductTagLinkRequest
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置小店通过关联账号绑定的企业微信 id（官方 <c>wecom_corp_id</c>）。</summary>
    [JsonPropertyName("wecom_corp_id")]
    public string? WecomCorpId { get; set; }

    /// <summary>获取或设置小店通过关联账号绑定的企业微信下的成员 id（官方 <c>wecom_user_id</c>）。</summary>
    [JsonPropertyName("wecom_user_id")]
    public string? WecomUserId { get; set; }
}

/// <summary>商品微信口令（<c>taglink/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetProductTagLinkResponse : ChannelsResponse
{
    /// <summary>获取或设置商品微信口令（官方 <c>product_taglink</c>，只支持微信内打开）。</summary>
    [JsonPropertyName("product_taglink")]
    public string? ProductTagLink { get; set; }
}

/// <summary>商品二维码（<c>qrcode/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetProductQrcodeRequest
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置二维码类型（官方 <c>qrcode_type</c>，缺省 1）：1 二维码，2 标准物料，3 送礼物物料。</summary>
    [JsonPropertyName("qrcode_type")]
    public int? QrcodeType { get; set; }

    /// <summary>获取或设置小店通过关联账号绑定的企业微信 id（官方 <c>wecom_corp_id</c>）。</summary>
    [JsonPropertyName("wecom_corp_id")]
    public string? WecomCorpId { get; set; }

    /// <summary>获取或设置小店通过关联账号绑定的企业微信下的成员 id（官方 <c>wecom_user_id</c>）。</summary>
    [JsonPropertyName("wecom_user_id")]
    public string? WecomUserId { get; set; }
}

/// <summary>商品二维码（<c>qrcode/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetProductQrcodeResponse : ChannelsResponse
{
    /// <summary>获取或设置商品二维码链接（官方 <c>product_qrcode</c>）。</summary>
    [JsonPropertyName("product_qrcode")]
    public string? ProductQrcode { get; set; }
}

/// <summary>商品 H5 短链（<c>h5url/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetProductH5UrlRequest
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置小店通过关联账号绑定的企业微信 id（官方 <c>wecom_corp_id</c>）。</summary>
    [JsonPropertyName("wecom_corp_id")]
    public string? WecomCorpId { get; set; }

    /// <summary>获取或设置小店通过关联账号绑定的企业微信下的成员 id（官方 <c>wecom_user_id</c>）。</summary>
    [JsonPropertyName("wecom_user_id")]
    public string? WecomUserId { get; set; }
}

/// <summary>商品 H5 短链（<c>h5url/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetProductH5UrlResponse : ChannelsResponse
{
    /// <summary>获取或设置商品 H5 短链（官方 <c>product_h5url</c>）。</summary>
    [JsonPropertyName("product_h5url")]
    public string? ProductH5Url { get; set; }
}

/// <summary>商品跳转 scheme（<c>scheme/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetProductSchemeRequest
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置来源 appid（官方 <c>from_appid</c>，必填）。</summary>
    [JsonPropertyName("from_appid")]
    public string FromAppid { get; set; } = string.Empty;

    /// <summary>获取或设置过期时间（官方 <c>expire</c>，必填，单位：秒）。</summary>
    [JsonPropertyName("expire")]
    public long Expire { get; set; }

    /// <summary>获取或设置附加信息（官方 <c>ext_info</c>）。</summary>
    [JsonPropertyName("ext_info")]
    public string? ExtInfo { get; set; }
}

/// <summary>商品跳转 scheme（<c>scheme/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsGetProductSchemeResponse : ChannelsResponse
{
    /// <summary>获取或设置商品跳转 scheme 码（官方 <c>openlink</c>）。</summary>
    [JsonPropertyName("openlink")]
    public string? OpenLink { get; set; }
}

// ---- 枚举常量 ---------------------------------------------------------------

/// <summary>商品线上状态常量（官方 <c>status</c> 枚举值）。</summary>
public static class ChannelsProductStatuses
{
    /// <summary>初始值。</summary>
    public const int Initial = 0;

    /// <summary>上架。</summary>
    public const int Listing = 5;

    /// <summary>回收站。</summary>
    public const int Recycled = 6;

    /// <summary>初始值（草稿）。</summary>
    public const int EditInit = 10;

    /// <summary>自主下架。</summary>
    public const int ManualDelisting = 11;

    /// <summary>资质审核失败。</summary>
    public const int QualificationAuditFailed = 13;

    /// <summary>审核失败。</summary>
    public const int AuditFailed = 14;

    /// <summary>违规下架。</summary>
    public const int ViolationDelisting = 15;

    /// <summary>审核中。</summary>
    public const int Auditing = 16;

    /// <summary>审核通过。</summary>
    public const int AuditPassed = 17;

    /// <summary>待提交审核。</summary>
    public const int PendingAudit = 19;

    /// <summary>平台已下架。</summary>
    public const int PlatformDelisting = 20;
}

/// <summary>商品草稿状态常量（官方 <c>edit_status</c> 枚举值）。</summary>
public static class ChannelsProductEditStatuses
{
    /// <summary>初始值。</summary>
    public const int Initial = 0;

    /// <summary>草稿。</summary>
    public const int Draft = 1;

    /// <summary>审核中。</summary>
    public const int Auditing = 2;

    /// <summary>审核通过。</summary>
    public const int AuditPassed = 3;

    /// <summary>审核失败。</summary>
    public const int AuditFailed = 4;
}

/// <summary>商品类型常量（官方 <c>product_type</c> 枚举值）。</summary>
public static class ChannelsProductTypes
{
    /// <summary>小店普通自营商品。</summary>
    public const int Normal = 1;

    /// <summary>福袋抽奖商品（只读，不支持编辑、上架）。</summary>
    public const int LuckyBag = 2;

    /// <summary>直播间闪电购商品（只读，不支持编辑、上架）。</summary>
    public const int FlashBuy = 3;
}

/// <summary>偏远地区中转集运开关常量（官方 <c>express_type</c> 枚举值）。</summary>
public static class ChannelsProductExpressTypes
{
    /// <summary>默认，普通类型。</summary>
    public const int Off = 0;

    /// <summary>中转集运类型。</summary>
    public const int On = 1;
}

/// <summary>限购周期类型常量（官方 <c>period_type</c> 枚举值）。</summary>
public static class ChannelsProductLimitedPeriodTypes
{
    /// <summary>无限购（默认）。</summary>
    public const int Unlimited = 0;

    /// <summary>按自然日限购。</summary>
    public const int Daily = 1;

    /// <summary>按自然周限购。</summary>
    public const int Weekly = 2;

    /// <summary>按自然月限购。</summary>
    public const int Monthly = 3;

    /// <summary>按自然年限购。</summary>
    public const int Yearly = 4;
}

/// <summary>七天无理由退货类型常量（官方 <c>seven_day_return</c> 枚举值）。</summary>
public static class ChannelsSevenDayReturnTypes
{
    /// <summary>不支持七天无理由。</summary>
    public const int NotSupported = 0;

    /// <summary>支持七天无理由。</summary>
    public const int Supported = 1;

    /// <summary>支持七天无理由（定制商品除外）。</summary>
    public const int SupportedExceptCustom = 2;

    /// <summary>支持七天无理由（使用后不支持）。</summary>
    public const int SupportedUsedNot = 3;
}

/// <summary>商品受限场景常量（官方 <c>scene</c> 枚举值，<c>getproductrestrictedinfo</c>）。</summary>
public static class ChannelsProductRestrictedScenes
{
    /// <summary>直播间。</summary>
    public const int LiveRoom = 1;

    /// <summary>短视频。</summary>
    public const int ShortVideo = 2;

    /// <summary>店铺主页及达人橱窗。</summary>
    public const int StoreWindow = 3;

    /// <summary>搜索。</summary>
    public const int Search = 4;

    /// <summary>广告。</summary>
    public const int Advertisement = 5;

    /// <summary>订单及卡片。</summary>
    public const int Order = 6;

    /// <summary>优选联盟。</summary>
    public const int League = 7;

    /// <summary>商品推荐。</summary>
    public const int Recommend = 8;

    /// <summary>商品售卖。</summary>
    public const int Sale = 9;

    /// <summary>隐藏不合规信息。</summary>
    public const int HideInvalidInfo = 10;
}

/// <summary>受限策略来源常量（官方 <c>source</c> 枚举值，<c>getproductrestrictedinfo</c>）。</summary>
public static class ChannelsProductRestrictedSources
{
    /// <summary>上架策略-低风险。</summary>
    public const int LowRiskStrategy = 1;

    /// <summary>上架策略-相似品。</summary>
    public const int DuplicatedStrategy = 2;

    /// <summary>上架策略-隐藏图片。</summary>
    public const int HideImageStrategy = 3;

    /// <summary>试运营类目。</summary>
    public const int TrialCategory = 4;

    /// <summary>橱窗隐藏。</summary>
    public const int WindowHide = 5;

    /// <summary>极简模式。</summary>
    public const int SimpleMode = 6;

    /// <summary>售卖限制。</summary>
    public const int SaleLimit = 7;

    /// <summary>库存限制（优选联盟观察期）。</summary>
    public const int StockLimit = 8;
}

/// <summary>第三方货源场景值常量（官方 <c>scene_value</c> 枚举值，<c>addproductthirdpartysource</c>）。</summary>
public static class ChannelsThirdPartySourceScenes
{
    /// <summary>分销铺货。</summary>
    public const int Distribution = 1;

    /// <summary>商品搬家。</summary>
    public const int ProductMove = 2;

    /// <summary>其他场景。</summary>
    public const int Other = 3;
}