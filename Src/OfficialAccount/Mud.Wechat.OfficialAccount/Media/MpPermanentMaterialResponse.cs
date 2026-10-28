// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Media;

/// <summary>
/// 获取永久素材（<c>material/get_material</c>）的 JSON 形态响应（图文 / 视频两种形态的字段超集）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<b>POST</b> + 请求体 <c>{"media_id":…}</c>；
/// 响应按素材类型分流——<b>图文</b>返回 <c>news_item</c> 数组；<b>视频</b>返回
/// <c>title</c>/<c>description</c>/<c>down_url</c>；<b>图片 / 语音</b>官方原文
/// 「响应的直接为素材的内容，开发者可以自行保存为文件」（二进制，不走本 DTO——
/// 由 <c>IMpMediaDownloadService.GetPermanentMaterialAsync</c> 的下载信封承载）。
/// </para>
/// <para>
/// <b>形态判定</b>：<c>NewsItems != null</c> ⇒ 图文；<c>VideoDownUrl != null</c> ⇒ 视频。
/// </para>
/// <para>
/// <b>官方文档矛盾（照录）</b>：视频返回示例 JSON 末尾带尾逗号且值为裸占位符无引号（页面原文如此）；
/// 本页 <c>news_item</c> 无 <c>need_open_comment</c>/<c>only_fans_can_comment</c> 字段（旧版文档有，
/// 新版页面不存在——SDK 按新版页面建模）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpPermanentMaterialResponse
{
    /// <summary>获取或设置图文素材内容（官方 <c>news_item</c>；<b>仅图文素材返回</b>）。</summary>
    [JsonPropertyName("news_item")]
    public List<MpMaterialNewsItem>? NewsItems { get; set; }

    /// <summary>获取或设置视频素材标题（官方 <c>title</c>；<b>仅视频素材返回</b>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置视频素材描述（官方 <c>description</c>；<b>仅视频素材返回</b>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置视频下载地址（官方 <c>down_url</c>；<b>仅视频素材返回</b>）。</summary>
    [JsonPropertyName("down_url")]
    public string? DownUrl { get; set; }
}
