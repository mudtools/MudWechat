// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 「仅房间 ID」请求体（<c>POST /wxaapi/broadcast/room/deletesubanchor</c>，官方请求体只有 <c>roomId</c> 一字段）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_deletesubanchor.html"/>。
/// </para>
/// <para>
/// <b>与 <see cref="WxaLiveDeleteRoomRequest"/> 的区别</b>：删除直播间用 <c>id</c>、删除主播副号用 <c>roomId</c>
/// —— 同为「传一个房间标识的 POST」但字段名不同，故两类型不共用（照录官方原文）。
/// </para>
/// <para>官方错误码：<c>-1</c>（系统繁忙）。删除副号后房间仍保留主播主号。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveRoomIdRequest
{
    /// <summary>房间 ID（<c>roomId</c>，必填）。</summary>
    [JsonPropertyName("roomId")]
    public long? RoomId { get; set; }
}
