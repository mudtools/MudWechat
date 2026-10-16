// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.WebizpayStore;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「微企付企业支付 · 门店主体匹配」域 SDK（2 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径。本域每支路由官方<b>只印一页</b> ⇒ <b>不存在</b>「同路由多页字段表比对」问题，一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>「普通服务商」身份口径</b>：本族 12 页「支持商户」栏<b>恒为【普通服务商】</b>（无【平台商户】/【普通商户】分身页），
/// 请求一律以 <c>sp_mchid</c>（服务商商户号）+ <c>sub_mchid</c>（出资子商户号）双层定位 ⇒ 服务商身份由调用侧签名承载；
/// <b>唯一例外</b>是门店主体匹配两支只印 <c>sp_mchid</c>（守卫 WEB-ST）⇒ 不得跨域补字段。</para>
/// <para><b>本域两支是微企付 12 页中唯一只印 <c>sp_mchid</c>、零 <c>sub_mchid</c> 的一对</b>：发起支 Body 只有 <c>sp_mchid</c> / <c>out_batch_id</c> / <c>match_details</c>，查询支只有必填 query <c>sp_mchid</c> ⇒ 不得按本族其余十支的「服务商 + 出资子商户」双层形态补字段。</para>
/// <para><b>受理 ≠ 结果</b>：发起支应答 5 行只回批次号与 <c>state</c>（<c>ACCEPTED</c> / <c>IN_PROGRESS</c> / <c>COMPLETED</c> / <c>CLOSED</c>），匹配明细只出现在查询支的 <c>match_results</c> ⇒ 非 <c>COMPLETED</c> 须继续轮询而非报错； 且官方明写「若结果为空，列表长度为 0」⇒ <c>COMPLETED</c> + 空数组是合法结果（无一家门店匹配到有效商户号）。</para>
/// <para><b><c>match_details</c> 官方标选填、但元素三字段全必填</b>；<c>product_type</c> 可选取值只有 <c>CODE_PAY</c> / <c>NONE</c>（正文另有一句「目前仅支持 CODEPAY」为无分隔写法）⇒ 名单照官方<b>可选取值列表</b>取 <c>CODE_PAY</c>，不做拼写归一。</para>
/// <para><b>幂等标识 <c>out_batch_id</c></b>：官方口径「6-32 个字符，只能是数字、大小写字母 <c>-_|*</c> 且在同一个商户号下唯一」 ⇒ 商户批次单号必须稳定可重放，不得每次随机生成。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "WebizpayStore", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayWebizpayStoreService
{
    /// <summary>
    /// 发起门店主体匹配。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507136"/>（【普通服务商】，官方页更新于 2025.05.09）。
    /// </summary>
    /// <param name="request">发起门店主体匹配请求体，字段见 <see cref="WebizpayStoreEntityMatchRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>发起门店主体匹配，见 <see cref="WebizpayStoreEntityMatchResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/webizpay/stores/entity-matches</c>；无 path / query 参数。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="WebizpayStoreEntityMatchResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Post("/v3/webizpay/stores/entity-matches")]
    Task<WebizpayStoreEntityMatchResponse> MatchStoreEntitiesAsync(
        [Body] WebizpayStoreEntityMatchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询门店主体匹配结果。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507133"/>（【普通服务商】，官方页更新于 2025.05.09）。
    /// </summary>
    /// <param name="batchId">【微信支付主体匹配批次单号】 微信支付主体匹配批次单号，标记某一次响应商户主体匹配信息对应的处理单号，在微信支付体系下唯一。从发起门店主体匹配时微信支付的返回中获得。（官方 path <c>batch_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="spMchid">【服务商商户号】 是由微信支付系统生成并分配给每个服务商的唯一标识符，具体请参考 服务商模式开发必要参数说明。（官方 query <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询门店主体匹配结果，见 <see cref="WebizpayStoreEntityMatchQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/webizpay/stores/entity-matches/{batch_id}</c>；path <c>batch_id</c>，query <c>sp_mchid</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sp_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Get("/v3/webizpay/stores/entity-matches/{batchId}")]
    Task<WebizpayStoreEntityMatchQueryResponse> QueryStoreEntityMatchAsync(
        [Path] string batchId,
        [Query("sp_mchid")] string spMchid,
        CancellationToken cancellationToken = default);
}
