// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.FreePublish;

/// <summary>发布状态查询（<c>freepublish/get</c>）响应。</summary>
/// <remarks>
/// <b>publish_status 为标量 number</b>（逐页核验修正：方案文档预判「publish_status 数组」不成立；
/// 数组是 article_detail.item 与 fail_idx）。取值：0 成功 / 1 发布中 / 2 原创失败 / 3 常规失败 /
/// 4 平台审核不通过 / 5 成功后用户删除所有文章 / 6 成功后系统封禁所有文章。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "FreePublish")]
public class MpFreePublishGetResponse : MpResponse
{
    /// <summary>获取或设置发布任务 id（官方 <c>publish_id</c>）。</summary>
    [JsonPropertyName("publish_id")]
    public string? PublishId { get; set; }

    /// <summary>获取或设置发布状态（官方 <c>publish_status</c>：0~6，见类型 remarks）。</summary>
    [JsonPropertyName("publish_status")]
    public int? PublishStatus { get; set; }

    /// <summary>获取或设置成功时的图文 article_id（官方 <c>article_id</c>）。</summary>
    [JsonPropertyName("article_id")]
    public string? ArticleId { get; set; }

    /// <summary>获取或设置文章详情（官方 <c>article_detail</c>；发布状态为 0（成功）时返回）。</summary>
    [JsonPropertyName("article_detail")]
    public MpFreePublishArticleDetail? ArticleDetail { get; set; }

    /// <summary>获取或设置失败文章编号列表（官方 <c>fail_idx</c>）。</summary>
    [JsonPropertyName("fail_idx")]
    public List<long>? FailIdx { get; set; }
}
