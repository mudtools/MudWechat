// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Nontax;

/// <summary>
/// 「下载缴费对账单」请求（官方 <c>downloadbill</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本端点不走声明式管线</b>：成功响应是 <b>CSV 文本</b>（官方 FAQ 原文「以 csv 格式的文本返回」），
/// 失败才是 JSON <c>errcode</c> 形态——经 <c>MpNontaxBillDownloadService</c> 独立下载通道承载
/// （对齐素材下载 I3 裁决），本 DTO 仅承载其 JSON 请求体。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxDownloadBillRequest
{
    /// <summary>获取或设置公众号 <c>appid</c>（官方 <c>appid</c>，可选）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置商户号（官方 <c>mch_id</c>，可选）。</summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>获取或设置账单日期（官方 <c>bill_date</c>，yyyyMMdd，必填）。</summary>
    [JsonPropertyName("bill_date")]
    public string BillDate { get; set; } = string.Empty;

    /// <summary>获取或设置账单类型（官方 <c>bill_type</c>，可选）。</summary>
    [JsonPropertyName("bill_type")]
    public string? BillType { get; set; }
}
