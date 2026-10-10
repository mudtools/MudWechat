// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.ResignedInheritance;

/// <summary>
/// 获取待分配的离职成员列表响应体（<c>/cgi-bin/externalcontact/get_unassigned_list</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ResignedInheritance")]
public class GetUnassignedListResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置离职成员的客户列表。
    /// </summary>
    [JsonPropertyName("info")]
    public List<UnassignedCustomerItem>? Info { get; set; }

    /// <summary>
    /// 获取或设置是否为最后一条记录。
    /// </summary>
    [JsonPropertyName("is_last")]
    public bool? IsLast { get; set; }

    /// <summary>
    /// 获取或设置分页查询游标（已查完则为空字符串；以 page_id 分页时不返回）。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }
}
