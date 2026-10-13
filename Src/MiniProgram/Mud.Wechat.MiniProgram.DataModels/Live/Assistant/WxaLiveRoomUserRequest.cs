// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 「房间 + 单个微信号」请求体（<c>{ roomId, username }</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用裁决</b>：三端点的官方请求体字段集<b>完全一致</b>（均只有 <c>roomId</c> + <c>username</c>，均必填）⇒
/// 共用本类型，不各自造同形 DTO：
/// <c>POST /wxaapi/broadcast/room/removeassistant</c>（删除小助手）、
/// <c>POST /wxaapi/broadcast/room/addsubanchor</c>（添加主播副号）、
/// <c>POST /wxaapi/broadcast/room/modifysubanchor</c>（修改主播副号）。
/// </para>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_removeassistant.html"/>。
/// </para>
/// <para>
/// <b>副号专属错误码</b>：<c>500004</c>（不能设置重复的副号）/ <c>500005</c>（副号不能和主号重复）/
/// <c>500002</c>（副号未实名，仅修改副号时出现）。小助手侧无该族错误码。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveRoomUserRequest
{
    /// <summary>房间 ID（<c>roomId</c>，必填）。</summary>
    [JsonPropertyName("roomId")]
    public long? RoomId { get; set; }

    /// <summary>用户微信号（<c>username</c>，必填）。</summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }
}
