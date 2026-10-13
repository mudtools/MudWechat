// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 发送直播开始事件应答（<c>POST /wxa/business/push_message</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/subscribe-management/api_pushmessage.html"/>。
/// </para>
/// <para>
/// <b><c>errcode = 0</c> 只代表「触发群发成功」，不代表群发结果</b>：官方原文「若 errcode=0 仅表明触发接口成功，
/// 最终群发结果请看【长期订阅群发结果回调】」⇒ 本应答<b>不是</b>群发完成凭证，宿主须另接回调面拿最终结果，
/// 不得据 <see cref="WxaResponse.IsSuccess"/> 判定用户已收到通知。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLivePushMessageResponse : WxaResponse
{
    /// <summary>此次群发消息的标识 ID（<c>message_id</c>，官方原文用于对应「长期订阅群发结果回调」）。</summary>
    [JsonPropertyName("message_id")]
    public string? MessageId { get; set; }
}
