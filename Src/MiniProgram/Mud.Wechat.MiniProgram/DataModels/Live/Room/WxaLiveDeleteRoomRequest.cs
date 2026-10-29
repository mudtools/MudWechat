// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 删除直播间请求体（<c>POST /wxaapi/broadcast/room/deleteroom</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_deleteroom.html"/>。
/// </para>
/// <para>
/// <b>字段名反直觉（照录不规整）</b>：删除房间与禁言管理（<c>updatecomment</c>）的定位键官方为 <c>id</c>，
/// 而同域其余端点一律为 <c>roomId</c>；官方示例即 <c>{ "id": 6491 }</c>。两族拼写不一致，不得「顺手统一」。
/// </para>
/// <para>官方错误码：<c>300022</c>（此房间号不存在）/ <c>300023</c>（房间状态拦截，当前状态不允许删除）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveDeleteRoomRequest
{
    /// <summary>房间 ID（<c>id</c>，必填；官方字段名非 <c>roomId</c>）。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }
}
