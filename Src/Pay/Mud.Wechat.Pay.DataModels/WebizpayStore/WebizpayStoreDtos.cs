// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.WebizpayStore;

/// <summary>发起门店主体匹配请求·【主体匹配明细列表】 主体匹配明细列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507136"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayStore")]
public class WebizpayStoreEntityMatchRequestMatchDetailsItem
{
    /// <summary>【门店统一社会信用代码】 门店统一社会信用代码（官方字段路径 <c>match_details.organization_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("organization_code")]
    public string? OrganizationCode { get; set; }

    /// <summary>【门店名称】 门店名称,全称表述（官方字段路径 <c>match_details.store_name</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("store_name")]
    public string? StoreName { get; set; }

    /// <summary>【需要查询的支付产品】 需要查询的支付产品，目前仅支持 CODEPAY。可选取值：`CODE_PAY` 付款码 / `NONE` 不查询（官方字段路径 <c>match_details.product_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("product_type")]
    public string? ProductType { get; set; }

}

/// <summary>发起门店主体匹配请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507136"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayStore")]
public class WebizpayStoreEntityMatchRequest
{
    /// <summary>【服务商商户号】 是由微信支付系统生成并分配给每个服务商的唯一标识符，具体请参考 服务商模式开发必要参数说明（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【商户主体匹配批次单号】 商户主体匹配批次单号，要求6-32个字符内，只能是数字、大小写字母_-&amp;#124;\* 且在同一个商户号下唯一（官方字段路径 <c>out_batch_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_batch_id")]
    public string? OutBatchId { get; set; }

    /// <summary>【主体匹配明细列表】 主体匹配明细列表（官方字段路径 <c>match_details</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("match_details")]
    public List<WebizpayStoreEntityMatchRequestMatchDetailsItem>? MatchDetails { get; set; }

}

/// <summary>发起门店主体匹配响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507136"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayStore")]
public class WebizpayStoreEntityMatchResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 服务商商户号，商户请求时传入（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【商户主体匹配批次单号】 商户主体匹配批次单号，由商户发起门店信息匹配时传入（官方字段路径 <c>out_batch_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_batch_id")]
    public string? OutBatchId { get; set; }

    /// <summary>【微信支付主体匹配批次单号】 微信支付主体匹配批次单号，标记某一次响应商户主体匹配信息对应的处理单号，在微信支付体系下唯一（官方字段路径 <c>batch_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("batch_id")]
    public string? BatchId { get; set; }

    /// <summary>【批次创建时间】 批次创建时间，微信支付处理完成此批次的时间。 遵循 rfc3339 标准格式： yyyy-MM-DDTHH:mm:ss+TIMEZONE。 yyyy-MM-DD 表示年月日； T 字符用于分隔日期和时间部分； HH:mm:ss 表示具体的时分秒； TIMEZONE 表示时区（例如， +08:00 对应东八区时间，即北京时间）。 示例： 2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29分35秒（官方字段路径 <c>time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("time")]
    public string? Time { get; set; }

    /// <summary>【批次状态】 批次状态：已受理ACCEPTED、处理中IN_PROGRESS、已完成COMPLETED、已关闭CLOSED。可选取值：`ACCEPTED` 已受理 / `IN_PROGRESS` 处理中 / `COMPLETED` 已完成 / `CLOSED` 已关闭（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

}

/// <summary>查询门店主体匹配结果响应·【主体匹配明细列表】 主体匹配明细列表，若结果为空，列表长度为0。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507133"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayStore")]
public class WebizpayStoreEntityMatchQueryResponseMatchResultsItem
{
    /// <summary>【门店统一社会信用代码】 门店统一社会信用代码（官方字段路径 <c>match_results.organization_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("organization_code")]
    public string? OrganizationCode { get; set; }

    /// <summary>【门店名称】 门店名称，完整全称表述（官方字段路径 <c>match_results.store_name</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("store_name")]
    public string? StoreName { get; set; }

    /// <summary>【是否存在满足条件微信支付商户号】 是否存在满足条件微信支付商户号。满足条件：1\. 门店统一社会信用代码存在任意商户主体；2. 商户主体所关联商户号中，存在任意一个未被不收管控的商户号；3. 若商户主体所关联商户号中，存在任意一个商户号开通了「付款码支付」或已授权「服务商付款码支付」（官方字段路径 <c>match_results.has_valid_mchid</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("has_valid_mchid")]
    public bool? HasValidMchid { get; set; }

}

/// <summary>查询门店主体匹配结果响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507133"/></summary>
[HttpJsonSerializable(SerializerClassName = "WebizpayStore")]
public class WebizpayStoreEntityMatchQueryResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 服务商商户号，从商户发起匹配门店主体匹配时传入（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【商户主体匹配批次单号】 商户主体匹配批次单号，从商户发起匹配门店主体匹配时传入（官方字段路径 <c>out_batch_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_batch_id")]
    public string? OutBatchId { get; set; }

    /// <summary>【微信支付主体匹配批次单号】 微信支付主体匹配批次单号，标记某一次响应商户主体匹配信息对应的处理单号，在微信支付体系下唯一（官方字段路径 <c>batch_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("batch_id")]
    public string? BatchId { get; set; }

    /// <summary>【批次状态】 批次状态：已受理ACCEPTED、处理中IN_PROGRESS、已完成COMPLETED、已关闭CLOSED。可选取值：`ACCEPTED` 已受理 / `IN_PROGRESS` 处理中 / `COMPLETED` 已完成 / `CLOSED` 已关闭（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【主体匹配明细列表】 主体匹配明细列表，若结果为空，列表长度为0（官方字段路径 <c>match_results</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("match_results")]
    public List<WebizpayStoreEntityMatchQueryResponseMatchResultsItem>? MatchResults { get; set; }

}
