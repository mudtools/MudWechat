// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.CorpGroup.Rules;

/// <summary>
/// 上下游关系（对接）规则详情（获取规则详情响应中的 <c>rule_info</c>；
/// 新增 / 更新对接规则请求中的 <c>rule_info</c>，三处结构官方一致）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Rules")]
public class ChainRuleInfo
{
    /// <summary>
    /// 获取或设置上游企业的对接人规则（下游企业可以看到并联系的成员或部门）。
    /// </summary>
    [JsonPropertyName("owner_corp_range")]
    public ChainRuleOwnerRange? OwnerCorpRange { get; set; }

    /// <summary>
    /// 获取或设置下游企业规则范围。
    /// </summary>
    [JsonPropertyName("member_corp_range")]
    public ChainRuleMemberRange? MemberCorpRange { get; set; }
}
