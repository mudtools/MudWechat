// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>获取商品推广带货者列表（<c>promotion/talent/list</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsListSpuPromotionTalentsResponse : ChannelsResponse
{
    /// <summary>获取或设置商品总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }

    /// <summary>获取或设置后面是否还有内容（官方 <c>continue_flag</c>：true 还有内容；false 已结束）。</summary>
    [JsonPropertyName("continue_flag")]
    public bool? ContinueFlag { get; set; }

    /// <summary>获取或设置商品信息列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsLeaguePromotionTalentItem>? List { get; set; }
}
