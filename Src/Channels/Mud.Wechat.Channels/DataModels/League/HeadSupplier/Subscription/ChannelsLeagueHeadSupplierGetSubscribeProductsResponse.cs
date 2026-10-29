// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>获取订阅的商品列表响应（官方 <c>headsupplier/subscription/getsubscribe</c> 返回体）。</summary>
/// <remarks>官方示例把 <c>has_more</c> 写成字符串 <c>"false"</c>、参数表定为 boolean ⇒ 以参数表为权威。</remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetSubscribeProductsResponse
{
    /// <summary>获取或设置订阅的商品信息列表（官方 <c>subscribe_info_list</c>）。</summary>
    [JsonPropertyName("subscribe_info_list")]
    public List<ChannelsLeagueHeadSupplierSubscribeProductInfo>? SubscribeInfoList { get; set; }

    /// <summary>获取或设置符合条件的记录总数（官方 <c>total_num</c>，须请求侧 <c>need_total_num=true</c>）。</summary>
    [JsonPropertyName("total_num")]
    public long? TotalNum { get; set; }

    /// <summary>获取或设置翻页参数（官方 <c>next_key</c>）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置是否还有下一页（官方 <c>has_more</c>）。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }
}
