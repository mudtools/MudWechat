// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 直播间小助手项（<c>list[]</c>，<c>GET /wxaapi/broadcast/room/getassistantlist</c> 应答）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getassistantlist.html"/>。
/// </para>
/// <para>
/// <b>微信号字段名为 <c>alias</c></b>（不是 <c>username</c>）：请求侧用 <c>username</c> 定位、应答侧回 <c>alias</c>，
/// 官方两词同义而字段名不同 ⇒ 照录，宿主做映射时不要按名字对齐。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveAssistantItem
{
    /// <summary>修改时间（<c>timestamp</c>，官方类型 number，秒级时间戳）。</summary>
    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; set; }

    /// <summary>头像（<c>headimg</c>，官方字段名为<b>单行小写</b> <c>headimg</c>，非 <c>headimgurl</c>）。</summary>
    [JsonPropertyName("headimg")]
    public string? HeadImg { get; set; }

    /// <summary>昵称（<c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>微信号（<c>alias</c>，官方原文如此，请求侧对应 <c>username</c>）。</summary>
    [JsonPropertyName("alias")]
    public string? Alias { get; set; }

    /// <summary>openid（<c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }
}
