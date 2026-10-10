// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 模板详情单选/多选控件选项（selector.options 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalTemplateSelectorOption
{
    /// <summary>
    /// 获取或设置选项key（选项的唯一id，可用于发起审批申请，为单选/多选控件赋值）。
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>
    /// 获取或设置选项值（多语言）。
    /// </summary>
    /// <remarks>
    /// <para>获取审批模板详情的响应示例按数组传输（如 [{"text":"选项1","lang":"zh_CN"}]），与创建/更新模板请求示例的单对象形态不一致；本模型为响应侧模型，按数组承载。</para>
    /// </remarks>
    [JsonPropertyName("value")]
    public List<ApprovalLangTextItem>? Value { get; set; }
}
