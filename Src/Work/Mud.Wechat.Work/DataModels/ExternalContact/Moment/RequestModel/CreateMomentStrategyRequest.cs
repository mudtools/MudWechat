// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Moment;

/// <summary>
/// 创建客户朋友圈规则组请求体（<c>/cgi-bin/externalcontact/moment_strategy/create</c>）。
/// <para>
/// <b>危险操作约束</b>：该接口仅支持串行调用，请勿并发创建规则组；
/// <see cref="StrategyName"/> / <see cref="AdminList"/> / <see cref="Range"/> 为官方必填；
/// 管理员列表不可包含超级管理员，每个规则组最多 20 个负责人；
/// 若创建具有父规则组的规则组，其管理范围必须是父规则组的子集且完全继承父规则组的权限配置（privilege 将被忽略）；
/// 管理组最大层级 5 层，每个管理组的管理范围内最多支持 3000 个节点。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class CreateMomentStrategyRequest
{
    /// <summary>
    /// 获取或设置父规则组 id（不填表示创建根规则组；有父规则组时管理范围须为其子集且完全继承其权限配置）。
    /// </summary>
    [JsonPropertyName("parent_id")]
    public long? ParentId { get; set; }

    /// <summary>
    /// 获取或设置规则组名称（官方必填）。
    /// </summary>
    [JsonPropertyName("strategy_name")]
    public string? StrategyName { get; set; }

    /// <summary>
    /// 获取或设置规则组的管理员 userid 列表（官方必填；不可包含超级管理员，每个规则组最多 20 个负责人）。
    /// </summary>
    [JsonPropertyName("admin_list")]
    public List<string>? AdminList { get; set; }

    /// <summary>
    /// 获取或设置规则组的权限配置（有父规则组时将被忽略）。
    /// </summary>
    [JsonPropertyName("privilege")]
    public MomentStrategyPrivilege? Privilege { get; set; }

    /// <summary>
    /// 获取或设置规则组的管理范围节点列表（官方必填）。
    /// </summary>
    [JsonPropertyName("range")]
    public List<MomentStrategyRangeNode>? Range { get; set; }
}
