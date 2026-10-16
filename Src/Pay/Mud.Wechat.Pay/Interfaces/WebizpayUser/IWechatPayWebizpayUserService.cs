// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.WebizpayUser;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「微企付企业支付 · 员工开通授权」域 SDK（2 端点）。
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
/// <para><b>两支是「商户员工 ID ↔ 微信授权关系 ID」互查的两端</b>：<c>user_id</c>（官方【商户企业员工 ID】， 6-32 个字符、只能是数字、大小写字母 <c>-_|*</c> 且同一商户号下唯一）在 path， 授权状态支应答的 <c>employee_list</c> 回 <c>employee_id</c>（<c>WeBizPay_…</c>）⇒ 额度卡域各支要的是 <c>employee_id</c>，须先由本域授权状态支取得。</para>
/// <para><b>200 应答里零链接不是错误</b>：授权链接支 Body <c>application_type</c> 必填（<c>H5</c> / <c>MINIPROGRAM</c>）， 应答 8 行中 <c>authorization_url</c> / <c>expire_time</c> / <c>mp_query</c> / <c>mp_business_type</c> <b>全为选填</b>， 官方口径「仅当可以开通时返回」⇒ 调用侧须按「有链接 / 无链接」分支，不得断言非空。</para>
/// <para><b>小程序参数有效期 35 分钟</b>（比跳转链接更短）且仅在 <c>MINIPROGRAM</c> 且允许拉起时返回； 其消费面是小程序 JSAPI <c>wx.openBusinessView</c>（官方页 4014997783，【普通商户】侧、非 HTTP 端点）⇒ <b>本域只透传 <c>mp_query</c> / <c>mp_business_type</c>，零端点</b>。</para>
/// <para><b>微企付另有 5 支「通知」页</b>（关系授权完成 4014612130、额度卡失效 4015764389、支付成功 4015764398、 退款成功 4015764405、解除授权 4015764414）：方向是<b>微信 → 商户回调地址</b>、回调地址由运营配置， 不属商户侧主动调用的 HTTP 契约面 ⇒ <b>本域零端点</b>（回调落地属支付线回调面另案）。</para>
/// <para><b>授权状态支 <c>employee_list</c> 可为 0 长度</b>（官方：同一商户下相同企业员工 ID 未开通过企业支付）， 列表元素四字段（<c>employee_id</c> / <c>authorization_state</c> / <c>authorization_time</c> / <c>authorization_revoked_time</c>）全必填 ⇒ 空数组是正常态、不是失败。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "WebizpayUser", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayWebizpayUserService
{
    /// <summary>
    /// 企业商户发起员工开通企业支付授权。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507153"/>（【普通服务商】，官方页更新于 2025.05.09）。
    /// </summary>
    /// <param name="userId">【商户企业员工ID】 商户企业员工ID，由企业系统提供的唯一标识员工的ID。要求6-32个字符内，只能是数字、大小写字母_-&amp;#124;\* 且在同一个商户号下唯一。（官方 path <c>user_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">企业商户发起员工开通企业支付授权请求体，字段见 <see cref="WebizpayUserAuthorizationUrlRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>企业商户发起员工开通企业支付授权，见 <see cref="WebizpayUserAuthorizationUrlResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/webizpay/users/{user_id}/authorization-url</c>；path <c>user_id</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="WebizpayUserAuthorizationUrlResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Post("/v3/webizpay/users/{userId}/authorization-url")]
    Task<WebizpayUserAuthorizationUrlResponse> CreateUserAuthorizationUrlAsync(
        [Path] string userId,
        [Body] WebizpayUserAuthorizationUrlRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 企业商户查询企业员工开通授权状态。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507163"/>（【普通服务商】，官方页更新于 2025.05.09）。
    /// </summary>
    /// <param name="userId">【企业员工 ID】 商户企业员工ID，由企业系统提供的唯一标识员工的ID。要求6-32个字符内，只能是数字、大小写字母_-&amp;#124;\* 且在同一个商户号下唯一。（官方 path <c>user_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="spMchid">【服务商商户号】 是由微信支付系统生成并分配给每个服务商的唯一标识符，具体请参考 服务商模式开发必要参数说明。（官方 query <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【出资子商户号】 由服务商为子商户进件后获取，具体请参考 服务商模式开发必要参数说明。（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>企业商户查询企业员工开通授权状态，见 <see cref="WebizpayUserAuthorizationStateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/webizpay/users/{user_id}/authorization-state</c>；path <c>user_id</c>，query <c>sp_mchid</c> / <c>sub_mchid</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sp_mchid</c> 与 <c>sub_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Get("/v3/webizpay/users/{userId}/authorization-state")]
    Task<WebizpayUserAuthorizationStateResponse> QueryUserAuthorizationStateAsync(
        [Path] string userId,
        [Query("sp_mchid")] string spMchid,
        [Query("sub_mchid")] string subMchid,
        CancellationToken cancellationToken = default);
}
