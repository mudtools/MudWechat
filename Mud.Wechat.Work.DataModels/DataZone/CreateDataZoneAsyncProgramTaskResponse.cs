// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DataZone;

/// <summary>
/// 创建专区程序调用任务（应用异步调用专区程序）响应体（<c>/cgi-bin/chatdata/async_program_task</c>）。
/// <para>任务完成结果须凭 jobid 调用「获取专区程序任务结果」查询。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataZone")]
public class CreateDataZoneAsyncProgramTaskResponse : WechatWorkResponse
{
    /// <summary>获取或设置任务 id（官方 jobid；用于查询专区程序任务结果）。</summary>
    [JsonPropertyName("jobid")]
    public string? Jobid { get; set; }
}
