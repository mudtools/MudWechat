// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DataZone;

/// <summary>
/// 应用同步调用专区程序请求体（<c>/cgi-bin/chatdata/sync_call_program</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataZone")]
public class SyncCallDataZoneProgramRequest
{
    /// <summary>获取或设置应用关联的程序 id（官方必填）。</summary>
    [JsonPropertyName("program_id")]
    public string? ProgramId { get; set; }

    /// <summary>获取或设置程序关联的能力 id（官方必填）。</summary>
    [JsonPropertyName("ability_id")]
    public string? AbilityId { get; set; }

    /// <summary>获取或设置通知 id（官方选填；由「专区通知应用」返回）。</summary>
    [JsonPropertyName("notify_id")]
    public string? NotifyId { get; set; }

    /// <summary>
    /// 获取或设置请求的输入 JSON（官方必填），要求与配置的输入协议格式匹配。
    /// <para>官方示例形如 <c>"{\"input\":\"xxx\"}"</c>；如使用专区程序示例，留意 Java 版本 demo 的输入无需在
    /// request_data 内包裹一层 input 对象。</para>
    /// </summary>
    [JsonPropertyName("request_data")]
    public string? RequestData { get; set; }
}
