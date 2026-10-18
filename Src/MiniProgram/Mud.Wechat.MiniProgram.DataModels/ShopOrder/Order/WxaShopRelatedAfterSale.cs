// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 关联售后单标识对（官方 <c>RelatedAfterSale</c> 表：微信侧 id + 商家侧 id）。
/// </summary>
/// <remarks>
/// <para><b>两侧 id 类型不同</b>：<c>aftersale_id</c> 官方类型 <c>number</c>、<c>out_aftersale_id</c> 类型 <c>string</c>
/// ⇒ 不得为了「统一单号类型」把商家侧 id 数值化。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopRelatedAfterSale
{
    /// <summary>aftersale_id（官方类型 <c>number</c>）：微信侧售后单 id。</summary>
    [JsonPropertyName("aftersale_id")]
    public long? AftersaleId { get; set; }

    /// <summary>out_aftersale_id（官方类型 <c>string</c>）：商家侧售后单 id。</summary>
    [JsonPropertyName("out_aftersale_id")]
    public string? OutAftersaleId { get; set; }
}
