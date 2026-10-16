// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.WebizpayEmployee;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「微企付企业支付 · 额度卡与员工授权」域 SDK（6 端点）。
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
/// <para><b>六支建五型</b>：按 <c>card_no</c> 与按 <c>out_card_no</c> 两支查询的官方应答表<b>逐字段同形且必填口径一致</b>（20 行） ⇒ 共用 <see cref="WebizpayQuotaCardQueryResponse"/>；下发 / 作废 / 支付链接 / 解除授权各建一型。</para>
/// <para><b>定位键分工（本域最易踩）</b>：path 一律 <c>employee_id</c>（官方【微信授权关系 ID】，形如 <c>WeBizPay_a968402a-…</c>，由注册回调或员工查询面产出，<b>不是</b>商户企业员工 ID <c>user_id</c>）； 卡级操作只认<b>微信卡号 <c>card_no</c></b> ⇒ 作废与支付链接两支<b>不接受商户卡号</b>， 只持有 <c>out_card_no</c> 的调用方须先走 <c>out-quota-cards</c> 查询支换取 <c>card_no</c>。</para>
/// <para><b>同名 <c>sp_mchid</c> / <c>sub_mchid</c> 按动词落位不同</b>：GET 三支（两路查询 + 支付链接）落<b>必填 query</b>、零请求体， POST 三支落 <b>Body</b> ⇒ 请求 DTO 与查询签名不得互套；支付链接支另有必填 query <c>application_type</c> （<c>H5</c> / <c>MINIPROGRAM</c>）。</para>
/// <para><b>解除授权支请求用 <c>employee_id</c>、应答回显 <c>user_id</c></b>（商户企业员工 ID），且官方口径为<b>同步</b>： 应答 <c>authorization_state</c> 返回 <c>REVOKED</c> 即认定该员工在本服务商-出资子商户下授权已解除、 其在本商户下的企业支付额度卡<b>全部作废</b> ⇒ 不得把该支当异步受理。</para>
/// <para><b>查询应答 20 行的 6 个选填字段</b>（<c>reason</c> / <c>card_usage_time</c> / <c>transaction_id</c> / <c>card_disabled_time</c> / <c>card_remark</c> / <c>avail_mch_range_id</c>）⇒ 判「卡可用」须同时看 <c>card_state</c>（仅 <c>ACTIVE</c> / <c>DISABLED</c> 两值）与 <c>effective_end_time</c>，<b>不得</b>只看 <c>remain_amount</c> &gt; 0。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "WebizpayEmployee", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayWebizpayEmployeeService
{
    /// <summary>
    /// 企业商户为员工下发企业支付额度卡。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507148"/>（【普通服务商】，官方页更新于 2025.05.09）。
    /// </summary>
    /// <param name="employeeId">【微信授权关系 ID】 微信授权关系ID，微信支付平台生成的唯一标识授权关系的ID，用户注册时会回调商户获取微信授权关系ID或者商户通过企业下唯一的员工ID查询企业支付员工接口获得。 示例：WeBizPay_a968402a-73bb-43e2-9e1a-8371b0ca3d7c（官方 path <c>employee_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="request">企业商户为员工下发企业支付额度卡请求体，字段见 <see cref="WebizpayQuotaCardApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>企业商户为员工下发企业支付额度卡，见 <see cref="WebizpayQuotaCardApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/webizpay/employees/{employee_id}/quota-cards</c>；path <c>employee_id</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="WebizpayQuotaCardApplyResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Post("/v3/webizpay/employees/{employeeId}/quota-cards")]
    Task<WebizpayQuotaCardApplyResponse> ApplyQuotaCardAsync(
        [Path] string employeeId,
        [Body] WebizpayQuotaCardApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 企业商户查询企业支付额度卡。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507119"/>（【普通服务商】，官方页更新于 2025.05.09）。
    /// </summary>
    /// <param name="employeeId">【微信授权关系 ID】 微信授权关系ID，微信支付平台生成的唯一标识授权关系的ID，用户注册时会回调商户获取微信授权关系ID或者商户通过企业下唯一的员工ID查询企业支付员工接口获得。 示例：WeBizPay_a968402a-73bb-43e2-9e1a-8371b0ca3d7c（官方 path <c>employee_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="cardNo">【企业支付额度卡卡号】 企业支付额度卡卡号，微信支付平台生成的唯一标识额度卡的编号。通过创建卡和查询卡返回。 示例：1068019671702503425（官方 path <c>card_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="spMchid">【服务商商户号】 是由微信支付系统生成并分配给每个服务商的唯一标识符，具体请参考 服务商模式开发必要参数说明。（官方 query <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【出资子商户号】 由服务商为子商户进件后获取，具体请参考 服务商模式开发必要参数说明。（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>企业商户查询企业支付额度卡，见 <see cref="WebizpayQuotaCardQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/webizpay/employees/{employee_id}/quota-cards/{card_no}</c>；path <c>employee_id</c> / <c>card_no</c>，query <c>sp_mchid</c> / <c>sub_mchid</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sp_mchid</c> 与 <c>sub_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Get("/v3/webizpay/employees/{employeeId}/quota-cards/{cardNo}")]
    Task<WebizpayQuotaCardQueryResponse> QueryQuotaCardByCardNoAsync(
        [Path] string employeeId,
        [Path] string cardNo,
        [Query("sp_mchid")] string spMchid,
        [Query("sub_mchid")] string subMchid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 企业商户使用商户卡号查询企业支付额度卡。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015001625"/>（【普通服务商】，官方页更新于 2025.05.29）。
    /// </summary>
    /// <param name="employeeId">【微信授权关系 ID】 微信授权关系ID，微信支付平台生成的唯一标识授权关系的ID，用户注册时会回调商户获取微信授权关系ID或者商户通过企业下唯一的员工ID查询企业支付员工接口获得。 示例：WeBizPay_a968402a-73bb-43e2-9e1a-8371b0ca3d7c（官方 path <c>employee_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="outCardNo">【企业支付额度卡商户卡号】 由商户在创建额度卡时传入,由商户系统生成的唯一标识额度卡的编号（官方 path <c>out_card_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="spMchid">【服务商商户号】 是由微信支付系统生成并分配给每个服务商的唯一标识符，具体请参考 服务商模式开发必要参数说明。（官方 query <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【出资子商户号】 由服务商为子商户进件后获取，具体请参考 服务商模式开发必要参数说明。（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>企业商户使用商户卡号查询企业支付额度卡，见 <see cref="WebizpayQuotaCardQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/webizpay/employees/{employee_id}/out-quota-cards/{out_card_no}</c>；path <c>employee_id</c> / <c>out_card_no</c>，query <c>sp_mchid</c> / <c>sub_mchid</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sp_mchid</c> 与 <c>sub_mchid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Get("/v3/webizpay/employees/{employeeId}/out-quota-cards/{outCardNo}")]
    Task<WebizpayQuotaCardQueryResponse> QueryQuotaCardByOutCardNoAsync(
        [Path] string employeeId,
        [Path] string outCardNo,
        [Query("sp_mchid")] string spMchid,
        [Query("sub_mchid")] string subMchid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 企业商户作废员工企业支付额度卡。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507127"/>（【普通服务商】，官方页更新于 2025.05.09）。
    /// </summary>
    /// <param name="employeeId">【微信授权关系 ID】 微信授权关系ID，微信支付平台生成的唯一标识授权关系的ID，用户注册时会回调商户获取微信授权关系ID或者商户通过企业下唯一的员工ID查询企业支付员工接口获得。 示例：WeBizPay_a968402a-73bb-43e2-9e1a-8371b0ca3d7c（官方 path <c>employee_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="cardNo">【企业支付额度卡卡号】 企业支付额度卡卡号，由请求微信支付发放额度卡时返回 示例：1068019671702503425（官方 path <c>card_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="request">企业商户作废员工企业支付额度卡请求体，字段见 <see cref="WebizpayQuotaCardCancelRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>企业商户作废员工企业支付额度卡，见 <see cref="WebizpayQuotaCardCancelResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/webizpay/employees/{employee_id}/quota-cards/{card_no}/cancel</c>；path <c>employee_id</c> / <c>card_no</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="WebizpayQuotaCardCancelResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Post("/v3/webizpay/employees/{employeeId}/quota-cards/{cardNo}/cancel")]
    Task<WebizpayQuotaCardCancelResponse> CancelQuotaCardAsync(
        [Path] string employeeId,
        [Path] string cardNo,
        [Body] WebizpayQuotaCardCancelRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 企业商户获取指定额度卡支付链接。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507160"/>（【普通服务商】，官方页更新于 2025.05.29）。
    /// </summary>
    /// <param name="employeeId">【微信授权关系 ID】 微信授权关系ID，微信支付平台生成的唯一标识授权关系的ID，用户注册时会回调商户获取微信授权关系ID或者商户通过企业下唯一的员工ID查询企业支付员工接口获得。 示例：WeBizPay_a968402a-73bb-43e2-9e1a-8371b0ca3d7c（官方 path <c>employee_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="cardNo">【企业支付额度卡卡号】 企业支付额度卡卡号，微信支付平台生成的唯一标识额度卡的编号。通过创建卡和查询卡返回。 示例：1068019671702503425（官方 path <c>card_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="spMchid">【服务商商户号】 是由微信支付系统生成并分配给每个服务商的唯一标识符，具体请参考 服务商模式开发必要参数说明。（官方 query <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="subMchid">【出资子商户号】 由服务商为子商户进件后获取，具体请参考 服务商模式开发必要参数说明。（官方 query <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="applicationType">【拉起付款码页面的企业应用类型】 用户从企业应用跳转至微信付款码，需传入拉起付款码页面的企业应用类型。可选取值：`H5` 表示从商户的应用是外部H5，跳转微信拉起企业支付相关能力 / `MINIPROGRAM` 表示从商户的应用是微信小程序，跳转微信拉起企业支付相关能力（官方 query <c>application_type</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>企业商户获取指定额度卡支付链接，见 <see cref="WebizpayQuotaCardPaymentUrlResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/webizpay/employees/{employee_id}/quota-cards/{card_no}/payment-url</c>；path <c>employee_id</c> / <c>card_no</c>，query <c>sp_mchid</c> / <c>sub_mchid</c> / <c>application_type</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>sp_mchid</c> 与 <c>sub_mchid</c> 与 <c>application_type</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Get("/v3/webizpay/employees/{employeeId}/quota-cards/{cardNo}/payment-url")]
    Task<WebizpayQuotaCardPaymentUrlResponse> GetQuotaCardPaymentUrlAsync(
        [Path] string employeeId,
        [Path] string cardNo,
        [Query("sp_mchid")] string spMchid,
        [Query("sub_mchid")] string subMchid,
        [Query("application_type")] string applicationType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 企业商户发起解除员工企业支付授权。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014507156"/>（【普通服务商】，官方页更新于 2025.05.16）。
    /// </summary>
    /// <param name="employeeId">【微信授权关系 ID】 微信授权关系ID，微信支付平台生成的唯一标识授权关系的ID，用户注册时会回调商户获取微信授权关系ID或者商户通过企业下唯一的员工ID查询企业支付员工接口获得。 示例：WeBizPay_a968402a-73bb-43e2-9e1a-8371b0ca3d7c（官方 path <c>employee_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="request">企业商户发起解除员工企业支付授权请求体，字段见 <see cref="WebizpayEmployeeRevokeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>企业商户发起解除员工企业支付授权，见 <see cref="WebizpayEmployeeRevokeResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/webizpay/employees/{employee_id}/revoke</c>；path <c>employee_id</c>。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="WebizpayEmployeeRevokeResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。</para>
    /// </remarks>
    [Post("/v3/webizpay/employees/{employeeId}/revoke")]
    Task<WebizpayEmployeeRevokeResponse> RevokeEmployeeAsync(
        [Path] string employeeId,
        [Body] WebizpayEmployeeRevokeRequest request,
        CancellationToken cancellationToken = default);
}
