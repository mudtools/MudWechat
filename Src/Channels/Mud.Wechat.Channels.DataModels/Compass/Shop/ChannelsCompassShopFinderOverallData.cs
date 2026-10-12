// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Compass;

/// <summary>带货数据概览（官方 <c>data</c> 对象，<c>getshopfinderoverall</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopFinderOverallData
{
    /// <summary>获取或设置成交金额（官方 <c>pay_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("pay_gmv")]
    public long? PayGmv { get; set; }

    /// <summary>获取或设置动销达人数（官方 <c>pay_sales_finder_cnt</c>）。</summary>
    [JsonPropertyName("pay_sales_finder_cnt")]
    public long? PaySalesFinderCnt { get; set; }

    /// <summary>获取或设置动销商品数（官方 <c>pay_product_id_cnt</c>）。</summary>
    [JsonPropertyName("pay_product_id_cnt")]
    public long? PayProductIdCnt { get; set; }

    /// <summary>获取或设置点击-成交转化率（官方 <c>click_to_pay_uv_ratio</c>）。</summary>
    [JsonPropertyName("click_to_pay_uv_ratio")]
    public double? ClickToPayUvRatio { get; set; }
}
