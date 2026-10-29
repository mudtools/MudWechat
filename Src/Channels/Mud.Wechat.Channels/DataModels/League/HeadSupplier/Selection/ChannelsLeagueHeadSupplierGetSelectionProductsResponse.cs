// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 选品广场商品列表查询响应（机构侧官方 <c>headsupplier/selectionproducts/list/get</c> 返回体）。
/// </summary>
/// <remarks>
/// <para>官方返回参数表把 <c>product_list</c> 标为 objarray，但同一页的 5.1 返回示例把它写成「单对象」
/// （<c>"product_list": { ... }</c>）、5.2 示例写成「数组」⇒ 以参数表 + 5.2 示例的数组形态建模。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetSelectionProductsResponse : ChannelsResponse
{
    /// <summary>获取或设置商品列表（官方 <c>product_list</c>，单页最多 <c>page_size</c> 条、上限 20）。</summary>
    [JsonPropertyName("product_list")]
    public List<ChannelsLeagueHeadSupplierSelectionProduct>? ProductList { get; set; }

    /// <summary>获取或设置翻页上下文，下一页写入请求的 <c>next_key</c>（官方 <c>next_key</c>）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}
