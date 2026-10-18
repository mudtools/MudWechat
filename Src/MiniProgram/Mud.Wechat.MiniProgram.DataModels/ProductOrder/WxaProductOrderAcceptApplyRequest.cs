// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 同意售后请求体（<c>POST /product/order/acceptapply</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>order_id</c> 是售后单号而非订单号</b>：官方示例注释明写「售后单号」⇒ 字段语义与名字相反（照录）。</para>
/// <para><c>address_id</c> 为同意退货时必传的地址簿 ID（走 <c>/product/address/*</c> 维护）。</para>
/// <para><b>官方本页无参数表</b>：请求字段只来自示例 ⇒ SDK 按示例建模、不做额外必填收紧。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductOrderAcceptApplyRequest
{
    /// <summary>售后单号（官方示例注释；非订单号）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>同意退货时传入的地址 ID。</summary>
    [JsonPropertyName("address_id")]
    public long? AddressId { get; set; }
}
