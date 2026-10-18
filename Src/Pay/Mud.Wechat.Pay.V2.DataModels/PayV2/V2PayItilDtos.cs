// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 交易保障上报（<c>/payitil/report</c>）请求体。
/// </summary>
/// <remarks>
/// <para><b>这是一支「只上报、不取数」的接口</b>：官方应用场景原文说明它用于把「接口调用耗时和返回信息」
/// 主动报给微信支付，用于改善网络部署与服务监控⇒ 业务上属旁路遥测，<b>不得</b>放在交易主链路的关键路径上
/// （失败可忽略，成功也没有可消费的业务字段）。</para>
/// <para><b><c>execute_time_</c> 末尾真的带下划线</b>：官方该格原文「注意：该参数最后带有下划线“_”，
/// 参数设计如此，非文档问题。」⇒ 这是<b>全 APIv2 唯一</b>带尾下划线的字段名，
/// 拼错（写成 <c>execute_time</c>）不会报参数错误而是<b>整支字段被签名器漏掉 ⇒ 恒 <c>SIGN_ERROR</c></b>。</para>
/// <para><b><c>sign_type</c> 默认 MD5</b>：官方「签名类型，目前支持HMAC-SHA256和MD5，默认为MD5」
/// （示例值写的是 <c>HMAC-SHA256</c>，但默认档写的是 MD5）⇒ 本类<b>不给</b>默认值，
/// 留空即落到签名器的 MD5 缺省档，要换算法必须显式赋值。</para>
/// <para><b><c>interface_url</c> 是「被上报那一次调用」的完整 URL</b>，不是本接口自己的地址；
/// 官方对刷卡支付另给一条专用上报 URL——
/// <c>https://api.mch.weixin.qq.com/pay/micropay/total</c>，并要求「两种接入模式都应在门店侧
/// 对一次刷卡支付做一次单独的整体上报」，其余接口仍按「调用一次、上报一次」。</para>
/// <para><b>必填面比通用形状宽</b>：官方把 <c>return_code</c>／<c>return_msg</c>／<c>result_code</c>
/// 也列为<b>入参</b>（要报的就是上一次调用的结果），且 <c>user_ip</c> 必填——
/// 与「请求只带业务参数」的直觉相反，照抄不得省略。</para>
/// <para><b>HTTP 方法官方页未写</b>：该页只有「接口地址」与「是否需要证书：不需要」两栏，
/// 全文无「请求方式」字样 ⇒ 本仓按 APIv2 全站惯例落 <c>POST</c>（该选择由契约守卫锁定，不声称出自该页）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2ReportTransactionRequest
{
    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）：微信分配（企业号 corpid 即此 appid）。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }

    /// <summary>商户号（<c>mch_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>设备号（<c>device_info</c>，选填 string(32)）：微信支付分配或商户自定义。</summary>
    [JsonPropertyName("device_info")] public string? DeviceInfo { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>，必填 string(32)）：不长于 32 位。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>，必填 string(32)）：由签名器按字段表补写，调用方留空。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }

    /// <summary>
    /// 签名类型（<c>sign_type</c>，选填 string(32)）：官方支持 <c>HMAC-SHA256</c> 与 <c>MD5</c>，
    /// <b>默认 <c>MD5</c></b> ⇒ 不给默认值，需要 HMAC-SHA256 时显式赋值。
    /// </summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }

    /// <summary>
    /// 被上报那次调用的完整接口 URL（<c>interface_url</c>，必填 string(127)）：
    /// 形如 <c>https://api.mch.weixin.qq.com/pay/unifiedorder</c>；
    /// 刷卡支付的整体上报按官方指定写 <c>https://api.mch.weixin.qq.com/pay/micropay/total</c>。
    /// </summary>
    [JsonPropertyName("interface_url")] public string? InterfaceUrl { get; set; }

    /// <summary>
    /// 接口耗时（<c>execute_time_</c>，必填 int，单位毫秒）：
    /// <b>线上传输名带尾下划线</b>（官方「参数设计如此，非文档问题」）。
    /// </summary>
    [JsonPropertyName("execute_time_")] public int? ExecuteTime { get; set; }

    /// <summary>被上报那次调用的通信标识（<c>return_code</c>，必填 string(16)）：SUCCESS/FAIL。</summary>
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    /// <summary>被上报那次调用的返回信息（<c>return_msg</c>，必填 string(128)）：FAIL 时为错误原因。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>被上报那次调用的业务结果（<c>result_code</c>，必填 string(16)）：SUCCESS/FAIL。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    /// <summary>错误代码（<c>err_code</c>，选填 string(32)）：官方列 <c>ORDERNOTEXIST</c>—订单不存在、<c>SYSTEMERROR</c>—系统错误。</summary>
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }

    /// <summary>错误代码描述（<c>err_code_des</c>，选填 string(128)）。</summary>
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，选填 string(32)）：上报时带上便于微信侧定位。</summary>
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }

    /// <summary>访问接口 IP（<c>user_ip</c>，<b>必填</b> string(16)）：发起那次调用时的机器 IP。</summary>
    [JsonPropertyName("user_ip")] public string? UserIp { get; set; }

    /// <summary>商户上报时间（<c>time</c>，选填 string(14)）：格式 <c>yyyyMMddHHmmss</c>。</summary>
    [JsonPropertyName("time")] public string? Time { get; set; }
}

/// <summary>
/// APIv2 交易保障上报应答。
/// </summary>
/// <remarks>
/// <para><b>应答面只有三行、且没有 <c>sign</c></b>：官方返回表为 <c>return_code</c>／<c>return_msg</c>
/// 加「以下字段在 return_code 为 SUCCESS 的时候有返回」段里的 <c>result_code</c>，
/// 全表无签名行 ⇒ 本支<b>不可验签</b>（收发器的应答验签在此天然放行，不是漏验）。</para>
/// <para><b>无 <c>err_code</c></b>：官方本页「错误码」章节正文就一个字「无」⇒ 失败只体现在
/// <c>return_code</c>=FAIL + <c>return_msg</c> 文本里，调用方不应指望结构化错误码。</para>
/// <para><b>上报里的结果字段可能含敏感文本</b>：<c>return_msg</c>／<c>err_code_des</c> 是商户上一次调用的
/// 原文回显，SDK 不把本应答的任何字段写日志（与 APIv2 全线「异常与日志不回显业务原文」口径一致）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2ReportTransactionResponse
{
    /// <summary>通信标识（<c>return_code</c>）：SUCCESS/FAIL。</summary>
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    /// <summary>返回信息（<c>return_msg</c>）：FAIL 时为错误原因（签名失败 / 参数格式校验错误）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>业务结果（<c>result_code</c>）：<c>return_code</c> 为 SUCCESS 时返回。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
}
