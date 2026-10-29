// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Data;

/// <summary>
/// 发布任务台账（方案 §7.3）：官方无 <c>PUBLISHJOBFINISH</c> 事件可用，只能轮询
/// ⇒ 把 <c>publish_id</c> 与最新状态落库，进程重启后仍可从 <c>/api/articles/jobs</c> 续查。
/// </summary>
public sealed class PublishJob
{
    public int Id { get; set; }

    /// <summary>官方发布任务 id（台账唯一键）。</summary>
    public string PublishId { get; set; } = string.Empty;

    /// <summary>来源草稿的 <c>media_id</c>；对「本 Demo 未提交过、仅被单独查询过状态」的任务为空串。</summary>
    public string WechatMediaId { get; set; } = string.Empty;

    /// <summary>最近一次 <c>publish_status</c>（0~6，官方原样），提交后未轮询前为 null。</summary>
    public int? LatestStatus { get; set; }

    public string? ArticleId { get; set; }

    /// <summary>官方 <c>fail_idx</c> 数组，存 JSON 文本（§7.3：减少 Provider 行为差异，读取时手工反序列化）。</summary>
    public string? FailIdxJson { get; set; }

    public DateTimeOffset SubmittedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>累计轮询次数。</summary>
    public int Attempts { get; set; }

    public List<PublishStatusSnapshot> Snapshots { get; } = new();
}
