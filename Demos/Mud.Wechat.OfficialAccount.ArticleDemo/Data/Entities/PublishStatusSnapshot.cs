// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Data;

/// <summary>发布状态时间线（1:n 挂 <see cref="PublishJob"/>，任务删除时级联）。</summary>
public sealed class PublishStatusSnapshot
{
    public int Id { get; set; }

    public int PublishJobId { get; set; }

    public PublishJob? PublishJob { get; set; }

    /// <summary>本轮 <c>publish_status</c>；官方未给出时为 null（此时 <see cref="ErrorCode"/> 有值）。</summary>
    public int? Status { get; set; }

    /// <summary>本轮官方 <c>errcode</c>（非零才记，用于排查「一直发布中」到底是哪种失败）。</summary>
    public int? ErrorCode { get; set; }

    public DateTimeOffset PolledAt { get; set; }

    /// <summary>本轮在所属任务内的序号（从 1 开始）。</summary>
    public int AttemptNo { get; set; }
}
