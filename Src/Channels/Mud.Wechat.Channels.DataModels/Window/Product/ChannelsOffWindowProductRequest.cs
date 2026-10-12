// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>下架橱窗商品（<c>offwindowproduct</c>）请求体。</summary>
/// <remarks>
/// 官方契约：官方注明该 API「已无实际作用」——橱窗中自营商品的上下架状态会自动同步为商品在店铺中的状态。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsOffWindowProductRequest
{
    /// <summary>获取或设置橱窗商品 ID（官方 <c>product_id</c>，string(uint64)，必填。不支持带货中心来源的商品）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置商品来源店铺的 appid（官方 <c>appid</c>，必填）。</summary>
    [JsonPropertyName("appid")]
    public string Appid { get; set; } = string.Empty;
}
