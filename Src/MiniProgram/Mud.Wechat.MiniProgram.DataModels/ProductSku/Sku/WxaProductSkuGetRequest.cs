// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductSku;

/// <summary>获取 SKU 信息请求（<c>POST /product/sku/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "ProductSku")]
public class WxaProductSkuGetRequest
{
    /// <summary>小商店内部 SKU ID，与 <c>out_sku_id</c> 二选一。</summary>
    [JsonPropertyName("sku_id")]
    public long? SkuId { get; set; }

    /// <summary>商家自定义 SKU ID，与 <c>sku_id</c> 二选一。</summary>
    [JsonPropertyName("out_sku_id")]
    public string? OutSkuId { get; set; }

    /// <summary>库存口径：0 获取草稿库存（默认）/ 1 获取线上真实库存。</summary>
    [JsonPropertyName("need_real_stock")]
    public int? NeedRealStock { get; set; }

    /// <summary>数据版本：0 获取线上数据（默认）/ 1 获取草稿数据。</summary>
    [JsonPropertyName("need_edit_sku")]
    public int? NeedEditSku { get; set; }
}
