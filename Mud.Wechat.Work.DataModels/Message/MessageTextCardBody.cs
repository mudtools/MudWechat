// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 文本卡片消息体（<c>msgtype=textcard</c>），被发送应用消息（<c>/cgi-bin/message/send</c>）与发送「学校通知」（<c>/cgi-bin/externalcontact/message/send</c>）共用。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class MessageTextCardBody
{
    /// <summary>
    /// 获取或设置标题，不超过 128 个字符，超过会自动截断（支持 id 转译）。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置描述，不超过 512 个字符，超过会自动截断（支持 id 转译；支持 &lt;br&gt; 标签或空格换行，&lt;div&gt; 标签 class 内置 gray/highlight/normal 三种文字颜色）。
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 获取或设置点击后跳转的链接，最长 2048 字节，须包含协议头（http/https）。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 获取或设置按钮文字，默认"详情"，不超过 4 个文字，超过自动截断。
    /// </summary>
    [JsonPropertyName("btntxt")]
    public string? BtnTxt { get; set; }
}
