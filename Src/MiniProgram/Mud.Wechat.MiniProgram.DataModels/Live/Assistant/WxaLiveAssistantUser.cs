// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 直播间小助手用户项（<c>users[].{username, nickname}</c>，<c>POST /wxaapi/broadcast/room/addassistant</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_addveassistant.html"/>。
/// </para>
/// <para>
/// <b>微信号而非 openid</b>：官方字段为「用户微信号」（<c>username</c>），与成员角色域的 <c>username</c> 同口径；
/// 应答侧（<c>getassistantlist</c>）才同时返回 <c>alias</c> 与 <c>openid</c> ⇒ 请求与应答字段名不同源，不共用 DTO。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveAssistantUser
{
    /// <summary>用户微信号（<c>username</c>，必填）。</summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    /// <summary>用户昵称（<c>nickname</c>，必填）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }
}
