// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopSpu;

/// <summary>更新商品响应数据。</summary>
[HttpJsonSerializable(SerializerClassName = "ShopSpu")]
public class WxaShopSpuUpdateData
{
    /// <summary>交易组件平台内部商品 ID。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>商家自定义商品 ID。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>更新时间，<c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>SKU 回显列表。</summary>
    [JsonPropertyName("skus")]
    public List<WxaShopSpuSkuResult>? Skus { get; set; }
}
