// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 客服族（客服消息事件，多消息结构互斥形态）。
// 事件键：commkf_send_msg_to_kf。
// 建模说明：msg_type / msg_event 官方参数表标 object，但实为判别值（「参考发送消息中的消息类型」
// 「当 msg_event 为 EVENT_TYPE_EVALUATE_RESULT 时」；Body.msg_type / Body.msg_event 子表只是把顶层
// 同名字段重复罗列，为官方文档格式噪音）——按本仓 P2 客服 DTO 先例建模为 string。
// 本事件官方页面无代码示例。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 客服消息事件回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.KfMsgEvent"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 当微信客户、接待人员发消息或有行为动作时，商家客服后台将事件回调数据包发送到指定 URL。
/// 各消息结构（<c>text</c>/<c>video</c>/<c>image</c>/<c>file</c>/<c>product_share</c>/<c>order_share</c>/
/// <c>voice</c>/<c>reference</c>/<c>evaluate_result</c>）按 <c>msg_type</c>/<c>msg_event</c> 取值互斥出现。
/// </para>
/// <para>
/// <b>建模裁决</b>：<c>msg_type</c>/<c>msg_event</c> 官方参数表标 object，实为判别值，按本仓
/// P2 客服 DTO 先例建模为 string（详见文件头说明）。<c>evaluate_result.reason_list</c> 官方未标注
/// 元素类型，按字符串列表建模。本事件官方页面无代码示例。
/// </para>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）。重试：回包不符合预期时重试推送，总重试约 3.5 小时，共 10 次。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/kf_callback/kf_msg_event.html">客服消息事件</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.KfMsgEvent })]
public sealed class ChannelsKfMsgEventPayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置小店用户的 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>commkf_send_msg_to_kf</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置用户发送该条消息所捆绑的单调自增 msg_id（官方 <c>msg_id</c>，官方类型为 string）。</summary>
    [JsonPropertyName("msg_id")]
    public string? MsgId { get; set; }

    /// <summary>获取或设置消息类型判别值（官方 <c>msg_type</c>，如 text/video/image/file/product_share/order_share/voice/reference；参数表标 object，实为判别值，建模 string）。</summary>
    [JsonPropertyName("msg_type")]
    public string? MsgTypeValue { get; set; }

    /// <summary>获取或设置事件类型判别值（官方 <c>msg_event</c>，如 EVENT_TYPE_SEND_MSG / EVENT_TYPE_EVALUATE_RESULT；参数表标 object，实为判别值，建模 string）。</summary>
    [JsonPropertyName("msg_event")]
    public string? MsgEvent { get; set; }

    /// <summary>获取或设置文本消息结构（官方 <c>text</c>，msg_type 为 text 时返回）。</summary>
    [JsonPropertyName("text")]
    public ChannelsKfTextInfo? Text { get; set; }

    /// <summary>获取或设置视频消息结构（官方 <c>video</c>，msg_type 为 video 时返回）。</summary>
    [JsonPropertyName("video")]
    public ChannelsKfVideoInfo? Video { get; set; }

    /// <summary>获取或设置图片消息结构（官方 <c>image</c>，msg_type 为 image 时返回；与 file 共用媒体结构）。</summary>
    [JsonPropertyName("image")]
    public ChannelsKfMediaInfo? Image { get; set; }

    /// <summary>获取或设置文件消息结构（官方 <c>file</c>，msg_type 为 file 时返回；与 image 共用媒体结构）。</summary>
    [JsonPropertyName("file")]
    public ChannelsKfMediaInfo? File { get; set; }

    /// <summary>获取或设置商品卡片消息结构（官方 <c>product_share</c>，msg_type 为 product_share 时返回）。</summary>
    [JsonPropertyName("product_share")]
    public ChannelsKfProductShareInfo? ProductShare { get; set; }

    /// <summary>获取或设置订单卡片消息结构（官方 <c>order_share</c>，msg_type 为 order_share 时返回）。</summary>
    [JsonPropertyName("order_share")]
    public ChannelsKfOrderShareInfo? OrderShare { get; set; }

    /// <summary>获取或设置语音消息结构（官方 <c>voice</c>，msg_type 为 voice 时返回）。</summary>
    [JsonPropertyName("voice")]
    public ChannelsKfVoiceInfo? Voice { get; set; }

    /// <summary>获取或设置用户 unionid（官方 <c>union_id</c>）。</summary>
    [JsonPropertyName("union_id")]
    public string? UnionId { get; set; }

    /// <summary>获取或设置引用消息结构（官方 <c>reference</c>，msg_type 为 reference 时返回）。</summary>
    [JsonPropertyName("reference")]
    public ChannelsKfReferenceInfo? Reference { get; set; }

    /// <summary>获取或设置满意度评价结果结构（官方 <c>evaluate_result</c>，msg_event 为 EVENT_TYPE_EVALUATE_RESULT 时返回）。</summary>
    [JsonPropertyName("evaluate_result")]
    public ChannelsKfEvaluateResultInfo? EvaluateResult { get; set; }
}
