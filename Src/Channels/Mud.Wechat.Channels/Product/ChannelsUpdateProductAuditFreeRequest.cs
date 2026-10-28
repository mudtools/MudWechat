// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// 更新商品免审（<c>product/auditfree</c>）请求体 —— 部分字段更新（sku / 库存 / 运费 / 限购等免审）。
/// </summary>
/// <remarks>
/// <para>官方契约：<b>POST</b> + 请求体；该接口仅对曾经上架成功过的商品适用（草稿状态非审核中，<c>edit_status != 2</c>）。</para>
/// <para>本地生活商品不受该规则约束。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsUpdateProductAuditFreeRequest
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>，平台生成的 id）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置商品编码（官方 <c>spu_code</c>）。</summary>
    [JsonPropertyName("spu_code")]
    public string? SpuCode { get; set; }

    /// <summary>获取或设置需要进行更新的 sku 列表（官方 <c>skus</c>）。</summary>
    [JsonPropertyName("skus")]
    public List<ChannelsProductSkuUpdate> Skus { get; set; } = new();

    /// <summary>获取或设置限购信息（官方 <c>limited_info</c>）。</summary>
    [JsonPropertyName("limited_info")]
    public ChannelsProductLimitedInfo? LimitedInfo { get; set; }

    /// <summary>获取或设置运费信息（官方 <c>express_info</c>）。</summary>
    [JsonPropertyName("express_info")]
    public ChannelsProductExpressInfo? ExpressInfo { get; set; }

    /// <summary>获取或设置额外服务（官方 <c>extra_service</c>）。</summary>
    [JsonPropertyName("extra_service")]
    public ChannelsProductExtraService? ExtraService { get; set; }

    /// <summary>获取或设置发货方式（官方 <c>deliver_method</c>：0 快递发货（默认），1 无需快递；仅对部分类目开放）。</summary>
    [JsonPropertyName("deliver_method")]
    public int? DeliverMethod { get; set; }

    /// <summary>获取或设置是否在店铺首页隐藏（官方 <c>hide_in_window</c>：0 不隐藏，1 隐藏）。</summary>
    [JsonPropertyName("hide_in_window")]
    public int? HideInWindow { get; set; }

    /// <summary>获取或设置商品待开售信息（官方 <c>timing_onsale_info</c>）。</summary>
    [JsonPropertyName("timing_onsale_info")]
    public ChannelsTimingOnsaleInfo? TimingOnsaleInfo { get; set; }

    /// <summary>获取或设置 spu 维度配置全部 sku 预售规则（官方 <c>spu_deliver_info</c>）。</summary>
    [JsonPropertyName("spu_deliver_info")]
    public ChannelsSpuDeliverInfo? SpuDeliverInfo { get; set; }

    /// <summary>获取或设置售后 / 退货地址（官方 <c>after_sale_info</c>）。</summary>
    [JsonPropertyName("after_sale_info")]
    public ChannelsProductAfterSaleInfo? AfterSaleInfo { get; set; }
}
