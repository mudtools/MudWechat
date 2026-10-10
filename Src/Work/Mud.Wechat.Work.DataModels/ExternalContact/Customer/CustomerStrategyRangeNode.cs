// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Customer;

/// <summary>
/// 客户联系规则组管理范围节点（<c>range</c> / <c>range_add</c> / <c>range_del</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Customer")]
public class CustomerStrategyRangeNode
{
    /// <summary>
    /// 获取或设置节点类型：1-成员，2-部门。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置管理范围内配置的成员 userid（仅 type = 1 时有效 / 返回）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置管理范围内配置的部门 partyid（仅 type = 2 时有效 / 返回）。
    /// </summary>
    [JsonPropertyName("partyid")]
    public long? Partyid { get; set; }
}
