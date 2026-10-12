// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 账号级概况详细数据（官方 <c>getbizsummary</c> 的 detail 对象；阅读/分享/赞/留言/收藏/跳转原文/发布篇数聚合）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpDataBizDetail
{
    /// <summary>获取或设置阅读人数（官方 <c>read_user</c>）。</summary>
    [JsonPropertyName("read_user")]
    public int? ReadUser { get; set; }

    /// <summary>获取或设置阅读数据来源分布（官方 <c>read_user_source</c>）。</summary>
    [JsonPropertyName("read_user_source")]
    public List<MpDataSceneCount>? ReadUserSource { get; set; }

    /// <summary>获取或设置分享人数（官方 <c>share_user</c>）。</summary>
    [JsonPropertyName("share_user")]
    public int? ShareUser { get; set; }

    /// <summary>获取或设置爱心赞人数（官方 <c>zaikan_user</c>——官方拼写 zaikan，勿「修正」）。</summary>
    [JsonPropertyName("zaikan_user")]
    public int? ZaiKanUser { get; set; }

    /// <summary>获取或设置拇指赞人数（官方 <c>like_user</c>）。</summary>
    [JsonPropertyName("like_user")]
    public int? LikeUser { get; set; }

    /// <summary>获取或设置留言条数（官方 <c>comment_count</c>）。</summary>
    [JsonPropertyName("comment_count")]
    public int? CommentCount { get; set; }

    /// <summary>获取或设置微信收藏人数（官方 <c>collection_user</c>）。</summary>
    [JsonPropertyName("collection_user")]
    public int? CollectionUser { get; set; }

    /// <summary>获取或设置跳转原文人数（官方 <c>redirect_ori_page_user</c>）。</summary>
    [JsonPropertyName("redirect_ori_page_user")]
    public int? RedirectOriPageUser { get; set; }

    /// <summary>获取或设置发布篇数（官方 <c>send_page_count</c>）。</summary>
    [JsonPropertyName("send_page_count")]
    public int? SendPageCount { get; set; }
}
