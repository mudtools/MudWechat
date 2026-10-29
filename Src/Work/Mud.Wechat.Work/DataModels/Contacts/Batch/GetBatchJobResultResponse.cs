// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contacts.Batch;

/// <summary>
/// 获取异步任务结果响应体（<c>/cgi-bin/batch/getresult</c>；只能查询已经提交过的历史任务）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Batch")]
public class GetBatchJobResultResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置任务状态：1 任务开始、2 任务进行中、3 任务已完成。
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>
    /// 获取或设置操作类型：<c>sync_user</c>（增量更新成员）、<c>replace_user</c>（全量覆盖成员）、
    /// <c>replace_party</c>（全量覆盖部门）、<c>invite_user</c>（邀请成员关注）。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置任务运行总条数。
    /// </summary>
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    /// <summary>
    /// 获取或设置目前运行百分比（任务完成时为 100）。
    /// </summary>
    [JsonPropertyName("percentage")]
    public int? Percentage { get; set; }

    /// <summary>
    /// 获取或设置预估剩余时间（单位：分钟；任务完成时为 0）。
    /// </summary>
    [JsonPropertyName("remaintime")]
    public int? RemainTime { get; set; }

    /// <summary>
    /// 获取或设置详细的处理结果（任务完成后此字段有效；元素形状按 <see cref="Type"/> 区分，
    /// 见 <see cref="BatchTaskResultItem"/>）。
    /// </summary>
    [JsonPropertyName("result")]
    public List<BatchTaskResultItem>? Result { get; set; } = [];
}
