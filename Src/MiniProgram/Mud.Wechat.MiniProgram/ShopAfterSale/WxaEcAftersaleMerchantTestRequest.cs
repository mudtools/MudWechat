// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// 售后超时模拟请求体（<c>POST /shop/ecaftersale/merchanttest</c>）。
/// </summary>
/// <remarks>
/// <para><b>字段名官方自相矛盾（照录并裁决）</b>：参数表把操作键印成 <c>operatoin</c>（拼写错误），</para>
/// <para>而请求示例写 <c>operation</c> ⇒ 本类取<b>示例拼写</b> <c>operation</c>（示例是可直接发送的报文，表列疑为文档笔误），</para>
/// <para>且不在注释中臆造其它别名。可用值：<c>aftersale_user_upload_return_info_overdue</c> /</para>
/// <para><c>aftersale_user_apply_platform_judge_overdue</c> / <c>aftersale_merchant_handle_return_overdue</c> /</para>
/// <para><c>aftersale_merchant_handle_refund_overdue</c> / <c>aftersale_merchant_confirm_receipt_overdue</c>。</para>
/// <para><b>是模拟器不是业务写口</b>：用于联调超时流转，生产环境须禁用。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaEcAftersaleMerchantTestRequest
{
    /// <summary>商家自定义售后 ID（与 <c>aftersale_id</c> 二选一）。</summary>
    [JsonPropertyName("out_aftersale_id")]
    public string? OutAftersaleId { get; set; }

    /// <summary>微信侧售后单号（与 <c>out_aftersale_id</c> 二选一）。</summary>
    [JsonPropertyName("aftersale_id")]
    public long? AftersaleId { get; set; }

    /// <summary>要触发的超时操作（必填；官方表误印为 <c>operatoin</c>）。</summary>
    [JsonPropertyName("operation")]
    public string? Operation { get; set; }
}
