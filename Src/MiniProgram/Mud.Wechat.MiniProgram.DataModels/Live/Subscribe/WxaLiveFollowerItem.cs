// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 长期订阅用户条目（<c>followers[]</c>，官方 4 字段，<c>POST /wxa/business/get_wxa_followers</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/subscribe-management/api_getfollowers.html"/>。
/// </para>
/// <para>
/// <b>官方类型列的两处异常（照录并留档）</b>：① <c>openid</c> 的类型列写作 <b>array</b>，但该字段在
/// 「接收长期订阅用户」语义下为单个用户标识，且群发侧的 <c>user_openid</c> 才是数组
/// —— 本 SDK 按语义建为 <see cref="string"/>，并在守卫 LB5 锁定该裁决；
/// ② <c>subscribe_time</c> 的类型列写作 <b>timestamp</b>（本域唯一），按秒级时间戳建为 <see cref="long"/>?。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveFollowerItem
{
    /// <summary>长期订阅用户 openid（<c>openid</c>；官方类型列异常写作 array，本 SDK 按语义建为字符串）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>长期订阅用户订阅时间（<c>subscribe_time</c>，官方类型列为 timestamp，秒级时间戳）。</summary>
    [JsonPropertyName("subscribe_time")]
    public long? SubscribeTime { get; set; }

    /// <summary>用户订阅时所处房间（<c>room_id</c>，官方类型 number）。</summary>
    [JsonPropertyName("room_id")]
    public long? RoomId { get; set; }

    /// <summary>用户订阅时的房间状态（<c>room_status</c>，官方类型 number，取值口径同 <see cref="WxaLiveRoomStatus"/>）。</summary>
    [JsonPropertyName("room_status")]
    public long? RoomStatus { get; set; }
}
