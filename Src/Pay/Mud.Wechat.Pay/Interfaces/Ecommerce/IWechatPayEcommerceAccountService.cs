// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Ecommerce;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台收付通 · 账户注销 / 个人收款方 / 主体子商户」域 SDK（<b>10 端点</b>：
/// 注销并提现 5 + 子商户注销申请 2 + 个人收款方签约查询 2 + 主体名下子商户列表 1）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（2026-10-15 逐页逐字段核验）：注销并提现申请
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013892756"/>、按微信单号查
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013892765"/>、按商户单号查
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013892759"/>、平台代商户确认
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013892772"/>、注销资格前置校验
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4016420099"/>、注销申请
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476217"/>、注销申请查询
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476223"/>、按 <c>openid</c> 查个人收款方授权
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012739528"/>、按业务申请单号查签约详情
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015851492"/>、主体名下子商户
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4033742105"/>。</para>
/// <para><b>两支「注销」是两套契约，不是同一件事的两个入口</b>：
/// <see cref="ApplyCancelWithdrawAsync"/>（<c>/account/apply-cancel-withdraw</c>，【平台商户】）
/// 是「注销<b>并可提走余额</b>」，面向<b>已被微信支付管控且无法解脱</b>的二级商户（如营业执照注销吊销），
/// 审核通过后把账户<b>全部余额</b>打到商户银行卡、多账户会多笔入账；
/// <see cref="ApplyCancelAsync"/>（<c>/account/cancel-applications</c>，【平台商户】）是普通的
/// 「电商子商户注销申请」，走 <c>application_info</c> 资料 + 媒体 ID 路线。
/// 两支的单据定位键、状态枚举、应答字段表都不同 ⇒ 各自一型，不得互相套用。</para>
/// <para><b>「先校验、再申请、后确认」是本域的强制时序</b>：<see cref="ValidateCancelAsync"/> 回
/// <c>validate_result</c> 两值 + <c>block_reasons[]</c>（官方原文要求<b>发起清退前再次调用确认</b>，
/// 因为返回的余额是实时数、之后仍可能有新交易入账）；申请支的受理应答<b>没有状态</b>；
/// 单据进入「待商户确认」后还要 <see cref="ConfirmCancelWithdrawAsync"/> 代商户确认，
/// 而官方明文「审核通过后<b>超过 30 天</b>未经商户确认的单据将做撤销处理」⇒ 三个动作缺一不可、
/// 且确认动作有到期日。</para>
/// <para><b>加密面覆盖两支</b>（逐页核对 Header 参数表，本域其余八页无 <c>Wechatpay-Serial</c>）：
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013892756"/> 要求 <c>account_name</c>/
/// <c>account_number</c>/<c>identification_name</c>/<c>identification_no</c> 传密文，
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4033742105"/> 要求 <c>cert_number</c> 传密文
/// （且它是 <b>Query</b> 参数，不是包体字段）⇒ 这两支调用必须带 <c>Wechatpay-Serial</c>，
/// DTO 侧只承载密文 string（守卫 EC-C1 锁定「本域加密面恰两支」这一核对结论）。</para>
/// <para><b>本域含全线唯一的「Query 里传敏感信息」</b>：<c>cert_number</c> 是主体证件号码，
/// 官方要求加密后放入 Query ⇒ 密文仍会出现在 URL 上，宿主不得把请求 URI 原文写进日志或异常消息
/// （支付线的 <c>WechatPayException.RequestUri</c> 已剥离 query，自定义埋点须走同一口径）。</para>
/// <para><b>两支个人收款方查询的定位键不同、应答形态也不同</b>：按 <c>openid</c> 支回三字段（须带
/// <b>必填</b> query <c>permission_type</c>，官方只给 <c>INDIVIDUAL_PAYEE</c> 一个取值）；
/// 按 <c>out_contract_code</c> 支回八字段（多 <c>signed_time</c>、<c>individual_openid</c> 与
/// <c>real_name_auth_result</c> 子树）。<b>两支的 <c>appid</c> 都是必填 query</b> ⇒ 不得用同一 DTO
/// 「超集」糊过去（共用/分建裁决见两个响应类型的类级注释）。</para>
/// <para><b>应答里的 <c>access_token</c> 是实名授权访问令牌</b>（<see cref="EcommerceRealNameAuthResult"/>）：
/// 它<b>不可刷新</b>、过期后须重新走实名授权流程 ⇒ 属凭据，<b>禁止</b>进日志、遥测或异常消息，
/// 也不得回传给前端。</para>
/// <para><b>频率上限</b>：本域十页页内<b>均无端点级频率数字</b>（逐页复核）；官方只给出通用 429
/// <c>FREQUENCY_LIMITED</c> 兜底 ⇒ 宿主需自备节流，不能依赖 SDK 限流。</para>
/// <para><b>客户端与凭据</b>：走支付专用命名客户端 <c>wechat-pay</c>，签名主体是当前作用域的平台商户号
/// （<b>例外两支</b>：<see cref="ValidateCancelAsync"/>（<c>4016420099</c>）与 <see cref="QueryCancelApplicationAsync"/>
/// （<c>4012476223</c>）官方标【普通服务商】⇒ 本域十页里八页平台商户、两页普通服务商，
/// 这两支须以服务商自有商户配置进入调用作用域）；
/// 本域<b>零 <c>[Token]</c></b>。<c>media_id</c> 由 <see cref="Mud.Wechat.Pay.Ecommerce.IWechatPayEcommerceCancelMediaService"/>
/// 手工通道取得（multipart 无法声明式表达），不在本接口内。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{"code","message"}</c>，
/// 加上后错误体原样落到 <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/> 与
/// <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Message"/>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Ecommerce", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayEcommerceAccountService
{
    /// <summary>
    /// 「商户号注销并提现申请」：代被管控的二级商户发起注销、并把账户余额打款到商户银行卡。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013892756"/>。
    /// </summary>
    /// <param name="request">注销并提现请求，见 <see cref="EcommerceCancelWithdrawApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>只回 <c>applyment_id</c> / <c>out_request_no</c>，<b>没有状态</b>，见 <see cref="EcommerceCancelWithdrawApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/account/apply-cancel-withdraw</c>；页内无频率数字。</para>
    /// <para><b>必须带 <c>Wechatpay-Serial</c></b>：收款户名/账号与经办人姓名/证件号四个字段是密文，
    /// 明文直传既被拒又违规。</para>
    /// <para><b>适用面很窄</b>：官方限定「仅针对被微信支付管控、且无法解脱该管控的商户」⇒
    /// 正常退出应走 <see cref="ApplyCancelAsync"/>，不要把本支当通用提现出口。</para>
    /// <para><b>受理 ≠ 生效</b>：必须落库单号后经查询支读 <c>cancel_state</c>；审核通过后<b>超过 30 天</b>
    /// 未经商户确认会被撤销。资金清退是「账户全部遗留资金」⇒ 调用前应先用 <see cref="ValidateCancelAsync"/> 复核。</para>
    /// </remarks>
    [Post("/v3/ecommerce/account/apply-cancel-withdraw")]
    Task<EcommerceCancelWithdrawApplyResponse> ApplyCancelWithdrawAsync(
        [Body] EcommerceCancelWithdrawApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「按微信支付注销单号查询注销提现申请」：读审批与出款进度。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013892765"/>。
    /// </summary>
    /// <param name="applymentId">微信支付注销单号（官方 path <c>applyment_id</c>，必填 string(32)）：申请受理时返回。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>注销提现单详情，见 <see cref="EcommerceCancelWithdrawQueryResponse"/>（与按商户单号查<b>共用同一应答类型</b>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/account/apply-cancel-withdraw/applyment-id/{applyment_id}</c>；
    /// 无 query、无包体。</para>
    /// <para><b>两支查询应答 18 行逐字段相同</b> ⇒ 共用一型；差异只在定位键（微信单号对商户单号）。</para>
    /// <para><b><c>WAITING_MERCHANT_CONFIRM</c> 态下本支是唯一能看见待确认事实的地方</b>（应答的
    /// <c>confirm_cancel</c> 子树）⇒ 确认后仍以查询支的终态为准，<see cref="ConfirmCancelWithdrawAsync"/> 的成功应答不代表注销完成。</para>
    /// </remarks>
    [Get("/v3/ecommerce/account/apply-cancel-withdraw/applyment-id/{applymentId}")]
    Task<EcommerceCancelWithdrawQueryResponse> QueryCancelWithdrawByApplymentIdAsync(
        [Path] string applymentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「按商户注销申请单号查询注销提现申请」：与上一支<b>同一应答类型</b>，定位键换成商户自有单号。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013892759"/>。
    /// </summary>
    /// <param name="outRequestNo">商户注销申请单号（官方 path <c>out_request_no</c>，必填 string(32)，仅字母数字）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与按 <c>applyment_id</c> 查<b>同一应答类型</b>，见 <see cref="EcommerceCancelWithdrawQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/account/apply-cancel-withdraw/out-request-no/{out_request_no}</c>。</para>
    /// <para><b>本支是「申请应答丢失」时的唯一自救入口</b>：只要宿主存了自己的 <c>out_request_no</c>，
    /// 就不必依赖 <c>applyment_id</c> 落库成功。</para>
    /// </remarks>
    [Get("/v3/ecommerce/account/apply-cancel-withdraw/out-request-no/{outRequestNo}")]
    Task<EcommerceCancelWithdrawQueryResponse> QueryCancelWithdrawByOutRequestNoAsync(
        [Path] string outRequestNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「平台代商户确认注销」：单据处于「待商户确认注销」时代商户执行确认动作。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013892772"/>。
    /// </summary>
    /// <param name="request">确认请求，见 <see cref="EcommerceCancelWithdrawConfirmRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>确认结果，见 <see cref="EcommerceCancelWithdrawConfirmResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/account/apply-cancel-withdraw/confirm</c>；页内无频率数字。</para>
    /// <para><b><c>applyment_id</c> 与 <c>out_request_no</c> 官方标「二选一」</b>（两者都列选填）⇒
    /// SDK 不做条件必填校验，一个都不传由官方判 <c>PARAM_ERROR</c>。</para>
    /// <para><b><c>operator_identification</c> 是留证字段，不是鉴权字段</b>：官方原文「仅做留存，
    /// 未来出现商户纠纷时依据此标识调证」⇒ 必须传真实操作员标识（≤100，允许大小写字母、数字与汉字），
    /// 填占位值等于把纠纷举证责任丢掉。<c>confirm_time</c> 与 <c>cancel_contract_version</c> 同为必填，
    /// 版本号须与商户实际勾选的《微信支付注销申请书》一致。</para>
    /// </remarks>
    [Post("/v3/ecommerce/account/apply-cancel-withdraw/confirm")]
    Task<EcommerceCancelWithdrawConfirmResponse> ConfirmCancelWithdrawAsync(
        [Body] EcommerceCancelWithdrawConfirmRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「注销资格前置校验」：发起注销提现前判断商户是否满足注销条件，并带回各账户实时余额与阻塞原因。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016420099"/>。
    /// </summary>
    /// <param name="subMchId">二级商户号（官方 path <c>sub_mchid</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商户号状态 + 资格结论 + 余额清单 + 不可注销原因，见 <see cref="EcommerceValidateCancelResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/account/apply-cancel-withdraw/validate-cancel/{sub_mchid}</c>；
    /// 「支持商户」【普通服务商】（本域两支非【平台商户】页之一，另一支见 <see cref="QueryCancelApplicationAsync"/>；
/// 两页路由仍是 <c>/v3/ecommerce/</c> 前缀）。</para>
    /// <para><b>余额是实时数，不是锁定数</b>：官方明文「查询后商户可能仍有新的交易入账导致余额变动，
    /// 发起资金清退申请前请再次调用本接口进行确认」⇒ 本支只用于<b>引导与决策</b>，
    /// 提交 <see cref="ApplyCancelWithdrawAsync"/> 前须现查现用，不能拿历史值做资金承诺。</para>
    /// <para><b><c>out_account_type</c> 是另一套四值枚举</b>（<c>BASIC_ACCOUNT</c>/<c>OPERATE_ACCOUNT</c>/
    /// <c>MARGIN_ACCOUNT</c>/<c>TRADE_FEE_ACCOUNT</c>，见 <see cref="EcommerceOutAccountTypes"/>）⇒
    /// 与提现支的 <c>BASIC</c>/<c>FEES</c>/<c>OPERATION</c> 三值<b>不通用</b>，跨字段照抄必错。</para>
    /// <para><b><c>block_reasons</c> 可能多条</b>（同个商户多个不可注销原因）⇒ 只取第一条会误导商户。</para>
    /// </remarks>
    [Get("/v3/ecommerce/account/apply-cancel-withdraw/validate-cancel/{subMchId}")]
    Task<EcommerceValidateCancelResponse> ValidateCancelAsync(
        [Path] string subMchId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「电商子商户注销申请」：提交普通注销申请（资料以 <c>application_media_id</c> 承载）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476217"/>。
    /// </summary>
    /// <param name="request">注销申请请求，见 <see cref="EcommerceCancelApplicationRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>注销申请单，见 <see cref="EcommerceCancelApplicationResponse"/>（与查询支<b>共用同一应答类型</b>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/ecommerce/account/cancel-applications</c>；页内无频率数字。</para>
    /// <para><b><c>application_media_id</c> 必须先经本域媒体上传通道换取</b>
    /// （<see cref="Mud.Wechat.Pay.Ecommerce.IWechatPayEcommerceCancelMediaService"/>，
    /// <c>POST /v3/ecommerce/account/cancel-applications/media</c>，只收 JPG/BMP/PNG、≤2M）⇒
    /// 直传本地路径或图片字节必然失败。</para>
    /// <para><b><c>out_apply_no</c> 是幂等键</b>（商户侧唯一）：重复提交同单号不得当成新申请。</para>
    /// </remarks>
    [Post("/v3/ecommerce/account/cancel-applications")]
    Task<EcommerceCancelApplicationResponse> ApplyCancelAsync(
        [Body] EcommerceCancelApplicationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询子商户注销申请状态」：按商户申请单号读注销进展。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476223"/>。
    /// </summary>
    /// <param name="outApplyNo">商户注销申请单号（官方 path <c>out_apply_no</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与提交支<b>同一应答类型</b>，见 <see cref="EcommerceCancelApplicationResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/account/cancel-applications/out-apply-no/{out_apply_no}</c>；
    /// 「支持商户」【普通服务商】。<b>只有 <c>out_apply_no</c> 一条定位路径</b>（官方没有按微信单号查的页面）
    /// ⇒ <c>out_apply_no</c> 必须落库。</para>
    /// <para><b>两支应答表 5 行逐字段相同</b> ⇒ 共用一型；状态口径见 <see cref="EcommerceCancelApplicationStates"/>。</para>
    /// </remarks>
    [Get("/v3/ecommerce/account/cancel-applications/out-apply-no/{outApplyNo}")]
    Task<EcommerceCancelApplicationResponse> QueryCancelApplicationAsync(
        [Path] string outApplyNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询个人收款方授权结果」：按用户 <c>openid</c> 取该用户在平台下的个人收款方授权状态与授权 ID。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012739528"/>。
    /// </summary>
    /// <param name="openId">用户 OpenID（官方 path <c>openid</c>，必填 string(128)）。</param>
    /// <param name="appId">应用 AppID（官方 query <c>appid</c>，<b>必填</b> string(128)）：须与 <paramref name="openId"/> 同应用。</param>
    /// <param name="permissionType">权限类型（官方 query <c>permission_type</c>，<b>必填</b>）：官方唯一取值见 <see cref="EcommercePermissionTypes"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>三字段应答，见 <see cref="EcommerceIndividualContractByOpenIdResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/individual-contracts/{openid}?appid=…&amp;permission_type=INDIVIDUAL_PAYEE</c>；
    /// 页内无频率数字。</para>
    /// <para><b>本支的官方用途</b>：① 个人卖家开通授权收款后回查；② <b>交易下单前取 <c>individual_auth_id</c></b>
    /// ——它是本线个人收款方退款/异常退款支的身份定位键，拿不到就无法对这笔交易退款。</para>
    /// <para><b>三个必填参数一个都不能省</b>：官方把 <c>appid</c> 与 <c>permission_type</c> 都列 <c>true</c>；
    /// <c>permission_type</c> 虽然只有一个合法取值，仍是必填（SDK 不代为补默认值，补默认值会把
    /// 「将来出现第二种权限」的官方扩展静默吞掉）。</para>
    /// </remarks>
    [Get("/v3/ecommerce/individual-contracts/{openId}")]
    Task<EcommerceIndividualContractByOpenIdResponse> QueryIndividualContractByOpenIdAsync(
        [Path] string openId,
        [Query("appid")] string appId,
        [Query("permission_type")] string permissionType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「按业务申请单号查询个人收款方签约详情」：用户完成预签约后取签约信息、用户 OpenID 与实名授权结果。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015851492"/>。
    /// </summary>
    /// <param name="outContractCode">业务申请单号（官方 path <c>out_contract_code</c>，必填 string(128)）：服务商下唯一，仅数字、字母、<c>-</c>、<c>_</c>。</param>
    /// <param name="appId">签约和付款的 AppID（官方 query <c>appid</c>，<b>必填</b> string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>八字段应答（比 <c>openid</c> 支多签约时间与实名授权子树），见 <see cref="EcommerceIndividualContractResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/individual-contracts/out-contract-code/{out_contract_code}?appid=…</c>。</para>
    /// <para><b>签约结果有延迟</b>：官方原文「当用户用预签约 ID 完成签约后才返回 <c>individual_auth_id</c>、
    /// <c>individual_openid</c>」⇒ 这两字段标选填，<b>空值表示用户还没签</b>，不是数据缺失，须轮询而不是报错。</para>
    /// <para><b><c>real_name_auth_result</c> 只在预签约用到实名授权/实名验证时返回</b>，其
    /// <c>access_token</c> <b>不可刷新</b>、过期即须重新走授权流程 ⇒ 属凭据，禁止进日志与遥测。</para>
    /// <para><b><c>appid</c> 的长度口径与上一支不同</b>（本页 string(32)、上一页 string(128)）⇒
    /// 照抄校验规则会误拒，本域不做长度校验。</para>
    /// </remarks>
    [Get("/v3/ecommerce/individual-contracts/out-contract-code/{outContractCode}")]
    Task<EcommerceIndividualContractResponse> QueryIndividualContractByOutContractCodeAsync(
        [Path] string outContractCode,
        [Query("appid")] string appId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询主体名下子商户列表」：按主体类型 + 证件号码定位主体，取其未注销且与当前服务商有效绑定的子商户。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4033742105"/>。
    /// </summary>
    /// <param name="organizationType">主体类型（官方 query <c>organization_type</c>，<b>必填</b>）：见 <see cref="EcommerceSubjectOrganizationTypes"/>。</param>
    /// <param name="certNumber">证件号码密文（官方 query <c>cert_number</c>，<b>必填</b> string(512)）：须以微信支付公钥/平台证书加密，并带 <c>Wechatpay-Serial</c>。</param>
    /// <param name="limit">每页条数（官方 query <c>limit</c>，选填 integer）。</param>
    /// <param name="offset">偏移量（官方 query <c>offset</c>，选填 integer）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>分页列表，见 <see cref="EcommerceSubjectSubMerchantsResponse"/>（元素见 <see cref="EcommerceSubjectSubMerchantItem"/>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/subject-sub-merchants?organization_type=…&amp;cert_number=…</c>；
    /// 页内无频率数字。</para>
    /// <para><b>必须带 <c>Wechatpay-Serial</c> 且 <c>cert_number</c> 是密文 Query</b>：明文即违规，
    /// 密文也仍会进入 URL ⇒ 请求 URI 不得落日志（见本接口 Remarks 的敏感 Query 警示）。</para>
    /// <para><b>结果集被官方收窄过三层</b>：只返回「属于指定主体」且「与当前服务商存在<b>有效父子绑定</b>」
    /// 且「<b>未注销</b>」的子商户 ⇒ 查不到不等于该主体没有子商户，历史/已注销的必须另走查询支取证。</para>
    /// <para><b>分页是 <c>limit</c>/<c>offset</c> 双选填</b>（官方未给上限口径）⇒ SDK 不补默认值，
    /// 也不把「空列表」当错误。</para>
    /// </remarks>
    [Get("/v3/ecommerce/subject-sub-merchants")]
    Task<EcommerceSubjectSubMerchantsResponse> QuerySubjectSubMerchantsAsync(
        [Query("organization_type")] string organizationType,
        [Query("cert_number")] string certNumber,
        [Query("limit")] int? limit = null,
        [Query("offset")] int? offset = null,
        CancellationToken cancellationToken = default);
}
