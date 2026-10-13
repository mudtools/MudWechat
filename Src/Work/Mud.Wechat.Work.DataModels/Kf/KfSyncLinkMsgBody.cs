// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 微信客服同步消息的链接消息体（<c>/cgi-bin/kf/sync_msg</c> 的 link 消息）。
/// <para>
/// 与发送侧 <see cref="KfLinkMsgBody"/> 的差异：缩略图同步侧回传 URL 字段 <c>pic_url</c>，
/// 发送侧入参为临时素材 <c>thumb_media_id</c>，两者字段名与形态均不同，不得合并为一套契约面。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfSyncLinkMsgBody
{
    /// <summary>
    /// 获取或设置消息标题。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置消息描述（官方字段名为 desc）。
    /// </summary>
    [JsonPropertyName("desc")]
    public string? Description { get; set; }

    /// <summary>
    /// 获取或设置链接地址。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 获取或设置图文封面的 URL（官方字段名为 pic_url）。
    /// </summary>
    [JsonPropertyName("pic_url")]
    public string? PicUrl { get; set; }
}
