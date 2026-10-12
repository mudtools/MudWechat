// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Security;

/// <summary>文本内容安全识别应答（<c>POST /wxa/msg_sec_check</c>）。</summary>
/// <remarks>
/// <b>勿据 <c>errcode</c> 判合规</b>：<c>errcode = 0</c> 只表示调用成功，违规与否在
/// <see cref="Result"/>（<c>suggest</c>）与 <see cref="Detail"/> 中。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class WxaMsgSecCheckResponse : WxaResponse
{
    /// <summary>详细命中信息（<c>detail</c>，数组），见 <see cref="WxaSecCheckDetail"/>。</summary>
    [JsonPropertyName("detail")]
    public List<WxaSecCheckDetail>? Detail { get; set; }

    /// <summary>唯一请求标识（<c>trace_id</c>，排障与官方工单用）。</summary>
    [JsonPropertyName("trace_id")]
    public string? TraceId { get; set; }

    /// <summary>综合结论（<c>result</c>），见 <see cref="WxaSecCheckResult"/>。</summary>
    [JsonPropertyName("result")]
    public WxaSecCheckResult? Result { get; set; }
}
