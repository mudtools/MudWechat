// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>
/// 查询授权信息（<c>POST /card/invoice/getauthdata</c>）响应。
/// </summary>
/// <remarks>
/// 官方文档缺陷（照录）：① 接口名为「查询授权信息」、描述为「查询订单是否有被用户授权」，但返回字段
/// <c>invoice_status</c> 的说明为「发票状态」、示例值语义更像授权状态，字段语义与接口用途不完全一致；
/// ② 请求参数<b>无</b> <c>card_id</c>，但错误码 <c>40078</c> 描述涉及 <c>card_id</c> 未授权（族级表复用）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceAuthDataResponse : MpResponse
{
    /// <summary>获取或设置发票状态（官方 <c>invoice_status</c>；官方未给枚举表，字段语义见类型 remarks）。</summary>
    [JsonPropertyName("invoice_status")]
    public string? InvoiceStatus { get; set; }

    /// <summary>获取或设置授权时间戳（官方 <c>auth_time</c>；官方未说明单位）。</summary>
    [JsonPropertyName("auth_time")]
    public long? AuthTime { get; set; }
}
