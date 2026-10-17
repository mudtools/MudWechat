// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>获取某个带货者可推广文章列表（<c>promoter/get_talent_biz_article_list</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterGetTalentBizArticleListResponse : ChannelsResponse
{
    /// <summary>获取或设置下一页的请求参数（官方 <c>next_key</c>）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置是否还有下一页（官方 <c>has_more</c>）。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>获取或设置文章列表（官方 <c>article_list</c>）。</summary>
    [JsonPropertyName("article_list")]
    public List<ChannelsPromoterTalentArticleInfo>? ArticleList { get; set; }
}
