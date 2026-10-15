// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.BrandRedpacket;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「品牌商户红包批次」域 SDK（5 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径。本域每支路由官方<b>只印一页</b>（不像营销/商户经营面存在同一路径的【普通商户】与【普通服务商】双页）
/// ⇒ <b>不存在</b>「同路由多页字段表比对」问题，一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 7 个官方页树<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 RATELIMIT_EXCEEDED</c> /
/// <c>FREQUENCY_LIMITED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>分页口径不统一（照页，不得互套）</b>：本域列表支存在<b>两种</b>整型分页形态
/// —— <c>offset</c>/<c>limit</c>（会员卡模板列表、用户会员卡列表、品牌门店列表）与
/// <c>page_index</c>/<c>page_size</c>（交易连接名片规则列表）；且同为 <c>offset</c>/<c>limit</c>，
/// 会员卡两支官方<b>必填</b>、门店列表支官方<b>选填</b> ⇒ 必填口径也须照页。应答总数字段官方为选填。</para>
/// <para><b><c>brand_id</c> 是本域的「商户作用域参数」</b>：除品牌入驻面（<c>/brand/applyments/*</c>）与免密契约面外，
/// 本域其余每支都把 <c>brand_id</c> 印为<b>必填 query</b> ⇒ 它不是可选过滤条件，缺省即 400；
/// 长度上限官方<b>不统一</b>（名片配置/连接/门店面为 <c>string(20)</c>，会员卡面为 <c>string(32)</c>），照页不做本地归一。</para>
/// <para><b>两套定位键</b>：批次查询支有「微信批次号」与「商家批次号」两支，官方两页应答表<b>逐字段同形</b> ⇒ 共用一个响应类型； 明细查询支同理（微信明细号 / 商家明细号）⇒ 亦共用。</para>
/// <para><b>发放支请求体庞大</b>：官方请求表 17 行（含 <c>stock_type</c> / <c>send_name</c> / <c>total_amount</c> 等）， 与查询支的纯 query 形态<b>不得互套</b>。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "BrandRedpacket", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayBrandRedpacketService
{
    /// <summary>
    /// 品牌商户红包批次发放。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014310358"/>（【普通商户】）。
    /// </summary>
    /// <param name="request">品牌商户红包批次发放请求体，字段见 <see cref="BrandRedpacketBatchCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌商户红包批次发放，见 <see cref="BrandRedpacketBatchCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/fund-app/brand-redpacket/brand-merchant-batches</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandRedpacketBatchCreateResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：429 FREQUENCY_LIMIT_EXCEED 请求接口频率过快 / 403 NOT_ENOUGH 资金不足。</para>
    /// </remarks>
    [Post("/v3/fund-app/brand-redpacket/brand-merchant-batches")]
    Task<BrandRedpacketBatchCreateResponse> CreateBrandRedpacketBatchAsync(
        [Body] BrandRedpacketBatchCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 品牌商户红包批次查询（微信批次号）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014310369"/>（【普通商户】）。
    /// </summary>
    /// <param name="batchNo">【微信支付品牌红包批次单号】 微信支付品牌红包批次单号，微信商家品牌红包系统返回的唯一标识（官方 path <c>batch_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="needQueryDetail">【是否需要查询品牌红包明细单】 枚举值： true：是； false：否，默认否。 商户可选择是否查询指定状态的品牌红包明细单，当品牌红包批次单状态为“FINISHED”（已完成）时，才会返回满足条件的品牌红包明细单（官方 query <c>need_query_detail</c>，选填，类型 <c>boolean</c>）。</param>
    /// <param name="detailState">【品牌红包明细单状态】 查询指定状态的品牌红包明细单信息。可选取值：`DETAIL_VIEW_ALL` 需要同时查询转账成功、失败和待确认的品牌红包明细单 / `DETAIL_VIEW_SUCCESS` 只查询发送成功的品牌红包明细单 / `DETAIL_VIEW_FAIL` 只查询发送失败的品牌红包明细单（官方 query <c>detail_state</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌商户红包批次查询（微信批次号），见 <see cref="BrandRedpacketBatchQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/fund-app/brand-redpacket/brand-merchant-batches/{batch_no}</c>；path <c>batch_no</c>，query <c>need_query_detail</c> / <c>detail_state</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：429 FREQUENCY_LIMIT_EXCEED 请求接口频率过快 / 404 NOT_FOUND 记录不存在。</para>
    /// </remarks>
    [Get("/v3/fund-app/brand-redpacket/brand-merchant-batches/{batchNo}")]
    Task<BrandRedpacketBatchQueryResponse> QueryBrandRedpacketBatchByBatchNoAsync(
        [Path] string batchNo,
        [Query("need_query_detail")] bool? needQueryDetail = null,
        [Query("detail_state")] string? detailState = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 品牌商户红包明细查询（微信明细号）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014310384"/>（【普通商户】）。
    /// </summary>
    /// <param name="batchNo">【微信支付品牌红包批次单号】 微信支付品牌红包批次单号，微信商家品牌红包系统返回的唯一标识（官方 path <c>batch_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="detailNo">【微信支付品牌红包明细单号】 微信支付系统内部区分品牌红包批次单下不同品牌红包明细单的唯一标识（官方 path <c>detail_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌商户红包明细查询（微信明细号），见 <see cref="BrandRedpacketDetailQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/fund-app/brand-redpacket/brand-merchant-batches/{batch_no}/details/{detail_no}</c>；path <c>batch_no</c> / <c>detail_no</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：429 FREQUENCY_LIMIT_EXCEED 请求接口频率过快 / 404 NOT_FOUND 记录不存在。</para>
    /// </remarks>
    [Get("/v3/fund-app/brand-redpacket/brand-merchant-batches/{batchNo}/details/{detailNo}")]
    Task<BrandRedpacketDetailQueryResponse> QueryBrandRedpacketDetailByDetailNoAsync(
        [Path] string batchNo,
        [Path] string detailNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 品牌商户红包批次查询（商家批次号）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014310378"/>（【普通商户】）。
    /// </summary>
    /// <param name="outBatchNo">【商家品牌红包批次单号】 商户系统内部的商家品牌红包批次单号，要求此参数只能由数字、大小写字母组成，在商户系统内部唯一（官方 path <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="needQueryDetail">【是否需要查询品牌红包明细单】 枚举值： true：是； false：否，默认否。 商户可选择是否查询指定状态的品牌红包明细单，当品牌红包批次单状态为“FINISHED”（已完成）时，才会返回满足条件的品牌红包明细单（官方 query <c>need_query_detail</c>，选填，类型 <c>boolean</c>）。</param>
    /// <param name="detailState">【品牌红包明细单状态】 查询指定状态的品牌红包明细单信息。可选取值：`DETAIL_VIEW_ALL` 需要同时查询转账成功、失败和待确认的品牌红包明细单 / `DETAIL_VIEW_SUCCESS` 只查询发送成功的品牌红包明细单 / `DETAIL_VIEW_FAIL` 只查询发送失败的品牌红包明细单（官方 query <c>detail_state</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌商户红包批次查询（商家批次号），见 <see cref="BrandRedpacketBatchQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/fund-app/brand-redpacket/brand-merchant-out-batches/{out_batch_no}</c>；path <c>out_batch_no</c>，query <c>need_query_detail</c> / <c>detail_state</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：429 FREQUENCY_LIMIT_EXCEED 请求接口频率过快 / 404 NOT_FOUND 记录不存在。</para>
    /// </remarks>
    [Get("/v3/fund-app/brand-redpacket/brand-merchant-out-batches/{outBatchNo}")]
    Task<BrandRedpacketBatchQueryResponse> QueryBrandRedpacketBatchByOutBatchNoAsync(
        [Path] string outBatchNo,
        [Query("need_query_detail")] bool? needQueryDetail = null,
        [Query("detail_state")] string? detailState = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 品牌商户红包明细查询（商家明细号）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014310391"/>（【普通商户】）。
    /// </summary>
    /// <param name="outBatchNo">【商家品牌红包批次单号】 商户系统内部的商家品牌红包批次单号，要求此参数只能由数字、大小写字母组成，在商户系统内部唯一（官方 path <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="outDetailNo">【商家品牌红包明细单号】 商户系统内部区分品牌红包批次单下不同品牌红包明细单的唯一标识，要求此参数只能由数字、大小写字母组成（官方 path <c>out_detail_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>品牌商户红包明细查询（商家明细号），见 <see cref="BrandRedpacketDetailQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/fund-app/brand-redpacket/brand-merchant-out-batches/{out_batch_no}/out-details/{out_detail_no}</c>；path <c>out_batch_no</c> / <c>out_detail_no</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：429 FREQUENCY_LIMIT_EXCEED 请求接口频率过快 / 404 NOT_FOUND 记录不存在。</para>
    /// </remarks>
    [Get("/v3/fund-app/brand-redpacket/brand-merchant-out-batches/{outBatchNo}/out-details/{outDetailNo}")]
    Task<BrandRedpacketDetailQueryResponse> QueryBrandRedpacketDetailByOutDetailNoAsync(
        [Path] string outBatchNo,
        [Path] string outDetailNo,
        CancellationToken cancellationToken = default);
}
