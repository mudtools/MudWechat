// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 按日期查询自动提现（<c>/fund/queryautowithdrawbydate</c>）请求体。
/// </summary>
/// <remarks>
/// <para><b>本支是「服务商侧小微商户」接口</b>：<c>sub_mch_id</c> 与 <c>mch_id</c> <b>同为必填</b>，
/// 且请求面<b>没有</b> <c>appid</c> 与 <c>sub_appid</c> ⇒ 照抄不得补。</para>
/// <para><b><c>sign_type</c> 必填且仅 HMAC-SHA256、并参与 <c>sign</c></b>：官方原文
/// 「签名类型，目前支持HMAC-SHA256。该字段需参与签名sign的计算」⇒ 默认值钉为
/// <c>HMAC-SHA256</c>（该字段既是算法开关又是签名入参，清空即换算法、必 <c>SIGNERROR</c>）。</para>
/// <para><b>日期口径反直觉</b>：格式 <c>YYYYMMDD</c>，且<b>提现单日期比交易日晚一天</b>
/// （官方：提现单在交易次日汇总前一天的结算结果后生成 ⇒ <c>20180602</c> 的单对应 <c>20180601</c> 的交易）；
/// 窗口<b>仅最近 30 天</b>。调用方按「查某天的钱」直接传那个交易日会拿错单。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2AutoWithdrawQueryRequest
{
    /// <summary>微信支付分配的商户号（服务商／主体商户）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>小微商户号（官方必填，本支面向小微商户的自动提现）。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>随机字符串，不长于 32 位。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名类型：官方仅支持 <c>HMAC-SHA256</c> 且该字段参与签名计算 ⇒ 默认值即契约面，勿清空。</summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; } = "HMAC-SHA256";

    /// <summary>
    /// 提现单日期，格式 <c>YYYYMMDD</c>；仅支持最近 <b>30</b> 天。
    /// 注意提现单日期对应<b>前一天</b>的交易汇总。
    /// </summary>
    [JsonPropertyName("date")] public string? Date { get; set; }

    /// <summary>签名（由证书收发器按字段表计算，调用方无需赋值）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 按日期查询自动提现应答 —— <b>单个平面对象，不是列表</b>。
/// </summary>
/// <remarks>
/// <para><b>形态判定依据是官方示例</b>：成功报文里每个节点各出现一次，没有重复子节点、没有下标形态、
/// 也没有整段 JSON 字符串 ⇒ 一天最多一条提现单，本类按平面对象建模（若误按列表建模，
/// 反序列化会静默丢字段）。</para>
/// <para><b>「有没有单」看 <c>withdraw_status</c>，不看 <c>withdraw_id</c></b>：
/// <c>SUPPORT_WITHDRAW</c>／<c>NOT_SUPPORT_WITHDRAW</c>／<c>NO_WITHDRAW_AUTH</c> 三态下
/// 当日<b>不存在</b>提现单，<c>withdraw_id</c> 与金额字段全部缺省；
/// 只有 <c>PROCESSING</c>／<c>SUCCESS</c>／<c>REFUNDED</c>／<c>SUPPORT_RE_WITHDRAW</c> 才带单据。</para>
/// <para><b>官方示例的自相矛盾之处（照抄留痕，不做「修正」）</b>：
/// ① 成功示例里 <c>err_code</c> 回显的是 <c>SUCCESS</c> 而不是缺省；
/// ② 示例里出现 <c>err_msg</c> 节点，而返回表写的是 <c>err_code_des</c> ⇒ 两个字段都建模、
/// 解析保持宽松，不把任一处判成格式错误。</para>
/// <para><b>错误码是下划线形态</b>：<c>SYSTEM_ERROR</c>（返回表的示例值却写 <c>SYSTEMERROR</c>，
/// 以错误码表为准）、<c>CA_ERROR</c>、<c>FREQUENCY_LIMITED</c>、<c>NO_AUTH</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2AutoWithdrawQueryResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>业务结果：SUCCESS / FAIL。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    /// <summary>错误代码（官方成功示例亦回显 <c>SUCCESS</c>）。</summary>
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }

    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }

    /// <summary>
    /// 官方<b>成功示例</b>里出现的描述节点（返回表却写作 <c>err_code_des</c>）⇒ 两者并存、宽松解析。
    /// </summary>
    [JsonPropertyName("err_msg")] public string? ErrMsg { get; set; }

    /// <summary>签名（官方必填栏为「否」：缺失时收发器放行，存在时按双试口径验签）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }

    /// <summary>签名类型（回显，官方必填栏为「否」）。</summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }

    /// <summary>提现单日期，格式 <c>YYYYMMDD</c>。</summary>
    [JsonPropertyName("date")] public string? Date { get; set; }

    /// <summary>微信支付分配的商户号（回显）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>小微商户号（回显）。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>随机字符串（与请求一致）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>
    /// 单据状态：
    /// <c>PROCESSING</c> 处理中 / <c>SUCCESS</c> 成功 / <c>REFUNDED</c> 银行处理失败退票 /
    /// <c>SUPPORT_RE_WITHDRAW</c> 可重新发起提现 /
    /// <c>SUPPORT_WITHDRAW</c> 当日无单且净交易额大于 0，可发起该日自动提现 /
    /// <c>NOT_SUPPORT_WITHDRAW</c> 超过 30 天，或当日无单且净交易额不大于 0 /
    /// <c>NO_WITHDRAW_AUTH</c> 商户无提现权限。
    /// </summary>
    [JsonPropertyName("withdraw_status")] public string? WithdrawStatus { get; set; }

    /// <summary>单据失败原因。</summary>
    [JsonPropertyName("fail_reason")] public string? FailReason { get; set; }

    /// <summary>提现单单据 Id；该日存在提现单据时才有值。</summary>
    [JsonPropertyName("withdraw_id")] public string? WithdrawId { get; set; }

    /// <summary>提现金额，单位<b>分</b>（RMB）。</summary>
    [JsonPropertyName("amount")] public int? Amount { get; set; }

    /// <summary>单据创建时间，格式 <c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("create_time")] public string? CreateTime { get; set; }

    /// <summary>单据成功时间，格式 <c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("success_time")] public string? SuccessTime { get; set; }

    /// <summary>单据退票时间，格式 <c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("refund_time")] public string? RefundTime { get; set; }
}
