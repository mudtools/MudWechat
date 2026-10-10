// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 知识库获取分组列表响应体（<c>/cgi-bin/kf/knowledge/list_group</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class GetKfKnowledgeGroupListResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置分页游标（has_more = 1 时用于拉取下一页）。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>
    /// 获取或设置是否还有更多数据：0 - 否，1 - 是。
    /// </summary>
    [JsonPropertyName("has_more")]
    public int? HasMore { get; set; }

    /// <summary>
    /// 获取或设置分组列表。
    /// </summary>
    [JsonPropertyName("group_list")]
    public List<KfKnowledgeGroup>? GroupList { get; set; }
}

/// <summary>
/// 知识库分组信息（<see cref="GetKfKnowledgeGroupListResponse.GroupList"/> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfKnowledgeGroup
{
    /// <summary>
    /// 获取或设置分组 ID。
    /// </summary>
    [JsonPropertyName("group_id")]
    public string? GroupId { get; set; }

    /// <summary>
    /// 获取或设置分组名称。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置是否为默认分组：0 - 否，1 - 是（默认分组不可修改、不可删除）。
    /// </summary>
    [JsonPropertyName("is_default")]
    public int? IsDefault { get; set; }
}
