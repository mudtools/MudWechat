// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// 更新售后单请求体（<c>POST /shop/aftersale/update</c>）。
/// </summary>
/// <remarks>
/// <para><b>只改状态与完结标记</b>：本端点不重传商品与金额，仅以三支定位键改 <c>status</c> 与</para>
/// <para><c>finish_all_aftersale</c>；官方表头称 <c>out_order_id</c> 与 <c>order_id</c> 二选一，但表中无 <c>order_id</c> ⇒ 照表建模。</para>
/// <para><b>非幂等</b>：状态推进不可逆（15~19 为换货分支终态），不得放进自动重试管道。</para>
/// <para><b>敏感值</b>：<c>openid</c> 禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaShopAftersaleUpdateRequest
{
    /// <summary>商家自定义订单 ID（必填）。</summary>
    [JsonPropertyName("out_order_id")]
    public string? OutOrderId { get; set; }

    /// <summary>商家自定义售后 ID（必填）。</summary>
    [JsonPropertyName("out_aftersale_id")]
    public string? OutAftersaleId { get; set; }

    /// <summary>用户的 <c>openid</c>（必填，敏感）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>目标售后状态（必填），见 <see cref="WxaShopAftersaleStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>订单维度是否售后全部完成（必填）：0 未结束 / 1 结束并流转订单状态。</summary>
    [JsonPropertyName("finish_all_aftersale")]
    public int? FinishAllAftersale { get; set; }
}
