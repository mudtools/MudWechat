// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AccountId;

/// <summary>
/// 客户标签 ID 转换结果元素。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AccountId")]
public class ExternalTagIdConvertItem
{
    /// <summary>
    /// 获取或设置企业主体下的客户标签 ID。
    /// </summary>
    [JsonPropertyName("external_tagid")]
    public string? ExternalTagId { get; set; }

    /// <summary>
    /// 获取或设置服务商主体下的客户标签 ID（传入的 external_tagid 已是服务商主体下的 ID 时与本字段相同）。
    /// </summary>
    [JsonPropertyName("open_external_tagid")]
    public string? OpenExternalTagId { get; set; }
}
