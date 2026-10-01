// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Users;

/// <summary>
/// 获取选人 ticket 对应的用户请求体（<c>/cgi-bin/user/list_selected_ticket_user</c>，仅支持成员授权模式的第三方应用可调用）。
/// </summary>
public class ListSelectedTicketUserRequest
{
    /// <summary>
    /// 获取或设置选人 JS-SDK 返回的 selectedTicket。
    /// </summary>
    [JsonPropertyName("selected_ticket")]
    public string SelectedTicket { get; set; } = string.Empty;
}
