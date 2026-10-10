// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Emergency;

/// <summary>
/// 自动语音来电呼叫状态（发起语音电话响应 <c>states</c> 列表元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Emergency")]
public class EmergencyCallState
{
    /// <summary>获取或设置呼叫结果状态：0 表示成功发起呼叫，非 0 表示失败。</summary>
    [JsonPropertyName("code")]
    public int? Code { get; set; }

    /// <summary>获取或设置唯一标识一通呼叫的 id（后续查询接听状态时作为 callid 传入）。</summary>
    [JsonPropertyName("callid")]
    public string? Callid { get; set; }

    /// <summary>获取或设置被呼叫人 userid。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }
}
