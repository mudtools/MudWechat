// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 发送直播开始事件请求体（<c>POST /wxa/business/push_message</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/subscribe-management/api_pushmessage.html"/>。
/// </para>
/// <para>
/// <b>单次群发人数上限 2000</b>（官方注意事项原文「群发订阅用户个数限制：2000/次」）⇒ 宿主须自行切分
/// <see cref="UserOpenid"/> 数组；SDK 不做本地切分（避免静默改变调用语义）。
/// </para>
/// <para>
/// <b>一日内只推一个房间</b>：官方错误码 <c>9410005</c>「当日另外一个房间已推送过」与
/// <c>9410006</c>「推送的直播间非直播中状态」⇒ 本端点是<b>日粒度、房间唯一</b>的推送，不可并发或补推其他房间。
/// </para>
/// <para>调用额度：5000 次/天。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLivePushMessageRequest
{
    /// <summary>直播开始事件的房间 ID（<c>room_id</c>，必填；须处于「直播中」状态）。</summary>
    [JsonPropertyName("room_id")]
    public long? RoomId { get; set; }

    /// <summary>接收该群发开播事件的订阅用户 OpenId 列表（<c>user_openid</c>，必填；官方类型 array，单次上限 2000）。</summary>
    [JsonPropertyName("user_openid")]
    public List<string>? UserOpenid { get; set; }
}
