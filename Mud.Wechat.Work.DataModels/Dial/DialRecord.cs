// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Dial;

/// <summary>
/// 公费电话单条拨打记录（<c>/cgi-bin/dial/get_dial_record</c> 响应内嵌结构）。
/// </summary>
/// <remarks>
/// <para>
/// 应用可见范围外用户相关的记录会被官方过滤，不会返回。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Dial")]
public class DialRecord
{
    /// <summary>获取或设置拨出时间戳（官方 call_time）。</summary>
    [JsonPropertyName("call_time")]
    public long? CallTime { get; set; }

    /// <summary>获取或设置总通话时长（官方 total_duration，单位为分钟；单人通话等于单人通话时长，多人通话等于包括主叫用户在内的每个接入用户的通话时长之和）。</summary>
    [JsonPropertyName("total_duration")]
    public long? TotalDuration { get; set; }

    /// <summary>获取或设置通话类型（官方 call_type）：1 - 单人通话；2 - 多人通话。</summary>
    [JsonPropertyName("call_type")]
    public long? CallType { get; set; }

    /// <summary>获取或设置主叫用户信息（官方 caller，结构见 <see cref="DialCaller"/>）。</summary>
    [JsonPropertyName("caller")]
    public DialCaller? Caller { get; set; }

    /// <summary>获取或设置被叫用户信息列表（官方 callee，结构见 <see cref="DialCallee"/>）。</summary>
    [JsonPropertyName("callee")]
    public List<DialCallee>? Callee { get; set; }
}
