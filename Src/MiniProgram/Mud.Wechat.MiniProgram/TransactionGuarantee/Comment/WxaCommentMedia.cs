// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 评价媒体项（<c>media[]</c>；官方原文「有 <c>img</c> 的时候没有 <c>video</c> / <c>videoCover</c> / <c>videoDuration</c>」）。
/// </summary>
/// <remarks>
/// <para>
/// <b>三处共用</b>：列表侧 <c>commentList[].content.media</c>、详情侧 <c>info.content.content.media</c>
/// 与旧评价 <c>oldComment.content.media</c> 的官方参数表逐字段一致（官方原文亦称「跟上面提到的媒体结构一致」）
/// ⇒ 一份类型覆盖三处。
/// </para>
/// <para><b>cdn 链接有时效</b>：官方对本域图片资源统一提示存在过期时间，需重新调用接口获取，不得持久化存储 URL。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentMedia
{
    /// <summary>图片 cdn（<c>img</c>）。</summary>
    [JsonPropertyName("img")]
    public string? Img { get; set; }

    /// <summary>缩略图 cdn（<c>thumbImg</c>）。</summary>
    [JsonPropertyName("thumbImg")]
    public string? ThumbImg { get; set; }

    /// <summary>视频资源 cdn（<c>video</c>；有 <c>img</c> 时无本字段）。</summary>
    [JsonPropertyName("video")]
    public string? Video { get; set; }

    /// <summary>视频封面缩略图 cdn（<c>videoCover</c>；有 <c>img</c> 时无本字段）。</summary>
    [JsonPropertyName("videoCover")]
    public string? VideoCover { get; set; }

    /// <summary>视频时长（<c>videoDuration</c>，<b>单位秒</b>；有 <c>img</c> 时无本字段）。</summary>
    [JsonPropertyName("videoDuration")]
    public long? VideoDuration { get; set; }
}
