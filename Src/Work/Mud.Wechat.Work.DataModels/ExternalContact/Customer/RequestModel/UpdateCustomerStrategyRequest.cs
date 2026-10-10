// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Customer;

/// <summary>
/// 编辑规则组及其管理范围请求体（<c>/cgi-bin/externalcontact/customer_strategy/edit</c>）。
/// </summary>
/// <remarks>
/// <para><b>危险操作约束</b>：该接口仅支持串行调用，请勿并发修改规则组；单次最多可配置 20 个管理员和
/// 100 个管理节点。</para>
/// <para><see cref="AdminList"/> 为空则不编辑负责人，传值则整体覆盖旧负责人列表；
/// <see cref="Privilege"/> 为空则不编辑权限，传值则整体覆盖旧权限配置；
/// 若规则组具有父管理组，其管理范围必须是父规则组的子集且权限配置被完全继承（privilege 将被忽略）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Customer")]
public class UpdateCustomerStrategyRequest
{
    /// <summary>
    /// 获取或设置规则组 id。
    /// </summary>
    [JsonPropertyName("strategy_id")]
    public long StrategyId { get; set; }

    /// <summary>
    /// 获取或设置规则组名称（不填则不修改）。
    /// </summary>
    [JsonPropertyName("strategy_name")]
    public string? StrategyName { get; set; }

    /// <summary>
    /// 获取或设置管理员列表（空则不编辑，传值则整体覆盖）。
    /// </summary>
    [JsonPropertyName("admin_list")]
    public List<string>? AdminList { get; set; }

    /// <summary>
    /// 获取或设置权限配置（空则不编辑，传值则整体覆盖）。
    /// </summary>
    [JsonPropertyName("privilege")]
    public CustomerStrategyPrivilege? Privilege { get; set; }

    /// <summary>
    /// 获取或设置向管理范围添加的节点列表。
    /// </summary>
    [JsonPropertyName("range_add")]
    public List<CustomerStrategyRangeNode>? RangeAdd { get; set; }

    /// <summary>
    /// 获取或设置从管理范围删除的节点列表。
    /// </summary>
    [JsonPropertyName("range_del")]
    public List<CustomerStrategyRangeNode>? RangeDel { get; set; }
}
