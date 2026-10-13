// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 微信客服同步消息（<c>/cgi-bin/kf/sync_msg</c> 返回的 msg_list 元素）。
/// <para>
/// 官方为「一份扁平报文 + 按 <see cref="MessageType"/> 取用对应消息体」的形态：本类型同时承载
/// 全部消息类型的字段位，按 <see cref="MessageType"/> 判别哪个消息体非空，
/// 不做运行时多态（AOT 源生成按声明类型序列化）。
/// </para>
/// <para>
/// 消息体类型复用发送侧的 <see cref="KfMediaMsgBody"/>（image / voice / video / file）、
/// <see cref="KfMiniProgramMsgBody"/>、<see cref="KfMenuMsgBody"/>、<see cref="KfLocationMsgBody"/>
/// （官方字段面与发送侧完全一致），仅同步侧独有字段面的类型（text / link / business_card 等）单独建型。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfSyncMessage
{
    /// <summary>
    /// 获取或设置消息 ID（在客服账号内唯一，撤回与失败事件均以该值定位）。
    /// </summary>
    [JsonPropertyName("msgid")]
    public string? MessageId { get; set; }

    /// <summary>
    /// 获取或设置消息类型（text / image / voice / video / file / link / miniprogram / msgmenu /
    /// location / business_card / event / channels / channels_shop_product / channels_shop_order /
    /// merged_msg / meeting / calendar / note，取值表以官方 94670「消息类型」章节为准）。
    /// <para>官方口径：正文「支持的消息类型」一句只列了文本~事件共 11 类，但同页「消息类型」章节另给出
    /// 视频号商品 / 视频号订单 / 聊天记录 / 视频号 / 会议 / 日程 / 笔记的独立小节；其中
    /// <c>meeting</c> / <c>calendar</c> / <c>note</c> 三类官方标注「目前暂不返回详细消息内容」，
    /// 即这三类只有 msgtype 而没有消息体字段位。</para>
    /// </summary>
    [JsonPropertyName("msgtype")]
    public string? MessageType { get; set; }

    /// <summary>
    /// 获取或设置消息来源（官方 origin 枚举：3-微信客户发送的消息；4-系统推送的事件消息；5-接待人员在企业微信客户端发送的消息）。
    /// </summary>
    [JsonPropertyName("origin")]
    public int? Origin { get; set; }

    /// <summary>
    /// 获取或设置客服账号 ID（官方：<c>msgtype</c> 为 event 时该字段不返回）。
    /// </summary>
    [JsonPropertyName("open_kfid")]
    public string? OpenKfId { get; set; }

    /// <summary>
    /// 获取或设置接待人员的 userid（第三方 / 代开发应用获取到的是密文 userid 即 open_userid）。
    /// <para>官方口径：仅 <c>origin</c> 为 5（接待人员在企业微信客户端发送）时才返回；<c>msgtype</c> 为 event 时不返回。</para>
    /// </summary>
    [JsonPropertyName("servicer_userid")]
    public string? ServicerUserId { get; set; }

    /// <summary>
    /// 获取或设置客户的 external_userid（官方：<c>msgtype</c> 为 event 时该字段不返回）。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserId { get; set; }

    /// <summary>
    /// 获取或设置消息发送时间戳（秒级）。
    /// </summary>
    [JsonPropertyName("send_time")]
    public long? SendTime { get; set; }

    /// <summary>
    /// 获取或设置文本消息体（<see cref="MessageType"/> 为 text 时填充；同步侧比发送侧多回传
    /// 客户点击的菜单项 ID）。
    /// </summary>
    [JsonPropertyName("text")]
    public KfSyncTextMsgBody? Text { get; set; }

    /// <summary>
    /// 获取或设置图片消息体（<see cref="MessageType"/> 为 image 时填充）。
    /// </summary>
    [JsonPropertyName("image")]
    public KfMediaMsgBody? Image { get; set; }

    /// <summary>
    /// 获取或设置语音消息体（<see cref="MessageType"/> 为 voice 时填充）。
    /// </summary>
    [JsonPropertyName("voice")]
    public KfMediaMsgBody? Voice { get; set; }

    /// <summary>
    /// 获取或设置视频消息体（<see cref="MessageType"/> 为 video 时填充）。
    /// </summary>
    [JsonPropertyName("video")]
    public KfMediaMsgBody? Video { get; set; }

    /// <summary>
    /// 获取或设置文件消息体（<see cref="MessageType"/> 为 file 时填充）。
    /// </summary>
    [JsonPropertyName("file")]
    public KfMediaMsgBody? File { get; set; }

    /// <summary>
    /// 获取或设置链接消息体（<see cref="MessageType"/> 为 link 时填充；同步侧缩略图回传
    /// <c>pic_url</c>，与发送侧的 <c>thumb_media_id</c> 不同名）。
    /// </summary>
    [JsonPropertyName("link")]
    public KfSyncLinkMsgBody? Link { get; set; }

    /// <summary>
    /// 获取或设置小程序消息体（<see cref="MessageType"/> 为 miniprogram 时填充）。
    /// </summary>
    [JsonPropertyName("miniprogram")]
    public KfMiniProgramMsgBody? MiniProgram { get; set; }

    /// <summary>
    /// 获取或设置菜单消息体（<see cref="MessageType"/> 为 msgmenu 时填充）。
    /// </summary>
    [JsonPropertyName("msgmenu")]
    public KfMenuMsgBody? Menu { get; set; }

    /// <summary>
    /// 获取或设置位置消息体（<see cref="MessageType"/> 为 location 时填充）。
    /// </summary>
    [JsonPropertyName("location")]
    public KfLocationMsgBody? Location { get; set; }

    /// <summary>
    /// 获取或设置名片消息体（<see cref="MessageType"/> 为 business_card 时填充）。
    /// </summary>
    [JsonPropertyName("business_card")]
    public KfSyncBusinessCardMsgBody? BusinessCard { get; set; }

    /// <summary>
    /// 获取或设置事件消息体（<see cref="MessageType"/> 为 event 时填充）。
    /// </summary>
    [JsonPropertyName("event")]
    public KfSyncEventMsgBody? Event { get; set; }

    /// <summary>
    /// 获取或设置视频号消息体（<see cref="MessageType"/> 为 channels 时填充）。
    /// </summary>
    [JsonPropertyName("channels")]
    public KfSyncChannelsMsgBody? Channels { get; set; }

    /// <summary>
    /// 获取或设置视频号商品消息体（<see cref="MessageType"/> 为 channels_shop_product 时填充）。
    /// </summary>
    [JsonPropertyName("channels_shop_product")]
    public KfSyncChannelsShopProductMsgBody? ChannelsShopProduct { get; set; }

    /// <summary>
    /// 获取或设置视频号订单消息体（<see cref="MessageType"/> 为 channels_shop_order 时填充）。
    /// </summary>
    [JsonPropertyName("channels_shop_order")]
    public KfSyncChannelsShopOrderMsgBody? ChannelsShopOrder { get; set; }

    /// <summary>
    /// 获取或设置聊天记录消息体（<see cref="MessageType"/> 为 merged_msg 时填充）。
    /// </summary>
    [JsonPropertyName("merged_msg")]
    public KfSyncMergedMsgBody? MergedMessage { get; set; }
}
