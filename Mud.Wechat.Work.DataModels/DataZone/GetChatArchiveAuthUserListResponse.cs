// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DataZone;

/// <summary>
/// 获取会话存档授权成员列表响应体（<c>/cgi-bin/chatdata/get_auth_user_list</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataZone")]
public class GetChatArchiveAuthUserListResponse : WechatWorkResponse
{
    /// <summary>获取或设置生效成员列表（官方 auth_user_list；<see cref="ChatArchiveAuthUser"/>：userid / edition_list）。</summary>
    [JsonPropertyName("auth_user_list")]
    public List<ChatArchiveAuthUser>? AuthUserList { get; set; }

    /// <summary>获取或设置下一次查询时使用的游标（官方 next_cursor；将值填到请求包的 cursor 字段中）。</summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>
    /// 获取或设置是否还有更多未拉取的数据（官方 has_more：1 - 是；0 - 否）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 官方文档参数表标注 <c>has_more</c> 为 Integer（1：是；0：否），但响应示例作布尔 <c>true</c>，
    /// 两处形态不一致；本模型以参数表为准按整数承载，处理器不应假设必有值。
    /// </para>
    /// </remarks>
    [JsonPropertyName("has_more")]
    public long? HasMore { get; set; }
}
