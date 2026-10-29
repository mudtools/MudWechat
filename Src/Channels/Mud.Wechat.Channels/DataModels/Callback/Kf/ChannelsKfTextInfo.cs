// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 客服族（客服消息事件，多消息结构互斥形态）。
// 事件键：commkf_send_msg_to_kf。
// 建模说明：msg_type / msg_event 官方参数表标 object，但实为判别值（「参考发送消息中的消息类型」
// 「当 msg_event 为 EVENT_TYPE_EVALUATE_RESULT 时」；Body.msg_type / Body.msg_event 子表只是把顶层
// 同名字段重复罗列，为官方文档格式噪音）——按本仓 P2 客服 DTO 先例建模为 string。
// 本事件官方页面无代码示例。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>客服文本消息结构（官方 <c>text</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsKfTextInfo
{
    /// <summary>获取或设置文本内容（官方 <c>content</c>，msg_type 为 text 时必返回）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
