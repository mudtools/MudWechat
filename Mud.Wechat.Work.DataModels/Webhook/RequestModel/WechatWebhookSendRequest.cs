// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Webhook;

/// <summary>
/// 群机器人发送消息请求基类（<c>/cgi-bin/webhook/send</c>，官方文档 91770）。
/// <para>本类仅承载信封公共字段，由各消息类型请求子类继承；
/// 每个机器人发送的消息不能超过 20 条/分钟（超过上限后官方报错/丢弃）。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Webhook")]
public class WechatWebhookSendRequest
{
    /// <summary>
    /// 获取或设置消息类型（官方必填），由各子类对应固定取值
    /// （text、markdown、markdown_v2、image、news、file、voice、template_card）。
    /// </summary>
    [JsonPropertyName("msgtype")]
    public string? MsgType { get; set; }
}
