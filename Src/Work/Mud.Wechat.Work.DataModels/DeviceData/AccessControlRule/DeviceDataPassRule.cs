// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DeviceData;

/// <summary>
/// 门禁放行规则（官方 pass_rule / remote_pass_rule 在添加与修改请求侧的单对象形态）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DeviceData")]
public class DeviceDataPassRule
{
    /// <summary>获取或设置门禁放行规则列表（官方 rule_list）。</summary>
    [JsonPropertyName("rule_list")]
    public List<string> RuleList { get; set; } = new();

    /// <summary>获取或设置生效成员列表（官方 effect_open_userid_list，元素结构见 <see cref="DeviceDataPassRuleEffectUser"/>）。</summary>
    [JsonPropertyName("effect_open_userid_list")]
    public List<DeviceDataPassRuleEffectUser> EffectUserList { get; set; } = new();
}
