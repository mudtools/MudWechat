// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 商品所属店铺信息（官方 <c>headsupplier/productdetail/get</c> 返回的 <c>item.shop</c> 对象）。
/// </summary>
/// <remarks>与「商品推广参数详情」里的机构信息 <see cref="ChannelsLeagueHeadSupplierInfo"/>（<c>name</c> / <c>head_img</c> / <c>appid</c>）字段集不同 ⇒ 分建。</remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierShopInfo
{
    /// <summary>获取或设置店铺名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置店铺评分（官方 <c>score</c>，如 <c>463</c>）。</summary>
    [JsonPropertyName("score")]
    public int? Score { get; set; }

    /// <summary>获取或设置店铺图标（官方 <c>icon</c>）。</summary>
    [JsonPropertyName("icon")]
    public string? Icon { get; set; }
}
