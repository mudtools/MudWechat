// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------
namespace Mud.Wechat.Channels.DataModels.Talent;

/// <summary>获取佣金单列表响应（官方 <c>talent/get_order_list</c> 返回体）。</summary>
[HttpJsonSerializable(SerializerClassName = "Talent")]
public class ChannelsTalentGetOrderListResponse
{
    /// <summary>获取或设置佣金单列表（官方 <c>order_list</c>，参数表类型列渲染异常，示例为数组 ⇒ 按示例取列表形态）。</summary>
    [JsonPropertyName("order_list")]
    public List<ChannelsTalentOrderListItem>? OrderList { get; set; }

    /// <summary>获取或设置是否还有剩余订单（官方 <c>has_more</c>）。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>获取或设置本次翻页上下文（官方 <c>next_key</c>，用于顺序翻页请求）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}
