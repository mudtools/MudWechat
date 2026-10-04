// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 审批多语言文本项（模板名称、控件名称、控件说明、选项值、摘要行等场景通用的 text + lang 结构）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalLangTextItem
{
    /// <summary>
    /// 获取或设置文本内容（模板名称、控件名称、选项值、摘要行文字等场景的具体文字）。
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// 获取或设置显示语言，中文：zh_CN（注意不是zh-CN），英文：en。
    /// </summary>
    [JsonPropertyName("lang")]
    public string? Lang { get; set; }
}
