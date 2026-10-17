// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>获取分发给其他机构的优惠券（<c>promoter/get_coupon_to_headsupplier</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterGetCouponToHeadSupplierResponse : ChannelsResponse
{
    /// <summary>获取或设置券信息的列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsPromoterCouponToHeadSupplierInfo>? List { get; set; }

    /// <summary>获取或设置下一页的翻页参数（官方 <c>page_context</c>）。</summary>
    [JsonPropertyName("page_context")]
    public string? PageContext { get; set; }

    /// <summary>获取或设置总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public int? TotalCount { get; set; }
}
