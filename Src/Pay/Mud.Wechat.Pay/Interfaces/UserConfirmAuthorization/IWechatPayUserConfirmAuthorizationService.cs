// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.UserConfirmAuthorization;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「免确认收款授权」域 SDK（3 端点）。
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
/// <para><b>三段式生命周期</b>：创建（<c>POST /user-confirm-authorization</c>）→ 查询（<c>GET .../{out_authorization_no}</c>）→ 解除（<c>POST .../close</c>） ⇒ 三支语义不可折叠，应答<b>不同形</b>（创建 6 行、查询 8 行、解除无应表）。</para>
/// <para><b>创建支请求体含敏感字段</b>：<c>user_name</c> 需 RSA-OAEP 加密（同普通转账域的姓名加密口径）⇒ 宿主须在调用前完成加密。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "UserConfirmAuthorization", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayUserConfirmAuthorizationService
{
    /// <summary>
    /// 发起免确认收款授权。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4015901167"/>（【普通商户】）。
    /// </summary>
    /// <param name="request">发起免确认收款授权请求体，字段见 <see cref="UserConfirmAuthorizationCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>发起免确认收款授权，见 <see cref="UserConfirmAuthorizationCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/fund-app/mch-transfer/user-confirm-authorization</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>（官方原样）：- 单个商户的接口频率限制为50次/s。</para>
    /// <para><b>应答</b>见 <see cref="UserConfirmAuthorizationCreateResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 RATELIMIT_EXCEEDED 频率超限 / 400 INVALID_REQUEST 当前申请单已解除或已授权成功 / 400 INVALID_REQUEST 用户在当前场景下的授权数量达到限制，不支持发起授权。</para>
    /// </remarks>
    [Post("/v3/fund-app/mch-transfer/user-confirm-authorization")]
    Task<UserConfirmAuthorizationCreateResponse> CreateUserConfirmAuthorizationAsync(
        [Body] UserConfirmAuthorizationCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 免确认收款授权结果查询。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014399423"/>（【普通商户】）。
    /// </summary>
    /// <param name="outAuthorizationNo">【商户侧授权单号】 商户系统内部的免确认收款授权单号。（官方 path <c>out_authorization_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="isDisplayAuthorization">【是否展示用户授权信息】 true-是；false-否，默认否。商户可选择是否在响应中返回 package 信息，用于通过 JSAPI 或 APP 方式调起用户授权信息页。（官方 query <c>is_display_authorization</c>，选填，类型 <c>boolean</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>免确认收款授权结果查询，见 <see cref="UserConfirmAuthorizationQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/fund-app/mch-transfer/user-confirm-authorization/out-authorization-no/{out_authorization_no}</c>；path <c>out_authorization_no</c>，query <c>is_display_authorization</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>（官方原样）：- 单个商户的接口频率限制为100次/s。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 NOT_FOUND 免确认收款授权不存在 / 429 RATELIMIT_EXCEEDED 频率超限。</para>
    /// </remarks>
    [Get("/v3/fund-app/mch-transfer/user-confirm-authorization/out-authorization-no/{outAuthorizationNo}")]
    Task<UserConfirmAuthorizationQueryResponse> QueryUserConfirmAuthorizationAsync(
        [Path] string outAuthorizationNo,
        [Query("is_display_authorization")] bool? isDisplayAuthorization = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 解除免确认收款授权。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4015653811"/>（【普通商户】）。
    /// </summary>
    /// <param name="outAuthorizationNo">【商户侧授权单号】 商户系统内部的免确认收款授权单号。（官方 path <c>out_authorization_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>解除免确认收款授权，见 <see cref="UserConfirmAuthorizationCloseResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/fund-app/mch-transfer/user-confirm-authorization/out-authorization-no/{out_authorization_no}/close</c>；path <c>out_authorization_no</c>。</para>
    /// <para><b>支持商户</b>【普通商户】。</para>
    /// <para><b>频率限制</b>（官方原样）：注：单个商户的接口频率限制为50次/s。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 NOT_FOUND 免确认收款授权不存在 / 429 RATELIMIT_EXCEEDED 频率超限。</para>
    /// </remarks>
    [Post("/v3/fund-app/mch-transfer/user-confirm-authorization/out-authorization-no/{outAuthorizationNo}/close")]
    Task<UserConfirmAuthorizationCloseResponse> CloseUserConfirmAuthorizationAsync(
        [Path] string outAuthorizationNo,
        CancellationToken cancellationToken = default);
}
