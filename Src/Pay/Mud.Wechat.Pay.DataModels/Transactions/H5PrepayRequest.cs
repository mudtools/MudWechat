// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Transactions;

/// <summary>
/// H5（移动端网页）下单请求体（微信支付 APIv3，普通商户直连）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<c>POST /v3/pay/transactions/h5</c>，SKIT 登记的 docId
/// <c>4012791834</c>（<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791834"/>）。
/// <b>字段表以本地 SKIT <c>TenpayV3</c> 的 <c>CreatePayTransactionH5Request</c> 为对齐基准</b>（2026-10-10），
/// 官方逐页核验待补。</para>
/// <para><b>与 <see cref="NativePrepayRequest"/> 的唯一差异在 <c>scene_info</c> 的子表</b>：H5 页的
/// <c>scene_info</c> <b>多</b> <c>h5_info</c>（H5 支付场景信息）⇒ 顶层字段表虽同，子表不同，故本请求
/// 用 <see cref="H5SceneInfo"/> 而<b>不</b>复用 <see cref="JsapiSceneInfo"/>（本线纪律：表相同则共用，表不同则分建；
/// 复用会让 H5 调用方<b>传不了</b>官方要求的 <c>h5_info</c>）。</para>
/// <para><b>必填</b>：<c>appid</c> / <c>mchid</c> / <c>description</c> / <c>out_trade_no</c> /
/// <c>notify_url</c> / <c>amount</c>。选填一律可空。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class H5PrepayRequest
{
    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）。H5 场景须为<b>公众号</b> AppID。</summary>
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

    /// <summary>场景信息（<c>scene_info</c>，选填），见 <see cref="H5SceneInfo"/>（<b>含</b> H5 专属 <c>h5_info</c>）。</summary>
    [JsonPropertyName("scene_info")]
    public H5SceneInfo? SceneInfo { get; set; }

    /// <summary>结算信息（<c>settle_info</c>，选填）：与 JSAPI 页同表 ⇒ 复用 <see cref="JsapiSettleInfo"/>。</summary>
    [JsonPropertyName("settle_info")]
    public JsapiSettleInfo? SettleInfo { get; set; }
}

/// <summary>
/// H5 下单的场景信息（<c>scene_info</c>）—— <b>比 JSAPI / Native / APP 多一节 <c>h5_info</c></b>。
/// </summary>
/// <remarks>
/// <para><b>为何不与 <see cref="JsapiSceneInfo"/> 共用</b>：H5 页的该表<b>多</b> <c>h5_info</c>
/// 一项（SKIT 侧写作「继承 app 的 Scene 再补 <c>H5Info</c>」）⇒ 复用会让 H5 调用方无法上送官方要求的
/// 场景信息；反向把 <c>h5_info</c> 塞进 <see cref="JsapiSceneInfo"/> 又让 JSAPI 能传该页<b>未定义</b>的字段。
/// 两条路都错，故分建（守卫锁定：本类有 <c>H5Info</c> 而 <see cref="JsapiSceneInfo"/> 没有）。</para>
/// <para><b>前三项与 JSAPI 同表</b>（<c>payer_client_ip</c> / <c>device_id</c> / <c>store_info</c>）
/// —— 重复三字段是有意的：本仓 DTO 为扁平形态（不做类型继承），照合单域
/// <c>CombineH5SceneInfo</c> 的既有先例处理。</para>
/// <para><b>待核</b>：SKIT 的 app/native 嵌套 <c>Scene</c> 另含 <c>device_ip</c>、<c>store_info.out_id</c>，
/// H5 继承后同样携带；本仓采 JSAPI 页逐字段核验过的形态，见 <see cref="NativePrepayRequest"/> 的同款说明。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class H5SceneInfo
{
    /// <summary>用户终端 IP（<c>payer_client_ip</c>，必填 string(45)，支持 IPv4 / IPv6）。</summary>
    [JsonPropertyName("payer_client_ip")]
    public string? PayerClientIp { get; set; }

    /// <summary>商户端设备号（<c>device_id</c>，选填 string(32)），门店号或收银设备 ID。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

    /// <summary>商户门店信息（<c>store_info</c>，选填）：与 JSAPI 页同表 ⇒ 复用 <see cref="JsapiStoreInfo"/>。</summary>
    [JsonPropertyName("store_info")]
    public JsapiStoreInfo? StoreInfo { get; set; }

    /// <summary>H5 场景信息（<c>h5_info</c>，选填），见 <see cref="H5Info"/>。</summary>
    [JsonPropertyName("h5_info")]
    public H5Info? H5Info { get; set; }
}

/// <summary>
/// H5 支付场景信息（<c>h5_info</c>，<b>H5 下单专属</b>）。
/// </summary>
/// <remarks>
/// <b>对齐基准</b>：SKIT <c>CreatePayTransactionH5Request.Types.Scene.Types.H5Info</c>（2026-10-10）。
/// SKIT 给出 <c>type</c> 默认值 <c>Wap</c>；合单域核验过的同类字段表显示取值集合为
/// <c>Wap</c> / <c>iOS</c> / <c>Android</c>（<b>跨页事实不得互推</b>，本页取值待官方逐页核验）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class H5Info
{
    /// <summary>H5 支付场景类型（<c>type</c>，选填 string(32)），SKIT 侧默认值 <c>Wap</c>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>应用名（<c>app_name</c>，选填 string(64)）。</summary>
    [JsonPropertyName("app_name")]
    public string? AppName { get; set; }

    /// <summary>网站 URL（<c>app_url</c>，选填 string(128)）。</summary>
    [JsonPropertyName("app_url")]
    public string? AppUrl { get; set; }

    /// <summary>iOS bundle id（<c>bundle_id</c>，选填 string(128)）。</summary>
    [JsonPropertyName("bundle_id")]
    public string? BundleId { get; set; }

    /// <summary>Android 包名（<c>package_name</c>，选填 string(128)）。</summary>
    [JsonPropertyName("package_name")]
    public string? PackageName { get; set; }
}

/// <summary>
/// H5 下单应答（<b>仅 <c>h5_url</c> 一个业务字段</b>）。
/// </summary>
/// <remarks>
/// <para><b>为何不与 <see cref="AppPrepayResponse"/> 共用</b>：APP / JSAPI 给的是 <c>prepay_id</c>（需客户端二次签名调起），
/// H5 给的是<b>可直接跳转的支付链接 <c>h5_url</c></b> —— 形态与用法都不同，不可互换。</para>
/// <para><b>严禁改写该链接</b>：须按官方《H5 调起支付》指引原样跳转（含 <c>Referer</c> 等要求），
/// 自行拼装或改写会导致官方侧校验失败且症状表现为「跳转后报参数错」，最难定位。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class H5PrepayResponse : WechatPayResponse
{
    /// <summary>H5 支付跳转链接（<c>h5_url</c>，string(512)）：须原样跳转，不得改写或拼装。</summary>
    [JsonPropertyName("h5_url")]
    public string? H5Url { get; set; }
}
