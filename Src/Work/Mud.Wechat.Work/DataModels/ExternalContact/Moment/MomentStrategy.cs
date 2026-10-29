// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Moment;

/// <summary>
/// 客户朋友圈规则组（<c>moment_strategy/get</c> 响应中的 <c>strategy</c> 对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class MomentStrategy
{
    /// <summary>
    /// 获取或设置朋友圈规则组 id。
    /// </summary>
    [JsonPropertyName("strategy_id")]
    public long? StrategyId { get; set; }

    /// <summary>
    /// 获取或设置父规则组 id（无父规则组时为 0）。
    /// </summary>
    [JsonPropertyName("parent_id")]
    public long? ParentId { get; set; }

    /// <summary>
    /// 获取或设置规则组名称。
    /// </summary>
    [JsonPropertyName("strategy_name")]
    public string? StrategyName { get; set; }

    /// <summary>
    /// 获取或设置规则组的创建时间（Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>
    /// 获取或设置规则组的管理员 userid 列表。
    /// </summary>
    [JsonPropertyName("admin_list")]
    public List<string>? AdminList { get; set; }

    /// <summary>
    /// 获取或设置规则组的权限配置。
    /// </summary>
    [JsonPropertyName("privilege")]
    public MomentStrategyPrivilege? Privilege { get; set; }
}
