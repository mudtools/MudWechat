// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.PlatsolutionInsuranceReservation;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台商户保险理赔商家转账 · 预约商家转账（保险场景）」域 SDK（4 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径。本域每支路由官方<b>只印一页</b> ⇒ <b>不存在</b>「同路由多页字段表比对」问题，一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>「平台商户」身份口径</b>：本域 13 页「支持商户」栏<b>恒为【平台商户】</b>（无【普通服务商】分身页），
/// 请求体与查询 query 统一以 <c>sub_mchid</c>（二级商户号，出资方）定位 ⇒ 平台商户身份由调用侧签名承载，本域<b>零</b> <c>sp_mchid</c> 请求参数。</para>
/// <para><b>四支建三型</b>：受理支应答 6 行、含<b>必填</b> <c>package_info</c>（拉起微信支付客户端用户确认页的跳转信息）； 按 <c>out_reservation_no</c> 查询、按 <c>reservation_id</c> 查询与关闭三支的官方应答表<b>逐字段同形且必填口径一致</b>（18 行） ⇒ 共用 <see cref="InsuranceReservationQueryResponse"/>；<b>关闭支官方印有应答表</b> ⇒ 既不得返回裸 <c>Task</c>，也不得凭空造请求 DTO。</para>
/// <para><b>幂等与时效（官方原文）</b>：预创建的预约记录<b>24 小时内有效</b>，用户未及时确认由系统自动关闭；接口支持幂等， 幂等标识为 <c>out_reservation_no</c>、幂等有效性 <b>5 年</b> ⇒ 商户单号必须稳定可重放，不得每次随机生成。</para>
/// <para><b>敏感字段</b>：<c>user_real_name</c> 官方要求<b>加密后传入</b>（<c>string(1024)</c>，选填， 但「预计转账金额 ≥ 2,000 元时必须填写」）⇒ 请求头须带 <c>Wechatpay-Serial</c>；本 SDK <b>不做加密也不代填</b>，密文由调用侧产出后原样传入。</para>
/// <para><b>查询两支与关闭支的 <c>sub_mchid</c> 是官方必填 query</b>（受理支走 Body 必填）⇒ 同一字段在受理侧与后三支落位不同， 请求 DTO 与查询签名不得互套；<b>本域 4 支官方页均未印</b>端点级 QPS ⇒ 不得凭第三方 SDK 补数。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PlatsolutionInsuranceReservation", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayPlatsolutionInsuranceReservationService
{
    /// <summary>
    /// 受理单次预约商家转账。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504216"/>（【平台商户】，官方页更新于 2025.01.21）。
    /// </summary>
    /// <param name="request">受理单次预约商家转账请求体，字段见 <see cref="InsuranceReservationApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>受理单次预约商家转账，见 <see cref="InsuranceReservationApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/platsolution/insurance/mch-transfer/reservation/apply</c>；无 path / query 参数。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="InsuranceReservationApplyResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Post("/v3/platsolution/insurance/mch-transfer/reservation/apply")]
    Task<InsuranceReservationApplyResponse> ApplyInsuranceReservationAsync(
        [Body] InsuranceReservationApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商户预约单号查询预约商家转账记录。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504218"/>（【平台商户】，官方页更新于 2025.01.21）。
    /// </summary>
    /// <param name="outReservationNo">【商户预约单号】 商户系统内部的预约商家转账单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方 path <c>out_reservation_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【二级商户号】 微信支付分配的商户号，出资商户号（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商户预约单号查询预约商家转账记录，见 <see cref="InsuranceReservationQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/platsolution/insurance/mch-transfer/reservation/out-reservation-no/{out_reservation_no}</c>；path <c>out_reservation_no</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sub_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Get("/v3/platsolution/insurance/mch-transfer/reservation/out-reservation-no/{outReservationNo}")]
    Task<InsuranceReservationQueryResponse> QueryInsuranceReservationByOutReservationNoAsync(
        [Path] string outReservationNo,
        [Query("sub_mchid")] string subMchid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 微信支付预约单号查询预约商家转账记录。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504217"/>（【平台商户】，官方页更新于 2025.01.21）。
    /// </summary>
    /// <param name="reservationId">【微信转账预约单号】 微信商家转账系统返回的预约记录唯一标识（官方 path <c>reservation_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="subMchid">【二级商户号】 微信支付分配的商户号，出资商户号（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>微信支付预约单号查询预约商家转账记录，见 <see cref="InsuranceReservationQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/platsolution/insurance/mch-transfer/reservation/reservation-id/{reservation_id}</c>；path <c>reservation_id</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sub_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Get("/v3/platsolution/insurance/mch-transfer/reservation/reservation-id/{reservationId}")]
    Task<InsuranceReservationQueryResponse> QueryInsuranceReservationByReservationIdAsync(
        [Path] string reservationId,
        [Query("sub_mchid")] string subMchid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 关闭预约商家转账记录。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504219"/>（【平台商户】，官方页更新于 2025.01.21）。
    /// </summary>
    /// <param name="outReservationNo">【商户预约单号】 商户系统内部的预约商家转账单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方 path <c>out_reservation_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【二级商户号】 微信支付分配的商户号，出资商户号（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>关闭预约商家转账记录，见 <see cref="InsuranceReservationQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/platsolution/insurance/mch-transfer/reservation/out-reservation-no/{out_reservation_no}/close</c>；path <c>out_reservation_no</c>，query <c>sub_mchid</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sub_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Post("/v3/platsolution/insurance/mch-transfer/reservation/out-reservation-no/{outReservationNo}/close")]
    Task<InsuranceReservationQueryResponse> CloseInsuranceReservationAsync(
        [Path] string outReservationNo,
        [Query("sub_mchid")] string subMchid,
        CancellationToken cancellationToken = default);
}
