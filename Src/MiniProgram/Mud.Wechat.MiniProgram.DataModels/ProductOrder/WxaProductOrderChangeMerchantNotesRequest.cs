// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 修改订单备注请求体（<c>POST /product/order/change_merchant_notes</c>）。
/// </summary>
/// <remarks>
/// <para><b>覆盖式写入</b>：<c>merchant_notes</c> 整份替换原备注，漏传即清空？官方未述；</para>
/// <para>按「必填 + 覆盖」处理，宿主应自行读回再写。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductOrderChangeMerchantNotesRequest
{
    /// <summary>订单 ID（必填）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>备注内容（必填）。</summary>
    [JsonPropertyName("merchant_notes")]
    public string? MerchantNotes { get; set; }
}
