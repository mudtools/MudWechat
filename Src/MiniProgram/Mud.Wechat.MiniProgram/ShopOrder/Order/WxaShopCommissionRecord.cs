// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 分账记录（官方 <c>CommissionRecord</c> 表，按「商户号 + 商品」维度记账）。
/// </summary>
/// <remarks>
/// <para><b>官方值域极窄</b>：<c>account_type</c> 官方只列 0 商户、<c>commission_type</c> 官方只列 1 服务商且注明「目前仅支持 1」
/// ⇒ 不得按「支付侧账户类型」扩大取值。</para>
/// <para><b>敏感值</b>：<c>mchid</c>（商户号）不得写入日志、遥测或异常消息。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopCommissionRecord
{
    /// <summary>mchid（官方类型 <c>string</c>）：商户号（敏感，不得进日志）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>account_type（官方类型 <c>number</c>）：账户类型，官方仅列 0 商户。</summary>
    [JsonPropertyName("account_type")]
    public int? AccountType { get; set; }

    /// <summary>out_product_id（官方类型 <c>string</c>）：外部商品 spu id。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>out_sku_id（官方类型 <c>string</c>）：外部商品 sku id。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>amount（官方类型 <c>number</c>）：分账金额，单位分。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>commission_type（官方类型 <c>number</c>）：分账类型，官方注明「1 服务商，目前仅支持 1」。</summary>
    [JsonPropertyName("commission_type")]
    public int? CommissionType { get; set; }
}
