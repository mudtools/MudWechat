// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 商品信息仓库条目（<c>goods[]</c>，官方 10 字段，<c>POST /wxa/business/getgoodswarehouse</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_getgoodsauditinfo.html"/>。
/// </para>
/// <para>
/// <b>本条目比「商品列表」条目多 <c>audit_status</c>、少…无</b>：官方两页字段集不同（本页 10 字段含审核状态，
/// 列表页 9 字段不含）⇒ 两类型分立，各自照录本页拼写。
/// </para>
/// <para>
/// <b>同一条目内 snake_case 与 camelCase 混排（照录）</b>：<c>goods_id</c> / <c>cover_img_url</c> /
/// <c>audit_status</c> / <c>third_party_tag</c> 与 <c>priceType</c> / <c>thirdPartyAppid</c> 共存于同一对象。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveWarehouseGoodsItem
{
    /// <summary>商品 ID（<c>goods_id</c>，官方类型 <b>string</b>）。</summary>
    [JsonPropertyName("goods_id")]
    public string? GoodsId { get; set; }

    /// <summary>商品名称（<c>name</c>，最长 14 个汉字，1 个汉字相当于 2 个字符）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>商品图片（<c>cover_img_url</c>，mediaID 口径，图片尺寸最大 300 像素 * 300 像素）。</summary>
    [JsonPropertyName("cover_img_url")]
    public string? CoverImgUrl { get; set; }

    /// <summary>商品详情页的小程序路径（<c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>价格类型（<c>priceType</c>，官方原文此处即 camelCase），取值见 <see cref="WxaLivePriceType"/>。</summary>
    [JsonPropertyName("priceType")]
    public long? PriceType { get; set; }

    /// <summary>价格（<c>price</c>，最多两位小数，单位元）。</summary>
    [JsonPropertyName("price")]
    public double? Price { get; set; }

    /// <summary>价格二（<c>price2</c>，最多两位小数，单位元）。</summary>
    [JsonPropertyName("price2")]
    public double? Price2 { get; set; }

    /// <summary>审核状态（<c>audit_status</c>，官方类型列为 <b>string</b>，取值含义见 <see cref="WxaLiveGoodsAuditStatus"/>：0 未审核 / 1 审核中 / 2 审核通过 / 3 审核失败）。</summary>
    [JsonPropertyName("audit_status")]
    public string? AuditStatus { get; set; }

    /// <summary>来源标记（<c>third_party_tag</c>，官方原文「1、2：表示是为 API 添加商品，否则是直播控制台添加的商品」）。</summary>
    [JsonPropertyName("third_party_tag")]
    public long? ThirdPartyTag { get; set; }

    /// <summary>第三方小程序 appid（<c>thirdPartyAppid</c>，官方原文此处即 camelCase）。</summary>
    [JsonPropertyName("thirdPartyAppid")]
    public string? ThirdPartyAppid { get; set; }
}
