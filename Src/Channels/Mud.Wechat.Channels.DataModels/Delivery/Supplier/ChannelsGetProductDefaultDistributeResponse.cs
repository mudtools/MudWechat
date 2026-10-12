// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>获取商品对应的自动分配供货商（<c>get_product_default_distribute</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsGetProductDefaultDistributeResponse : ChannelsResponse
{
    /// <summary>获取或设置是否存在已设置自动分配的供货商（官方 <c>exist_default_supplier</c>）。</summary>
    [JsonPropertyName("exist_default_supplier")]
    public bool? ExistDefaultSupplier { get; set; }

    /// <summary>
    /// 获取或设置自动分配的供货商 appid（官方 <c>supplier_appid</c>）——
    /// 当且仅当 <c>exist_default_supplier = true</c> 时有意义。
    /// </summary>
    [JsonPropertyName("supplier_appid")]
    public string? SupplierAppid { get; set; }
}
