// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.ResignedInheritance;

/// <summary>
/// 获取待分配的离职成员列表请求体（<c>/cgi-bin/externalcontact/get_unassigned_list</c>）。
/// <para>用于拉取全部离职成员的客户列表，再交由「分配离职成员的客户」接口重新分配。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ResignedInheritance")]
public class GetUnassignedListRequest
{
    /// <summary>
    /// 获取或设置每次返回的最大记录数（默认 1000，最大 1000）。
    /// </summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }

    /// <summary>
    /// 获取或设置分页查询游标，适用于数据量较大的情况，由上一次调用返回；
    /// 使用该参数时无需填写 <see cref="PageId"/>（二者互斥）。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置分页查询 page_id（兼容旧分页形态），由上一次调用返回；
    /// 使用 <see cref="Cursor"/> 时不填。以该参数查询时响应不返回 next_cursor。
    /// </summary>
    [JsonPropertyName("page_id")]
    public string? PageId { get; set; }
}
