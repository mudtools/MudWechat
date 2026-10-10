// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 获取智能表格自动化创建的群聊会话响应体（<c>/cgi-bin/wedoc/smartsheet/groupchat/get</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class GetSmartSheetGroupChatResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置群聊名称。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置群主。
    /// </summary>
    [JsonPropertyName("owner")]
    public string? Owner { get; set; }

    /// <summary>
    /// 获取或设置群成员列表（仅返回当前企业成员列表）。
    /// </summary>
    [JsonPropertyName("user_list")]
    public List<string>? UserList { get; set; }
}
