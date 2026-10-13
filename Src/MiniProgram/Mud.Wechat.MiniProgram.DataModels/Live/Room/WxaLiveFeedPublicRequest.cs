// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 官方收录管理请求体（<c>POST /wxaapi/broadcast/room/updatefeedpublic</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_updatefeedpublic.html"/>。
/// </para>
/// <para>
/// <b>开关三兄弟形态同构但字段各异</b>：官方为「官方收录 <c>isFeedsPublic</c>」「回放 <c>closeReplay</c>」
/// 「客服 <c>closeKf</c>」各开一个端点，且前者的开关<b>语义相反</b>（1 开启）而后两者是 <c>close*</c>
/// （1 关闭）⇒ 三个请求 DTO 各自建模，不做「roomId + 开关」的通用抽象，避免调用方传反。
/// </para>
/// <para>调用额度：10000 次/天。官方错误码：<c>-1</c>（系统繁忙）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveFeedPublicRequest
{
    /// <summary>房间 ID（<c>roomId</c>，必填）。</summary>
    [JsonPropertyName("roomId")]
    public long? RoomId { get; set; }

    /// <summary>是否开启官方收录（<c>isFeedsPublic</c>，必填）：1 开启、0 关闭。注意本字段<b>非</b> <c>close*</c> 反语义口径。</summary>
    [JsonPropertyName("isFeedsPublic")]
    public long? IsFeedsPublic { get; set; }
}
