// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 消息反馈信息（<c>feedback</c>）：<c>{ "id": "FEEDBACKID" }</c>。
/// </summary>
/// <remarks>
/// <para>
/// 官方在三处使用<b>同一结构</b>的反馈信息：模板卡片（<c>template_card.feedback</c>）、
/// 流式消息（<c>stream.feedback</c>）、markdown 消息（<c>markdown.feedback</c>）
/// —— 故本仓库<b>只声明一次</b>并跨域复用（同结构双声明属架构缺陷）。
/// </para>
/// <para>
/// 语义：字段不为空值时，该回复消息被用户反馈时会触发 <c>feedback_event</c> 回调事件；
/// <c>id</c> 有效长度 256 字节以内，必须为 utf-8 编码。
/// </para>
/// <para>
/// 官方出处：智能机器人 101031（被动回复消息）、101138（主动回复消息）、101463（长连接）。
/// 注意 101032（模板卡片类型）类型页未声明根级 <c>feedback</c>，该字段属<b>应答上下文</b>字段。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class MessageFeedbackBody
{
    /// <summary>反馈 id（有效长度 256 字节以内，必须为 utf-8 编码）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }
}
