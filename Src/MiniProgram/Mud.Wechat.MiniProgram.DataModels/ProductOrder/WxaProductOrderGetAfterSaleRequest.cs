// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 获取售后单请求体（<c>POST /product/order/getaftersaleorder</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档交叉（照录）</b>：本路由的契约正文挂在名为 <c>batchgetaftersaleorder.html</c> 的页面上，</para>
/// <para>而 <c>getaftersaleorder.html</c> 实际描述的是 <c>/product/order/batchgetaftersaleorder</c> ⇒ 两页文件名与路由互换。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductOrderGetAfterSaleRequest
{
    /// <summary>售后单号。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public long? AfterSaleOrderId { get; set; }
}
