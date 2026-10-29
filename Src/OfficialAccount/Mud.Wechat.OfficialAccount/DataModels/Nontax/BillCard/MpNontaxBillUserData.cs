// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Nontax;

/// <summary>
/// 票据用户数据（官方 <c>invoice_user_data</c>；插卡请求的第三层嵌套）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxBillUserData
{
    /// <summary>获取或设置发票抬头（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置实付金额（官方 <c>fee</c>，单位分）。</summary>
    [JsonPropertyName("fee")]
    public int Fee { get; set; }

    /// <summary>获取或设置缴费流水号（官方 <c>billing_no</c>）。</summary>
    [JsonPropertyName("billing_no")]
    public string? BillingNo { get; set; }

    /// <summary>获取或设置缴费编码（官方 <c>billing_code</c>）。</summary>
    [JsonPropertyName("billing_code")]
    public string? BillingCode { get; set; }

    /// <summary>获取或设置缴费时间（官方 <c>billing_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("billing_time")]
    public long BillingTime { get; set; }

    /// <summary>获取或设置票据 PDF 的临时素材 ID（官方 <c>s_pdf_media_id</c>）。</summary>
    [JsonPropertyName("s_pdf_media_id")]
    public string? PdfMediaId { get; set; }
}
