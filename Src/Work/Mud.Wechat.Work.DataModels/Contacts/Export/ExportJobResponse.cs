// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contacts.Export;

/// <summary>
/// 异步导出任务受理响应体（<c>/cgi-bin/export/simple_user</c>、<c>/cgi-bin/export/user</c>、
/// <c>/cgi-bin/export/department</c>、<c>/cgi-bin/export/taguser</c> 共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Export")]
public class ExportJobResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置任务 ID（用于 <c>/cgi-bin/export/get_result</c> 查询任务结果；
    /// 亦可等待「导出任务完成通知」回调事件 <c>batch_job_result</c> 推送）。
    /// </summary>
    [JsonPropertyName("jobid")]
    public string? JobId { get; set; }
}
