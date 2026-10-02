// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 第三方模板消息跳转小程序（<c>template_msg.miniprogram</c>），用于发送应用消息（<c>/cgi-bin/message/send</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class MessageTemplateMsgMiniProgram
{
    /// <summary>
    /// 获取或设置小程序 appid，必须是与当前应用关联的小程序。
    /// </summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>
    /// 获取或设置点击消息卡片后的小程序页面，仅限本小程序内的页面。
    /// </summary>
    [JsonPropertyName("pagepath")]
    public string? PagePath { get; set; }
}
