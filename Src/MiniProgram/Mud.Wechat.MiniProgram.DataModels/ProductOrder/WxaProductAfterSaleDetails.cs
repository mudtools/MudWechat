// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 售后详情（<c>details</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方标注「没用」的字段照录</b>：<c>time</c> 与 <c>receive_product</c> 在示例注释里明确写「没用」</para>
/// <para>⇒ 本类保留但宿主不应依赖。<c>time</c> 与 <c>receive_product</c> 官方示例为数字。</para>
/// <para><b>敏感值</b>：<c>desc</c>、<c>prove_imgs</c> 与 <c>tel_number</c> 属个人信息，禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductAfterSaleDetails
{
    /// <summary>退款数量。</summary>
    [JsonPropertyName("num")]
    public long? Num { get; set; }

    /// <summary>退款描述（敏感）。</summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; set; }

    /// <summary>官方标注「没用」。</summary>
    [JsonPropertyName("time")]
    public long? Time { get; set; }

    /// <summary>官方标注「没用」。</summary>
    [JsonPropertyName("receive_product")]
    public long? ReceiveProduct { get; set; }

    /// <summary>取消时间。</summary>
    [JsonPropertyName("cancel_time")]
    public long? CancelTime { get; set; }

    /// <summary>用户上传的凭证。</summary>
    [JsonPropertyName("prove_imgs")]
    public List<string>? ProveImgs { get; set; }

    /// <summary>用户电话（敏感）。</summary>
    [JsonPropertyName("tel_number")]
    public string? TelNumber { get; set; }
}
