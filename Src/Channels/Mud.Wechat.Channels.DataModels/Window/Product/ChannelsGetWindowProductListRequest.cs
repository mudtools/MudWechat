// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>获取已添加到橱窗的商品列表（<c>getwindowproductlist</c>）请求体。</summary>
/// <remarks>官方契约：appid 与 query_cps_product 互斥，两者都传时以 appid 优先（仅返回来源指定店铺的自营商品）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsGetWindowProductListRequest
{
    /// <summary>获取或设置用于指定查询某个店铺来源的商品（官方 <c>appid</c>，选填；与 query_cps_product 互斥）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>获取或设置用于指定查询属于某个分店 ID 下的商品（官方 <c>branch_id</c>，选填）。</summary>
    [JsonPropertyName("branch_id")]
    public long? BranchId { get; set; }

    /// <summary>获取或设置单页商品数（官方 <c>page_size</c>，必填，不超过 200；page_size × page_index ≤ 10000）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置页面下标（官方 <c>page_index</c>，选填，下标从 1 开始，默认为 1）。</summary>
    [JsonPropertyName("page_index")]
    public int? PageIndex { get; set; }

    /// <summary>获取或设置顺序翻页上下文（官方 <c>last_buffer</c>，选填；由上次请求返回，填了该值后 page_index 不生效）。</summary>
    [JsonPropertyName("last_buffer")]
    public string? LastBuffer { get; set; }

    /// <summary>获取或设置是否需要返回满足筛选条件的商品总数（官方 <c>need_total_num</c>，选填）。</summary>
    [JsonPropertyName("need_total_num")]
    public bool? NeedTotalNum { get; set; }

    /// <summary>获取或设置是否仅查询橱窗中的带货商品（官方 <c>query_cps_product</c>，选填；与 appid 互斥）。</summary>
    [JsonPropertyName("query_cps_product")]
    public bool? QueryCpsProduct { get; set; }
}
