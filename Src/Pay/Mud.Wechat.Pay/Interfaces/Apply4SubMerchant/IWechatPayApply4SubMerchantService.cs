// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Apply4SubMerchant;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「特约商户进件」域 SDK（服务商为特约商户提交进件申请 + 查询进件状态 +
/// 查询 / 修改特约商户结算账户 + 查询结算账户修改申请状态，<b>6 端点</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（服务商文档中心，2026-10-14 逐页核验）：提交申请单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012719997"/>、申请单号查询申请状态
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012697052"/>、业务申请编号查询申请状态
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012697168"/>、修改结算账户
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012761102"/>、查询结算账户
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012761113"/>、查询结算账户修改申请状态
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012761169"/>。
/// 支持的商户类型：<b>普通服务商</b>（进件三页）／<b>普通服务商 + 平台商户</b>（结算账户三页，两文档树各一页）。
/// </para>
/// <para>
/// <b>本域是「两个资源族 + 两种定位键」的组合，不要按前缀想当然</b>：进件申请单在
/// <c>/v3/applyment4sub/applyment…</c>，结算账户在 <c>/v3/apply4sub/sub_merchants/{sub_mchid}…</c> ——
/// 前者以 <c>applyment_id</c> 或 <c>business_code</c> 定位、<b>不含商户号 path</b>（申请时还没有商户号），
/// 后者恒以 <c>sub_mchid</c> 定位。两族前缀互为「4sub」的镜像（<c>applyment4sub</c> vs <c>apply4sub</c>），
/// 是本域最易写错的一处，守卫 A4S-B1 逐条锁定。
/// </para>
/// <para>
/// <b>两个提交类端点都是异步审核</b>：进件提交返回 <c>applyment_id</c>、修改结算账户返回
/// <c>application_no</c>，都只代表受理 ⇒ 生效与否必须轮询对应查询端点，
/// 且进件在 <c>TO_BE_CONFIRMED</c> / <c>TO_BE_SIGNED</c> 两态还需要<b>超级管理员人工</b>用 <c>sign_url</c>
/// 完成账户验证 / 签约，SDK 无法代为推进。
/// </para>
/// <para>
/// <b>敏感字段全量加密</b>：进件与修改结算账户请求里的姓名、证件号、手机号、邮箱、证件地址、
/// 开户名称、银行账号须用<b>微信支付公钥</b>（推荐）或平台证书公钥以 RSAES-OAEP 加密，
/// 并把 <c>Wechatpay-Serial</c> 指向对应公钥 ID / 证书序列号（由传输层按商户配置写入）；
/// 查询侧返回的银行账号与开户名称均为<b>掩码</b>，掩码规则由 <c>account_number_rule</c> 指定。
/// </para>
/// <para>
/// <b>媒体字段是「先上传、再引用」</b>：本域所有照片 / PDF / 视频字段填的都是
/// 「图片上传 API」（<see href="https://pay.weixin.qq.com/doc/v3/partner/4012760490"/>）或
/// 「视频上传 API」（<see href="https://pay.weixin.qq.com/doc/v3/partner/4012761084"/>）返回的 MediaID，
/// 那两个端点属 P0-7 商户经营批次，本域不重复声明。
/// </para>
/// <para>
/// <b>无 <c>[Token]</c>、走商户签名</b>（守卫 <c>PAY-B1</c>）：签名主体是<b>服务商</b>商户号
/// （<c>Authorization</c> 头的 <c>mchid</c> 由 <c>WechatPayMerchantConfig.AuthorizationMchId</c> 给出）。
/// <b>本域要用哪种商户配置形态</b>是一个容易踩空的地方：服务商形态（<c>SpMchId</c> + <c>SubMchId</c>）
/// 由 <c>Validate()</c> 强制两者成对，而进件阶段<b>特约商户号还不存在</b> ⇒ 进件族三端点应当以
/// <b>服务商自有商户号注册的普通商户形态配置</b>调用（此时 <c>AuthorizationMchId == MchId ==</c> 服务商号，
/// 签名用的就是服务商自己的 API 证书）；结算账户三端点虽然 path 带 <c>sub_mchid</c>，
/// <b>它是每次调用传入的参数，不是商户作用域</b> —— 本域任何端点都<b>不得</b>依赖
/// <c>WechatPayMerchantConfig.SubMchId</c>。形态与 <see cref="IWechatPayTransactionsService"/> 一致，详见其 remarks。
/// </para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Apply4SubMerchant", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayApply4SubMerchantService
{
    /// <summary>
    /// 提交特约商户进件申请单。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012719997"/>。
    /// </summary>
    /// <param name="request">进件申请单，见 <see cref="Apply4SubApplymentRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>受理结果（<b>仅</b> <c>applyment_id</c>），见 <see cref="Apply4SubApplymentResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/applyment4sub/applyment/</c>；无 path / query 参数；
    /// 必带 <c>Content-Type: application/json</c> 与 <c>Wechatpay-Serial</c>（含敏感字段）。</para>
    /// <para><b>⚠️ 结尾斜杠是官方原文</b>，不可删。</para>
    /// <para><b>重入而非幂等</b>：同一 <c>business_code</c> 在旧单仍「流程进行中」时重复提交返回
    /// <c>400 PROCESSING</c>（<see cref="Apply4SubMerchantErrorCodes.Processing"/>）⇒ 改资料重试须先撤单或换编号。</para>
    /// <para><b>主体条件必填</b>（官方原文，类型上均为可空）：个体户 / 企业 ⇒ <c>business_license_info</c>；
    /// 政府机关 / 事业单位 / 社会组织 ⇒ <c>certificate_info</c>；金融机构 ⇒ <c>finance_institution_info</c>；
    /// 企业 / 社会组织且受益人不止经营者一人 ⇒ <c>ubo_info_list</c>（最多 4 人）。</para>
    /// <para><b>错误码</b>：<c>PARAM_ERROR</c> / <c>INVALID_REQUEST</c> / <c>SIGN_ERROR</c> /
    /// <c>NO_AUTH</c> / <c>PROCESSING</c> / <c>SYSTEM_ERROR</c>。</para>
    /// </remarks>
    [Post("/v3/applyment4sub/applyment/")]
    Task<Apply4SubApplymentResponse> SubmitApplymentAsync(
        [Body] Apply4SubApplymentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 通过申请单号查询进件申请状态。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697052"/>。
    /// </summary>
    /// <param name="applymentId">申请单号（官方 path <c>applyment_id</c>，必填 Long，由提交申请单返回）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>申请单状态（含驳回详情 <c>audit_detail</c>），见 <see cref="Apply4SubApplymentStateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/applyment4sub/applyment/applyment_id/{applyment_id}</c>；仅 path 参数。</para>
    /// <para><b>占位符写法</b>：路由占位符用 C# 参数名 <c>{applymentId}</c>（生成器 <c>HTTPCLIENT013</c> 强制），
    /// 官方 snake_case 不出现在路由里；本仓 <c>[Path]</c> 参数统一 <c>string</c>（官方声明为 integer，
    /// 序列化时按字符串替换进路径，不影响线上报文）。</para>
    /// <para><b>判据只用 <c>applyment_state</c></b>（<see cref="Apply4SubApplymentStates"/>）：
    /// <c>applyment_state_msg</c> 是人读文本；<c>sub_mchid</c> 仅在 <c>FINISHED</c> 后出现。</para>
    /// <para><b>错误码</b>：<c>APPLYMENT_NOT_EXIST</c>（400，申请单不存在）、<c>NO_AUTH</c>、
    /// <c>RATE_LIMITED</c>（官方本页把频率限制标为 <b>403</b>，与其它页的 429 不同，照原文）。
    /// 轮询请控制频率，官方对本页有频率限制告警。</para>
    /// </remarks>
    [Get("/v3/applyment4sub/applyment/applyment_id/{applymentId}")]
    Task<Apply4SubApplymentStateResponse> QueryApplymentStateByApplymentIdAsync(
        [Path] string applymentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 通过业务申请编号查询进件申请状态。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697168"/>。
    /// </summary>
    /// <param name="businessCode">业务申请编号（官方 path <c>business_code</c>，必填 string(124)，商户自定义）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>申请单状态（与按申请单号查询<b>同一应答形态</b>），见 <see cref="Apply4SubApplymentStateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/applyment4sub/applyment/business_code/{business_code}</c>；仅 path 参数。</para>
    /// <para><b>两键等价</b>：本页与「申请单号查询」的应答字段表逐项相同 ⇒ 共用一个应答类型；
    /// 本端点的实用价值在于<b>提交失败或未落库申请单号时可凭自有编号回查</b>。</para>
    /// </remarks>
    [Get("/v3/applyment4sub/applyment/business_code/{businessCode}")]
    Task<Apply4SubApplymentStateResponse> QueryApplymentStateByBusinessCodeAsync(
        [Path] string businessCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改特约商户结算账户。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012761102"/>。
    /// </summary>
    /// <param name="subMchId">特约商户号（官方 path <c>sub_mchid</c>，必填 string(32)）：须为本服务商进件的子商户。</param>
    /// <param name="request">新的结算账户，见 <see cref="Apply4SubModifySettlementRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>修改申请单号（<b>异步受理</b>），见 <see cref="Apply4SubModifySettlementResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/apply4sub/sub_merchants/{sub_mchid}/modify-settlement</c>。</para>
    /// <para><b>频率限制</b>：接口 <b>20 次 / 分钟</b>，<b>同一特约商户每日仅 5 次</b>修改申请；
    /// 超限返回 <c>429 FREQENCY_LIMIT</c>（官方拼写如此）。</para>
    /// <para><b>开户名称约束</b>：<c>account_name</c> 只可改为与主体同名或经营者本人名称，
    /// 不一致时官方以 <c>INVALID_REQUEST</c>「开户名称与主体名称不一致」驳回 —— 拿到 4xx 不等于可换名重试。</para>
    /// <para><b>支行二选一</b>：<c>bank_name</c>（含支行全称）与 <c>bank_branch_id</c>（联行号）二选一，
    /// 是否需要支行由「查询支持个人 / 对公业务的银行列表」决定。</para>
    /// <para><b>敏感字段</b>：<c>account_number</c>（必填）、<c>account_name</c>（选填）须加密并带 <c>Wechatpay-Serial</c>。</para>
    /// </remarks>
    [Post("/v3/apply4sub/sub_merchants/{subMchId}/modify-settlement")]
    Task<Apply4SubModifySettlementResponse> ModifySettlementAsync(
        [Path] string subMchId,
        [Body] Apply4SubModifySettlementRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询特约商户结算账户。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012761113"/>。
    /// </summary>
    /// <param name="subMchId">特约商户号（官方 path <c>sub_mchid</c>，必填 string(32)）：本服务商进件、已签约的子商户。</param>
    /// <param name="accountNumberRule">银行账号展示规则（官方 query <c>account_number_rule</c>，选填）：见 <see cref="Apply4SubAccountNumberRules"/>，不传默认 <c>MASK_V1</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>当前生效的结算账户（掩码账号），见 <see cref="Apply4SubSettlementResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/apply4sub/sub_merchants/{sub_mchid}/settlement?account_number_rule=…</c>。</para>
    /// <para><b>本端点查的是「当前生效账户」</b>，不是修改申请的状态 —— 修改申请须用
    /// <see cref="QuerySettlementModifyApplicationAsync"/>；两页的 <c>verify_result</c> 取值集也不同。</para>
    /// <para><b>频率限制</b>：100 次 / 秒。<b>错误码</b>：<c>NO_AUTH</c>（该商户非本服务商进件）等公共码。</para>
    /// </remarks>
    [Get("/v3/apply4sub/sub_merchants/{subMchId}/settlement")]
    Task<Apply4SubSettlementResponse> QuerySettlementAsync(
        [Path] string subMchId,
        [Query("account_number_rule")] string? accountNumberRule = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询结算账户修改申请状态。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012761169"/>。
    /// </summary>
    /// <param name="subMchId">特约商户号（官方 path <c>sub_mchid</c>，必填 string(32)）。</param>
    /// <param name="applicationNo">修改结算账户申请单号（官方 path <c>application_no</c>，必填 string(64)，由 <see cref="ModifySettlementAsync"/> 返回）。</param>
    /// <param name="accountNumberRule">银行账号展示规则（官方 query <c>account_number_rule</c>，选填）：见 <see cref="Apply4SubAccountNumberRules"/>，不传默认 <c>MASK_V1</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>申请审核状态与审核后的账户信息（掩码），见 <see cref="Apply4SubSettlementModifyApplicationResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b>
    /// <c>/v3/apply4sub/sub_merchants/{sub_mchid}/application/{application_no}?account_number_rule=…</c>；
    /// 两个 path 参数均必填。</para>
    /// <para><b>终态判定</b>：<c>verify_result</c> 取 <c>AUDITING</c>（非终态，继续轮询）/ <c>AUDIT_SUCCESS</c> /
    /// <c>AUDIT_FAIL</c>（读 <c>verify_fail_reason</c>）；<c>verify_finish_time</c> 是审核结果<b>更新</b>时间。</para>
    /// <para><b>错误码</b>：<c>404 ORDER_NOT_EXIST</c>（申请单不存在）、<c>403 NO_AUTH</c>。</para>
    /// </remarks>
    [Get("/v3/apply4sub/sub_merchants/{subMchId}/application/{applicationNo}")]
    Task<Apply4SubSettlementModifyApplicationResponse> QuerySettlementModifyApplicationAsync(
        [Path] string subMchId,
        [Path] string applicationNo,
        [Query("account_number_rule")] string? accountNumberRule = null,
        CancellationToken cancellationToken = default);
}
