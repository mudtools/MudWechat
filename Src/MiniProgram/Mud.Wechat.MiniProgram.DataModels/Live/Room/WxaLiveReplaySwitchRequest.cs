// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 回放功能管理请求体（<c>POST /wxaapi/broadcast/room/updatereplay</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_updatereplay.html"/>。
/// </para>
/// <para>
/// <b>直播开始后仍允许开启回放</b>（创建接口原文口径），与点赞 / 货架 / 评论「直播开始后不允许开启」不同。
/// 取回放内容须再用 <c>getliveinfo</c> 带 <c>action = "get_replay"</c> 拉取。
/// </para>
/// <para>调用额度：10000 次/天。官方错误码：<c>300023</c>（房间状态拦截）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveReplaySwitchRequest
{
    /// <summary>房间 ID（<c>roomId</c>，必填）。</summary>
    [JsonPropertyName("roomId")]
    public long? RoomId { get; set; }

    /// <summary>是否关闭回放（<c>closeReplay</c>，必填）：0 开启、1 关闭。</summary>
    [JsonPropertyName("closeReplay")]
    public long? CloseReplay { get; set; }
}
