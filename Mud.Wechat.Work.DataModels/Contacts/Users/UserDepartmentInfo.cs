// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contracts.Users;

/// <summary>
/// 用户-部门关系（获取成员 ID 列表 <c>/cgi-bin/user/list_id</c> 响应中的 <c>dept_user[]</c> 元素；用户在多个部门下时会有多条记录）。
/// </summary>
/// <remarks>自建应用返回 <see cref="UserId"/>；第三方/代开发应用返回 <see cref="OpenUserId"/>（密文 userid）。</remarks>
public class UserDepartmentInfo
{
    /// <summary>
    /// 获取或设置用户 userid（自建应用返回）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    /// <summary>
    /// 获取或设置用户 open_userid（第三方/代开发应用返回）。
    /// </summary>
    [JsonPropertyName("open_userid")]
    public string? OpenUserId { get; set; }

    /// <summary>
    /// 获取或设置用户所属部门 ID。
    /// </summary>
    [JsonPropertyName("department")]
    public int? Department { get; set; }
}
