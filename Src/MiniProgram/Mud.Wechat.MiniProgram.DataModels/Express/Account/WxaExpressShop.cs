// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>商品信息（<c>shop</c>，会展示到物流服务通知和电子面单中）。</summary>
/// <remarks>官方字段以商家寄件 <c>order/add</c> 的「Body.shop Object Payload」为准。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressShop
{
    /// <summary>商家小程序的路径（<c>wxa_path</c>，必填）。</summary>
    [JsonPropertyName("wxa_path")]
    public string? MiniProgramPath { get; set; }

    /// <summary>商品缩略图 URL（<c>img_url</c>）。</summary>
    [JsonPropertyName("img_url")]
    public string? ImageUrl { get; set; }

    /// <summary>商品名称（<c>goods_name</c>）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>商品数量（<c>goods_count</c>）。</summary>
    [JsonPropertyName("goods_count")]
    public int? GoodsCount { get; set; }

    /// <summary>商品详情列表（<c>detail_list</c>）。</summary>
    [JsonPropertyName("detail_list")]
    public List<WxaExpressShopGood>? GoodsList { get; set; }
}
