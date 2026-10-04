// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.MsgAudit;

/// <summary>
/// 获取群聊会话同意情况请求体（<c>/cgi-bin/msgaudit/check_room_agree</c>）。
/// <para>
/// 官方业务限制：access_token 必须由「会话内容存档」应用 secret 获取；
/// 可查询对应 roomid 里所有外企业外部联系人的同意情况；调用频率不可超过 1500 次/分钟。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MsgAudit")]
public class CheckMsgAuditRoomAgreeRequest
{
    /// <summary>
    /// 获取或设置待查询的群 roomid（官方必填，填入会话内容存档中获取到的 roomid）。
    /// </summary>
    [JsonPropertyName("roomid")]
    public string? RoomId { get; set; }
}
