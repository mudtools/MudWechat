// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Gov;

/// <summary>
/// 添加网格请求体（<c>/cgi-bin/report/grid/add</c>，政民沟通配置网格结构域）。
/// <para>
/// 官方业务限制：网格名称不能超过 30 个字，同一个目标网格下不能存在同名的同级子网格；
/// 网格结构层级最多支持 10 层；每个网格至少 1 个、最多 20 个负责人；
/// 成员列表不能超过 100 个；同一成员最多能成功担任 10 个网格的管理员。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Gov")]
public class GovAddGridRequest
{
    /// <summary>
    /// 获取或设置网格名称（官方必填）。不能超过 30 个字，同一个目标网格下不能存在同名的同级子网格。
    /// </summary>
    [JsonPropertyName("grid_name")]
    public string? GridName { get; set; }

    /// <summary>
    /// 获取或设置父节点的网格 id（官方必填）。网格结构层级最多支持 10 层。
    /// </summary>
    [JsonPropertyName("grid_parent_id")]
    public string? GridParentId { get; set; }

    /// <summary>
    /// 获取或设置网格「负责人」userid 列表（官方必填）。每个网格至少 1 个，最多 20 个负责人。
    /// </summary>
    [JsonPropertyName("grid_admin")]
    public List<string>? GridAdmin { get; set; }

    /// <summary>
    /// 获取或设置该节点的成员 userid 列表。不能超过 100 个；同一成员最多能成功担任 10 个网格的管理员。
    /// </summary>
    [JsonPropertyName("grid_member")]
    public List<string>? GridMember { get; set; }
}
