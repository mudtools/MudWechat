// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Nontax;

/// <summary>
/// 「查询欠费」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxQueryFeeResponse : MpResponse
{
    /// <summary>获取或设置应缴金额（官方 <c>fee</c>，单位分）。</summary>
    [JsonPropertyName("fee")]
    public int Fee { get; set; }

    /// <summary>获取或设置缴款人姓名（官方 <c>user_name</c>，可选）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>获取或设置执收单位编码（官方 <c>department_code</c>，可选）。</summary>
    [JsonPropertyName("department_code")]
    public string? DepartmentCode { get; set; }

    /// <summary>获取或设置执收单位名称（官方 <c>department_name</c>）。</summary>
    [JsonPropertyName("department_name")]
    public string? DepartmentName { get; set; }

    /// <summary>获取或设置通知书类型（官方 <c>payment_notice_type</c>，可选）。</summary>
    [JsonPropertyName("payment_notice_type")]
    public int? PaymentNoticeType { get; set; }

    /// <summary>获取或设置通知书号（官方 <c>payment_notice_no</c>）。</summary>
    [JsonPropertyName("payment_notice_no")]
    public string? PaymentNoticeNo { get; set; }

    /// <summary>获取或设置通知书生成时间（官方 <c>payment_notice_create_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("payment_notice_create_time")]
    public long PaymentNoticeCreateTime { get; set; }

    /// <summary>获取或设置缴款书过期日期（官方 <c>payment_expire_date</c>；yyyyMMdd 日期串的数值形态）。</summary>
    [JsonPropertyName("payment_expire_date")]
    public long PaymentExpireDate { get; set; }

    /// <summary>获取或设置地区编码（官方 <c>region_code</c>）。</summary>
    [JsonPropertyName("region_code")]
    public string? RegionCode { get; set; }

    /// <summary>获取或设置收费明细列表（官方 <c>items</c>）。</summary>
    [JsonPropertyName("items")]
    public MpNontaxBillItem[]? ItemList { get; set; }
}
