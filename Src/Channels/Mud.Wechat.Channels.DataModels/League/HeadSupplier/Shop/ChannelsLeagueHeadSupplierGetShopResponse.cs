// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>获取商家合作详情响应（官方 <c>headsupplier/shop/get</c> 返回体）。</summary>
/// <remarks>
/// 官方 2025-11-24 变更日志把返回根字段从 <c>shop_list</c> 改为 <c>shop_detail</c>；
/// 本页代码示例仍残留请求侧 <c>page_size</c> 字段（属文档陈旧，参数表无该字段）⇒ 不建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetShopResponse
{
    /// <summary>获取或设置商家合作详情（官方 <c>shop_detail</c>）。</summary>
    [JsonPropertyName("shop_detail")]
    public ChannelsLeagueHeadSupplierShopDetail? ShopDetail { get; set; }
}
