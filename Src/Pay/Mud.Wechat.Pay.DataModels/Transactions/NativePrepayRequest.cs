// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Transactions;

/// <summary>
/// Native（扫码支付）下单请求体（微信支付 APIv3，普通商户直连）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<c>POST /v3/pay/transactions/native</c>，SKIT 登记的 docId
/// <c>4012791877</c>（<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791877"/>）。
/// <b>字段表以本地 SKIT <c>TenpayV3</c> 的 <c>CreatePayTransactionNativeRequest</c> 为对齐基准</b>
/// （2026-10-10），官方逐页核验待补 —— 与 <see cref="JsapiPrepayRequest"/> 的差项已逐项比对，
/// 该页事实<b>不得</b>从 JSAPI 页互推。</para>
/// <para><b>为何单独建类型而不复用 <see cref="JsapiPrepayRequest"/></b>：本请求<b>没有</b> <c>payer</c>
/// （扫码支付由用户扫码后自行确认，下单时<b>不存在</b>支付者标识）。复用会让调用方能在 Native 请求里
/// 塞进官方<b>未定义</b>的 <c>payer</c> —— 本线纪律（合单域 CB5 已确立）：<b>表相同则共用，表不同则分建</b>。</para>
/// <para><b>可复用的部分</b>：<c>amount</c> / <c>detail</c> / <c>scene_info</c> / <c>settle_info</c>
/// 四张子表与 JSAPI 页<b>逐项一致</b>（SKIT 侧即直接继承同一支嵌套类型）⇒ 共用
/// <see cref="JsapiAmountInfo"/> / <see cref="JsapiGoodsDetail"/> / <see cref="JsapiSceneInfo"/> /
/// <see cref="JsapiSettleInfo"/>，守卫锁定该复用关系。</para>
/// <para><b>待核的嵌套字段差</b>：SKIT 的 app/native 嵌套 <c>Scene</c> 另含 <c>device_ip</c>、
/// <c>store_info.out_id</c>，<c>Settlement</c> 另含 <c>subsidy_amount</c>；本仓 <see cref="JsapiSceneInfo"/> /
/// <see cref="JsapiSettleInfo"/> 在 JSAPI 页逐字段核验时官方<b>未列</b>这几项，故沿用已核验形态。
/// 若 Native / APP 页核验后确认官方确有，应<b>新建</b>该族的场景/结算类型而非改动 JSAPI 那支。</para>
/// <para><b>必填</b>：<c>appid</c> / <c>mchid</c> / <c>description</c> / <c>out_trade_no</c> /
/// <c>notify_url</c> / <c>amount</c>。选填一律可空（AOT 下可空性即「是否随报文上送」的唯一表达）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class NativePrepayRequest
{
    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）。须与 <c>mchid</c> 有绑定关系。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商品描述（<c>description</c>，必填 string(127)），用户微信账单的商品字段中可见。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 商户订单号（<c>out_trade_no</c>，必填 string(32)）：6-32 字符，
    /// 只能是数字、大小写字母 <c>_-|*</c>，同一商户号下唯一。
    /// </summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>
    /// 支付结束时间（<c>time_expire</c>，选填 string(64)，rfc3339）。超时后无法支付；
    /// 官方建议超时先调关闭订单接口再以新商户订单号重下。
    /// </summary>
    [JsonPropertyName("time_expire")]
    public string? TimeExpire { get; set; }

    /// <summary>商户数据包（<c>attach</c>，选填 string(128)），对用户不可见，查单与回调均原样返回。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>商户回调地址（<c>notify_url</c>，必填 string(255)），支付成功回调通知的接收地址。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>订单优惠标记（<c>goods_tag</c>，选填 string(32)），代金券按标记匹配优惠。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>电子发票入口开放标识（<c>support_fapiao</c>，选填 boolean）。</summary>
    [JsonPropertyName("support_fapiao")]
    public bool? SupportFapiao { get; set; }

    /// <summary>订单金额（<c>amount</c>，必填）：与 JSAPI 页同表 ⇒ 复用 <see cref="JsapiAmountInfo"/>。</summary>
    [JsonPropertyName("amount")]
    public JsapiAmountInfo? Amount { get; set; }

    /// <summary>商品详情（<c>detail</c>，选填）：与 JSAPI 页同表 ⇒ 复用 <see cref="JsapiGoodsDetail"/>。</summary>
    [JsonPropertyName("detail")]
    public JsapiGoodsDetail? Detail { get; set; }

    /// <summary>场景信息（<c>scene_info</c>，选填）：与 JSAPI 页同表 ⇒ 复用 <see cref="JsapiSceneInfo"/>。</summary>
    [JsonPropertyName("scene_info")]
    public JsapiSceneInfo? SceneInfo { get; set; }

    /// <summary>结算信息（<c>settle_info</c>，选填）：与 JSAPI 页同表 ⇒ 复用 <see cref="JsapiSettleInfo"/>。</summary>
    [JsonPropertyName("settle_info")]
    public JsapiSettleInfo? SettleInfo { get; set; }
}

/// <summary>
/// Native 下单应答（<b>仅 <c>code_url</c> 一个业务字段</b>）。
/// </summary>
/// <remarks>
/// <para><b>为何不与 <see cref="PrepayIdResponse"/> 共用</b>：JSAPI 给的是 <c>prepay_id</c>
/// （供微信内 <c>requestPayment</c> 调起），Native 给的是<b>支付二维码链接 <c>code_url</c></b> ——
/// 官方两页应答表各自只有一个字段且字段名不同，<b>二者不可互换</b>。</para>
/// <para><b><c>code_url</c> 有效期 2 小时</b>：失效后须<b>重新调用下单接口</b>取新链接，
/// <b>不得</b>对旧链接做拼接或改写。服务端须自行把它生成二维码展示（Native 侧无官方页面）。</para>
/// <para>继承 <see cref="WechatPayResponse"/> 以承载失败态的 <c>code</c>/<c>message</c>（判错面），理由同 <see cref="PrepayIdResponse"/>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class NativePrepayResponse : WechatPayResponse
{
    /// <summary>二维码链接（<c>code_url</c>，string(64)）：<b>有效期 2 小时</b>，由商户自行生成二维码。</summary>
    [JsonPropertyName("code_url")]
    public string? CodeUrl { get; set; }
}
