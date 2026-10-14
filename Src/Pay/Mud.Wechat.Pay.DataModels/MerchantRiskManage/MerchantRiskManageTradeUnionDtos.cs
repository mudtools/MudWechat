// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.MerchantRiskManage;

/// <summary>
/// 上报订单关联信息（风险共建）请求（<b>POST /v3/merchant-risk-manage/report-trade-union-information</b> 的包体）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692160"/>，
/// 支持商户【从业机构（银行）】【从业机构（支付机构）】【普通服务商】【渠道商】，2026-10-14 逐字段核验。</para>
/// <para><b>四类商户标识并存</b>：<c>sp_mchid</c>（服务商）/ <c>acquiring_bank_id</c>（从业机构）/
/// <c>channel_id</c>（渠道）/ <c>sub_mchid</c>（收单商户）—— 官方按调用方身份择一承载，
/// 与 <c>PartnerTransactions</c> 的 <c>sp_mchid</c>+<c>sub_mchid</c> 两标识体系不同。</para>
/// <para><b>本域是支付线唯一把 <c>Wechatpay-Serial</c> 列成参数表行的页面</b>：
/// <c>phone</c> 与 <c>certificates_number</c>（身份证后 6 位）须以微信支付公钥（推荐）或平台证书加密，
/// 密文串照原样承载，SDK 不做加解密。</para>
/// <para><b>除 <c>extra_fields1/2/3</c> 标 string(4096) 外，官方本页几乎不印长度约束</b>（两轮复核一致）⇒
/// 各字段摘要只照抄「选填 string / 选填 integer」，不臆造上限。</para>
/// <para><b>六个整型字段官方确实未给取值字典</b>（<c>risk_level</c> / <c>phone_from</c> /
/// <c>recharge_fields.account_type</c> / <c>living_expenses_fields.type</c> / <c>high_risk_goods</c>，
/// 均已二轮定向复核）⇒ 本 SDK 一律 <c>int?</c> 透传，注释不得凭第三方 SDK 补数（守卫 MRB-B5 记录该空缺）。</para>
/// <para><b><c>same_deviced</c> 官方即此拼写</b>（多一个 d）；<c>sub_orders[].mch_id</c> 用下划线形态，
/// 与顶层 <c>*_mchid</c> 命名体系不一致 —— 两处都是"照页"红线（守卫 MRB-B3）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantRiskManage")]
public class MerchantRiskManageTradeUnionReportRequest
{
    /// <summary>服务商商户号（<c>sp_mchid</c>，选填 string）：子商户调用时可选传入对应服务商号。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>从业机构商户号（<c>acquiring_bank_id</c>，选填 string）：银行/支付机构侧标识。</summary>
    [JsonPropertyName("acquiring_bank_id")]
    public string? AcquiringBankId { get; set; }

    /// <summary>渠道号（<c>channel_id</c>，选填 string）：渠道商侧标识。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，选填 string）：本次上报订单风险数据归属的商户号。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，选填 string）：数字/大小写字母 <c>_-*</c>，同商户号下唯一。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>用户标识（<c>openid</c>，选填 string）：用户在第三方 App 关联微信、商户 AppID 下的 openid。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>用户手机号（<c>phone</c>，选填 string）：<b>须加密</b>，随附 <c>Wechatpay-Serial</c> 请求头；本 SDK 只承载密文。</summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>用户实名证件号（<c>certificates_number</c>，选填 string）：官方注明是<b>身份证后 6 位</b>，<b>须加密</b>。</summary>
    [JsonPropertyName("certificates_number")]
    public string? CertificatesNumber { get; set; }

    /// <summary>用户终端 IP（<c>client_ip</c>，选填 string）。</summary>
    [JsonPropertyName("client_ip")]
    public string? ClientIp { get; set; }

    /// <summary>商户判定的交易风险等级（<c>risk_level</c>，选填 integer）：<b>官方未给取值字典</b>，原样透传。</summary>
    [JsonPropertyName("risk_level")]
    public int? RiskLevel { get; set; }

    /// <summary>线上线下标识（<c>line_type</c>，选填 integer）：官方 1-线上交易；2-线下交易。</summary>
    [JsonPropertyName("line_type")]
    public int? LineType { get; set; }

    /// <summary>虚拟实物标识（<c>goods_type</c>，选填 integer）：官方 1-虚拟交易；2-实物交易。</summary>
    [JsonPropertyName("goods_type")]
    public int? GoodsType { get; set; }

    /// <summary>物品发布类型（<c>seller_type</c>，选填 integer）：官方 1-个人；2-商家（平台类商户发布交易物品的主体）。</summary>
    [JsonPropertyName("seller_type")]
    public int? SellerType { get; set; }

    /// <summary>是否需要物流发货（<c>is_need_deliver</c>，选填 boolean）。</summary>
    [JsonPropertyName("is_need_deliver")]
    public bool? IsNeedDeliver { get; set; }

    /// <summary>用户设备类型（<c>device_type</c>，选填 integer）：官方 1-苹果；2-安卓；3-其它。</summary>
    [JsonPropertyName("device_type")]
    public int? DeviceType { get; set; }

    /// <summary>付款方用户标识（<c>userid</c>，选填 string）：商户侧账号，账号维度唯一。</summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    /// <summary>手机号来源（<c>phone_from</c>，选填 integer）：<b>官方未给取值字典</b>，原样透传。</summary>
    [JsonPropertyName("phone_from")]
    public int? PhoneFrom { get; set; }

    /// <summary>收款方用户标识（<c>seller_userid</c>，选填 string）。</summary>
    [JsonPropertyName("seller_userid")]
    public string? SellerUserId { get; set; }

    /// <summary>业务场景（<c>scene</c>，选填 integer）：官方 0-无明确场景；1-充值；2-生活缴费。</summary>
    [JsonPropertyName("scene")]
    public int? Scene { get; set; }

    /// <summary>充值场景信息（<c>recharge_fields</c>，选填 object）：见 <see cref="MerchantRiskManageRechargeFields"/>。</summary>
    [JsonPropertyName("recharge_fields")]
    public MerchantRiskManageRechargeFields? RechargeFields { get; set; }

    /// <summary>生活缴费场景信息（<c>living_expenses_fields</c>，选填 object）：见 <see cref="MerchantRiskManageLivingExpensesFields"/>。</summary>
    [JsonPropertyName("living_expenses_fields")]
    public MerchantRiskManageLivingExpensesFields? LivingExpensesFields { get; set; }

    /// <summary>扩展字段 1（<c>extra_fields1</c>，选填 string(4096)）：官方要求 JSON 格式串。</summary>
    [JsonPropertyName("extra_fields1")]
    public string? ExtraFields1 { get; set; }

    /// <summary>扩展字段 2（<c>extra_fields2</c>，选填 string(4096)）：官方要求 JSON 格式串。</summary>
    [JsonPropertyName("extra_fields2")]
    public string? ExtraFields2 { get; set; }

    /// <summary>扩展字段 3（<c>extra_fields3</c>，选填 string(4096)）：官方要求 JSON 格式串。</summary>
    [JsonPropertyName("extra_fields3")]
    public string? ExtraFields3 { get; set; }

    /// <summary>高风险商品标签（<c>high_risk_goods</c>，选填 integer）：<b>官方未给取值字典</b>，原样透传。</summary>
    [JsonPropertyName("high_risk_goods")]
    public int? HighRiskGoods { get; set; }

    /// <summary>下单设备与支付设备是否一致（<c>same_deviced</c>，选填 boolean）：<b>官方拼写即 <c>same_deviced</c></b>，勿改。</summary>
    [JsonPropertyName("same_deviced")]
    public bool? SameDeviced { get; set; }

    /// <summary>下单设备是否常用设备（<c>reliable_device</c>，选填 boolean）。</summary>
    [JsonPropertyName("reliable_device")]
    public bool? ReliableDevice { get; set; }

    /// <summary>收货省份（<c>delivery_province</c>，选填 string）。</summary>
    [JsonPropertyName("delivery_province")]
    public string? DeliveryProvince { get; set; }

    /// <summary>收货城市（<c>delivery_city</c>，选填 string）。</summary>
    [JsonPropertyName("delivery_city")]
    public string? DeliveryCity { get; set; }

    /// <summary>收货地址是否新增（<c>new_address</c>，选填 boolean）。</summary>
    [JsonPropertyName("new_address")]
    public bool? NewAddress { get; set; }

    /// <summary>上一笔支付 openid（<c>last_pay_openid</c>，选填 string）。</summary>
    [JsonPropertyName("last_pay_openid")]
    public string? LastPayOpenId { get; set; }

    /// <summary>高风险类目（<c>risky_goods</c>，选填 integer）：官方 1-游戏点卡,2-话费充值,3-商超购物卡,4-电费,5-燃气费,6-其他卡类。</summary>
    [JsonPropertyName("risky_goods")]
    public int? RiskyGoods { get; set; }

    /// <summary>订单识别标识（<c>offline_delivery_label</c>，选填 integer）：官方 1-小时购；2-自提订单。</summary>
    [JsonPropertyName("offline_delivery_label")]
    public int? OfflineDeliveryLabel { get; set; }

    /// <summary>是否为代付订单（<c>proxy_payment</c>，选填 boolean）。</summary>
    [JsonPropertyName("proxy_payment")]
    public bool? ProxyPayment { get; set; }

    /// <summary>下单人手机号（<c>order_phone_number</c>，选填 string）：官方要求<b>脱敏前 3 后 4</b>。</summary>
    [JsonPropertyName("order_phone_number")]
    public string? OrderPhoneNumber { get; set; }

    /// <summary>下单人风险等级（<c>user_risk_level</c>，选填 integer）：官方 1-高风险；2-中高风险；3-中风险；4-低风险；5-未知；6/7-可扩展。</summary>
    [JsonPropertyName("user_risk_level")]
    public int? UserRiskLevel { get; set; }

    /// <summary>商户风险等级（<c>sp_risk_level</c>，选填 integer）：值集同 <see cref="UserRiskLevel"/>（官方两处同行内枚举）。</summary>
    [JsonPropertyName("sp_risk_level")]
    public int? SpRiskLevel { get; set; }

    /// <summary>手机号所属地（<c>phone_number_location</c>，选填 string）。</summary>
    [JsonPropertyName("phone_number_location")]
    public string? PhoneNumberLocation { get; set; }

    /// <summary>商品类目（<c>goods_category</c>，选填 string）。</summary>
    [JsonPropertyName("goods_category")]
    public string? GoodsCategory { get; set; }

    /// <summary>前置打开行为异常（<c>pre_opening_behavior</c>，选填 string）。</summary>
    [JsonPropertyName("pre_opening_behavior")]
    public string? PreOpeningBehavior { get; set; }

    /// <summary>卖家是否新账号（<c>new_store</c>，选填 boolean）。</summary>
    [JsonPropertyName("new_store")]
    public bool? NewStore { get; set; }

    /// <summary>店铺标识（<c>store_identifier</c>，选填 string）：商户侧店铺 ID。</summary>
    [JsonPropertyName("store_identifier")]
    public string? StoreIdentifier { get; set; }

    /// <summary>商品上架时间（<c>goods_listing_time</c>，选填 <b>string</b>）：本域时间字段一律字符串透传。</summary>
    [JsonPropertyName("goods_listing_time")]
    public string? GoodsListingTime { get; set; }

    /// <summary>合单商户订单号（<c>combine_out_trade_no</c>，选填 string）。</summary>
    [JsonPropertyName("combine_out_trade_no")]
    public string? CombineOutTradeNo { get; set; }

    /// <summary>合单商户号（<c>combine_mchid</c>，选填 string）：合单发起方商户号。</summary>
    [JsonPropertyName("combine_mchid")]
    public string? CombineMchId { get; set; }

    /// <summary>合单子单列表（<c>sub_orders</c>，选填 <b>array[object]</b>）：见 <see cref="MerchantRiskManageTradeUnionSubOrder"/>。</summary>
    [JsonPropertyName("sub_orders")]
    public List<MerchantRiskManageTradeUnionSubOrder>? SubOrders { get; set; }
}

/// <summary>
/// 充值场景信息（官方 <c>recharge_fields</c>）。
/// </summary>
/// <remarks><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692160"/>，2026-10-14 逐字段核验。
/// <c>account_type</c> 官方<b>未给</b>充值账号类型的取值字典（二轮定向复核确认）。</remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantRiskManage")]
public class MerchantRiskManageRechargeFields
{
    /// <summary>充值账号类型（<c>account_type</c>，选填 integer）：<b>官方未给取值字典</b>，原样透传。</summary>
    [JsonPropertyName("account_type")]
    public int? AccountType { get; set; }

    /// <summary>充值账号（<c>account</c>，选填 string）：用户实际充值的账号。</summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }
}

/// <summary>
/// 生活缴费场景信息（官方 <c>living_expenses_fields</c>）。
/// </summary>
/// <remarks><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692160"/>，2026-10-14 逐字段核验。
/// <c>type</c>（费种）官方正文只举例「水、电、燃气」，<b>未给数值映射</b>（二轮定向复核确认）⇒ 不做枚举。
/// <c>amount</c> 是本域唯一印了单位的金额字段：<b>单位为分</b>、币种人民币。</remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantRiskManage")]
public class MerchantRiskManageLivingExpensesFields
{
    /// <summary>户号（<c>acc_no</c>，选填 string）。</summary>
    [JsonPropertyName("acc_no")]
    public string? AccNo { get; set; }

    /// <summary>费种（<c>type</c>，选填 integer）：官方仅举例水/电/燃气，<b>未给数值字典</b>。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>缴费城市（<c>city</c>，选填 string）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>户号对应地址（<c>address</c>，选填 string）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>缴费户名（<c>acc_name</c>，选填 string）。</summary>
    [JsonPropertyName("acc_name")]
    public string? AccName { get; set; }

    /// <summary>机构名称（<c>company_name</c>，选填 string）。</summary>
    [JsonPropertyName("company_name")]
    public string? CompanyName { get; set; }

    /// <summary>缴费金额（<c>amount</c>，选填 integer）：官方注明<b>单位为分</b>，币种人民币。</summary>
    [JsonPropertyName("amount")]
    public int? Amount { get; set; }
}

/// <summary>
/// 合单子单信息（官方 <c>sub_orders[]</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692160"/>，2026-10-14 逐字段核验。</para>
/// <para><b><c>mch_id</c> 是带下划线的形态</b>，而同一请求的顶层用 <c>sp_mchid</c> / <c>sub_mchid</c> /
/// <c>combine_mchid</c>（无下划线）⇒ 官方自身两套命名并存，照页不得"统一"（守卫 MRB-B3）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantRiskManage")]
public class MerchantRiskManageTradeUnionSubOrder
{
    /// <summary>子单商户订单号（<c>out_trade_no</c>，选填 string）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>子单发起方商户号（<c>mch_id</c>，选填 string）：<b>官方此处即带下划线</b>，与顶层 <c>*_mchid</c> 不同形。</summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>收货省份（<c>delivery_province</c>，选填 string）。</summary>
    [JsonPropertyName("delivery_province")]
    public string? DeliveryProvince { get; set; }

    /// <summary>收货城市（<c>delivery_city</c>，选填 string）。</summary>
    [JsonPropertyName("delivery_city")]
    public string? DeliveryCity { get; set; }

    /// <summary>高风险类目（<c>risky_goods</c>，选填 integer）：值集同顶层同名字段（官方 1~6）。</summary>
    [JsonPropertyName("risky_goods")]
    public int? RiskyGoods { get; set; }

    /// <summary>下单人风险等级（<c>user_risk_level</c>，选填 integer）：值集同顶层同名字段（官方 1~7）。</summary>
    [JsonPropertyName("user_risk_level")]
    public int? UserRiskLevel { get; set; }

    /// <summary>商户风险等级（<c>sp_risk_level</c>，选填 integer）：值集同顶层同名字段（官方 1~7）。</summary>
    [JsonPropertyName("sp_risk_level")]
    public int? SpRiskLevel { get; set; }
}

/// <summary>
/// 上报订单关联信息的应答：官方回显请求的五项标识字段，<b>全部标选填且均未印长度</b>。
/// </summary>
/// <remarks>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692160"/>，2026-10-14 逐字段核验。
/// 本支<b>没有</b>状态字段 ⇒ 上报结果以 HTTP 码判定，业务失败须读 <c>code</c> / <c>message</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantRiskManage")]
public class MerchantRiskManageTradeUnionReportResponse : WechatPayResponse
{
    /// <summary>服务商商户号（<c>sp_mchid</c>，选填 string）：回显请求值。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>从业机构商户号（<c>acquiring_bank_id</c>，选填 string）：回显请求值。</summary>
    [JsonPropertyName("acquiring_bank_id")]
    public string? AcquiringBankId { get; set; }

    /// <summary>渠道号（<c>channel_id</c>，选填 string）：回显请求值。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，选填 string）：回显请求值。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，选填 string）：回显请求值。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }
}
