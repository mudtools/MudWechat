// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 直播间信息条目（<c>room_info[]</c>，官方 18 字段，<c>POST /wxa/business/getliveinfo</c> 应答）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getliveinfo.html"/>。
/// </para>
/// <para>
/// <b>房间 ID 在本条目里是 <c>roomid</c>（全小写、无下划线）</b>：与创建应答的 <c>roomId</c>、其余端点请求体的
/// <c>roomId</c>、删除端点的 <c>id</c> 四种官方拼写并存 —— 本域命名最不一致的一处，一律照录，禁止归一。
/// </para>
/// <para>
/// <b>开关字段在本条目为「镜像口径」</b>：创建时用 <c>closeLike</c> 等表达「是否关闭」，本条目亦为
/// <c>close_*</c>（0 开启 / 1 关闭），语义一致，可回填编辑请求。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveRoomInfoItem
{
    /// <summary>直播间名称（<c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>直播间背景图链接（<c>cover_img</c>）。</summary>
    [JsonPropertyName("cover_img")]
    public string? CoverImg { get; set; }

    /// <summary>直播间开始时间（<c>start_time</c>，官方类型 number）；官方原文「列表按照 start_time 降序排列」。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>直播计划结束时间（<c>end_time</c>，官方类型 number）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>主播名（<c>anchor_name</c>）。</summary>
    [JsonPropertyName("anchor_name")]
    public string? AnchorName { get; set; }

    /// <summary>直播间 ID（<c>roomid</c>，官方原文拼写为<b>全小写</b>，非 <c>roomId</c>）。</summary>
    [JsonPropertyName("roomid")]
    public long? Roomid { get; set; }

    /// <summary>商品列表（<c>goods</c>，官方类型 objarray），见 <see cref="WxaLiveRoomGoodsItem"/>。</summary>
    [JsonPropertyName("goods")]
    public List<WxaLiveRoomGoodsItem>? Goods { get; set; }

    /// <summary>直播间状态（<c>live_status</c>），取值见 <see cref="WxaLiveRoomStatus"/>（101 直播中 ~ 107 已过期）。</summary>
    [JsonPropertyName("live_status")]
    public long? LiveStatus { get; set; }

    /// <summary>直播间分享图链接（<c>share_img</c>）。</summary>
    [JsonPropertyName("share_img")]
    public string? ShareImg { get; set; }

    /// <summary>直播类型（<c>live_type</c>，官方原文「1 推流 0 手机直播」），取值见 <see cref="WxaLiveRoomType"/>。</summary>
    [JsonPropertyName("live_type")]
    public long? LiveType { get; set; }

    /// <summary>是否关闭点赞（<c>close_like</c>，0 开启 / 1 关闭；关闭后直播开始后不允许开启）。</summary>
    [JsonPropertyName("close_like")]
    public long? CloseLike { get; set; }

    /// <summary>是否关闭货架（<c>close_goods</c>，0 开启 / 1 关闭）。</summary>
    [JsonPropertyName("close_goods")]
    public long? CloseGoods { get; set; }

    /// <summary>是否关闭评论（<c>close_comment</c>，0 开启 / 1 关闭）。</summary>
    [JsonPropertyName("close_comment")]
    public long? CloseComment { get; set; }

    /// <summary>是否关闭客服（<c>close_kf</c>，0 开启 / 1 关闭，默认关闭；直播开始后允许开启）。</summary>
    [JsonPropertyName("close_kf")]
    public long? CloseKf { get; set; }

    /// <summary>是否关闭回放（<c>close_replay</c>，0 开启 / 1 关闭，默认关闭；直播开始后允许开启）。</summary>
    [JsonPropertyName("close_replay")]
    public long? CloseReplay { get; set; }

    /// <summary>是否开启官方收录（<c>is_feeds_public</c>，1 开启 / 0 关闭）。</summary>
    [JsonPropertyName("is_feeds_public")]
    public long? IsFeedsPublic { get; set; }

    /// <summary>创建者 openid（<c>creater_openid</c>，官方原文即 <b>creater</b> 拼写）。</summary>
    [JsonPropertyName("creater_openid")]
    public string? CreaterOpenid { get; set; }

    /// <summary>官方收录封面（<c>feeds_img</c>）。</summary>
    [JsonPropertyName("feeds_img")]
    public string? FeedsImg { get; set; }
}
