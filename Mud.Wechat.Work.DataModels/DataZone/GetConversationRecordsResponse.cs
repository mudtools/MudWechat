// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DataZone;

/// <summary>
/// 获取会话记录响应体（<c>/cgi-bin/data/get_conversation_records</c>）。
/// <para>分页拉取以 <c>has_more</c> 判定是否拉完，后续请求以 <c>next_cursor</c> 续拉。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataZone")]
public class GetConversationRecordsResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置是否还有更多数据（true - 以 <see cref="NextCursor"/> 续拉）。
    /// </summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>
    /// 获取或设置下次拉取的游标（<see cref="HasMore"/> 为 true 时用于续拉）。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>
    /// 获取或设置本页会话记录列表。
    /// </summary>
    [JsonPropertyName("records")]
    public List<ConversationRecord>? Records { get; set; }
}
