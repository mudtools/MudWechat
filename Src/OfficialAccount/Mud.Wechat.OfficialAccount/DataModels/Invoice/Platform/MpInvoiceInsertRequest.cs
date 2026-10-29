// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

// ---------------------------------------------------------------- 将电子发票卡券插入用户卡包（POST /card/invoice/insert）

/// <summary>
/// 将电子发票卡券插入用户卡包（<c>POST /card/invoice/insert</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<c>order_id</c> / <c>card_id</c> / <c>appid</c> / <c>card_ext</c> 均必填。
/// </para>
/// <para>
/// <b>调用方约束（官方接口描述原文，勿弱化）</b>：本接口由开票平台或自建平台商户调用；
/// <b>「需要使用之前调用获取 <c>s_pappid</c> 接口时的开票平台公众号 appid 调用本接口，否则会造成报错，插卡失败」</b>
/// —— 但官方字段表把 <c>appid</c> 说明为「该订单号授权时使用的 appid，<b>一般为商户 appid</b>」，两处口径不一（照录）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceInsertRequest
{
    /// <summary>获取或设置发票订单号（官方 <c>order_id</c>，必填；商户给用户授权开票的订单号）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置发票卡券 id（官方 <c>card_id</c>，必填；取自 <c>createcard</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置调用方 appid（官方 <c>appid</c>，必填；口径矛盾见类型级 remarks）。</summary>
    [JsonPropertyName("appid")]
    public string AppId { get; set; } = string.Empty;

    /// <summary>获取或设置发票具体内容（官方 <c>card_ext</c>，必填）。</summary>
    [JsonPropertyName("card_ext")]
    public MpInvoiceCardExt CardExt { get; set; } = new MpInvoiceCardExt();
}
