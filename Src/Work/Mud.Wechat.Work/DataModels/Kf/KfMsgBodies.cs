// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 微信客服文本消息体（<c>/cgi-bin/kf/send_msg</c> 与 <c>/cgi-bin/kf/send_msg_on_event</c> 的 text 消息）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfTextMsgBody
{
    /// <summary>
    /// 获取或设置消息内容（官方必填，最长不超过 2048 个字节，超过将截断）。
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

/// <summary>
/// 微信客服媒体消息体（image / voice / video / file 四类消息共用的 media_id 载体，
/// 素材通过上传临时素材接口获取）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfMediaMsgBody
{
    /// <summary>
    /// 获取或设置媒体文件 id（官方必填，调用上传临时素材接口获取）。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }
}

/// <summary>
/// 微信客服链接消息体（link 消息）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfLinkMsgBody
{
    /// <summary>
    /// 获取或设置消息标题（官方必填，不超过 128 个字节，超过将截断）。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置消息描述（不超过 512 个字节，超过将截断）。
    /// </summary>
    [JsonPropertyName("desc")]
    public string? Description { get; set; }

    /// <summary>
    /// 获取或设置链接地址（官方必填，最长不超过 2048 个字节，须包含 http / https 协议头）。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 获取或设置缩略图的临时素材 media_id（官方必填，通过素材管理接口获得）。
    /// </summary>
    [JsonPropertyName("thumb_media_id")]
    public string? ThumbMediaId { get; set; }
}

/// <summary>
/// 微信客服小程序消息体（miniprogram 消息）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfMiniProgramMsgBody
{
    /// <summary>
    /// 获取或设置小程序 appid（官方必填）。
    /// </summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>
    /// 获取或设置小程序消息的标题（最多 64 个字节，超过将截断）。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置小程序消息封面的临时素材 media_id（官方必填，封面图片建议尺寸为 520*416）。
    /// </summary>
    [JsonPropertyName("thumb_media_id")]
    public string? ThumbMediaId { get; set; }

    /// <summary>
    /// 获取或设置小程序消息的页面路径（官方必填，须以 .html 为后缀）。
    /// </summary>
    [JsonPropertyName("pagepath")]
    public string? PagePath { get; set; }
}

/// <summary>
/// 微信客服位置消息体（location 消息）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfLocationMsgBody
{
    /// <summary>
    /// 获取或设置位置名。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置地址详情。
    /// </summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>
    /// 获取或设置纬度（官方必填，浮点数，范围为 -90~90，负数表示南纬）。
    /// </summary>
    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    /// <summary>
    /// 获取或设置经度（官方必填，浮点数，范围为 -180~180，负数表示西经）。
    /// </summary>
    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }
}

/// <summary>
/// 微信客服获客链接消息体（ca_link 消息）。
/// <para>
/// 通过获客链接（ca_link）名片打开的链接不计入获客助手「打开链接的客户数」统计。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfCaLinkMsgBody
{
    /// <summary>
    /// 获取或设置获客链接（官方必填，通过获客助手创建，形如 <c>https://work.weixin.qq.com/ca/xxxxxx</c>）。
    /// </summary>
    [JsonPropertyName("link_url")]
    public string? LinkUrl { get; set; }
}
