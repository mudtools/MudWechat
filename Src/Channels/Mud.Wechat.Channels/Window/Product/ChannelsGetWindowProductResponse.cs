// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>获取橱窗商品详情（<c>getwindowproduct</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsGetWindowProductResponse : ChannelsResponse
{
    /// <summary>获取或设置自营商品详情（官方 <c>product</c>）。</summary>
    [JsonPropertyName("product")]
    public ChannelsWindowProduct? Product { get; set; }

    /// <summary>获取或设置橱窗中的带货商品详情列表（官方 <c>cps_products</c>，元素结构与 product 一致，另携带 <c>cps_item_type</c>）。</summary>
    [JsonPropertyName("cps_products")]
    public List<ChannelsWindowProduct>? CpsProducts { get; set; }
}
