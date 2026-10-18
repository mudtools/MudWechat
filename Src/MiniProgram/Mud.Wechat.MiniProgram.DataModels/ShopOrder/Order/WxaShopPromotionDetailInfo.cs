// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 微信支付券信息（官方 <c>PromotionDetailInfo</c> 表，只有一支数组）。
/// </summary>
/// <remarks>
/// <para>官方该表「类型」列印为 <c>PromotionDetail[]</c> 且「是否必填」列印「是」——出现在返回表里，属官方表格串列，
/// 本仓按语义建模为可空数组（不臆造必填语义）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopPromotionDetailInfo
{
    /// <summary>promotion_detail_list（官方类型 <c>PromotionDetail[]</c>）：优惠明细列表。</summary>
    [JsonPropertyName("promotion_detail_list")]
    public List<WxaShopPromotionDetail>? PromotionDetailList { get; set; }
}
