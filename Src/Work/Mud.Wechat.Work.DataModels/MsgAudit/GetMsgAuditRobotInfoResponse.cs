// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.MsgAudit;

/// <summary>
/// 获取机器人信息响应体（<c>/cgi-bin/msgaudit/get_robot_info</c>）。
/// <para>官方业务限制：调用频率不可超过 600 次/分钟；
/// 只能通过会话内容存档的 access_token 获取。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MsgAudit")]
public class GetMsgAuditRobotInfoResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置机器人信息（见 <see cref="MsgAuditRobotInfo"/>）。
    /// </summary>
    [JsonPropertyName("data")]
    public MsgAuditRobotInfo? Data { get; set; }
}

/// <summary>
/// 机器人信息（<see cref="GetMsgAuditRobotInfoResponse.Data"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MsgAudit")]
public class MsgAuditRobotInfo
{
    /// <summary>
    /// 获取或设置机器人 ID。
    /// </summary>
    [JsonPropertyName("robot_id")]
    public string? RobotId { get; set; }

    /// <summary>
    /// 获取或设置机器人名称。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置机器人创建者的 userid。
    /// </summary>
    [JsonPropertyName("creator_userid")]
    public string? CreatorUserId { get; set; }
}
