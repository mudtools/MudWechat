// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Users;

/// <summary>
/// 邀请成员响应体（<c>/cgi-bin/batch/invite</c>）。
/// </summary>
/// <remarks>邀请频率是异步检查的，调用返回成功并不代表接收者一定能收到邀请消息。</remarks>
public class InviteMembersResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置非法成员列表（无权限或不存在）。
    /// </summary>
    [JsonPropertyName("invaliduser")]
    public List<string>? InvalidUser { get; set; } = [];

    /// <summary>
    /// 获取或设置非法部门列表（无权限或不存在）。
    /// </summary>
    [JsonPropertyName("invalidparty")]
    public List<int>? InvalidParty { get; set; } = [];

    /// <summary>
    /// 获取或设置非法标签列表（无权限或不存在）。
    /// </summary>
    [JsonPropertyName("invalidtag")]
    public List<int>? InvalidTag { get; set; } = [];
}
