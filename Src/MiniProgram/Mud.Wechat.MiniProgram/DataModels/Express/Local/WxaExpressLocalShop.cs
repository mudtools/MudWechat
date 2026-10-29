// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>即时配送商品信息（<c>shop</c> 内嵌对象；会展示到物流通知消息中）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalShop
{
    /// <summary>商家小程序路径（<c>wxa_path</c>，可选；建议为订单页面）。</summary>
    [JsonPropertyName("wxa_path")]
    public string? WxaPath { get; set; }

    /// <summary>商品缩略图 URL（<c>img_url</c>）。</summary>
    [JsonPropertyName("img_url")]
    public string? ImageUrl { get; set; }

    /// <summary>商品名称（<c>goods_name</c>；不超过 128 字节）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>商品数量（<c>goods_count</c>）。</summary>
    [JsonPropertyName("goods_count")]
    public int? GoodsCount { get; set; }

    /// <summary>
    /// 下单小程序的 AppId（<c>wxa_appid</c>，可选；<b>仅即时配送 <c>order/add</c> 生效</b>）。
    /// </summary>
    /// <remarks>结算方式为「第三方向配送公司统一结算」时<b>必填</b>：第三方用自己的小程序替授权商户下单，并传授权小程序 AppId，配送通知可回流授权商户小程序。</remarks>
    [JsonPropertyName("wxa_appid")]
    public string? WxaAppId { get; set; }
}
