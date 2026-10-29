// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 修改直播间小助手请求体（<c>POST /wxaapi/broadcast/room/modifyassistant</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_modifyassistant.html"/>。
/// </para>
/// <para>
/// <b>与添加小助手的形态差</b>：添加走 <c>users[]</c> 数组（批量），修改走<b>单用户平铺</b>
/// （<c>roomId</c> + <c>username</c> + <c>nickname</c>）⇒ 两者不可共用；
/// 官方原文昵称字段说明为「用户微信昵称」（比批量项多「微信」二字，字段名同为 <c>nickname</c>）。
/// </para>
/// <para>调用额度：10000 次/天。官方错误码：<c>-1</c>（系统繁忙）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveModifyAssistantRequest
{
    /// <summary>房间 ID（<c>roomId</c>，必填）。</summary>
    [JsonPropertyName("roomId")]
    public long? RoomId { get; set; }

    /// <summary>用户微信号（<c>username</c>，必填）。</summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    /// <summary>用户微信昵称（<c>nickname</c>，必填）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }
}
