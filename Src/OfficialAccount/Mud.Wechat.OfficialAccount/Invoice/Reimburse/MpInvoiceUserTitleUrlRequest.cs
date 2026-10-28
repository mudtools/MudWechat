// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

// ---------------------------------------------------------------- 极速开发票（biz/* 与 scantitle）

/// <summary>
/// 录入抬头到用户微信（<c>POST /card/invoice/biz/getusertitleurl</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（全部字段标非必填，但语义上条件必填，SDK 不本地校验）：
/// <c>title</c>（<c>user_fill = 0</c> 时必填）/ <c>phone</c>（须数字或「-」）/
/// <c>tax_no</c>（须 15-20 位数字或英文字母）/ <c>addr</c> / <c>bank_type</c> / <c>bank_no</c> /
/// <c>user_fill</c>（0 企业设置的抬头 / 1 用户自己填写抬头）/ <c>out_title_id</c>（开票码）。
/// </para>
/// <para>官方文档缺陷（照录）：返回示例出现重复键 <c>errcode</c>（第二个应为 <c>errmsg</c>）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceUserTitleUrlRequest
{
    /// <summary>获取或设置发票抬头（官方 <c>title</c>；<c>user_fill</c> 为 0 时必填）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置联系方式（官方 <c>phone</c>；官方原文「必须是数字或 "-"」）。</summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>获取或设置税号（官方 <c>tax_no</c>；官方原文「必须是 15-20 位数字或英文字母」，SDK 不本地拦截）。</summary>
    [JsonPropertyName("tax_no")]
    public string? TaxNumber { get; set; }

    /// <summary>获取或设置地址（官方 <c>addr</c>）。</summary>
    [JsonPropertyName("addr")]
    public string? Address { get; set; }

    /// <summary>获取或设置银行类型（官方 <c>bank_type</c>）。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>获取或设置银行号码（官方 <c>bank_no</c>）。</summary>
    [JsonPropertyName("bank_no")]
    public string? BankNumber { get; set; }

    /// <summary>获取或设置抬头来源（官方 <c>user_fill</c>；0 企业设置的抬头 / 1 用户自己填写抬头）。</summary>
    [JsonPropertyName("user_fill")]
    public int? UserFill { get; set; }

    /// <summary>获取或设置开票码（官方 <c>out_title_id</c>）。</summary>
    [JsonPropertyName("out_title_id")]
    public string? OutTitleId { get; set; }
}
