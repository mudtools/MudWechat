// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 直播间信息条目内的商品对象（<c>room_info[].goods[]</c>，官方 8 字段）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getliveinfo.html"/>。
/// </para>
/// <para>
/// <b>价格单位与商品管理面不同源</b>：官方原文本对象 <c>price</c> 注「商品价格（<b>分</b>）」，
/// 而商品管理（添加 / 更新 / 列表）的 <c>price</c> 注「单位<b>元</b>，最多两位小数」⇒ 两族价格 DTO 不共用，
/// 宿主换算单位时须按字段来源判别（照录官方口径，不做统一换算）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveRoomGoodsItem
{
    /// <summary>商品名称（<c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>商品封面图链接（<c>cover_img</c>）。</summary>
    [JsonPropertyName("cover_img")]
    public string? CoverImg { get; set; }

    /// <summary>商品小程序路径（<c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>商品价格（<c>price</c>，官方原文<b>单位为分</b>，与商品管理侧的「元」口径不同）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>商品价格（<c>price2</c>，使用方式看 <see cref="PriceType"/>）。</summary>
    [JsonPropertyName("price2")]
    public long? Price2 { get; set; }

    /// <summary>价格类型（<c>price_type</c>），取值见 <see cref="WxaLivePriceType"/>（1 一口价 / 2 价格区间 / 3 显示折扣价）。</summary>
    [JsonPropertyName("price_type")]
    public long? PriceType { get; set; }

    /// <summary>商品 ID（<c>goods_id</c>，官方类型 number）。</summary>
    [JsonPropertyName("goods_id")]
    public long? GoodsId { get; set; }

    /// <summary>第三方商品 appid（<c>third_party_appid</c>，官方原文「当前小程序商品则为空」）。</summary>
    [JsonPropertyName("third_party_appid")]
    public string? ThirdPartyAppid { get; set; }
}
