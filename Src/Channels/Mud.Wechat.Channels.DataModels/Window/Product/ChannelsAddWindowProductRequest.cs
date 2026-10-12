// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>
/// 上架商品到橱窗（<c>addwindowproduct</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：官方注明该 API「已无实际作用」——橱窗中自营商品的上下架状态会自动同步为商品在店铺中的状态。
/// </para>
/// <para>
/// <b>官方文档矛盾照录</b>：参数表把 <c>product_id</c> 标注为 number、请求示例为带引号字符串
/// （"100234056"）——SDK 按参数表建模；同域 get/off/list 三端点的参数表均为 string(uint64)。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsAddWindowProductRequest
{
    /// <summary>获取或设置橱窗商品 ID（官方 <c>product_id</c>，必填。不支持带货中心来源的商品，其余商品的橱窗商品 ID 与商品来源处的平台内部商品 ID 相同）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置商品来源店铺的 appid（官方 <c>appid</c>，必填）。</summary>
    [JsonPropertyName("appid")]
    public string Appid { get; set; } = string.Empty;

    /// <summary>获取或设置是否需要在个人橱窗页隐藏（官方 <c>is_hide_for_window</c>，选填，默认为 false）。</summary>
    [JsonPropertyName("is_hide_for_window")]
    public bool? IsHideForWindow { get; set; }
}
