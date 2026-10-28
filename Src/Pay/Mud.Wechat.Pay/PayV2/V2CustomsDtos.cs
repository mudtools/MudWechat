// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 订单附加信息提交（报关申报）请求体。
/// </summary>
/// <remarks>
/// <para><b>本族报文没有 <c>nonce_str</c></b>（官方三支报关页的请求与应答面均无该字段），
/// 且<b>本页</b>请求侧连 <c>sign_type</c> 也没有（查询 / 重推两页的 <c>sign_type</c> 是选填）⇒
/// 申报请求不填 <c>sign_type</c> 时由传输层按缺省 MD5 计算，填了反而与官方表不符。</para>
/// <para><b>个人信息</b>：<see cref="CertId"/> / <see cref="Name"/> 是订购人实名信息，
/// SDK 侧不落日志、不入异常消息（PAY-B7 同线口径）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2CustomsDeclareRequest
{
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>商户订单号（必填）：与 <see cref="TransactionId"/> 一同定位支付单。</summary>
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }

    /// <summary>微信支付订单号（必填，String(28)）。</summary>
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }

    /// <summary>海关：官方请求侧枚举为 GUANGZHOU_ZS / HANGZHOU_ZS / NINGBO / ZHENGZHOU_BS / CHONGQING / SHANGHAI_ZS / SHENZHEN / ZHENGZHOU_ZH_ZS / TIANJIN / ZONGSHU。</summary>
    [JsonPropertyName("customs")] public string? Customs { get; set; }

    /// <summary>商户海关备案号：customs 非 NO 时必填（一般 6 位）。</summary>
    [JsonPropertyName("mch_customs_no")] public string? MchCustomsNo { get; set; }

    /// <summary>关税（分）：非必填，且<b>不会提交给海关</b>。</summary>
    [JsonPropertyName("duty")] public int? Duty { get; set; }

    /// <summary>报关类型：不传即 ADD（新增报关申请），另支持 MODIFY（修改报关信息）。</summary>
    [JsonPropertyName("action_type")] public string? ActionType { get; set; }

    /// <summary>商户子订单号：有拆单则必传（官方据此判定为拆单申报）。</summary>
    [JsonPropertyName("sub_order_no")] public string? SubOrderNo { get; set; }

    /// <summary>币种：有拆单则必传，官方暂只支持 CNY。</summary>
    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }

    /// <summary>子订单应付金额（分）：有拆单则必传，须满足 order_fee = transport_fee + product_fee。</summary>
    [JsonPropertyName("order_fee")] public int? OrderFee { get; set; }

    [JsonPropertyName("transport_fee")] public int? TransportFee { get; set; }
    [JsonPropertyName("product_fee")] public int? ProductFee { get; set; }

    /// <summary>证件类型：官方暂只支持固定值 IDCARD（大陆身份证）。</summary>
    [JsonPropertyName("cert_type")] public string? CertType { get; set; }

    /// <summary>证件号码：尾号字母 X 须大写。<b>属个人身份信息，不得进日志与异常消息。</b></summary>
    [JsonPropertyName("cert_id")] public string? CertId { get; set; }

    /// <summary>订购人姓名。<b>属个人身份信息，不得进日志与异常消息。</b></summary>
    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 订单附加信息提交（报关申报）应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2CustomsDeclareResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>签名类型：官方本页应答侧标注「暂只支持 MD5」，且<b>必填</b>（请求侧反而没有该字段）。</summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }

    [JsonPropertyName("sign")] public string? Sign { get; set; }
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }

    /// <summary>申报状态：UNDECLARED / SUBMITTED / PROCESSING / SUCCESS / FAIL / EXCEPT。</summary>
    [JsonPropertyName("state")] public string? State { get; set; }

    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    [JsonPropertyName("sub_order_no")] public string? SubOrderNo { get; set; }
    [JsonPropertyName("sub_order_id")] public string? SubOrderId { get; set; }
    [JsonPropertyName("modify_time")] public string? ModifyTime { get; set; }

    /// <summary>订购人与支付人身份校验结果：UNCHECKED / SAME / DIFFERENT。</summary>
    [JsonPropertyName("cert_check_result")] public string? CertCheckResult { get; set; }

    /// <summary>验核机构：UNIONPAY / NETSUNION / OTHERS（官方要求映射至海关 verDept 的 1/2/3）。</summary>
    [JsonPropertyName("verify_department")] public string? VerifyDepartment { get; set; }

    [JsonPropertyName("verify_department_trade_id")] public string? VerifyDepartmentTradeId { get; set; }
}

/// <summary>
/// APIv2 订单附加信息查询（报关查询）请求体。
/// </summary>
/// <remarks>四个订单标识<b>四选一</b>，官方给定的优先级为 sub_order_id &gt; sub_order_no &gt; transaction_id &gt; out_trade_no。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2CustomsQueryRequest
{
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }
    [JsonPropertyName("sub_order_no")] public string? SubOrderNo { get; set; }
    [JsonPropertyName("sub_order_id")] public string? SubOrderId { get; set; }

    /// <summary>海关（必填）：取值枚举同申报接口。</summary>
    [JsonPropertyName("customs")] public string? Customs { get; set; }

    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 订单附加信息查询（报关查询）应答。
/// </summary>
/// <remarks>
/// <para><b>官方应答是「顶层固定字段 + <c>count</c> 条下标记录」</b>：记录字段以 <c>sub_order_no_0</c>、
/// <c>state_0</c>、<c>order_fee_1</c> … 形态平铺在 <c>&lt;xml&gt;</c> 根下（<c>$n</c> 取 0 ~ count-1），
/// 而 <c>verify_department</c> / <c>verify_department_trade_id</c> 是<b>唯一两个不带下标</b>的记录字段。</para>
/// <para>APIv2 的签名作用域是「根元素顶层参数」，故传输层把整张扁平字段表交回本类型：
/// 固定字段落在对应属性上，下标字段落在 <see cref="IndexedFields"/>。
/// <b>不要</b>把它们当成同名兄弟列表 —— 官方就是平铺键。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2CustomsQueryResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }

    /// <summary>记录笔数：下标字段的可读上界（官方 PS：序号 0 ~ count-1）。</summary>
    [JsonPropertyName("count")] public int? Count { get; set; }

    /// <summary>验核机构（<b>不带下标</b>，与其余记录字段形态不同）。</summary>
    [JsonPropertyName("verify_department")] public string? VerifyDepartment { get; set; }

    /// <summary>验核机构交易流水号（<b>不带下标</b>）。</summary>
    [JsonPropertyName("verify_department_trade_id")] public string? VerifyDepartmentTradeId { get; set; }

    /// <summary>
    /// 官方以 <c>字段名_$n</c> 平铺的记录字段（<c>state_0</c>、<c>customs_0</c>、<c>order_fee_0</c>…）。
    /// 键为官方原文拼写、值为报文原值（数值字段以字符串形态出现，官方无类型信息）。
    /// </summary>
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? IndexedFields { get; set; }
}

/// <summary>
/// APIv2 订单附加信息重推（重新申报）请求体。
/// </summary>
/// <remarks>路径段是 <c>/cgi-bin/mch/<b>newcustoms</b>/…</c>（另两支为 <c>mch/customs</c>），照抄官方不得「顺手统一」。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2CustomsRedeclareRequest
{
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>商户订单号：与 <see cref="TransactionId"/> 二选一。</summary>
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }

    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }

    /// <summary>商户子订单号：无子订单可不填；一旦涉及子订单，与 <see cref="SubOrderId"/> 至少传一个。</summary>
    [JsonPropertyName("sub_order_no")] public string? SubOrderNo { get; set; }

    [JsonPropertyName("sub_order_id")] public string? SubOrderId { get; set; }
    [JsonPropertyName("customs")] public string? Customs { get; set; }
    [JsonPropertyName("mch_customs_no")] public string? MchCustomsNo { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 订单附加信息重推（重新申报）应答。
/// </summary>
/// <remarks>本支应答是扁平单记录形态（<b>无</b> <c>cert_check_result</c> 与 <c>verify_department*</c>）。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2CustomsRedeclareResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
    [JsonPropertyName("state")] public string? State { get; set; }
    [JsonPropertyName("transaction_id")] public string? TransactionId { get; set; }
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    [JsonPropertyName("sub_order_no")] public string? SubOrderNo { get; set; }
    [JsonPropertyName("sub_order_id")] public string? SubOrderId { get; set; }
    [JsonPropertyName("modify_time")] public string? ModifyTime { get; set; }

    /// <summary>申报结果说明：状态为失败/异常时给出原因。</summary>
    [JsonPropertyName("explanation")] public string? Explanation { get; set; }
}
