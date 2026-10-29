// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 乘车码订单查询（<c>/transit/queryorder</c>）请求体。
/// </summary>
/// <remarks>
/// <para><b>灰度中，未全量开放</b>：官方页原文「提示：本功能目前处于灰度测试阶段、暂未开放使用，
/// 待全量开放后将通过文档门户公告同步，感谢您的理解。」⇒ 本支声明为契约但<b>不保证可调</b>，
/// 权限未开通时以 <c>PAYAUTH_ERROR</c>（「商户号代扣产品权限」）返回。</para>
/// <para><b>只查代扣订单</b>：官方「该接口仅提供微信代扣订单的查询」⇒ 与基础支付的
/// <c>/pay/orderquery</c> 不通用，交易类型恒为 <c>PAP</c>。</para>
/// <para><b><c>transaction_id</c> 与 <c>out_trade_no</c> 二选一</b>：官方必填栏把两行并列为
/// 「二选一」（<c>out_trade_no</c> 那行留空），且注「微信的订单号，优先使用」⇒
/// SDK 不做参数校验，按调用方传入原样组签。</para>
/// <para><b><c>sign_type</c> 选填但默认 HMAC-SHA256</b>：官方「签名类型，默认为HMAC-SHA256」⇒
/// 本类给 <c>HMAC-SHA256</c> 默认值（默认值即算法开关，见 <c>WechatPayV2Signer</c> 的判定口径）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2TransitQueryOrderRequest
{
    /// <summary>应用 ID（<c>appid</c>，必填 string(32)）：mp/open 平台分配的公众号或移动应用 ID。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }

    /// <summary>商户号（<c>mch_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>微信订单号（<c>transaction_id</c>，与 <c>out_trade_no</c> 二选一，优先使用）。</summary>
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，与 <c>transaction_id</c> 二选一）。</summary>
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>，必填 string(32)）：不长于 32 位。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名类型（<c>sign_type</c>，选填 string(32)）：官方默认 <c>HMAC-SHA256</c>。</summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; } = "HMAC-SHA256";

    /// <summary>签名（<c>sign</c>，string(64)）：由签名器按字段表补写，调用方留空。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 乘车码订单查询应答。
/// </summary>
/// <remarks>
/// <para><b>返回条件分三段</b>：官方把返回表切成三段——<c>return_code</c> 恒有；
/// <c>return_code=SUCCESS</c> 才有 <c>result_code</c> 等第二段；
/// 「<c>return_code</c>、<c>result_code</c>、<c>trade_state</c> 都为 SUCCESS 时有返回，
/// 如 <c>trade_state</c> 不为 SUCCESS，则<b>只返回</b> <c>out_trade_no</c>」⇒
/// 第三段字段全部可空，判成功必须三值同判（<c>trade_state</c> 而非 <c>result_code</c> 才是终态）。</para>
/// <para><b><c>trade_state</c> 本页只有三值</b>：<c>SUCCESS</c>—支付成功 / <c>ACCEPT</c>—已接收，等待扣款 /
/// <c>PAY_FAIL</c>—支付失败(其他原因，如银行返回失败)。<b>本页无 <c>REFUND</c></b>
/// （服务商页有，跨页不一致照抄，见 <see cref="V2PartnerTransitQueryOrderResponse"/>）。</para>
/// <para><b>代金券字段是平铺下标键</b>：<c>coupon_type_$n</c> / <c>coupon_id_$n</c> / <c>coupon_fee_$n</c>，
/// <c>$n</c> 从 0 开始编号、数量由 <c>coupon_count</c> 给出 ⇒ 落 <see cref="IndexedFields"/>，
/// 不要当同名兄弟列表（APIv2 签名作用域就是这张扁平顶层表）。</para>
/// <para><b>官方自相矛盾处</b>：返回表 <c>bank_type</c> 示例值为 <c>CMC</c>，
/// 返回示例报文里却是 <c>CCB_DEBIT</c>；<c>openid</c> 在本页标 String(32)、服务商页标 String(128)
/// ⇒ 本类型统一按 <see cref="string"/> 宽松承接，不做长度校验。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2TransitQueryOrderResponse
{
    /// <summary>通信标识（<c>return_code</c>）：SUCCESS/FAIL，交易是否成功要看 <c>trade_state</c>。</summary>
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    /// <summary>返回信息（<c>return_msg</c>）：非空时为错误原因（签名失败 / 参数格式校验错误）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>应用 ID（<c>appid</c>，回显请求）。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }

    /// <summary>商户号（<c>mch_id</c>，回显请求）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>业务结果（<c>result_code</c>）：SUCCESS/FAIL。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    /// <summary>错误代码（<c>err_code</c>）：本页 6 个值，<c>SYSTEMERROR</c> 无下划线、其余带下划线。</summary>
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }

    /// <summary>错误代码描述（<c>err_code_des</c>）。</summary>
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }

    /// <summary>设备号（<c>device_info</c>）：微信支付分配的终端设备号。</summary>
    [JsonPropertyName("device_info")] public string? DeviceInfo { get; set; }

    /// <summary>用户标识（<c>openid</c>）：用户在商户 <c>appid</c> 下的唯一标识。</summary>
    [JsonPropertyName("openid")] public string? OpenId { get; set; }

    /// <summary>已废弃字段（<c>is_subscribe</c>）：官方「已废弃，默认统一返回N」，保留仅为照抄报文。</summary>
    [JsonPropertyName("is_subscribe")] public string? IsSubscribe { get; set; }

    /// <summary>交易类型（<c>trade_type</c>）：本页恒为 <c>PAP</c>（微信委托代扣支付）。</summary>
    [JsonPropertyName("trade_type")] public string? TradeType { get; set; }

    /// <summary>交易状态（<c>trade_state</c>）：<c>SUCCESS</c>/<c>ACCEPT</c>/<c>PAY_FAIL</c>（本页无 <c>REFUND</c>）。</summary>
    [JsonPropertyName("trade_state")] public string? TradeState { get; set; }

    /// <summary>付款银行（<c>bank_type</c>）：字符串型银行标识。</summary>
    [JsonPropertyName("bank_type")] public string? BankType { get; set; }

    /// <summary>订单总金额（<c>total_fee</c>，int）：单位为分。</summary>
    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }

    /// <summary>货币种类（<c>fee_type</c>）：ISO 4217 三位字母代码，默认 <c>CNY</c>。</summary>
    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }

    /// <summary>现金支付金额（<c>cash_fee</c>，int）。</summary>
    [JsonPropertyName("cash_fee")] public int? CashFee { get; set; }

    /// <summary>现金支付货币类型（<c>cash_fee_type</c>）：默认 <c>CNY</c>。</summary>
    [JsonPropertyName("cash_fee_type")] public string? CashFeeType { get; set; }

    /// <summary>应结订单金额（<c>settlement_total_fee</c>，int）：使用免充值优惠券后返回，= 订单金额 − 免充值券金额。</summary>
    [JsonPropertyName("settlement_total_fee")] public int? SettlementTotalFee { get; set; }

    /// <summary>代金券或立减优惠金额（<c>coupon_fee</c>，int）。</summary>
    [JsonPropertyName("coupon_fee")] public int? CouponFee { get; set; }

    /// <summary>代金券或立减优惠使用数量（<c>coupon_count</c>，int）：下标字段的条数来源。</summary>
    [JsonPropertyName("coupon_count")] public int? CouponCount { get; set; }

    /// <summary>微信支付订单号（<c>transaction_id</c>）。</summary>
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>）：<b><c>trade_state</c> 非 SUCCESS 时唯一返回的交易字段</b>。</summary>
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }

    /// <summary>附加数据（<c>attach</c>）：原样返回。</summary>
    [JsonPropertyName("attach")] public string? Attach { get; set; }

    /// <summary>支付完成时间（<c>time_end</c>）：格式 <c>yyyyMMddHHmmss</c>。</summary>
    [JsonPropertyName("time_end")] public string? TimeEnd { get; set; }

    /// <summary>交易状态描述（<c>trade_state_desc</c>）：状态说明与下一步操作指引。</summary>
    [JsonPropertyName("trade_state_desc")] public string? TradeStateDesc { get; set; }

    /// <summary>签名（<c>sign</c>，string(64)）：由收发器验签。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }

    /// <summary>签名类型（<c>sign_type</c>）：官方返回表未列，收发器按请求算法验签；宽松承接。</summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }

    /// <summary>
    /// 官方以 <c>字段名_$n</c> 平铺的代金券字段（<c>coupon_type_0</c>、<c>coupon_id_0</c>、<c>coupon_fee_0</c>…，
    /// <c>$n</c> 从 0 起、条数 = <c>coupon_count</c>）。键为官方原文拼写、值为报文原值。
    /// </summary>
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? IndexedFields { get; set; }
}

/// <summary>
/// APIv2 服务商乘车码订单查询（<c>/transit/partnerpay/queryorder</c>）请求体。
/// </summary>
/// <remarks>
/// <para><b>同样处于灰度</b>：官方页原文与商户页一致「本功能目前处于灰度测试阶段、暂未开放使用」。</para>
/// <para><b>比商户页多三个字段</b>：<c>sub_appid</c>（选填）、<c>sub_mch_id</c>（<b>必填</b>）、
/// <c>contract_id</c>（选填）与 <c>version</c>（选填，接口版本号，示例 <c>2.0</c>）；
/// 商户页的 <c>openid</c> 类型在这里是 String(128) 而非 String(32)，跨页不一致照抄。</para>
/// <para><b><c>version=2.0</c> 决定垫资形态</b>：官方注「当前支付订单如发生垫资，则会在查单结果中返回
/// 垫资相关信息（请注意对新增的信息做合适的处理，避免出现兼容问题）」，
/// 且 <c>bank_type</c> 在「发生垫资且 <c>version</c> 为 2.0」时固定返回 <c>BPA</c>，
/// 并附带 <c>user_repaid</c>⇒ 不传 <c>version</c> 就拿不到垫资信息。</para>
/// <para><b><c>transaction_id</c> / <c>out_trade_no</c> 二选一</b>，与商户页同形；
/// 请求示例用的是 <c>contract_id</c> + <c>transaction_id</c> + <c>version</c> 组合。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PartnerTransitQueryOrderRequest
{
    /// <summary>服务商应用 ID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }

    /// <summary>子商户公众账号 ID（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }

    /// <summary>服务商商户号（<c>mch_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>子商户号（<c>sub_mch_id</c>，<b>必填</b> string(32)）：微信支付分配。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>微信订单号（<c>transaction_id</c>，与 <c>out_trade_no</c> 二选一，优先使用）。</summary>
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，与 <c>transaction_id</c> 二选一）。</summary>
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }

    /// <summary>委托代扣协议 ID（<c>contract_id</c>，选填 string(32)）：签约成功后微信返回。</summary>
    [JsonPropertyName("contract_id")] public string? ContractId { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>，必填 string(32)）：不长于 32 位。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名类型（<c>sign_type</c>，选填 string(32)）：官方默认 <c>HMAC-SHA256</c>。</summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; } = "HMAC-SHA256";

    /// <summary>签名（<c>sign</c>，string(64)）：由签名器按字段表补写，调用方留空。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }

    /// <summary>接口版本号（<c>version</c>，选填 string(32)，示例 <c>2.0</c>）：官方表中「说明」栏为空白，
    /// 作用由垫资注记给出（传 <c>2.0</c> 才可能拿到 <c>BPA</c> 与 <c>user_repaid</c>）。</summary>
    [JsonPropertyName("version")] public string? Version { get; set; }
}

/// <summary>
/// APIv2 服务商乘车码订单查询应答。
/// </summary>
/// <remarks>
/// <para><b>字段面是商户页应答的严格超集</b>：多 <c>sub_appid</c>／<c>sub_mch_id</c>／
/// <c>sub_openid</c>／<c>user_repaid</c>／<c>contract_id</c> 五支，其余同名同义 ⇒ 两类型各自成形、不共用。</para>
/// <para><b><c>trade_state</c> 比商户页多一档 <c>REFUND</c></b>：本页四值
/// <c>SUCCESS</c>/<c>ACCEPT</c>/<c>PAY_FAIL</c>/<c>REFUND</c>，商户页三值。跨页不一致照抄，
/// 调用方按本页四值处理即可（商户页不会返回 <c>REFUND</c>）。</para>
/// <para><b>垫资两字段的前置条件</b>：官方对 <c>user_repaid</c> 原文「注意：使用此字段前需先确认
/// <c>bank_type</c> 字段值为 <c>BPA</c> 以及 <c>trade_state</c> 字段值为 <c>SUCCESS</c>」⇒
/// <c>user_repaid</c> 只在请求带 <c>version=2.0</c> 且订单垫资时出现，属<b>仅返回字段</b>。</para>
/// <para><b>错误码文案更细</b>：<c>PAYAUTH_ERROR</c> 在本页列三个待检查权限
/// （服务商商户号受理权限 / 子商户号代扣产品权限 / 子商户号授予父商户号代扣权限），商户页只有一句。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PartnerTransitQueryOrderResponse
{
    /// <summary>通信标识（<c>return_code</c>）：SUCCESS/FAIL，交易是否成功要看 <c>trade_state</c>。</summary>
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    /// <summary>返回信息（<c>return_msg</c>）：非空时为错误原因（签名失败 / 参数格式校验错误）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>服务商应用 ID（<c>appid</c>，回显请求）。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }

    /// <summary>子商户公众账号 ID（<c>sub_appid</c>，选返回）。</summary>
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }

    /// <summary>服务商商户号（<c>mch_id</c>，回显请求）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>子商户号（<c>sub_mch_id</c>，回显请求）。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>业务结果（<c>result_code</c>）：SUCCESS/FAIL。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    /// <summary>错误代码（<c>err_code</c>）：<c>SYSTEMERROR</c> 无下划线，其余 5 支带下划线。</summary>
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }

    /// <summary>错误代码描述（<c>err_code_des</c>）。</summary>
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }

    /// <summary>设备号（<c>device_info</c>）：微信支付分配的终端设备号。</summary>
    [JsonPropertyName("device_info")] public string? DeviceInfo { get; set; }

    /// <summary>用户标识（<c>openid</c>）：官方本页标 String(128)（商户页标 String(32)）。</summary>
    [JsonPropertyName("openid")] public string? OpenId { get; set; }

    /// <summary>用户子标识（<c>sub_openid</c>，选返回）：<c>sub_appid</c> 下用户的唯一标识。</summary>
    [JsonPropertyName("sub_openid")] public string? SubOpenId { get; set; }

    /// <summary>已废弃字段（<c>is_subscribe</c>）：官方「已废弃，默认统一返回N」。</summary>
    [JsonPropertyName("is_subscribe")] public string? IsSubscribe { get; set; }

    /// <summary>交易类型（<c>trade_type</c>）：恒为 <c>PAP</c>（微信委托代扣支付）。</summary>
    [JsonPropertyName("trade_type")] public string? TradeType { get; set; }

    /// <summary>
    /// 交易状态（<c>trade_state</c>）：本页 <c>SUCCESS</c>/<c>ACCEPT</c>/<c>PAY_FAIL</c>/<b><c>REFUND</c></b> 四值。
    /// </summary>
    [JsonPropertyName("trade_state")] public string? TradeState { get; set; }

    /// <summary>
    /// 付款银行（<c>bank_type</c>）：银行标识；<b>订单发生垫资且请求 <c>version</c> 为 <c>2.0</c> 时固定返回 <c>BPA</c></b>。
    /// </summary>
    [JsonPropertyName("bank_type")] public string? BankType { get; set; }

    /// <summary>用户是否已还款（<c>user_repaid</c>）：<c>Y</c>-已还款 / <c>N</c>-未还款；须先确认 <c>bank_type=BPA</c> 且 <c>trade_state=SUCCESS</c>。</summary>
    [JsonPropertyName("user_repaid")] public string? UserRepaid { get; set; }

    /// <summary>订单总金额（<c>total_fee</c>，int）：单位为分。</summary>
    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }

    /// <summary>货币种类（<c>fee_type</c>）：ISO 4217 三位字母代码，默认 <c>CNY</c>。</summary>
    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }

    /// <summary>现金支付金额（<c>cash_fee</c>，int）。</summary>
    [JsonPropertyName("cash_fee")] public int? CashFee { get; set; }

    /// <summary>现金支付货币类型（<c>cash_fee_type</c>）：默认 <c>CNY</c>。</summary>
    [JsonPropertyName("cash_fee_type")] public string? CashFeeType { get; set; }

    /// <summary>应结订单金额（<c>settlement_total_fee</c>，int）：使用免充值优惠券后返回。</summary>
    [JsonPropertyName("settlement_total_fee")] public int? SettlementTotalFee { get; set; }

    /// <summary>代金券或立减优惠金额（<c>coupon_fee</c>，int）。</summary>
    [JsonPropertyName("coupon_fee")] public int? CouponFee { get; set; }

    /// <summary>代金券或立减优惠使用数量（<c>coupon_count</c>，int）。</summary>
    [JsonPropertyName("coupon_count")] public int? CouponCount { get; set; }

    /// <summary>微信支付订单号（<c>transaction_id</c>）。</summary>
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>）：<b><c>trade_state</c> 非 SUCCESS 时唯一返回的交易字段</b>。</summary>
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }

    /// <summary>附加数据（<c>attach</c>）：原样返回。</summary>
    [JsonPropertyName("attach")] public string? Attach { get; set; }

    /// <summary>支付完成时间（<c>time_end</c>）：格式 <c>yyyyMMddHHmmss</c>。</summary>
    [JsonPropertyName("time_end")] public string? TimeEnd { get; set; }

    /// <summary>交易状态描述（<c>trade_state_desc</c>）。</summary>
    [JsonPropertyName("trade_state_desc")] public string? TradeStateDesc { get; set; }

    /// <summary>委托代扣协议 ID（<c>contract_id</c>，选返回）：签约成功后微信返回。</summary>
    [JsonPropertyName("contract_id")] public string? ContractId { get; set; }

    /// <summary>签名（<c>sign</c>，string(64)）：由收发器验签。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }

    /// <summary>签名类型（<c>sign_type</c>）：官方返回表未列，宽松承接。</summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }

    /// <summary>
    /// 官方以 <c>字段名_$n</c> 平铺的代金券字段（<c>coupon_type_$0</c>、<c>coupon_id_$n</c>、<c>coupon_fee_$n</c>，
    /// <c>$n</c> 从 0 起、条数 = <c>coupon_count</c>；本页 <c>coupon_type</c> 的类型栏只写 <c>String</c>，未给长度）。
    /// </summary>
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? IndexedFields { get; set; }
}
