// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 创建群聊会话请求体（<c>/cgi-bin/appchat/create</c>，官方文档 90245）。
/// <para>仅自建应用可调用，应用的可见范围必须为根部门，第三方应用不可调用。</para>
/// <para>每企业创建群数不可超过 1000 个/天；群成员人数不可超过管理端配置的「群成员人数上限」且最大不可超过
/// 2000 人（含应用）；刚创建的群如果没有下发消息，在旧版本企业微信终端上可能不会出现该群。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class CreateAppChatRequest
{
    /// <summary>
    /// 获取或设置群聊名，最多 50 个 utf8 字符，超过将截断。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置指定群主的 id；不指定则系统随机从 userlist 中选一人作为群主。
    /// </summary>
    [JsonPropertyName("owner")]
    public string? Owner { get; set; }

    /// <summary>
    /// 获取或设置群成员 id 列表（官方必填），至少 2 人，至多 2000 人。
    /// </summary>
    [JsonPropertyName("userlist")]
    public List<string>? UserList { get; set; }

    /// <summary>
    /// 获取或设置群聊唯一标志，不能与已有群重复；最长 32 字符，仅允许 0-9 及 a-zA-Z；不填则系统随机生成。
    /// </summary>
    [JsonPropertyName("chatid")]
    public string? ChatId { get; set; }
}
