// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 图文群发每日数据条目（官方 <c>getarticlesummary</c> 的 list 元素；官方声明<b>本接口已停止维护</b>，
/// 推荐替换 getarticleread / getarticleshare / getbizsummary / getarticletotaldetail）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpArticleSummaryItem
{
    /// <summary>获取或设置数据的日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置图文消息标识（官方 <c>msgid</c>：由 msg_data_id 与 index 组成）。</summary>
    [JsonPropertyName("msgid")]
    public string? MsgId { get; set; }

    /// <summary>获取或设置图文消息的标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置图文页阅读人数（官方 <c>int_page_read_user</c>）。</summary>
    [JsonPropertyName("int_page_read_user")]
    public int? IntPageReadUser { get; set; }

    /// <summary>获取或设置图文页阅读次数（官方 <c>int_page_read_count</c>）。</summary>
    [JsonPropertyName("int_page_read_count")]
    public int? IntPageReadCount { get; set; }

    /// <summary>获取或设置原文页阅读人数（官方 <c>ori_page_read_user</c>；无原文页为 0）。</summary>
    [JsonPropertyName("ori_page_read_user")]
    public int? OriPageReadUser { get; set; }

    /// <summary>获取或设置原文页阅读次数（官方 <c>ori_page_read_count</c>）。</summary>
    [JsonPropertyName("ori_page_read_count")]
    public int? OriPageReadCount { get; set; }

    /// <summary>获取或设置分享的人数（官方 <c>share_user</c>）。</summary>
    [JsonPropertyName("share_user")]
    public int? ShareUser { get; set; }

    /// <summary>获取或设置分享的次数（官方 <c>share_count</c>）。</summary>
    [JsonPropertyName("share_count")]
    public int? ShareCount { get; set; }

    /// <summary>获取或设置收藏的人数（官方 <c>add_to_fav_user</c>）。</summary>
    [JsonPropertyName("add_to_fav_user")]
    public int? AddToFavUser { get; set; }

    /// <summary>获取或设置收藏的次数（官方 <c>add_to_fav_count</c>）。</summary>
    [JsonPropertyName("add_to_fav_count")]
    public int? AddToFavCount { get; set; }
}
