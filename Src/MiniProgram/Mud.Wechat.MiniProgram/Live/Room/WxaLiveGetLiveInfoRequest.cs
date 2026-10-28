// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 获取直播间列表 / 回放请求体（<c>POST /wxa/business/getliveinfo</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getliveinfo.html"/>。
/// </para>
/// <para>
/// <b>一路由两形态（官方 <c>action</c> 驱动）</b>：不传 <c>action</c> ⇒ 返回 <c>room_info</c> 房间列表；
/// <c>action = "get_replay"</c> ⇒ 返回 <c>live_replay</c> 回放数组且<b>不返回</b> <c>room_info</c>，
/// 此时 <c>room_id</c> <b>必填</b>（官方原文「当 action 有值时该字段必填」）。
/// 两种形态共用同一响应类型 <see cref="WxaLiveInfoResponse"/>（各自数组缺省为 <c>null</c>）。
/// </para>
/// <para>
/// <b>字段名为下划线拼写</b>（<c>start</c> / <c>limit</c> / <c>action</c> / <c>room_id</c>）：本域多数端点用
/// camelCase（<c>roomId</c>），唯 <c>/wxa/business/</c> 两族（本端点与 <c>getgoodswarehouse</c>）用 snake_case
/// —— 官方原文如此，照录不统一。
/// </para>
/// <para>调用额度：<b>100000 次/天</b>（官方注意事项，本域最高）。官方错误码：<c>-1</c>（系统繁忙）/ <c>40001</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveGetLiveInfoRequest
{
    /// <summary>起始拉取视频（<c>start</c>，必填）：0 表示从第一个视频片段开始拉取。</summary>
    [JsonPropertyName("start")]
    public long? Start { get; set; }

    /// <summary>每次拉取数量（<c>limit</c>，必填）：官方原文「建议 100 以内」。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>动作（<c>action</c>，选填）：官方原文「只能填 <c>"get_replay"</c>，表示获取回放」，取值见 <see cref="WxaLiveInfoAction"/>。</summary>
    [JsonPropertyName("action")]
    public string? Action { get; set; }

    /// <summary>直播间 ID（<c>room_id</c>，<c>action</c> 有值时必填）。</summary>
    [JsonPropertyName("room_id")]
    public long? RoomId { get; set; }
}
