// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// 获取纠纷单应答（<c>POST /shop/complaint/get</c>）。
/// </summary>
/// <remarks>
/// <para><b>嵌套键与平铺键并存</b>：参数表只承认 <c>complaint_order_detail</c>，官方示例只给平铺的</para>
/// <para><c>flow_info</c> / <c>order</c> ⇒ 三支全部建模为可空（见 <see cref="WxaShopComplaintOrderDetail"/>）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaShopComplaintGetResponse : WxaResponse
{
    /// <summary>纠纷单详情（参数表口径）。</summary>
    [JsonPropertyName("complaint_order_detail")]
    public WxaShopComplaintOrderDetail? ComplaintOrderDetail { get; set; }

    /// <summary>纠纷流转信息（回包示例口径）。</summary>
    [JsonPropertyName("flow_info")]
    public WxaShopComplaintFlowInfo? FlowInfo { get; set; }

    /// <summary>纠纷单摘要（回包示例口径）。</summary>
    [JsonPropertyName("order")]
    public WxaShopComplaintOrder? Order { get; set; }
}
