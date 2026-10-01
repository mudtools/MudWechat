// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Users;

/// <summary>
/// 邮箱获取 userid 请求体（<c>/cgi-bin/user/get_userid_by_email</c>）。
/// </summary>
/// <remarks>请确保邮箱的正确性：若出错的次数超过企业人数上限的 20%，会导致 1 天不可调用。</remarks>
public class GetUserIdByEmailRequest
{
    /// <summary>
    /// 获取或设置邮箱。
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置邮箱类型（1 企业邮箱（默认）；2 个人邮箱）。
    /// </summary>
    [JsonPropertyName("email_type")]
    public int? EmailType { get; set; }
}
