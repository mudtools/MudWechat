// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>自定义字段（官方 <c>custom_field[]</c>；请求与响应同构 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceCustomField
{
    /// <summary>获取或设置字段名（官方 <c>key</c>，必填）。</summary>
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    /// <summary>获取或设置是否必填（官方 <c>is_require</c>，0 否 / 1 是，默认 0）。</summary>
    [JsonPropertyName("is_require")]
    public int? IsRequire { get; set; }

    /// <summary>获取或设置提示文案（官方 <c>notice</c>）。</summary>
    [JsonPropertyName("notice")]
    public string? Notice { get; set; }
}
