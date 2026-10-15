// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Ecommerce;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台收付通 · 二级商户进件」域 SDK（<b>7 端点</b>：进件提交 + 两支查询 + 两支撤销 + 账户托管进件 + 代理签约核身）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（2026-10-15 逐页逐字段核验，九页全标【平台商户】）：普通二级商户进件
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012713017"/>、按申请单号查状态
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691469">4012691469</see> /
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616714">4015616714</see>、按业务申请编号查状态
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691376">4012691376</see> /
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616708">4015616708</see>、按申请单号撤销
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616720"/>、按业务申请编号撤销
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616721"/>、账户托管进件
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616688"/>、提交代理签约核身信息
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616722"/>。</para>
/// <para><b>路由尾斜杠是官方原文，不是笔误</b>：进件提交的路由写成 <c>/v3/ecommerce/applyments/</c>（带尾 <c>/</c>），
/// 与查询/撤销各支都没有尾斜杠不同 ⇒ 照页保留，去掉即与官方契约不一致（守卫 EC-A1）。</para>
/// <para><b>同一路由由两页成文，本域共两处</b>：<c>GET /v3/ecommerce/applyments/{applyment_id}</c> 与
/// <c>GET /v3/ecommerce/applyments/out-request-no/{out_request_no}</c> 各由「普通进件」与「账户托管进件」两页分别文档化，
/// 2026-10-15 四页应答表逐字段比对为<b>完全一致</b>（22 行）⇒ 共用一个查询应答类型、共用一个方法，
/// 不按进件模式拆成两套（守卫 EC-A2）。</para>
/// <para><b>本域加密面恰两支</b>：只有进件提交 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012713017"/>
/// 与账户托管进件 <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616688"/> 把 <c>Wechatpay-Serial</c>
/// 列为参数表行；官方在普通进件页明文标注需加密的字段共 <b>12</b> 个：
/// <c>id_card_info.id_card_name</c>、<c>id_card_info.id_card_number</c>、<c>id_doc_info.id_doc_name</c>、
/// <c>id_doc_info.id_doc_number</c>、<c>ubo_info_list[].ubo_id_doc_name</c>、<c>ubo_info_list[].ubo_id_doc_number</c>、
/// <c>ubo_info_list[].ubo_id_doc_address</c>、<c>account_info.account_name</c>、<c>account_info.account_number</c>、
/// <c>contact_info.contact_name</c>、<c>contact_info.contact_id_card_number</c>、<c>contact_info.mobile_phone</c>
/// ⇒ 查询/撤销/核身四支不得支配该头（守卫 EC-A3）。官方推荐<b>微信支付公钥</b>加密（须附公钥 ID），
/// 也可用平台证书公钥（须先调接口取证书并解密）；加密属宿主证书资产，SDK 不代为加解密，
/// 上述字段<b>绝不写入日志、遥测或异常消息</b>。</para>
/// <para><b>「提交 ≠ 完成」：本域的主线是状态机轮询</b>：官方原文「当服务商提交申请单后，需要定期调用此接口查询
/// 申请单的审核状态」⇒ 进件受理后须按 <see cref="EcommerceApplymentStates"/> 轮询到 <c>FINISH</c>（或 <c>REJECTED</c> /
/// <c>FROZEN</c> / <c>CANCELED</c>）才算收敛。八个状态各有伴随字段：<c>sign_url</c> 只在 <c>NEED_SIGN</c> 返回、
/// <c>sub_mchid</c> 只在 <c>NEED_SIGN</c> 或 <c>FINISH</c> 返回、<c>account_validation</c> 只在
/// <c>ACCOUNT_NEED_VERIFY</c> 返回、<c>audit_detail</c> 只在 <c>REJECTED</c> 返回 ⇒
/// <b>判状态须用枚举比对，不能用「某字段是否为 null」倒推</b>。</para>
/// <para><b>应答侧同样有密文</b>：<c>account_validation.account_name</c>（付款户名）与
/// <c>account_validation.account_no</c>（付款卡号）官方明确要求「需进行解密处理」（参见
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013059060"/>）⇒ SDK 原样透传密文字符串、不解密。</para>
/// <para><b>「被驳回时填相同业务申请编号即覆盖修改原申请单」</b>（官方原文，两支提交皆然）：
/// 这是本域唯一的覆盖写语义 ⇒ <c>out_request_no</c> 是幂等键而不是唯一性键，重提时<b>不要</b>换新编号
/// （换新编号会造出并行申请单）。<c>out_request_no</c> 官方限 <b>124</b> 字符（撤销页标注），
/// 与查询页的路径参数同一口径。</para>
/// <para><b>撤销有六状态白名单</b>：官方原文「仅当申请单状态为资料校验中、待账户验证、审核中、已驳回、待签约、
/// 已冻结时才允许撤销」⇒ <c>FINISH</c>（已完成，商户号已下发）与 <c>CANCELED</c>（已作废）不可撤销，
/// 对这两个状态调用必失败；两支撤销<b>官方零 body 参数</b>（定位键全在 path）⇒ 方法不带请求体（守卫 EC-A4）。</para>
/// <para><b>两支撤销同应答、与两支提交同型</b>：撤销应答为 <c>{applyment_id, out_request_no}</c> 两行，
/// 与普通进件/账户托管提交的应答逐字段相同（仅 <c>out_request_no</c> 的长度标注写法不同）⇒
/// 三支共用 <see cref="EcommerceApplymentSubmitResponse"/>；而代理核身支的应答<b>只有 <c>applyment_id</c> 一行</b>
/// （2026-10-15 比对为 DIFF）⇒ 独立类型 <see cref="EcommerceSignVerifyInfoResponse"/>，不做超集。</para>
/// <para><b>两支提交的字段必填性分叉，故分建两个请求类型</b>：账户托管页把 <c>business_license_info</c> 整族降为选填、
/// 把 <c>id_card_info.id_card_name</c>/<c>id_card_number</c> 与 <c>id_doc_info</c> 三支姓名/号码升为必填，
/// 并多出 <c>id_card_complete_statement</c> 与 <b>必填的 <c>proxy_sign_verify_mode</c></b>；
/// 普通进件页无这两字段 ⇒ 共用一个请求类型即造出「一类型两套必填语义」。</para>
/// <para><b>代理核身支只在托管模式下存在</b>：官方原文「在申请单中声明『走平台代理签约和代理核身入驻模式』时，
/// 可使用该接口传入代理签约和代理核身的信息」⇒ <c>pre_sign_time</c> 与 <c>proxy_verify_identity_info</c>
/// 的条件必填由进件时 <c>proxy_sign_verify_mode=true</c> 决定；本支应答不含任何状态字段，
/// 核身是否被采纳须回到查询支读 <c>applyment_state</c>。</para>
/// <para><b>能力边界</b>：官方明文「本接口不支持进件境外个人小微商户」（两支提交同文）⇒
/// 该组合由官方判错，SDK 不加本地闸门。<b>各页均未给出频率数字</b>，只列通用错误码表
/// （400 <c>PARAM_ERROR</c> / 401 <c>SIGN_ERROR</c> / 500 <c>SYSTEM_ERROR</c>）⇒ SDK 不做限流与自动重试。</para>
/// <para><b>媒体前置</b>：证照图片类字段（<c>*_copy</c>、<c>finance_license_pics</c>、<c>business_addition_pics</c> 等）
/// 官方要求填<b>媒体库图片 ID</b> ⇒ 须先经
/// <see cref="Mud.Wechat.Pay.Media.IWechatPayMerchantMediaService.UploadMediaAsync"/> 或
/// <see cref="Mud.Wechat.Pay.Ecommerce.IWechatPayEcommerceCancelMediaService.UploadCancelProofImageAsync"/>
/// 上传换取 ID，不能直传二进制。</para>
/// <para><b>与特约商户进件域不共用</b>：<see cref="IWechatPayApply4SubMerchantService"/> 走
/// <c>/v3/applyment4sub/*</c>、<see cref="IWechatPayMerchantAlterApplyService"/> 走 <c>/v3/merchant-test/alterapply/*</c>，
/// 三者的状态枚举、字段名与加密面各自成文 ⇒ 本域类型一律 <c>Ecommerce</c> 前缀，不复用他域 DTO。</para>
/// <para><b>客户端与凭据</b>：走支付专用命名客户端 <c>wechat-pay</c>，签名主体是当前作用域商户号；
/// 本域<b>零 <c>[Token]</c></b>。<c>[AllowAnyStatusCode]</c> 使 APIv3 的 4xx/5xx 错误体原样落到
/// <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Ecommerce", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayEcommerceApplymentService
{
    /// <summary>
    /// 「二级商户进件」：平台商户代其二级商户提交入驻申请，成为微信支付商户（<b>受理语义，须轮询查状态</b>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012713017"/>。
    /// </summary>
    /// <param name="request">进件申请，见 <see cref="EcommerceApplymentRequest"/>（12 个敏感字段须<b>密文</b>上送）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>申请单定位键（<c>applyment_id</c> + <c>out_request_no</c>），见 <see cref="EcommerceApplymentSubmitResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/applyments/</c>（<b>路由带尾斜杠</b>，照页）。</para>
    /// <para><b>必须带 <c>Wechatpay-Serial</c></b>：官方推荐微信支付公钥加密（头填公钥 ID），也可用平台证书序列号。</para>
    /// <para><b><c>out_request_no</c> 是幂等键</b>：被驳回后用<b>相同编号</b>重提即覆盖原申请单；换新编号会造出并行申请单。</para>
    /// <para><b>不支持进件境外个人小微商户</b>（官方明文）；证照字段填媒体库图片 ID，须先走上传通道。</para>
    /// </remarks>
    [Post("/v3/ecommerce/applyments/")]
    Task<EcommerceApplymentSubmitResponse> SubmitApplymentAsync(
        [Body] EcommerceApplymentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「按申请单号查询进件状态」：官方要求提交后<b>定期轮询</b>至终态。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012691469">4012691469</see> /
    /// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616714">4015616714</see>（两页同契约）。
    /// </summary>
    /// <param name="applymentId">微信支付申请单号（官方 path <c>applyment_id</c>，必填 integer）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>进件状态全量（22 行），见 <see cref="EcommerceApplymentQueryResponse"/>（四页查询<b>共用同一应答类型</b>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/applyments/{applyment_id}</c>；无加密头。</para>
    /// <para><b>状态与伴随字段一一对应</b>：<c>sign_url</c>（仅 <c>NEED_SIGN</c>）、<c>sub_mchid</c>
    /// （仅 <c>NEED_SIGN</c>/<c>FINISH</c>）、<c>account_validation</c>（仅 <c>ACCOUNT_NEED_VERIFY</c>）、
    /// <c>audit_detail</c>（仅 <c>REJECTED</c>）⇒ 判状态用 <see cref="EcommerceApplymentStates"/> 比对，勿以字段是否为 null 倒推。</para>
    /// <para><b><c>account_validation</c> 内的户名与卡号是密文</b>，须宿主用 API 证书私钥解密；SDK 原样透传。</para>
    /// </remarks>
    [Get("/v3/ecommerce/applyments/{applymentId}")]
    Task<EcommerceApplymentQueryResponse> QueryApplymentByApplymentIdAsync(
        [Path] long applymentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「按业务申请编号查询进件状态」：与 <see cref="QueryApplymentByApplymentIdAsync"/> 同一应答类型，只是定位键不同。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012691376">4012691376</see> /
    /// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616708">4015616708</see>（两页同契约）。
    /// </summary>
    /// <param name="outRequestNo">业务申请编号（官方 path <c>out_request_no</c>，必填 string(124)）：服务商自定义的唯一编号。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与按申请单号查询<b>共用同一应答类型</b>，见 <see cref="EcommerceApplymentQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/applyments/out-request-no/{out_request_no}</c>。</para>
    /// <para><b>本支适合「宿主只保留自己的编号」的场景</b>：官方四页应答逐字段相同 ⇒ 取回的状态视图与申请单号支完全一致。</para>
    /// </remarks>
    [Get("/v3/ecommerce/applyments/out-request-no/{outRequestNo}")]
    Task<EcommerceApplymentQueryResponse> QueryApplymentByOutRequestNoAsync(
        [Path] string outRequestNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「按申请单号撤销进件申请」：仅在六个非终态下允许，<b>官方零 body 参数</b>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015616720"/>。
    /// </summary>
    /// <param name="applymentId">微信支付申请单号（官方 path <c>applyment_id</c>，必填 integer）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>撤销后的申请单定位键，见 <see cref="EcommerceApplymentSubmitResponse"/>（与提交、另支撤销<b>三支共用</b>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/applyments/{applyment_id}/revoke</c>；
    /// 定位键只在 path，<b>请求体为空</b>（官方参数表 Path 一行、Body 无）。</para>
    /// <para><b>六状态白名单</b>：官方原文「仅当申请单状态为资料校验中、待账户验证、审核中、已驳回、待签约、
    /// 已冻结时才允许撤销」⇒ <c>FINISH</c>/<c>CANCELED</c> 不可撤销。</para>
    /// <para><b>撤销后状态是 <c>CANCELED</c></b>：结果仍须回 <see cref="QueryApplymentByApplymentIdAsync"/> 确认，
    /// 应答本身不回状态。</para>
    /// </remarks>
    [Post("/v3/ecommerce/applyments/{applymentId}/revoke")]
    Task<EcommerceApplymentSubmitResponse> RevokeApplymentByApplymentIdAsync(
        [Path] long applymentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「按业务申请编号撤销进件申请」：与 <see cref="RevokeApplymentByApplymentIdAsync"/> 同契约，只是定位键不同。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015616721"/>。
    /// </summary>
    /// <param name="outRequestNo">业务申请编号（官方 path <c>out_request_no</c>，必填 string(124)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与另支撤销<b>共用同一应答类型</b>，见 <see cref="EcommerceApplymentSubmitResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/applyments/out-request-no/{out_request_no}/revoke</c>；
    /// 同样<b>零 body 参数</b>、同样六状态白名单。</para>
    /// <para><b>两页应答逐字段相同</b>（<c>applyment_id</c> + <c>out_request_no</c>）⇒ 两支共用一个类型。</para>
    /// </remarks>
    [Post("/v3/ecommerce/applyments/out-request-no/{outRequestNo}/revoke")]
    Task<EcommerceApplymentSubmitResponse> RevokeApplymentByOutRequestNoAsync(
        [Path] string outRequestNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「账户托管进件」：账号托管类平台商户的进件入口，多出代理签约/代理核身模式声明（<b>受理语义</b>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015616688"/>。
    /// </summary>
    /// <param name="request">托管进件申请，见 <see cref="EcommerceAccountHostingApplymentRequest"/>（敏感字段须<b>密文</b>上送）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与普通进件<b>共用同一应答类型</b>，见 <see cref="EcommerceApplymentSubmitResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/applyments/submit-account-hosting-applyment</c>。</para>
    /// <para><b>必须带 <c>Wechatpay-Serial</c></b>：本方法与 <see cref="SubmitApplymentAsync"/> 是本域两支加密端点。</para>
    /// <para><b>与普通进件的必填性分叉</b>：本页营业执照族整体降为选填、身份证/证件姓名与号码升为必填，
    /// 且<b>必填 <c>proxy_sign_verify_mode</c></b> 并多出 <c>id_card_complete_statement</c> ⇒ 故分建请求类型。</para>
    /// <para><b>声明代理签约/代理核身后须补提交核身信息</b>（<see cref="SubmitSignVerifyInfoAsync"/>），
    /// 时序是「进件声明模式 → 提交核身 → 轮询状态」。</para>
    /// </remarks>
    [Post("/v3/ecommerce/applyments/submit-account-hosting-applyment")]
    Task<EcommerceApplymentSubmitResponse> SubmitAccountHostingApplymentAsync(
        [Body] EcommerceAccountHostingApplymentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「提交代理签约核身信息」：账号托管进件里声明了「平台代理签约 + 代理核身入驻模式」后回填核身结果。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015616722"/>。
    /// </summary>
    /// <param name="request">核身信息，见 <see cref="EcommerceSignVerifyInfoRequest"/>（<c>out_request_no</c> 与 <c>applyment_id</c> 二选一定位）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>申请单号，见 <see cref="EcommerceSignVerifyInfoResponse"/>（<b>官方只回一行</b>，与三支提交/撤销的应答不同）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/applyments/submit-sign-verify-info</c>；无加密头
    /// （本支传的是核身<b>方式与结果布尔值</b>，不传证件明文）。</para>
    /// <para><b>只在托管模式下存在</b>：官方原文「在申请单中声明『走平台代理签约和代理核身入驻模式』时，可使用该接口」
    /// ⇒ <c>pre_sign_time</c> 与 <c>proxy_verify_identity_info</c> 的条件必填由 <c>proxy_sign_verify_mode=true</c> 决定。</para>
    /// <para><b>应答没有任何状态字段</b>：核身是否被采纳须回 <see cref="QueryApplymentByApplymentIdAsync"/> 读
    /// <c>applyment_state</c>；<c>verify_identity_result=false</c> 表示核身不通过，官方未定义额外重试入参。</para>
    /// </remarks>
    [Post("/v3/ecommerce/applyments/submit-sign-verify-info")]
    Task<EcommerceSignVerifyInfoResponse> SubmitSignVerifyInfoAsync(
        [Body] EcommerceSignVerifyInfoRequest request,
        CancellationToken cancellationToken = default);
}
