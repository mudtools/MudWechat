// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 获取智能表格自动化创建的群聊会话请求体（<c>/cgi-bin/wedoc/smartsheet/groupchat/get</c>）。
/// <para>仅自建应用可调用，需配置到文档「可调用应用」列表中的应用，可见范围需包含根部门；并发限制 20。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class GetSmartSheetGroupChatRequest
{
    /// <summary>
    /// 获取或设置智能表格 ID（官方必填；需为当前应用创建的智能表格）。
    /// </summary>
    [JsonPropertyName("docid")]
    public string? DocId { get; set; }

    /// <summary>
    /// 获取或设置群聊 ID（官方必填；需为智能表自动规则创建群聊 ID）。
    /// </summary>
    [JsonPropertyName("chat_id")]
    public string? ChatId { get; set; }
}
