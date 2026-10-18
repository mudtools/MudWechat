// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.BusinessCapability;

/// <summary>
/// 向 WMPF 推送消息请求体（<c>POST /wxa/business/runtime/pushappmsg</c>）。
/// </summary>
/// <remarks>
/// <para><b>消息体上限 5KB</b>（官方原文）；超限返回 <c>45002</c>。</para>
/// <para><b>令牌归属</b>：可用移动应用或小程序的 <c>access_token</c>（官方原文）；
/// <c>push_token</c> 由 WMPF 客户端 <c>getPushToken</c> 取得，属设备凭据、禁入日志。</para>
/// <para><b>错误码</b>：<c>40101</c> 缺少参数 / <c>45002</c> 消息体超限 / <c>48012</c> 推送目标无效。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BusinessCapability")]
public class WxaBusinessRuntimePushAppMsgRequest
{
    /// <summary>消息内容（必填，≤5KB）。</summary>
    [JsonPropertyName("msg")]
    public string? Msg { get; set; }

    /// <summary>设备推送令牌（必填，敏感）。</summary>
    [JsonPropertyName("push_token")]
    public string? PushToken { get; set; }
}
