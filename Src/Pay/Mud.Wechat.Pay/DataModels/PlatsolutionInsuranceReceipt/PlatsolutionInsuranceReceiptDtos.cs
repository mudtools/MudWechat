// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.PlatsolutionInsuranceReceipt;

/// <summary>受理商家转账批次汇总电子回单请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504226"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionInsuranceReceipt")]
public class InsuranceSummaryReceiptApplyRequest
{
    /// <summary>【二级商户号】 二级商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>受理商家转账批次汇总电子回单响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504226"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionInsuranceReceipt")]
public class InsuranceSummaryReceiptApplyResponse : WechatPayResponse
{
    /// <summary>【二级商户号】 二级商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【商家批次单号】 服务商系统内部的批次单号，在服务商系统内部唯一（官方字段路径 <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>【电子回单状态】 电子回单状态。可选取值：`ACCEPTED` 批次汇总电子回单已受理 / `FINISHED` 批次汇总电子回单生成成功（官方字段路径 <c>receipt_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("receipt_state")]
    public string? ReceiptState { get; set; }

}

/// <summary>查询商家转账批次汇总电子回单响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504227"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionInsuranceReceipt")]
public class InsuranceSummaryReceiptQueryResponse : WechatPayResponse
{
    /// <summary>【商家转账批次单号】 商家转账批次单号，微信商家转账系统返回的唯一标识（官方字段路径 <c>batch_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("batch_id")]
    public string? BatchId { get; set; }

    /// <summary>【商家批次单号】 服务商系统内部的批次单号，在服务商系统内部唯一（官方字段路径 <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(43)</c>）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>【二级商户号】 二级商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【电子回单状态】 电子回单状态。可选取值：`ACCEPTED` 批次汇总电子回单已受理 / `FINISHED` 批次汇总电子回单生成成功（官方字段路径 <c>receipt_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("receipt_state")]
    public string? ReceiptState { get; set; }

    /// <summary>【电子回单文件的hash方法】 计算电子回单文件的hash值所使用的hash方法 当电子回单状态为FINISHED时返回（官方字段路径 <c>hash_type</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("hash_type")]
    public string? HashType { get; set; }

    /// <summary>【电子回单文件的hash值】 电子回单文件的hash值，用于下载之后验证文件的完整、正确性 当电子回单状态为FINISHED时返回（官方字段路径 <c>hash_value</c>，选填，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("hash_value")]
    public string? HashValue { get; set; }

    /// <summary>【电子回单文件的下载地址】 电子回单文件的下载地址，当电子回单状态为 FINISHED 时返回 电子回单文件的下载地址有效时长为10分钟，10分钟后需要重新通过查询API获取下载地址 电子回单文件的下载地址域名和路径可能会发生变化（官方字段路径 <c>download_url</c>，选填，类型 <c>string(3000)</c>）。</summary>
    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }

}

/// <summary>受理商家转账明细电子回单请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504228"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionInsuranceReceipt")]
public class InsuranceDetailReceiptApplyRequest
{
    /// <summary>【二级商户号】 二级商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>受理商家转账明细电子回单响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504228"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionInsuranceReceipt")]
public class InsuranceDetailReceiptApplyResponse : WechatPayResponse
{
    /// <summary>【二级商户号】 二级商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【商家批次单号】 服务商系统内部的批次单号，在服务商系统内部唯一（官方字段路径 <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>【商家明细单号】 服务商系统内部区分转账批次单下不同转账明细单的唯一标识（官方字段路径 <c>out_detail_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }

    /// <summary>【电子回单状态】 电子回单状态。可选取值：`ACCEPTED` 明细电子回单已受理 / `FINISHED` 明细电子回单生成成功（官方字段路径 <c>receipt_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("receipt_state")]
    public string? ReceiptState { get; set; }

}

/// <summary>查询商家转账明细电子回单响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504229"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionInsuranceReceipt")]
public class InsuranceDetailReceiptQueryResponse : WechatPayResponse
{
    /// <summary>【商家转账批次单号】 商家转账批次单号，微信商家转账系统返回的唯一标识（官方字段路径 <c>batch_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("batch_id")]
    public string? BatchId { get; set; }

    /// <summary>【商家批次单号】 服务商系统内部的批次单号，在服务商系统内部唯一（官方字段路径 <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>【商家转账明细单号】 微信商家转账系统内部区分转账批次单下不同转账明细单的唯一标识（官方字段路径 <c>detail_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("detail_id")]
    public string? DetailId { get; set; }

    /// <summary>【商家明细单号】 服务商系统内部区分转账批次单下不同转账明细单的唯一标识（官方字段路径 <c>out_detail_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }

    /// <summary>【二级商户号】 二级商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【电子回单状态】 电子回单状态。可选取值：`ACCEPTED` 明细电子回单已受理 / `FINISHED` 明细电子回单生成成功（官方字段路径 <c>receipt_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("receipt_state")]
    public string? ReceiptState { get; set; }

    /// <summary>【电子回单文件的hash方法】 计算电子回单文件的hash值所使用的hash方法 当电子回单状态为FINISHED时返回（官方字段路径 <c>hash_type</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("hash_type")]
    public string? HashType { get; set; }

    /// <summary>【电子回单文件的hash值】 电子回单文件的hash值，用于下载之后验证文件的完整、正确性 当电子回单状态为FINISHED时返回（官方字段路径 <c>hash_value</c>，选填，类型 <c>string(1000)</c>）。</summary>
    [JsonPropertyName("hash_value")]
    public string? HashValue { get; set; }

    /// <summary>【电子回单文件的下载地址】 电子回单文件的下载地址，当电子回单状态为 FINISHED 时返回 电子回单文件的下载地址有效时长为10分钟，10分钟后需要重新通过查询API获取下载地址 电子回单文件的下载地址域名和路径可能会发生变化（官方字段路径 <c>download_url</c>，选填，类型 <c>string(3000)</c>）。</summary>
    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }

}
