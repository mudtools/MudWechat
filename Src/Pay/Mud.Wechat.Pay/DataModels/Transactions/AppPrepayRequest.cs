// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Transactions;

/// <summary>
/// APP 下单请求体（微信支付 APIv3，普通商户直连）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<c>POST /v3/pay/transactions/app</c>，SKIT 登记的 docId
/// <c>4013070347</c>（<see href="https://pay.weixin.qq.com/doc/v3/merchant/4013070347"/>）。
/// <b>字段表以本地 SKIT <c>TenpayV3</c> 的 <c>CreatePayTransactionAppRequest</c> 为对齐基准</b>（2026-10-10），
/// 官方逐页核验待补。</para>
/// <para><b>与 JSAPI / Native 的两处差异</b>：① <b>无</b> <c>payer</c>（APP 由客户端 SDK 调起，
/// 下单时服务端<b>拿不到</b>也不该猜支付者标识）；② <b>多</b> <c>subsidy_info</c>（补贴详情，
/// 见 <see cref="AppSubsidyInfo"/>）—— 这两项都使本请求<b>不能</b>与
/// <see cref="JsapiPrepayRequest"/> / <see cref="NativePrepayRequest"/> 共用（本线纪律：表相同则共用，表不同则分建）。</para>
/// <para><b>应答与 JSAPI 共用</b> <see cref="PrepayIdResponse"/>：官方两页应答表<b>逐项一致</b>
/// （均只有 <c>prepay_id</c>）⇒ 按「表相同则共用」不另建类型（Native / H5 应答形态不同，各自独立类型）。</para>
/// <para><b>必填</b>：<c>appid</c> / <c>mchid</c> / <c>description</c> / <c>out_trade_no</c> /
/// <c>notify_url</c> / <c>amount</c>。选填一律可空。</para>
/// <para><b>调起支付</b>：<c>prepay_id</c> 返回给 APP 客户端后，须由移动 SDK 按官方《APP 调起支付》
/// 组装 <c>appid/partnerId/prepayid/nonceStr/timestamp/package=Sign=WXPay</c> 并二次签名，
/// <b>服务端不得</b>替客户端签出该串（凭据与签名域不同）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class AppPrepayRequest
{
    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）。APP 场景须为<b>移动应用</b> AppID。</summary>
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

    /// <summary>
    /// 补贴详情（<c>subsidy_info</c>，选填）<b>—— APP 族相对 Native 多出的字段</b>，见 <see cref="AppSubsidyInfo"/>。
    /// </summary>
    [JsonPropertyName("subsidy_info")]
    public AppSubsidyInfo? SubsidyInfo { get; set; }
}

/// <summary>
/// 补贴详情（<c>subsidy_info</c>，APP 下单选填对象）。
/// </summary>
/// <remarks>
/// <b>对齐基准</b>：SKIT <c>CreatePayTransactionAppRequest.Types.Subsidy</c>（2026-10-10），
/// 官方 APP 页逐页核验待补。字段名照官方原文 <c>subsidy_detail</c> / <c>subsidy_period_type</c> /
/// <c>subsidy_plan</c> / <c>subsidy_installment_num</c> / <c>subsidy_percent</c>，
/// <b>不得驼峰化</b>（守卫 PAY-B5 锁定）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class AppSubsidyInfo
{
    /// <summary>补贴明细列表（<c>subsidy_detail</c>，选填 array），见 <see cref="AppSubsidyDetail"/>。</summary>
    [JsonPropertyName("subsidy_detail")]
    public List<AppSubsidyDetail>? SubsidyDetail { get; set; }
}

/// <summary>补贴明细（<c>subsidy_info.subsidy_detail[]</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class AppSubsidyDetail
{
    /// <summary>补贴周期类型（<c>subsidy_period_type</c>，选填 string）。</summary>
    [JsonPropertyName("subsidy_period_type")]
    public string? SubsidyPeriodType { get; set; }

    /// <summary>补贴方案列表（<c>subsidy_plan</c>，选填 array），见 <see cref="AppSubsidyPlan"/>。</summary>
    [JsonPropertyName("subsidy_plan")]
    public List<AppSubsidyPlan>? SubsidyPlan { get; set; }
}

/// <summary>补贴方案（<c>subsidy_detail[].subsidy_plan[]</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class AppSubsidyPlan
{
    /// <summary>补贴期数（<c>subsidy_installment_num</c>，选填整型）。</summary>
    [JsonPropertyName("subsidy_installment_num")]
    public long? SubsidyInstallmentNum { get; set; }

    /// <summary>补贴比例（<c>subsidy_percent</c>，选填整型）。</summary>
    [JsonPropertyName("subsidy_percent")]
    public long? SubsidyPercent { get; set; }
}
