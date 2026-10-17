// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>获取订阅的商品列表请求（官方 <c>headsupplier/subscription/getsubscribe</c> 请求体）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetSubscribeProductsRequest
{
    /// <summary>获取或设置筛选条件（官方 <c>filter</c>）。</summary>
    [JsonPropertyName("filter")]
    public ChannelsLeagueHeadSupplierSubscribeProductFilter? Filter { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填，不超过 1000）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置翻页参数（官方 <c>next_key</c>，从第二页起传上一页返回值）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置是否返回符合条件的记录总数（官方 <c>need_total_num</c>）。</summary>
    [JsonPropertyName("need_total_num")]
    public bool? NeedTotalNum { get; set; }
}
