// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Customer;

/// <summary>
/// 客户联系规则组详情（获取规则组详情响应中的 <c>strategy</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Customer")]
public class CustomerStrategy
{
    /// <summary>
    /// 获取或设置规则组 id。
    /// </summary>
    [JsonPropertyName("strategy_id")]
    public long? StrategyId { get; set; }

    /// <summary>
    /// 获取或设置父规则组 id（当前规则组没有父规则组时为 0）。
    /// </summary>
    [JsonPropertyName("parent_id")]
    public long? ParentId { get; set; }

    /// <summary>
    /// 获取或设置规则组名称。
    /// </summary>
    [JsonPropertyName("strategy_name")]
    public string? StrategyName { get; set; }

    /// <summary>
    /// 获取或设置规则组创建时间戳。
    /// </summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>
    /// 获取或设置规则组管理员 userid 列表。
    /// </summary>
    [JsonPropertyName("admin_list")]
    public List<string>? AdminList { get; set; }

    /// <summary>
    /// 获取或设置规则组权限配置。
    /// </summary>
    [JsonPropertyName("privilege")]
    public CustomerStrategyPrivilege? Privilege { get; set; }
}
