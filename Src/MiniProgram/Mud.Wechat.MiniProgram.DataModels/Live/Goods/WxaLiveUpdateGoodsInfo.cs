// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 更新商品信息（<c>goodsInfo</c>，官方 8 字段，用于「更新商品」）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_updategoodsinfo.html"/>。
/// </para>
/// <para>
/// <b>与 <see cref="WxaLiveGoodsInfo"/> 的唯一差</b>：本类型多必填的 <c>goodsId</c>（官方原文
/// 「需要更新哪个字段就传入哪个字段，goodsId 必传」）⇒ 官方<b>两页字段数不同</b>（7 vs 8），
/// 刻意不共用，避免添加侧误传 ID。
/// </para>
/// <para><b>部分更新语义</b>：本页允许只传要改的字段（与创建侧「必填即必传」不同），故除 <c>goodsId</c> 外各字段皆可为空。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveUpdateGoodsInfo
{
    /// <summary>商品 ID（<c>goodsId</c>，必填；官方原文「goodsId 必传」）。</summary>
    [JsonPropertyName("goodsId")]
    public long? GoodsId { get; set; }

    /// <summary>商品图片（<c>coverImgUrl</c>）：填 mediaID（三天内有效），图片尺寸最大 300 像素 * 300 像素。</summary>
    [JsonPropertyName("coverImgUrl")]
    public string? CoverImgUrl { get; set; }

    /// <summary>商品名称（<c>name</c>）：最长 14 个汉字，1 个汉字相当于 2 个字符。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>价格类型（<c>priceType</c>），取值见 <see cref="WxaLivePriceType"/>。</summary>
    [JsonPropertyName("priceType")]
    public long? PriceType { get; set; }

    /// <summary>价格（<c>price</c>，最多两位小数，单位元）。</summary>
    [JsonPropertyName("price")]
    public double? Price { get; set; }

    /// <summary>价格二（<c>price2</c>，最多两位小数，单位元）：含义随 <see cref="PriceType"/> 而变。</summary>
    [JsonPropertyName("price2")]
    public double? Price2 { get; set; }

    /// <summary>商品详情页的小程序路径（<c>url</c>）：路径参数存在 url 的须 encode 后填入。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>第三方小程序 appid（<c>thirdPartyAppid</c>）：第三方商品填对应 appid，自身商品填空。</summary>
    [JsonPropertyName("thirdPartyAppid")]
    public string? ThirdPartyAppid { get; set; }
}
