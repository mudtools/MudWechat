// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>获取橱窗商品详情（<c>getwindowproduct</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsGetWindowProductRequest
{
    /// <summary>获取或设置橱窗商品 ID（官方 <c>product_id</c>，string(uint64)，必填）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置商品来源店铺的 appid（官方 <c>appid</c>，必填；need_cps_product 为 true 时可留空）。</summary>
    [JsonPropertyName("appid")]
    public string Appid { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置是否查询带货商品（官方 <c>need_cps_product</c>，选填）。填 true 后 product_id 支持两种 ID 查询：
    /// 输入小店商品 ID 可获取该商品的所有带货计划；输入带货商品 ID 只能获取该带货商品的计划。
    /// </summary>
    [JsonPropertyName("need_cps_product")]
    public bool? NeedCpsProduct { get; set; }
}
