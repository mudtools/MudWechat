// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 创建/更新模板的单选/多选控件配置（config.selector 请求侧）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalTemplateSettingSelectorConfig
{
    /// <summary>
    /// 获取或设置选择方式：single-单选；multi-多选。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置选项列表，多选属性的选择控件允许输入多个。
    /// </summary>
    [JsonPropertyName("options")]
    public List<ApprovalTemplateSettingSelectorOption>? Options { get; set; }
}
