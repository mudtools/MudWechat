// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>机构合作商品列表查询请求（官方 <c>headsupplier/cooperativeitem/list/get</c> 请求体）。</summary>
/// <remarks>
/// <para>官方必填口径：<c>commission_type</c> / <c>page_size</c> / <c>next_key</c> 三者均<b>必填</b>（第一页 <c>next_key</c> 传空串），
/// 仅 <c>listing_time_range</c> 选填；<c>page_size</c> 最大 <c>20</c>。SDK 不做本地拦截，越界由 <c>10024000</c> 表达。</para>
/// <para>本接口是<b>升级版</b>列表：官方注明「旧接口当前仍可调用」（旧接口为 <c>headsupplier/item/list/get</c>，已脱树、本仓不实现）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetCooperativeItemListRequest
{
    /// <summary>获取或设置佣金分配类型（官方 <c>commission_type</c>，必填：0 商家指定达人佣金 / 1 机构指定达人佣金）。</summary>
    [JsonPropertyName("commission_type")]
    public int CommissionType { get; set; }

    /// <summary>获取或设置合作商品上架时间范围（官方 <c>listing_time_range</c>，选填）。</summary>
    [JsonPropertyName("listing_time_range")]
    public ChannelsLeagueHeadSupplierNumberRange? ListingTimeRange { get; set; }

    /// <summary>获取或设置单页商品数（官方 <c>page_size</c>，必填，最大 <c>20</c>）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>next_key</c>，必填；第一页传空串，后续页写入上一页返回的 <c>next_key</c>）。</summary>
    [JsonPropertyName("next_key")]
    public string NextKey { get; set; } = string.Empty;
}
