// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PartnerTransactions;

/// <summary>
/// 服务商下单的订单金额（<c>amount</c>，必填对象）—— 直连域 <c>JsapiAmountInfo</c> 的服务商同位类型。
/// </summary>
/// <remarks>
/// <para><b>官方来源</b>：小程序下单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012714911"/>、
/// Native 下单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012714902"/>、
/// APP 下单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012714669"/>、
/// H5 下单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012714759"/>、
/// 扫码支付下单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012492476"/>
/// （2026-10-14 逐页核验，五族该节字段表逐项一致）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerAmountInfo
{
    /// <summary>订单总金额（<c>total</c>，必填整型，单位为分，必须大于 0；1 元填 100）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>货币类型（<c>currency</c>，选填 string(16)），境内商户号固定 <c>CNY</c>。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}

/// <summary>
/// 服务商下单的结算信息（<c>settle_info</c>，选填对象）。
/// </summary>
/// <remarks>
/// <para><b>⚠️ <c>support_fapiao</c> 在本域位于 <c>settle_info</c> 内</b>（JSAPI / Native / APP / H5 四族皆然），
/// 而<b>扫码支付下单（<c>scannedpos</c>）把它放在请求顶层</b> —— 直连域 <c>JsapiPrepayRequest</c> 同样是顶层。
/// 该位置差异由守卫 PT-B4 锁定，不得为「四族统一美观」把某族字段搬位。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerSettleInfo
{
    /// <summary>
    /// 分账标识（<c>profit_sharing</c>，选填 boolean）：<c>true</c> 表示支付成功后资金冻结、待服务商调分账接口；
    /// 服务商模式下<b>要分账必须显式传 <c>true</c></b>，否则资金直接入子商户可用余额、后续分账无源。
    /// </summary>
    [JsonPropertyName("profit_sharing")]
    public bool? ProfitSharing { get; set; }

    /// <summary>
    /// 补差金额（<c>subsidy_amount</c>，选填整型，单位为分）：仅当 <c>profit_sharing = true</c> 时生效；
    /// 官方限制单笔订单最高补差 5000 元。
    /// </summary>
    [JsonPropertyName("subsidy_amount")]
    public long? SubsidyAmount { get; set; }

    /// <summary>电子发票入口开放标识（<c>support_fapiao</c>，选填 boolean）：传 <c>true</c> 时支付成功消息与支付详情页出现开票入口。</summary>
    [JsonPropertyName("support_fapiao")]
    public bool? SupportFapiao { get; set; }
}

/// <summary>服务商下单的支付者信息（<c>payer</c>）：JSAPI / 扫码支付两族使用，Native / APP / H5 <b>无此节</b>。</summary>
/// <remarks>
/// <para><b>官方约束（原文）</b>：<c>sp_openid</c> 和 <c>sub_openid</c> 两个字段<b>必须要填一个</b>；
/// 若传 <c>sub_openid</c>，则 <c>sub_appid</c> 必填。二者皆为选填是因为归属由 <c>sub_appid</c> 是否上送决定，
/// 类型层面无法表达「二选一」⇒ 由调用方保证，错配官方返回 <c>OPENID_MISMATCH</c>(500)。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerPayerInfo
{
    /// <summary>用户服务标识（<c>sp_openid</c>，选填 string(128)）：用户在<b>服务商</b> <c>sp_appid</c> 下的唯一标识。</summary>
    [JsonPropertyName("sp_openid")]
    public string? SpOpenId { get; set; }

    /// <summary>用户子标识（<c>sub_openid</c>，选填 string(128)）：用户在<b>子商户</b> <c>sub_appid</c> 下的唯一标识；传它则 <c>sub_appid</c> 必填。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenId { get; set; }
}

/// <summary>服务商下单的商品详情（<c>detail</c>，选填对象）；结构去除空格换行压缩后总长不超过 6144 字节。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerGoodsDetail
{
    /// <summary>订单原价（<c>cost_price</c>，选填整型，单位为分）。与支付金额不相等则不享受优惠。</summary>
    [JsonPropertyName("cost_price")]
    public long? CostPrice { get; set; }

    /// <summary>商品小票 ID（<c>invoice_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("invoice_id")]
    public string? InvoiceId { get; set; }

    /// <summary>单品列表（<c>goods_detail</c>，选填数组，至少 1 条），见 <see cref="PartnerGoodsDetailItem"/>。</summary>
    [JsonPropertyName("goods_detail")]
    public List<PartnerGoodsDetailItem>? GoodsDetail { get; set; }
}

/// <summary>服务商下单的单品明细（<c>detail.goods_detail[]</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerGoodsDetailItem
{
    /// <summary>商户侧商品编码（<c>merchant_goods_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("merchant_goods_id")]
    public string? MerchantGoodsId { get; set; }

    /// <summary>微信支付商品编码（<c>wechatpay_goods_id</c>，选填 string(32)），官方统一商品编号。</summary>
    [JsonPropertyName("wechatpay_goods_id")]
    public string? WechatpayGoodsId { get; set; }

    /// <summary>商品名称（<c>goods_name</c>，选填 string(256)）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>商品数量（<c>quantity</c>，必填整型）。</summary>
    [JsonPropertyName("quantity")]
    public long? Quantity { get; set; }

    /// <summary>商品单价（<c>unit_price</c>，必填整型，单位为分；须为商户优惠后的单价）。</summary>
    [JsonPropertyName("unit_price")]
    public long? UnitPrice { get; set; }
}

/// <summary>
/// 服务商下单的场景信息（<c>scene_info</c>，选填对象）—— JSAPI / Native / APP 三族共用。
/// </summary>
/// <remarks>
/// <para><b>本族 <c>payer_client_ip</c> 是必填</b>（对象本身选填）：一旦上送 <c>scene_info</c> 就缺不了它。</para>
/// <para><b>H5 与扫码支付的场景信息不是本类型</b>：H5 多一节 <c>h5_info</c>、扫码支付的 <c>store_info</c>
/// 只有 <c>id</c>/<c>out_id</c> 且整体必填 —— 见 <see cref="PartnerH5SceneInfo"/> 与
/// <see cref="PartnerScannedPosSceneInfo"/>（守卫 PT-B4 锁定三型不共用）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerSceneInfo
{
    /// <summary>用户终端 IP（<c>payer_client_ip</c>，必填 string(45)），支持 IPv4 / IPv6。</summary>
    [JsonPropertyName("payer_client_ip")]
    public string? PayerClientIp { get; set; }

    /// <summary>商户端设备号（<c>device_id</c>，选填 string(32)），门店号或收银设备 ID。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

    /// <summary>商户门店信息（<c>store_info</c>，选填对象），见 <see cref="PartnerStoreInfo"/>。</summary>
    [JsonPropertyName("store_info")]
    public PartnerStoreInfo? StoreInfo { get; set; }
}

/// <summary>服务商下单的商户门店信息（<c>scene_info.store_info</c>，JSAPI / Native / APP 族）。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerStoreInfo
{
    /// <summary>门店编号（<c>id</c>，必填 string(32)），商户侧门店编号。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>门店名称（<c>name</c>，选填 string(256)）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>地区编码（<c>area_code</c>，选填 string(32)），见官方省市区编号对照表。</summary>
    [JsonPropertyName("area_code")]
    public string? AreaCode { get; set; }

    /// <summary>详细地址（<c>address</c>，选填 string(512)）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }
}

/// <summary>
/// H5 下单的场景信息（<c>scene_info</c>，<b>必填</b>对象）：比 <see cref="PartnerSceneInfo"/> 多一节 <c>h5_info</c>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerH5SceneInfo
{
    /// <summary>用户终端 IP（<c>payer_client_ip</c>，必填 string(45)）。</summary>
    [JsonPropertyName("payer_client_ip")]
    public string? PayerClientIp { get; set; }

    /// <summary>商户端设备号（<c>device_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

    /// <summary>商户门店信息（<c>store_info</c>，选填对象），见 <see cref="PartnerStoreInfo"/>。</summary>
    [JsonPropertyName("store_info")]
    public PartnerStoreInfo? StoreInfo { get; set; }

    /// <summary>H5 场景信息（<c>h5_info</c>，<b>必填</b>），见 <see cref="PartnerH5Info"/> —— 官方对 H5 下单强制要求。</summary>
    [JsonPropertyName("h5_info")]
    public PartnerH5Info? H5Info { get; set; }
}

/// <summary>H5 场景信息（<c>scene_info.h5_info</c>，必填对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerH5Info
{
    /// <summary>场景类型（<c>type</c>，必填 string(32)），官方示例取值 <c>Wap</c>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>应用名称（<c>app_name</c>，选填 string(64)）。</summary>
    [JsonPropertyName("app_name")]
    public string? AppName { get; set; }

    /// <summary>网站 URL（<c>app_url</c>，选填 string(128)）。</summary>
    [JsonPropertyName("app_url")]
    public string? AppUrl { get; set; }

    /// <summary>iOS 平台 BundleID（<c>bundle_id</c>，选填 string(128)）。</summary>
    [JsonPropertyName("bundle_id")]
    public string? BundleId { get; set; }

    /// <summary>Android 平台 PackageName（<c>package_name</c>，选填 string(128)）。</summary>
    [JsonPropertyName("package_name")]
    public string? PackageName { get; set; }
}

/// <summary>
/// 扫码支付下单（<c>scannedpos</c>）的场景信息（<c>scene_info</c>，<b>必填</b>对象）。
/// </summary>
/// <remarks>
/// <para><b>与 <see cref="PartnerSceneInfo"/> 的三处差异</b>（守卫 PT-B4）：本族 <c>scene_info</c> 整体<b>必填</b>、
/// <c>payer_client_ip</c> 反而<b>选填</b>、且 <c>store_info</c> <b>必填</b>并换成本域形态的 <see cref="PartnerScannedPosStoreInfo"/>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerScannedPosSceneInfo
{
    /// <summary>用户终端 IP（<c>payer_client_ip</c>，选填 string(45)）。</summary>
    [JsonPropertyName("payer_client_ip")]
    public string? PayerClientIp { get; set; }

    /// <summary>商户端设备号（<c>device_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

    /// <summary>商户门店信息（<c>store_info</c>，<b>必填</b>），见 <see cref="PartnerScannedPosStoreInfo"/>。</summary>
    [JsonPropertyName("store_info")]
    public PartnerScannedPosStoreInfo? StoreInfo { get; set; }
}

/// <summary>扫码支付下单的商户门店信息（<c>scene_info.store_info</c>，必填对象）：官方本族只有两个字段。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerScannedPosStoreInfo
{
    /// <summary>门店编号（<c>id</c>，选填 string(64)），商户侧门店编号。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>门店外部编号（<c>out_id</c>，选填 string(64)）。</summary>
    [JsonPropertyName("out_id")]
    public string? OutId { get; set; }
}

/// <summary>
/// 扫码支付下单的结算信息（<c>settle_info</c>，选填对象）：官方本族<b>只有</b> <c>profit_sharing</c>。
/// </summary>
/// <remarks>
/// <para><b>为什么不复用 <see cref="PartnerSettleInfo"/>：</b>本族官方字段表内没有 <c>subsidy_amount</c>，
/// 而 <c>support_fapiao</c> 被提升到<b>请求顶层</b>。复用会造出「能上送官方不认字段」的非法状态。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerScannedPosSettleInfo
{
    /// <summary>分账标识（<c>profit_sharing</c>，选填 boolean）。</summary>
    [JsonPropertyName("profit_sharing")]
    public bool? ProfitSharing { get; set; }
}
