// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 编辑直播间请求体（<c>POST /wxaapi/broadcast/room/editroom</c>，官方 16 字段）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_editroom.html"/>。
/// </para>
/// <para>
/// <b>定位键为 <c>id</c> 而非 <c>roomId</c></b>（与 <see cref="WxaLiveDeleteRoomRequest"/> 同形，官方原文如此），
/// 其余字段名与创建接口一致。
/// </para>
/// <para>
/// <b>不可编辑项</b>：创建时的 <c>type</c>（推流 / 手机直播）、<c>subAnchorWechat</c>、<c>createrWechat</c>
/// 三字段在本页参数表中<b>不存在</b> ⇒ 副号改走 <c>addsubanchor</c>/<c>modifysubanchor</c>/<c>deletesubanchor</c>，
/// <c>closeShare</c> 直播开始后不允许修改。
/// </para>
/// <para>官方错误码：<c>300022</c>（此房间号不存在）/ <c>300023</c>（房间状态拦截）/ <c>300030</c>（主播微信号不合法）等 10 条。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveEditRoomRequest
{
    /// <summary>直播间 ID（<c>id</c>，必填；官方字段名非 <c>roomId</c>）。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>直播间名字（<c>name</c>，必填）：最短 3 个汉字，最长 17 个汉字，1 个汉字相当于 2 个字符。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>直播间背景图（<c>coverImg</c>，必填）：填 mediaID（三天内有效）；建议像素 1080*1920，不超过 2M。</summary>
    [JsonPropertyName("coverImg")]
    public string? CoverImg { get; set; }

    /// <summary>直播计划开始时间（<c>startTime</c>，必填；秒级时间戳）：须在当前时间 10 分钟后、且不在 6 个月后。</summary>
    [JsonPropertyName("startTime")]
    public long? StartTime { get; set; }

    /// <summary>直播计划结束时间（<c>endTime</c>，必填；秒级时间戳）：与开始时间间隔不得短于 30 分钟、不得超过 24 小时。</summary>
    [JsonPropertyName("endTime")]
    public long? EndTime { get; set; }

    /// <summary>主播昵称（<c>anchorName</c>，必填）：最短 2 个汉字，最长 15 个汉字。</summary>
    [JsonPropertyName("anchorName")]
    public string? AnchorName { get; set; }

    /// <summary>主播微信号（<c>anchorWechat</c>，必填）：须已实名认证，否则报 <c>300030</c>。</summary>
    [JsonPropertyName("anchorWechat")]
    public string? AnchorWechat { get; set; }

    /// <summary>直播间分享图（<c>shareImg</c>，必填）：填 mediaID（三天内有效）；建议像素 800*640，不超过 1M。</summary>
    [JsonPropertyName("shareImg")]
    public string? ShareImg { get; set; }

    /// <summary>购物直播频道封面图（<c>feedsImg</c>，必填）：填 mediaID（三天内有效）；建议像素 800*800，不超过 100KB。</summary>
    [JsonPropertyName("feedsImg")]
    public string? FeedsImg { get; set; }

    /// <summary>是否开启官方收录（<c>isFeedsPublic</c>，选填）：1 开启、0 关闭，默认开启；直播中变更走 <c>updatefeedpublic</c>。</summary>
    [JsonPropertyName("isFeedsPublic")]
    public long? IsFeedsPublic { get; set; }

    /// <summary>是否关闭点赞（<c>closeLike</c>，必填）：0 开启、1 关闭，直播开始后不允许开启。</summary>
    [JsonPropertyName("closeLike")]
    public long? CloseLike { get; set; }

    /// <summary>是否关闭货架（<c>closeGoods</c>，必填）：0 开启、1 关闭，直播开始后不允许开启。</summary>
    [JsonPropertyName("closeGoods")]
    public long? CloseGoods { get; set; }

    /// <summary>是否关闭评论（<c>closeComment</c>，必填）：0 开启、1 关闭，直播开始后不允许开启。</summary>
    [JsonPropertyName("closeComment")]
    public long? CloseComment { get; set; }

    /// <summary>是否关闭回放（<c>closeReplay</c>，选填）：0 开启、1 关闭，默认关闭（直播开始后允许开启）。</summary>
    [JsonPropertyName("closeReplay")]
    public long? CloseReplay { get; set; }

    /// <summary>是否关闭分享（<c>closeShare</c>，选填）：0 开启、1 关闭，默认开启（直播开始后<b>不允许修改</b>）。</summary>
    [JsonPropertyName("closeShare")]
    public long? CloseShare { get; set; }

    /// <summary>是否关闭客服（<c>closeKf</c>，选填）：0 开启、1 关闭，默认关闭（直播开始后允许开启）。</summary>
    [JsonPropertyName("closeKf")]
    public long? CloseKf { get; set; }
}
