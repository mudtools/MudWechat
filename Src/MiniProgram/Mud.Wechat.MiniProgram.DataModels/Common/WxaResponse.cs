// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels;

/// <summary>
/// 微信小程序统一响应基底（<c>errcode</c> / <c>errmsg</c> 错误信封）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与公众号 <c>MpResponse</c> 同形但不共用</b>：两者错误信封字段完全一致（<c>errcode</c> + <c>errmsg</c>），
/// 但小程序线<b>不得引用公众号 DataModels</b>（设计方案 §3.5 硬边界：只引用其 <b>Abstractions</b> 的令牌/多应用基座）。
/// 且 <c>JsonSerializerContext</c> 是按程序集生成的 —— 跨程序集复用 DTO 会让序列化元数据落错上下文。
/// 故本线自带一份同形基底（各产品线各持其响应基底，是本仓既定形态）。
/// </para>
/// <para>
/// <b>成功响应可能整体缺省 <c>errcode</c></b>（官方多页如此）⇒ <see cref="ErrorCode"/> 缺省 <c>0</c> 即视为成功。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Common")]
public class WxaResponse : IWechatApiResponse
{
    /// <summary>错误码（<c>errcode</c>，<c>0</c> 表示成功；成功响应可能整体缺省本字段）。</summary>
    [JsonPropertyName("errcode")]
    public virtual int ErrorCode { get; set; }

    /// <summary>错误信息（<c>errmsg</c>）。</summary>
    [JsonPropertyName("errmsg")]
    public virtual string? ErrorMessage { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    public bool IsSuccess => ErrorCode == 0;
}
