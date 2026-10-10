// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 查询导出任务结果请求体（<c>/cgi-bin/wedoc/smartdoc/get_export_result</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class GetSmartDocExportResultRequest
{
    /// <summary>获取或设置异步任务 ID（官方 <c>task_id</c>，必填），由「导出内容块」的提交导出任务接口返回。</summary>
    [JsonPropertyName("task_id")]
    public string? TaskId { get; set; }
}
