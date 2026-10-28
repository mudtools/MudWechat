// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Moment;

/// <summary>
/// 编辑客户朋友圈规则组及其管理范围请求体（<c>/cgi-bin/externalcontact/moment_strategy/edit</c>）。
/// <para>
/// <b>危险操作约束</b>：该接口仅支持串行调用，请勿并发修改规则组；
/// <see cref="StrategyId"/> 为官方必填；其余字段不填则不修改，有值则整体覆盖；
/// 编辑含父规则组的规则组时管理范围须为父规则组的子集且完全继承其权限配置（privilege 将被忽略）。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class UpdateMomentStrategyRequest
{
    /// <summary>
    /// 获取或设置朋友圈规则组 id（官方必填）。
    /// </summary>
    [JsonPropertyName("strategy_id")]
    public long? StrategyId { get; set; }

    /// <summary>
    /// 获取或设置规则组名称（不填则不修改）。
    /// </summary>
    [JsonPropertyName("strategy_name")]
    public string? StrategyName { get; set; }

    /// <summary>
    /// 获取或设置规则组的管理员 userid 列表（不填则不修改，有值则整体覆盖）。
    /// </summary>
    [JsonPropertyName("admin_list")]
    public List<string>? AdminList { get; set; }

    /// <summary>
    /// 获取或设置规则组的权限配置（不填则不修改，有值则整体覆盖）。
    /// </summary>
    [JsonPropertyName("privilege")]
    public MomentStrategyPrivilege? Privilege { get; set; }

    /// <summary>
    /// 获取或设置管理范围中添加的节点列表。
    /// </summary>
    [JsonPropertyName("range_add")]
    public List<MomentStrategyRangeNode>? RangeAdd { get; set; }

    /// <summary>
    /// 获取或设置管理范围中删除的节点列表。
    /// </summary>
    [JsonPropertyName("range_del")]
    public List<MomentStrategyRangeNode>? RangeDel { get; set; }
}
