// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Users;

/// <summary>
/// 获取部门成员（<c>/cgi-bin/user/simplelist</c>）返回的成员摘要信息。
/// </summary>
public class UserSimpleInfo
{
    /// <summary>
    /// 获取或设置成员 UserID（对应管理端账号）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置成员名称（自 2019-12-30 起第三方应用不再返回真实 name，以 userid 代替；代开发需管理员授权）。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置成员所属部门 ID 列表（列表项为 32 位整型部门 ID）。
    /// </summary>
    [JsonPropertyName("department")]
    public List<int>? Department { get; set; }

    /// <summary>
    /// 获取或设置成员全局唯一标识 open_userid（仅第三方应用可获取）。
    /// </summary>
    [JsonPropertyName("open_userid")]
    public string? OpenUserId { get; set; }
}
