// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 商品列表条目（<c>goods[]</c>，官方 9 字段，<c>GET /wxaapi/broadcast/goods/getapproved</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_getgoodsinfo.html"/>。
/// </para>
/// <para>
/// <b>同一条目内 camelCase 与 snake_case 混排（照录）</b>：官方本页参数表为
/// <c>goodsId</c> / <c>coverImgUrl</c> / <c>priceType</c> / <c>thirdPartyAppid</c>（camel）
/// 与 <c>third_party_tag</c>（snake）共存 —— 与「获取商品的信息与审核状态」页的
/// <c>goods_id</c> / <c>cover_img_url</c> / <c>audit_status</c> / <c>third_party_tag</c> 拼写又不一致
/// ⇒ 两页各建条目类型，宿主做字段映射时须按来源页判别（守卫 LB5 锁定本项混排事实）。
/// </para>
/// <para>
/// <b>本条目 <c>goodsId</c> 官方类型为 string</b>（而添加应答的 <c>goodsId</c> 为 number）⇒ 照录为字符串。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveApprovedGoodsItem
{
    /// <summary>商品 ID（<c>goodsId</c>，官方类型 <b>string</b>，与添加应答的 number 口径不同）。</summary>
    [JsonPropertyName("goodsId")]
    public string? GoodsId { get; set; }

    /// <summary>商品名称（<c>name</c>，最长 14 个汉字，1 个汉字相当于 2 个字符）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>商品图片（<c>coverImgUrl</c>，官方原文为 mediaID 口径，图片尺寸最大 300 像素 * 300 像素）。</summary>
    [JsonPropertyName("coverImgUrl")]
    public string? CoverImgUrl { get; set; }

    /// <summary>商品详情页的小程序路径（<c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>价格类型（<c>priceType</c>），取值见 <see cref="WxaLivePriceType"/>。</summary>
    [JsonPropertyName("priceType")]
    public long? PriceType { get; set; }

    /// <summary>价格（<c>price</c>，最多两位小数，单位元）。</summary>
    [JsonPropertyName("price")]
    public double? Price { get; set; }

    /// <summary>价格二（<c>price2</c>，最多两位小数，单位元）。</summary>
    [JsonPropertyName("price2")]
    public double? Price2 { get; set; }

    /// <summary>来源标记（<c>third_party_tag</c>，官方原文「1、2：表示是为 API 添加商品，否则是直播控制台添加的商品」）。</summary>
    [JsonPropertyName("third_party_tag")]
    public long? ThirdPartyTag { get; set; }

    /// <summary>第三方小程序 appid（<c>thirdPartyAppid</c>，自身小程序商品为空）。</summary>
    [JsonPropertyName("thirdPartyAppid")]
    public string? ThirdPartyAppid { get; set; }
}
