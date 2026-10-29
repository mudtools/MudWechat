// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DeviceData;

/// <summary>
/// 门禁规则条目（官方查询响应侧 pass_rule 的 items）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DeviceData")]
public class DeviceDataAccessControlRule
{
    /// <summary>获取或设置门禁规则 ID（官方 rule_id，修改与删除均以该值定位）。</summary>
    [JsonPropertyName("rule_id")]
    public string RuleId { get; set; } = string.Empty;

    /// <summary>获取或设置门禁规则名称（官方 name，注意查询响应侧字段名为 name 而添加 / 修改请求侧为 rule_name）。</summary>
    [JsonPropertyName("name")]
    public string RuleName { get; set; } = string.Empty;

    /// <summary>获取或设置门禁放行规则列表（官方 rule_list）。</summary>
    [JsonPropertyName("rule_list")]
    public List<string>? RuleList { get; set; }

    /// <summary>获取或设置生效成员列表（官方 effect_open_userid_list，元素结构见 <see cref="DeviceDataPassRuleEffectUser"/>）。</summary>
    [JsonPropertyName("effect_open_userid_list")]
    public List<DeviceDataPassRuleEffectUser>? EffectUserList { get; set; }

    /// <summary>获取或设置门禁规则生效时间戳（官方 effect_time）。</summary>
    [JsonPropertyName("effect_time")]
    public long EffectTimestamp { get; set; }
}
