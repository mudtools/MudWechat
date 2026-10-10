// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Aibot;

/// <summary>
/// 智能机器人主动回复消息（101138）的响应体：<c>errcode</c> / <c>errmsg</c>。
/// </summary>
/// <remarks>
/// <para>
/// 官方 101138 文档页<b>未列出</b>响应字段（仅给出请求示例），此处按企业微信<b>统一响应基底</b>
/// <see cref="WechatWorkResponse"/> 承载（<c>errcode</c> / <c>errmsg</c>），避免复活零引用的开放泛型
/// <c>WechatChatbotResponse&lt;TData&gt;</c>（开放泛型无法登记源生成元数据 ⇒ AOT 不可用）。
/// </para>
/// <para>
/// <b>不可重试</b>：每个 <c>response_url</c> 仅可调用一次、有效期 1 小时，失败后无法重发；
/// 故本 SDK <b>不做</b>任何 errcode 重试包装，errcode 直出给宿主自行决策。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotReplyResponse : WechatWorkResponse
{
}
