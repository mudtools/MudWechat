// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>商品活动补贴信息查询响应（官方 <c>headsupplier/product/getactivity</c> 返回体）。</summary>
/// <remarks>官方说明：本接口「获取联盟商品的活动信息（目前仅有平台补贴）」，<c>has_activity</c> 为 <c>false</c> 时列表可为空。</remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetProductActivityResponse : ChannelsResponse
{
    /// <summary>获取或设置是否存在活动（官方 <c>has_activity</c>）。</summary>
    [JsonPropertyName("has_activity")]
    public bool? HasActivity { get; set; }

    /// <summary>获取或设置商品活动信息列表（官方 <c>product_activity_list</c>）。</summary>
    [JsonPropertyName("product_activity_list")]
    public List<ChannelsLeagueHeadSupplierProductActivity>? ProductActivityList { get; set; }
}
