// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>橱窗上的机构商品概要（官方 <c>list[]</c>，<c>headsupplier/window/getall</c>）。
/// <c>product_window_id</c> 是移除 / 查详情端的定位键，与机构商品 ID 不同 ⇒ 两键并存。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierWindowProduct
{
    /// <summary>获取或设置机构 appid（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>获取或设置机构商品 ID（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品在橱窗的 ID（官方 <c>product_window_id</c>，用于移除橱窗操作）。</summary>
    [JsonPropertyName("product_window_id")]
    public long? ProductWindowId { get; set; }

    /// <summary>获取或设置是否有操作权限（官方 <c>is_operation_allowed</c>，true 可操作 / false 不可操作）。</summary>
    [JsonPropertyName("is_operation_allowed")]
    public bool? IsOperationAllowed { get; set; }
}
