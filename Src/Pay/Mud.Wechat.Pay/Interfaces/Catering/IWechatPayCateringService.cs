// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Catering;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「点餐订单同步（行业方案·餐饮）」域 SDK（1 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 1 支中<b>1 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 1 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>无应答体支的返回形态</b>：本域 1 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 <see cref="Task"/>，不建响应类型。</para>
/// <para><b>一支官方只印服务商页</b>（路由属 <c>/v3/partner</c> 文档树、正文身份标签缺省）⇒ 无商户面镜像， 一支一页一方法；<b>不得</b>按其余行业域的双页形态补建商户面。</para>
/// <para><b>「状态变化即上传全量明细」是官方强制语义</b>：页首原文「当点餐订单状态发生变化时，都上传<b>全量</b>的订单明细」， 并列出 <c>CREATE_DEAL</c>（下单）与 <c>PAY_SUCCESS</c>（支付成功）两态上报时机 ⇒ 本方法<b>不是</b>增量补丁接口， 每次调用都要带齐订单与菜品明细，SDK 不做本地增量合并。</para>
/// <para><b>204 无应答包体 ⇒ 返回裸 <see cref="Task"/></b>：官方应答节原文「无数据(Http状态码为204)」；该页内出现的 <c>code</c> / <c>message</c> / <c>detail</c> 属「<b>异常返回</b>」形态，<b>不得</b>建成应答 DTO（取证缓存已按页处置）。</para>
/// <para><b>31 行请求表是「订单 + 菜品」两层摊平</b>：<c>dish_list</c> 元素的 <c>out_dish_no</c> / <c>name</c> / <c>price</c> / <c>unit</c> / <c>count</c> / <c>taste</c> / <c>cuisine</c> 与订单级字段同表印制 ⇒ 类型上分建订单请求与菜品元素类型； <c>properties</c> 官方只写「对象」、不给元素结构 ⇒ 以 <c>JsonElement</c> 承载；<c>count</c> 官方类型是 <b><c>Float</c></b> ⇒ 落 <c>double</c>，不得做成整数或字符串。</para>
/// <para><b>该页官方原文大小写混排</b>（类型列写 <c>String(32)</c> 与 <c>int</c>，其余行业域一律小写 <c>string</c>）， 且 <c>login_token</c> / <c>order_entry</c> 是<b>本支必填</b>而 <c>transaction_id</c> / <c>pay_time</c> 选填 ⇒ 逐字段口径照页， 不按基础支付域「支付字段必填」的直觉改写。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Catering", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayCateringService
{
    /// <summary>
    /// 点餐订单信息同步。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015863180"/>（）。
    /// </summary>
    /// <param name="request">点餐订单信息同步请求体，字段见 <see cref="CateringOrderSyncStatusRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段（官方应答节原文「无应答包体」/「无参数返回」，正常示例状态码 <b>204 No Content</b>） ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/catering/orders/sync-status</c>；无 path / query 参数。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// </remarks>
    [Post("/v3/catering/orders/sync-status")]
    Task SyncCateringOrderStatusAsync(
        [Body] CateringOrderSyncStatusRequest request,
        CancellationToken cancellationToken = default);
}
