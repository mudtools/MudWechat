// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DataZone;

/// <summary>
/// 获取会话记录请求体（<c>/cgi-bin/data/get_conversation_records</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataZone")]
public class GetConversationRecordsRequest
{
    /// <summary>
    /// 获取或设置会话 ID（官方必填；单聊为成员间会话标识、群聊为群 ID）。
    /// </summary>
    [JsonPropertyName("chatid")]
    public string ChatId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置拉取起始时间（官方必填；Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("starttime")]
    public long StartTime { get; set; }

    /// <summary>
    /// 获取或设置拉取结束时间（官方必填；Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("endtime")]
    public long EndTime { get; set; }

    /// <summary>
    /// 获取或设置分页拉取的游标（初始传入为空串；后续请求传上一页响应的 <c>next_cursor</c>）。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置单次拉取数量限制（最大 1000，默认 100）。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
