// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// 纠纷单详情（回包参数表所称的 <c>ComplaintOrderDetail</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方表与示例不一致（照录）</b>：<c>complaint/get</c> 回包表写「<c>complaint_order_detail</c> Object，</para>
/// <para>见 ComplaintOrderDetail」，但回包示例把 <c>flow_info</c> 与 <c>order</c> 直接放在顶层 ⇒</para>
/// <para>本类按二者的并集承载（<c>flow_info</c> + <c>order</c>），响应类同时保留嵌套键与平铺键，两种形态均可反序列化。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaShopComplaintOrderDetail
{
    /// <summary>纠纷流转信息。</summary>
    [JsonPropertyName("flow_info")]
    public WxaShopComplaintFlowInfo? FlowInfo { get; set; }

    /// <summary>纠纷单摘要。</summary>
    [JsonPropertyName("order")]
    public WxaShopComplaintOrder? Order { get; set; }
}
