// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Users;

/// <summary>
/// 获取成员 ID 列表响应体（<c>/cgi-bin/user/list_id</c>，官方推荐的通讯录安全替代接口）。
/// </summary>
public class ListUserIdsResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置分页游标（下次请求时填写以获取之后分页的记录；返回空表示已没有更多数据）。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>
    /// 获取或设置用户-部门关系列表（用户在多个部门下时会有多条记录）。
    /// </summary>
    [JsonPropertyName("dept_user")]
    public List<UserDepartmentInfo>? DeptUser { get; set; } = [];
}
