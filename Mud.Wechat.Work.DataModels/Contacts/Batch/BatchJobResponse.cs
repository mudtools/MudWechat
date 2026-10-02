// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contacts.Batch;

/// <summary>
/// 异步导入任务受理响应体（<c>/cgi-bin/batch/syncuser</c>、<c>/cgi-bin/batch/replaceuser</c>、<c>/cgi-bin/batch/replaceparty</c> 共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Batch")]
public class BatchJobResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置异步任务 id（最大长度 64 字节；用于 <c>/cgi-bin/batch/getresult</c> 查询任务结果）。
    /// </summary>
    [JsonPropertyName("jobid")]
    public string? JobId { get; set; }
}
