// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 直播商品信息（<c>goodsInfo</c>，官方 7 字段，用于「添加并提审商品」）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_addgoods.html"/>。
/// </para>
/// <para>
/// <b>本类型刻意不含 <c>goodsId</c></b>：官方「添加并提审」的 <c>goodsInfo</c> 表为 7 字段，
/// 「更新商品」的同名对象为 8 字段（多 <c>goodsId</c>）⇒ 两口各建类型
/// （<see cref="WxaLiveUpdateGoodsInfo"/>），添加侧不得凭空加 ID 字段（守卫 LB5 锁该断言）。
/// </para>
/// <para>
/// <b>价格单位是「元」</b>（官方原文「数字，最多保留两位小数，单位元」）：与直播间信息内
/// <c>room_info[].goods[].price</c> 的「分」口径不同源，跨族取值不得直接互换。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveGoodsInfo
{
    /// <summary>商品图片（<c>coverImgUrl</c>，必填）：填 mediaID（三天内有效）；图片尺寸最大 300 像素 * 300 像素。</summary>
    [JsonPropertyName("coverImgUrl")]
    public string? CoverImgUrl { get; set; }

    /// <summary>商品名称（<c>name</c>，必填）：最长 14 个汉字，1 个汉字相当于 2 个字符。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>价格类型（<c>priceType</c>，必填），取值见 <see cref="WxaLivePriceType"/>。</summary>
    [JsonPropertyName("priceType")]
    public long? PriceType { get; set; }

    /// <summary>价格（<c>price</c>，必填；官方类型 number，最多两位小数，<b>单位元</b>）。priceType 为 2/3 时须同时给 <see cref="Price2"/>。</summary>
    [JsonPropertyName("price")]
    public double? Price { get; set; }

    /// <summary>价格二（<c>price2</c>，选填；官方类型 number，最多两位小数，单位元）：含义随 <see cref="PriceType"/> 而变。</summary>
    [JsonPropertyName("price2")]
    public double? Price2 { get; set; }

    /// <summary>商品详情页的小程序路径（<c>url</c>，必填）：路径参数存在 url 的，该参数的值需 encode 处理后再填入。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>第三方小程序 appid（<c>thirdPartyAppid</c>，选填）：商品为第三方小程序商品时填对应 appid，自身小程序商品填空。</summary>
    [JsonPropertyName("thirdPartyAppid")]
    public string? ThirdPartyAppid { get; set; }
}
