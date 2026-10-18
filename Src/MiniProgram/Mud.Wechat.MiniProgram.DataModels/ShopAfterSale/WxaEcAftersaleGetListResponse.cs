// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// 获取售后列表应答（<c>POST /shop/ecaftersale/get_list</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方回包示例残缺（照录）</b>：示例为 <c>"after_sales_orders":,</c> / <c>"has_more":</c>（无值、无闭合）；</para>
/// <para>回包表把 <c>after_sales_orders</c> 标为「Array string」、说明「售后单号列表」⇒ 本类建模为</para>
/// <para><c>List&lt;string&gt;</c>，<b>不</b>按对象列表臆测；<c>has_more</c> 控制是否继续翻页。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaEcAftersaleGetListResponse : WxaResponse
{
    /// <summary>售后单号列表（官方表类型「Array string」）。</summary>
    [JsonPropertyName("after_sales_orders")]
    public List<string>? AfterSalesOrders { get; set; }

    /// <summary>是否还有未拉取的售后单。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }
}
