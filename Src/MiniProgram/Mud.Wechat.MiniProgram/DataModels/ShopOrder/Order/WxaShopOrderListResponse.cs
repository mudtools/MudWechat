// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 订单列表通用应答（<c>/shop/order/get_list</c>、<c>get_list_by_finder</c>、<c>get_list_by_sharer</c> 共用）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopOrderListResponse : WxaResponse
{
    /// <summary>订单列表（官方返回参数表键名 <c>orders[]</c>）。</summary>
    [JsonPropertyName("orders")]
    public List<WxaShopOrder>? Orders { get; set; }

    /// <summary>满足条件的订单总数。</summary>
    [JsonPropertyName("total_num")]
    public long? TotalNum { get; set; }
}
