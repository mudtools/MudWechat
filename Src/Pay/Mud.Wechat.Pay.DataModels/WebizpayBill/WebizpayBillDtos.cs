// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.WebizpayBill;

/// <summary>申请企业商户企业支付出资凭证请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507157"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayBill")]
public class WebizpayBillProofRequest
{
    /// <summary>【服务商商户号】 是由微信支付系统生成并分配给每个服务商的唯一标识符，具体请参考 服务商模式开发必要参数说明（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【出资子商户号】 由服务商为子商户进件后获取，具体请参考 服务商模式开发必要参数说明（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【起始日期】 起始日期 格式yyyy-MM-DD，仅支持三个月内的账单下载申请（官方字段路径 <c>start_date</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("start_date")]
    public string? StartDate { get; set; }

    /// <summary>【结束日期】 结束日期 格式yyyy-MM-DD，仅支持三个月内的账单下载申请（官方字段路径 <c>end_date</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_date")]
    public string? EndDate { get; set; }

    /// <summary>【商户主体名称】 商户主体名称，传入指定商户主体名称时，则仅下载该主体下所有收单商户的企业支付订单，方便企业找对应收单商户开具发票（官方字段路径 <c>entity_name</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("entity_name")]
    public string? EntityName { get; set; }

    /// <summary>【商户操作流水号】 商户操作流水号，要求6-32个字符内，只能是数字、大小写字母_-&amp;#124;\* 且在同一个商户号下唯一（官方字段路径 <c>out_operation_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_operation_id")]
    public string? OutOperationId { get; set; }

}

/// <summary>申请企业商户企业支付出资凭证响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507157"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayBill")]
public class WebizpayBillProofResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 服务商商户号（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【商户操作流水号】 商户操作流水号，由商户发起本次请求时传入（官方字段路径 <c>out_operation_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_operation_id")]
    public string? OutOperationId { get; set; }

    /// <summary>【微信支付操作流水号】 微信支付操作流水号，微信支付系统唯一标识一次响应，在微信支付下唯一（官方字段路径 <c>operation_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("operation_id")]
    public string? OperationId { get; set; }

    /// <summary>【出资子商户号】 出资子商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【起始日期 】 起始日期 格式yyyy-MM-DD（官方字段路径 <c>start_date</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("start_date")]
    public string? StartDate { get; set; }

    /// <summary>【结束日期】 结束日期 格式yyyy-MM-DD（官方字段路径 <c>end_date</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("end_date")]
    public string? EndDate { get; set; }

    /// <summary>【商户主体名称】 商户主体名称（官方字段路径 <c>entity_name</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("entity_name")]
    public string? EntityName { get; set; }

    /// <summary>【出资凭证申请单单号】 出资凭证申请单单号（官方字段路径 <c>proof_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("proof_id")]
    public string? ProofId { get; set; }

    /// <summary>【凭证下载地址】 凭证下载地址，地址5分钟内有效（官方字段路径 <c>download_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }

    /// <summary>【凭证申请创建时间】 凭证申请创建时间，表示商户发起凭证申请的时间。 遵循 rfc3339 标准格式： yyyy-MM-DDTHH:mm:ss+TIMEZONE。 yyyy-MM-DD 表示年月日； T 字符用于分隔日期和时间部分； HH:mm:ss 表示具体的时分秒； TIMEZONE 表示时区（例如， +08:00 对应东八区时间，即北京时间）。 示例： 2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29分3…（官方字段路径 <c>created_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("created_time")]
    public string? CreatedTime { get; set; }

    /// <summary>【凭证状态更新时间】 凭证状态更新时间，表示微信支付处理凭证时最后更新时间。 遵循 rfc3339 标准格式： yyyy-MM-DDTHH:mm:ss+TIMEZONE。 yyyy-MM-DD 表示年月日； T 字符用于分隔日期和时间部分； HH:mm:ss 表示具体的时分秒； TIMEZONE 表示时区（例如， +08:00 对应东八区时间，即北京时间）。 示例： 2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点…（官方字段路径 <c>updated_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("updated_time")]
    public string? UpdatedTime { get; set; }

}

/// <summary>申请企业商户企业支付业务账单请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507142"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayBill")]
public class WebizpayBillTradeBillRequest
{
    /// <summary>【服务商商户号】 是由微信支付系统生成并分配给每个服务商的唯一标识符，具体请参考 服务商模式开发必要参数说明（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【出资子商户号】 由服务商为子商户进件后获取，具体请参考 服务商模式开发必要参数说明（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【账单日期】 账单日期，仅支持申请三个月内业务账单，格式yyyy-MM-DD。 仅支持按一天的纬度查询（官方字段路径 <c>date</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>【账单类型】 账单类型，当前默认只有QYZFALL。可选取值：`QYZFALL` 企业支付（官方字段路径 <c>type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

}

/// <summary>申请企业商户企业支付业务账单响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507142"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayBill")]
public class WebizpayBillTradeBillResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 服务商商户号，由商户请求时传入（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【出资子商户号】 出资子商户号，表示此出资商户的账单，又商户请求时传入（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【账单日期】 账单日期 格式yyyy-MM-DD，表示当日的账单（官方字段路径 <c>date</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>【账单类型】 账单类型，商户请求时传入。可选取值：`QYZFALL` 企业支付（官方字段路径 <c>type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【账单下载地址】 供下一步请求账单文件的下载地址，该地址5min内有效。参考 下载账单（官方字段路径 <c>download_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }

    /// <summary>【哈希类型】 哈希类型，固定为SHA1（官方字段路径 <c>hash_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("hash_type")]
    public string? HashType { get; set; }

    /// <summary>【哈希值】 账单文件的SHA1摘要值，用于服务商侧校验文件的一致性（官方字段路径 <c>hash_value</c>，<b>必填</b>，类型 <c>string(2048)</c>）。</summary>
    [JsonPropertyName("hash_value")]
    public string? HashValue { get; set; }

}
