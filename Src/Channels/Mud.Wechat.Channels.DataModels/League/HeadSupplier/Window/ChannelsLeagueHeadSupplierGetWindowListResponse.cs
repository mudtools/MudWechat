// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>查询橱窗上机构商品列表响应（官方 <c>headsupplier/window/getall</c> 返回体）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetWindowListResponse
{
    /// <summary>获取或设置商品概要列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsLeagueHeadSupplierWindowProduct>? List { get; set; }

    /// <summary>获取或设置下一页位置（官方 <c>next_offset</c>）。</summary>
    [JsonPropertyName("next_offset")]
    public long? NextOffset { get; set; }

    /// <summary>获取或设置后面是否还有商品（官方 <c>has_more</c>）。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>获取或设置商品总数（官方 <c>total_num</c>，须请求侧 <c>need_total_num=true</c>）。</summary>
    [JsonPropertyName("total_num")]
    public long? TotalNum { get; set; }
}
