// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contracts.Users;

/// <summary>
/// 获取选人 ticket 对应的用户响应体（<c>/cgi-bin/user/list_selected_ticket_user</c>）。
/// </summary>
public class ListSelectedTicketUserResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置选人用户的 open_userid。
    /// </summary>
    [JsonPropertyName("operator_open_userid")]
    public string? OperatorOpenUserId { get; set; }

    /// <summary>
    /// 获取或设置此次选人操作中在应用可见范围内的 open_userid 列表。
    /// </summary>
    [JsonPropertyName("open_userid_list")]
    public List<string>? OpenUserIdList { get; set; } = [];

    /// <summary>
    /// 获取或设置此次选人操作中不在应用可见范围内的 open_userid 列表。
    /// </summary>
    [JsonPropertyName("unauth_open_userid_list")]
    public List<string>? UnauthOpenUserIdList { get; set; } = [];

    /// <summary>
    /// 获取或设置用户选择的总人数。
    /// </summary>
    [JsonPropertyName("total")]
    public int? Total { get; set; }
}
