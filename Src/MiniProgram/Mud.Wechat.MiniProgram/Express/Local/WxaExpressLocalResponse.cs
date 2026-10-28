// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>
/// 即时配送响应<b>基类</b>：承载官方「运力返回」的 <c>resultcode</c> / <c>resultmsg</c>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何不直接用 <see cref="WxaResponse"/></b>：即时配送（<c>/cgi-bin/express/local/*</c>）全族
/// 官方返回体用 <c>resultcode</c> / <c>resultmsg</c> 表达运力侧结果，<b>而非</b> <c>errcode</c> / <c>errmsg</c>
/// （官方各页「返回体 Response Payload」2026-10-12 逐页核验）。本类型在 <see cref="WxaResponse"/> 之上
/// 补齐这两个字段，使调用方既能走通用信封、也能读运力侧结果码。
/// </para>
/// <para><b>判错口径</b>：<c>resultcode</c> 为 <c>0</c> 表示运力侧成功；非 0 时以 <c>resultmsg</c> 为准。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalResponse : WxaResponse
{
    /// <summary>运力返回的错误码（<c>resultcode</c>；<c>0</c> 表示成功）。</summary>
    [JsonPropertyName("resultcode")]
    public int ResultCode { get; set; }

    /// <summary>运力返回的错误描述（<c>resultmsg</c>）。</summary>
    [JsonPropertyName("resultmsg")]
    public string? ResultMessage { get; set; }
}
