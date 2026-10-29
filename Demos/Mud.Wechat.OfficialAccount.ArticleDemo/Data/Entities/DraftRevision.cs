// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Data;

/// <summary>
/// 草稿版本快照：每次<b>写微信成功之后</b>落一条，只读展示（方案 §7.3）。
/// 本地不建「草稿表」—— 草稿的唯一权威源是微信草稿箱，本表只记「写成功之后的样子」。
/// </summary>
public sealed class DraftRevision
{
    public int Id { get; set; }

    /// <summary>微信草稿 <c>media_id</c>（非空：只在写成功后落库，此刻官方必定已回发 id）。</summary>
    public string WechatMediaId { get; set; } = string.Empty;

    /// <summary>草稿内文章下标（官方 <c>index</c>，第一篇一号）。</summary>
    public int ItemIndex { get; set; }

    /// <summary>同一 <c>(WechatMediaId, ItemIndex)</c> 内自增的版本号。</summary>
    public int RevisionNo { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Author { get; set; }

    public string? Digest { get; set; }

    /// <summary>正文 HTML —— 含编辑者私密信息的可能性很高，见方案 §11.1：不写日志、不进遥测。</summary>
    public string? ContentHtml { get; set; }

    public string? ContentSourceUrl { get; set; }

    public string? ThumbMediaId { get; set; }

    public int? NeedOpenComment { get; set; }

    public int? OnlyFansCanComment { get; set; }

    /// <summary>来源：<c>Create</c>（新建草稿）/ <c>Update</c>（编辑草稿）。</summary>
    public string Source { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
}
