// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// 更新售后期限请求体（<c>POST /shop/aftersale/update_deadline</c>）。
/// </summary>
/// <remarks>
/// <para><b>期限窗口硬约束</b>：<c>after_sale_deadline</c> 为 Unix <b>秒</b>级时间戳，官方限定取值</para>
/// <para>「当前时间 + [7d，3 × 365d]」⇒ 早于 7 天或晚于 3 年均越界。</para>
/// <para><b>敏感值</b>：<c>openid</c> 禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaShopAftersaleUpdateDeadlineRequest
{
    /// <summary>微信侧内部单号（选填）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>商家自定义订单 ID（选填；字符集大小写字母数字，长度 &lt; 128）。</summary>
    [JsonPropertyName("out_order_id")]
    public string? OutOrderId { get; set; }

    /// <summary>用户的 <c>openid</c>（必填，敏感）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>售后期限，Unix 秒级时间戳（必填；当前时间 + [7d, 3 × 365d]）。</summary>
    [JsonPropertyName("after_sale_deadline")]
    public long? AfterSaleDeadline { get; set; }
}
