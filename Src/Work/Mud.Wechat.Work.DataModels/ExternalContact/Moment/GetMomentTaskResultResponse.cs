// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Moment;

/// <summary>
/// 获取任务创建结果响应体（<c>/cgi-bin/externalcontact/get_moment_task_result</c>）。
/// <para>只能查询已经提交过的历史任务；任务创建完成后 <see cref="Result"/> 才有效。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class GetMomentTaskResultResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置任务状态：1 - 开始创建，2 - 正在创建中，3 - 创建任务已完成。
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>
    /// 获取或设置操作类型（此处固定为 <c>add_moment_task</c>）。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置任务的详细处理结果（仅任务创建完成后有效）。
    /// </summary>
    [JsonPropertyName("result")]
    public MomentTaskResult? Result { get; set; }
}
