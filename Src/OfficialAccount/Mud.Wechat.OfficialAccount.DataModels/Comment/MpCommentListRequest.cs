// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Comment;

/// <summary>查看文章评论数据（<c>comment/list</c>）请求体。</summary>
/// <remarks>
/// <para>
/// <b>逐页核验修正</b>：参数形制为 <c>msg_data_id</c>/<c>index</c>/<c>begin</c>/<c>count</c>/<c>type</c>
/// （<b>非</b>方案文档预判的 article_id/begin/limit）；<c>count</c>「获取数目（&gt;=50 会被拒绝）」⇒ 上限 50
/// （官方错误码 88010 描述原文拼写「cout」——照录）。
/// </para>
/// <para><c>type</c>：0 普通评论 &amp; 精选评论 / 1 普通评论 / 2 精选评论（官方原文）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Comment")]
public class MpCommentListRequest
{
    /// <summary>获取或设置群发返回的 msg_data_id（官方 <c>msg_data_id</c>，必填）。</summary>
    [JsonPropertyName("msg_data_id")]
    public long MsgDataId { get; set; }

    /// <summary>获取或设置多图文序号（官方 <c>index</c>，从 0 开始；不带默认返回第一篇）。</summary>
    [JsonPropertyName("index")]
    public int? Index { get; set; }

    /// <summary>获取或设置起始位置（官方 <c>begin</c>，必填）。</summary>
    [JsonPropertyName("begin")]
    public int Begin { get; set; }

    /// <summary>获取或设置获取数目（官方 <c>count</c>，必填；&gt;=50 会被拒绝）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>获取或设置评论类型（官方 <c>type</c>，必填：0 全部 / 1 普通 / 2 精选）。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }
}
