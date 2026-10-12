// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>个人发票字段（官方 <c>user_field</c>；各 <c>show_*</c> 为 0 否 / 1 是）。</summary>
/// <remarks>官方文档缺陷（照录）：<c>require_phone</c>/<c>require_email</c> 标注为「仅 get_auth_field 返回」。</remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceUserField
{
    /// <summary>获取或设置是否填写抬头（官方 <c>show_title</c>，0 否 / 1 是）。</summary>
    [JsonPropertyName("show_title")]
    public int? ShowTitle { get; set; }

    /// <summary>获取或设置是否填写电话号码（官方 <c>show_phone</c>）。</summary>
    [JsonPropertyName("show_phone")]
    public int? ShowPhone { get; set; }

    /// <summary>获取或设置是否填写邮箱（官方 <c>show_email</c>）。</summary>
    [JsonPropertyName("show_email")]
    public int? ShowEmail { get; set; }

    /// <summary>获取或设置电话是否必填（官方 <c>require_phone</c>；仅 get_auth_field 返回）。</summary>
    [JsonPropertyName("require_phone")]
    public int? RequirePhone { get; set; }

    /// <summary>获取或设置邮箱是否必填（官方 <c>require_email</c>；仅 get_auth_field 返回）。</summary>
    [JsonPropertyName("require_email")]
    public int? RequireEmail { get; set; }

    /// <summary>获取或设置自定义字段（官方 <c>custom_field</c>）。</summary>
    [JsonPropertyName("custom_field")]
    public List<MpInvoiceCustomField>? CustomField { get; set; }
}
