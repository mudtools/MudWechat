// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DataZone;

/// <summary>
/// 获取会话存档授权成员列表请求体（<c>/cgi-bin/chatdata/get_auth_user_list</c>）。
/// <para>通过该接口可获取授权企业中「会话内容存档」所有生效中的成员列表（cursor + limit 翻页拉取）。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataZone")]
public class GetChatArchiveAuthUserListRequest
{
    /// <summary>获取或设置上一次调用时返回的 next_cursor（官方选填；第一次拉取可以不填）。</summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>获取或设置本次查询返回的最大条数（官方选填；不超过 1000，默认 200 条）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }
}
