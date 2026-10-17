// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>获取某个带货者可推广文章列表请求（官方 <c>promoter/get_talent_biz_article_list</c> 请求体）。</summary>
/// <remarks>官方契约：已兼容新老内容合作方式——后台先查「全部内容」合作，没有才查「部分内容」合作。</remarks>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterGetTalentBizArticleListRequest
{
    /// <summary>获取或设置带货者 appid（官方 <c>talent_appid</c>，必填）。</summary>
    [JsonPropertyName("talent_appid")]
    public string? TalentAppId { get; set; }

    /// <summary>获取或设置翻页参数（官方 <c>next_key</c>，必填，第一页为空，后续传上一页返回的值）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填，最大 10）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置推客 appid（官方 <c>sharer_appid</c>，必填，用于生成某个推客推广的 link）。</summary>
    [JsonPropertyName("sharer_appid")]
    public string? SharerAppId { get; set; }

    /// <summary>获取或设置公众号 appid（官方 <c>biz_appid</c>；全部内容合作时必填，部分内容合作方式可不填，获得方式见 <c>promoter/get_bind_talent_list</c>）。</summary>
    [JsonPropertyName("biz_appid")]
    public string? BizAppId { get; set; }
}
