// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 获取直播间列表 / 回放应答（<c>POST /wxa/business/getliveinfo</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getliveinfo.html"/>。
/// </para>
/// <para>
/// <b>两数组互斥而非并存</b>：官方对 <c>room_info</c> 注「action="get_replay" 不返回」、对 <c>live_replay</c>
/// 注「action="get_replay" 才返回」⇒ 同一次调用<b>只出现其一</b>，宿主须按请求是否带 <c>action</c> 判别读哪个数组，
/// 不得假设两者同时非空。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveInfoResponse : WxaResponse
{
    /// <summary>直播间信息列表（<c>room_info</c>，官方类型 objarray；按 <c>start_time</c> 降序）。拉回放时不返回本字段。</summary>
    [JsonPropertyName("room_info")]
    public List<WxaLiveRoomInfoItem>? RoomInfo { get; set; }

    /// <summary>拉取房间总数（<c>total</c>，官方类型 number）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>回放视频列表（<c>live_replay</c>，官方类型 objarray）；仅 <c>action = "get_replay"</c> 时返回。</summary>
    [JsonPropertyName("live_replay")]
    public List<WxaLiveReplayItem>? LiveReplay { get; set; }
}
