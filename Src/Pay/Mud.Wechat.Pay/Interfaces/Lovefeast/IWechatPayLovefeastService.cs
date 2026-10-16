// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Lovefeast;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「爱心餐（品牌捐赠 / 供餐信息查询）」域 SDK（1 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 1 支中<b>1 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>一支官方只印商户页</b>（「支持商户：【普通商户】」）⇒ 无服务商镜像；官方文档树的爱心餐章节 API 列表<b>恰此一支</b>（其余节点是产品介绍 / 开发接入准备 / 接入指引 / 查券使用指引）。</para>
/// <para><b>本域频率限制官方<b>未印</b></b>：页内无「接口频率」条目、错误码表也未给 429 ⇒ 不得凭第三方 SDK 补数，方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>「爱心餐查券」不属本域</b>：官方《爱心餐查券接口使用指引》把查券能力指向 <c>GET /v3/marketing/favor/users/{openid}/coupons</c>（既存 <see cref="IWechatPayMarketingFavorService"/>，守卫 MF 系列）⇒ <b>不得</b>在本域重复建一支查券方法，也不得为该指引另造路由。</para>
/// <para><b>入参是品牌 ID 而非商户号</b>：官方前置条件原文「商家进驻微信支付品牌商家后获得的品牌ID」⇒ <c>brand_id</c> 由品牌商家进驻流程产出，商户号本身不构成合法入参。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Lovefeast", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayLovefeastService
{
    /// <summary>
    /// 查询爱心餐品牌信息（捐赠总量与供餐总量）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012472863"/>（【普通商户】）。
    /// </summary>
    /// <param name="brandId">【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（官方 path <c>brand_id</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询爱心餐品牌信息，见 <see cref="LovefeastBrandInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/lovefeast/brands/{brand_id}</c>；path <c>brand_id</c>，<b>无查询参数</b>（官方请求头只列 <c>Authorization</c> / <c>Accept</c>，<b>不涉密</b>）。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级 QPS，错误码表亦无 429 条目。</para>
    /// <para><b>应答两键的语义不对称</b>：<c>donate_count</c> 必填（用户在该品牌下捐赠的总次数），<c>consume_count</c> <b>仅供餐品牌返回</b>（该品牌为环卫工人供餐总量）⇒ 普通品牌该键为 <c>null</c>，<b>不得</b>据其缺失判定调用失败。</para>
    /// <para><b>业务错误码</b>：500 SYSTEM_ERROR：系统错误。</para>
    /// </remarks>
    [Get("/v3/lovefeast/brands/{brandId}")]
    Task<LovefeastBrandInfoResponse> QueryLovefeastBrandAsync(
        [Path] long brandId,
        CancellationToken cancellationToken = default);
}
