// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.CorpGroup.Rules;

/// <summary>
/// 对接规则的上游企业对接人范围（<see cref="ChainRuleInfo.OwnerCorpRange"/>；
/// 下游企业可以看到并联系的成员或部门）。
/// </summary>
/// <remarks>官方契约：部门 id 和用户 id 两个必选填一个。</remarks>
public class ChainRuleOwnerRange
{
    /// <summary>
    /// 获取或设置部门 id 列表（官方示例以字符串传输；与 UserIds 至少填一个）。
    /// </summary>
    [JsonPropertyName("departmentids")]
    public List<string>? DepartmentIds { get; set; }

    /// <summary>
    /// 获取或设置用户 id 列表（官方示例以字符串传输；与 DepartmentIds 至少填一个）。
    /// </summary>
    [JsonPropertyName("userids")]
    public List<string>? UserIds { get; set; }
}
