// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Webhook;

/// <summary>
/// 群机器人发送消息（<c>/cgi-bin/webhook/send</c>）的响应体：<c>errcode</c> / <c>errmsg</c>。
/// </summary>
/// <remarks>
/// <para>
/// 官方 91770 成功时返回 <c>{"errcode":0,"errmsg":"ok"}</c>，无业务负载字段，
/// 此处按企业微信<b>统一响应基底</b> <see cref="WechatWorkResponse"/> 承载。
/// </para>
/// <para>
/// <b>频率超限不可重试</b>：每个机器人 20 条/分钟的频控由官方按窗口丢弃/报错，调用方重试会继续占用频控窗口，
/// errcode 直出给宿主自行决策。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Webhook")]
public class WechatWebhookSendResponse : WechatWorkResponse
{
}
