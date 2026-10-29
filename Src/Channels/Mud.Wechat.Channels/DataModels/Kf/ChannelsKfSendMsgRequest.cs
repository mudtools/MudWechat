// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Kf;

/// <summary>
/// 发送消息（<c>sendmsg</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：<c>msg_type</c> 必填并决定哪个内容对象必填（text / image / video / product_share /
/// order_share / file 五选一对应）；媒体类内容的 <c>cos_url</c> 须经「上传多媒体资源」接口获取。
/// <c>request_id</c> 填写时以该任务 id 去重。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class ChannelsKfSendMsgRequest
{
    /// <summary>获取或设置唯一任务 id（官方 <c>request_id</c>，选填；填写时以该任务 id 进行去重）。</summary>
    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }

    /// <summary>获取或设置用户 openid（官方 <c>open_id</c>，必填）。</summary>
    [JsonPropertyName("open_id")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置消息类型（官方 <c>msg_type</c>，必填，见 <see cref="ChannelsKfMsgTypes"/>）——
    /// 决定下方哪个内容对象必填。
    /// </summary>
    [JsonPropertyName("msg_type")]
    public string MsgType { get; set; } = string.Empty;

    /// <summary>获取或设置文本消息内容（官方 <c>text</c>；msg_type 为 text 时必填）。</summary>
    [JsonPropertyName("text")]
    public ChannelsKfTextContent? Text { get; set; }

    /// <summary>获取或设置图片消息内容（官方 <c>image</c>；msg_type 为 image 时必填）。</summary>
    [JsonPropertyName("image")]
    public ChannelsKfMediaContent? Image { get; set; }

    /// <summary>获取或设置视频消息内容（官方 <c>video</c>；msg_type 为 video 时必填）。</summary>
    [JsonPropertyName("video")]
    public ChannelsKfMediaContent? Video { get; set; }

    /// <summary>获取或设置商品卡片消息内容（官方 <c>product_share</c>；msg_type 为 product_share 时必填）。</summary>
    [JsonPropertyName("product_share")]
    public ChannelsKfProductShareContent? ProductShare { get; set; }

    /// <summary>获取或设置订单卡片消息内容（官方 <c>order_share</c>；msg_type 为 order_share 时必填）。</summary>
    [JsonPropertyName("order_share")]
    public ChannelsKfOrderShareContent? OrderShare { get; set; }

    /// <summary>获取或设置文件消息内容（官方 <c>file</c>；msg_type 为 file 时必填）。</summary>
    [JsonPropertyName("file")]
    public ChannelsKfMediaContent? File { get; set; }
}
