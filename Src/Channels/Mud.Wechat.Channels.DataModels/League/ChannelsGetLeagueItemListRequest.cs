// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>获取联盟商品列表（<c>item/list/get</c>）请求体。</summary>
/// <remarks>
/// 官方契约：<c>page_size × page_index ≤ 5000</c>（接口限制）。<b>官方文档矛盾照录</b>：注意事项
/// 建议命中限制时改用传 <c>last_buffer</c> 顺序翻页，但官方变更日志 2026-06-17 已删除
/// <c>last_buffer</c> 等字段、当前参数表无此入参——SDK 按参数表建模（无 last_buffer）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsGetLeagueItemListRequest
{
    /// <summary>获取或设置单页商品数（官方 <c>page_size</c>，必填，不超过 100）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置页面下标（官方 <c>page_index</c>，选填，下标从 1 开始，默认为 1）。</summary>
    [JsonPropertyName("page_index")]
    public int? PageIndex { get; set; }

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，选填，获取特殊推广商品时有效）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置是否需要返回满足筛选条件的商品总数（官方 <c>need_total_num</c>，选填）。</summary>
    [JsonPropertyName("need_total_num")]
    public bool? NeedTotalNum { get; set; }
}
