// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>取消分配代发单（<c>dropship/cancel</c>）请求体。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：官方请求示例中出现 <c>supplier_appid</c> 字段，但参数表未列出——
/// SDK 按参数表建模（仅 <c>order_id</c>/<c>ds_order_id</c>/<c>dropship_product_list</c>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDropshipCancelRequest
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置代发单号（官方 <c>ds_order_id</c>，必填）。</summary>
    [JsonPropertyName("ds_order_id")]
    public string DsOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置代发商品信息（官方 <c>dropship_product_list</c>，必填）。</summary>
    [JsonPropertyName("dropship_product_list")]
    public List<ChannelsDropshipProduct> DropshipProductList { get; set; } = new();
}
