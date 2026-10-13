// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 创建直播间请求体（<c>POST /wxaapi/broadcast/room/create</c>，官方 18 字段）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_createroom.html"/>。
/// </para>
/// <para>
/// <b>图片字段一律填 mediaID 而非 URL</b>：官方原文「填入 mediaID（mediaID 获取后，<b>三天内有效</b>）」，
/// mediaID 由新增临时素材接口取得（公众号线 <c>media/upload</c>）⇒ 三张图
/// （<c>coverImg</c> / <c>shareImg</c> / <c>feedsImg</c>）不得持久化复用，超过三天须重新上传。
/// </para>
/// <para>
/// <b>开播时间窗（官方原文约束）</b>：<c>startTime</c> 须在当前时间 <b>10 分钟之后</b>且<b>不在 6 个月之后</b>；
/// <c>endTime</c> 与开始时间间隔<b>不得短于 30 分钟、不得超过 24 小时</b>。
/// </para>
/// <para>
/// <b>与 <see cref="WxaLiveEditRoomRequest"/> 的字段差</b>：创建独有三字段
/// <c>type</c>（直播间类型，创建后不可改）、<c>subAnchorWechat</c>、<c>createrWechat</c>
/// ——编辑请求<b>不含</b>该三项，故两类各自建模，不共用超集 DTO。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveCreateRoomRequest
{
    /// <summary>直播间名字（<c>name</c>，必填）：最短 3 个汉字，最长 17 个汉字，1 个汉字相当于 2 个字符。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>直播间背景图（<c>coverImg</c>，必填）：填 mediaID（三天内有效）；建议像素 1080*1920，大小不超过 2M。</summary>
    [JsonPropertyName("coverImg")]
    public string? CoverImg { get; set; }

    /// <summary>直播计划开始时间（<c>startTime</c>，必填；官方类型 number，秒级时间戳）：须在当前时间 10 分钟后、且不在 6 个月后。</summary>
    [JsonPropertyName("startTime")]
    public long? StartTime { get; set; }

    /// <summary>直播计划结束时间（<c>endTime</c>，必填；秒级时间戳）：与开始时间间隔不得短于 30 分钟、不得超过 24 小时。</summary>
    [JsonPropertyName("endTime")]
    public long? EndTime { get; set; }

    /// <summary>主播昵称（<c>anchorName</c>，必填）：最短 2 个汉字，最长 15 个汉字，1 个汉字相当于 2 个字符。</summary>
    [JsonPropertyName("anchorName")]
    public string? AnchorName { get; set; }

    /// <summary>主播微信号（<c>anchorWechat</c>，必填）：未实名认证时须先前往「小程序直播」小程序实名验证，否则报 <c>300036</c>。</summary>
    [JsonPropertyName("anchorWechat")]
    public string? AnchorWechat { get; set; }

    /// <summary>主播副号微信号（<c>subAnchorWechat</c>，选填）：同样要求已实名认证；后续变更走副号三端点。</summary>
    [JsonPropertyName("subAnchorWechat")]
    public string? SubAnchorWechat { get; set; }

    /// <summary>创建者微信号（<c>createrWechat</c>，选填；官方原文即 <b>creater</b> 拼写，非 creator）：不传则房间对全部成员可见，传入则仅创建者、管理员、超管、直播间主播可见。</summary>
    [JsonPropertyName("createrWechat")]
    public string? CreaterWechat { get; set; }

    /// <summary>直播间分享图（<c>shareImg</c>，必填）：填 mediaID（三天内有效）；建议像素 800*640，大小不超过 1M。</summary>
    [JsonPropertyName("shareImg")]
    public string? ShareImg { get; set; }

    /// <summary>购物直播频道封面图（<c>feedsImg</c>，必填）：填 mediaID（三天内有效）；建议像素 800*800，大小不超过 100KB。</summary>
    [JsonPropertyName("feedsImg")]
    public string? FeedsImg { get; set; }

    /// <summary>是否开启官方收录（<c>isFeedsPublic</c>，选填）：1 开启、0 关闭，<b>默认开启</b>；后续变更走 <c>updatefeedpublic</c>。</summary>
    [JsonPropertyName("isFeedsPublic")]
    public long? IsFeedsPublic { get; set; }

    /// <summary>直播间类型（<c>type</c>，必填）：1 推流、0 手机直播，取值见 <see cref="WxaLiveRoomType"/>。<b>仅创建时可选</b>，编辑接口无此字段。</summary>
    [JsonPropertyName("type")]
    public long? Type { get; set; }

    /// <summary>是否关闭点赞（<c>closeLike</c>，必填）：0 开启、1 关闭。若关闭，观众端隐藏点赞按钮，<b>直播开始后不允许开启</b>。</summary>
    [JsonPropertyName("closeLike")]
    public long? CloseLike { get; set; }

    /// <summary>是否关闭货架（<c>closeGoods</c>，必填）：0 开启、1 关闭。若关闭，观众端隐藏商品货架，<b>直播开始后不允许开启</b>。</summary>
    [JsonPropertyName("closeGoods")]
    public long? CloseGoods { get; set; }

    /// <summary>是否关闭评论（<c>closeComment</c>，必填）：0 开启、1 关闭。若关闭，观众端隐藏评论入口，<b>直播开始后不允许开启</b>；直播中改禁言走 <c>updatecomment</c>。</summary>
    [JsonPropertyName("closeComment")]
    public long? CloseComment { get; set; }

    /// <summary>是否关闭回放（<c>closeReplay</c>，选填）：0 开启、1 关闭，<b>默认关闭回放</b>（直播开始后允许开启）。</summary>
    [JsonPropertyName("closeReplay")]
    public long? CloseReplay { get; set; }

    /// <summary>是否关闭分享（<c>closeShare</c>，选填）：0 开启、1 关闭，<b>默认开启分享</b>（直播开始后<b>不允许修改</b>）。</summary>
    [JsonPropertyName("closeShare")]
    public long? CloseShare { get; set; }

    /// <summary>是否关闭客服（<c>closeKf</c>，选填）：0 开启、1 关闭，<b>默认关闭客服</b>（直播开始后允许开启）。</summary>
    [JsonPropertyName("closeKf")]
    public long? CloseKf { get; set; }
}
