// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 明细控件子明细（value.children 元素；子控件的数据结构同一般控件）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalTableChild
{
    /// <summary>
    /// 获取或设置子明细列表，包含子明细所有子控件的值。
    /// </summary>
    /// <remarks>
    /// <para>提交审批申请时不能为空数组，至少需要包含一个子明细；子明细中必须包括模板中设置的全部子控件，如果子明细为空，则需要将所有子控件的值设为空。</para>
    /// </remarks>
    [JsonPropertyName("list")]
    public List<ApprovalControlItem>? List { get; set; }
}
