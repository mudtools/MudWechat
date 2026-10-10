// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 修改群聊会话请求体（<c>/cgi-bin/appchat/update</c>，官方文档 98913）。
/// <para>仅自建应用可调用，应用的可见范围必须为根部门，第三方应用不可调用。</para>
/// <para>chatid 所代表的群必须是该应用所创建；每企业变更群的次数不可超过 1000 次/小时；群成员人数不可超过 2000 人。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class UpdateAppChatRequest
{
    /// <summary>
    /// 获取或设置群聊 id（官方必填）。
    /// </summary>
    [JsonPropertyName("chatid")]
    public string? ChatId { get; set; }

    /// <summary>
    /// 获取或设置新的群聊名；若不需更新请忽略；最多 50 个 utf8 字符，超过将截断。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置新群主的 id；若不需更新请忽略；课程群聊群主必须拥有课程群创建权限，
    /// del_user_list 包含群主时本字段必填。
    /// </summary>
    [JsonPropertyName("owner")]
    public string? Owner { get; set; }

    /// <summary>
    /// 获取或设置添加成员的 id 列表。
    /// </summary>
    [JsonPropertyName("add_user_list")]
    public List<string>? AddUserList { get; set; }

    /// <summary>
    /// 获取或设置踢出成员的 id 列表。
    /// </summary>
    [JsonPropertyName("del_user_list")]
    public List<string>? DelUserList { get; set; }
}
