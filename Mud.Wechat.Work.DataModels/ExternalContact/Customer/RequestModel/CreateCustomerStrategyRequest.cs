// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Customer;

/// <summary>
/// 创建新的规则组请求体（<c>/cgi-bin/externalcontact/customer_strategy/create</c>）。
/// </summary>
/// <remarks>
/// <para><b>危险操作约束</b>：该接口仅支持串行调用，请勿并发创建规则组；单次最多可配置 20 个管理员和
/// 100 个管理节点；管理组最大层级 5 层；每个管理组的管理范围内最多支持 3000 个节点。</para>
/// <para>若创建具有父规则组的规则组，其管理范围必须是父规则组的子集，且将完全继承父规则组的权限配置
/// （<see cref="Privilege"/> 将被忽略）。<see cref="AdminList"/> 不可配置超级管理员。</para>
/// </remarks>
public class CreateCustomerStrategyRequest
{
    /// <summary>
    /// 获取或设置父规则组 id（无父规则组可不填）。
    /// </summary>
    [JsonPropertyName("parent_id")]
    public long? ParentId { get; set; }

    /// <summary>
    /// 获取或设置规则组名称。
    /// </summary>
    [JsonPropertyName("strategy_name")]
    public string? StrategyName { get; set; }

    /// <summary>
    /// 获取或设置规则组管理员 userid 列表（每个规则组最多可配置 20 个负责人）。
    /// </summary>
    [JsonPropertyName("admin_list")]
    public List<string>? AdminList { get; set; }

    /// <summary>
    /// 获取或设置规则组权限配置（有父规则组时被忽略）。
    /// </summary>
    [JsonPropertyName("privilege")]
    public CustomerStrategyPrivilege? Privilege { get; set; }

    /// <summary>
    /// 获取或设置规则组的管理范围节点列表（成员 / 部门混合）。
    /// </summary>
    [JsonPropertyName("range")]
    public List<CustomerStrategyRangeNode>? Range { get; set; }
}
