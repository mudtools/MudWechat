// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Message;

namespace Mud.Wechat.Work.DataModels.Webhook;

/// <summary>
/// 群机器人发送模板卡片消息请求体（msgtype 固定为 template_card，<c>/cgi-bin/webhook/send</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Webhook")]
public class WechatWebhookTemplateCardRequest : WechatWebhookSendRequest
{
    /// <summary>
    /// 获取或设置模板卡片消息体（官方必填；字段与发送应用消息的 template_card 一致，
    /// 复用 <see cref="TemplateCardBody"/> 扁平结构——发送/更新两端点共用的共用扁平结构形态）。
    /// </summary>
    [JsonPropertyName("template_card")]
    public TemplateCardBody? TemplateCard { get; set; }
}
