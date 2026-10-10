// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 随文扩展对象（官方 InlineAddon；如公式、签名等）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocDocumentInlineAddon
{
    /// <summary>获取或设置扩展对象 ID（官方 addon_id）。</summary>
    [JsonPropertyName("addon_id")]
    public string? AddonId { get; set; }

    /// <summary>
    /// 获取或设置扩展对象来源（官方 addon_source）：
    /// ADDON_SOURCE_TYPE_UNSPECIFIED / NONE / LATEX（公式）/ SIGN（签名）/ SIGN_BAR（签名占位图）。
    /// </summary>
    [JsonPropertyName("addon_source")]
    public string? AddonSource { get; set; }
}
