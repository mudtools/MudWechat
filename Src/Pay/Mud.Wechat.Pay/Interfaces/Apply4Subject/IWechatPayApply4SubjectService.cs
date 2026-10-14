// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Apply4Subject;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「主体进件 / 商户开户意愿确认」域 SDK（服务商为公司主体提交进件申请 +
/// 查询申请单审核结果 + 按业务编号 / 申请单号撤销 + 查询商户开户意愿确认状态，<b>5 端点</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（服务商文档中心「商户开户意愿确认」章节，2026-10-14 逐页核验）：提交申请单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012722388"/>、查询申请单审核结果
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012697715"/>、撤销申请单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012697627"/>、获取商户开户意愿确认状态
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012467549"/>。
/// 支持的商户类型：<b>渠道商</b> + <b>从业机构（银行）</b> + <b>从业机构（支付机构）</b>
/// —— 注意<b>不含</b>「普通服务商」，与本线的 <see cref="IWechatPayApply4SubMerchantService"/>（普通服务商）不同。
/// </para>
/// <para>
/// <b>本域与特约商户进件（<c>apply4sub</c>）是两套互不兼容的契约</b>，这是本域最大的踩空点，
/// 逐处记在 DTO remarks 并由守卫 A4SJ 系列锁定，四处差异任一被「顺手对齐」即破坏契约：
/// ① 资源前缀不同（<c>/v3/apply4subject/applyment</c> vs <c>/v3/applyment4sub/applyment</c>）；
/// ② 查询用 <b>query 参数</b>而不是两个 path 端点，且应答只有 4 个字段、<b>无</b> <c>sign_url</c> /
/// <c>sub_mchid</c> / <c>audit_detail</c>，代之以 base64 的 <c>qrcode_data</c>（小程序码<b>图片数据</b>）；
/// ③ <c>applyment_state</c> 八值只与进件域重合三值；
/// ④ 主体类型多「小微商户」、事业单位取值是 <c>SUBJECT_TYPE_INSTITUTIONS_CLONED</c>，
/// 营业执照块拼写为英式 <c>business_licence_info</c>。
/// </para>
/// <para>
/// <b>撤销有两个定位键变体、同一资源两种 path</b>：<c>/{business_code}/cancel</c> 与
/// <c>/{applyment_id}/cancel</c> 官方在同一页给出（本页参数表把两个 path 参数都标<b>选填</b>，
/// 即<b>二选一</b>），成功应答是 <b>204 No Content 且无包体</b> ⇒ 两个方法均声明为无返回值 <c>Task</c>，
/// 与交易域关单、支付分解除授权同款处置。
/// </para>
/// <para>
/// <b>异步受理 + 人工确认</b>：提交只回 <c>applyment_id</c>；申请单进入
/// <c>WAITTING_FOR_CONFIRM_CONTACT</c> / <c>WAITTING_FOR_CONFIRM_LEGALPERSON</c> 后须由商家
/// <b>扫码</b>（<c>qrcode_data</c> 解码成图片）完成开户意愿确认，SDK 无法代为推进；
/// <c>PASSED</c>（审核通过）同样<b>不等于</b>入驻完成 —— 本域没有「商户号已生成」这类终态可判。
/// </para>
/// <para>
/// <b>错误码表内没有本域专有码</b>（与进件域相反）：四页业务错误码只有 <c>INVALID_REQUEST</c> /
/// <c>NO_AUTH</c> / <c>SYSTEM_ERROR</c> 三项 ⇒ <b>不得</b>按 <c>APPLYMENT_NOT_EXIST</c> 判「单不存在」、
/// 也<b>没有</b> <c>PROCESSING</c> 判「同编号重入」；重入与「查无此单」都只能落到
/// <c>INVALID_REQUEST</c> / <c>NO_AUTH</c>，需要凭 <c>business_code</c> 换新编号重试。
/// </para>
/// <para>
/// <b>敏感字段加密</b>：联系人 / 法定代表人 / 受益人的姓名、证件号、手机号、证件居住地址须用
/// <b>微信支付公钥</b>（推荐）或平台证书公钥以 RSAES-OAEP 加密，请求头 <c>Wechatpay-Serial</c>
/// 指向对应公钥 ID / 证书序列号（由传输层按商户配置写入）；照片类字段是图片上传 API 的 MediaID，不加密。
/// </para>
/// <para>
/// <b>无 <c>[Token]</c>、走商户签名</b>（守卫 <c>PAY-B1</c>）：签名主体是<b>服务商 / 从业机构自己的商户号</b>
/// （<c>Authorization</c> 头的 <c>mchid</c> 取 <c>WechatPayMerchantConfig.AuthorizationMchId</c>）。
/// 与 <see cref="IWechatPayApply4SubMerchantService"/> 同理：进件阶段主体尚无商户号 ⇒
/// 本域四端点须以<b>服务商自有商户号注册的普通商户形态</b>配置调用；唯一带 <c>sub_mchid</c> 的
/// 「获取商户开户意愿确认状态」中，<c>sub_mchid</c> 是<b>调用期 path 参数</b>，
/// <b>不得</b>改从 <c>WechatPayMerchantConfig.SubMchId</c> 取（服务商形态还会被 <c>Validate()</c>
/// 强制与 <c>SpMchId</c> 成对，进件期根本凑不齐）。
/// 而 <c>channel_id</c>（从业机构下的渠道商户号）同样是<b>请求体字段</b>，不是商户作用域配置。
/// </para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Apply4Subject", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayApply4SubjectService
{
    /// <summary>
    /// 提交主体进件申请单（商户开户意愿确认）。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012722388"/>。
    /// </summary>
    /// <param name="request">主体进件申请单，见 <see cref="Apply4SubjectApplymentRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>受理结果（<b>仅</b> <c>applyment_id</c>），见 <see cref="Apply4SubjectApplymentResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/apply4subject/applyment/</c>；无 path / query 参数；
    /// 必带 <c>Content-Type: application/json</c> 与 <c>Wechatpay-Serial</c>（含敏感字段）。</para>
    /// <para><b>⚠️ 结尾斜杠是官方原文</b>，不可删；而同资源的查询端点<b>没有</b>尾斜杠 ——
    /// 两形并存于同一资源路径，守卫 A4SJ-B2 锁死。</para>
    /// <para><b>主体条件必填</b>（官方原文，类型上均可空）：企业 / 个体户 ⇒ <c>business_licence_info</c>；
    /// 政府机关 / 事业单位 / 社会组织 ⇒ <c>certificate_info</c>；小微商户 ⇒ <c>assist_prove_info</c>；
    /// 金融机构 ⇒ <c>finance_institution_info</c>；政府机关 / 事业单位另需 <c>company_prove_copy</c>。</para>
    /// <para><b>待确认商户号</b>：<c>addition_info.confirm_mchid_list</c>（最多 20 个）不满足
    /// 「全称与主体一致 / 结算规则与许可证匹配」时<b>创建申请单即失败</b>，且官方把多个商户号的失败原因
    /// 拼在同一条消息里（格式为「商户号:原因;…」）⇒ 剔除失败项须按标点切分，<b>不得按文案匹配</b>。</para>
    /// <para><b>错误码</b>：<c>PARAM_ERROR</c> / <c>INVALID_REQUEST</c> / <c>SIGN_ERROR</c> /
    /// <c>NO_AUTH</c> / <c>SYSTEM_ERROR</c>（<b>无</b> <c>PROCESSING</c>，与进件域不同）。</para>
    /// </remarks>
    [Post("/v3/apply4subject/applyment/")]
    Task<Apply4SubjectApplymentResponse> SubmitApplymentAsync(
        [Body] Apply4SubjectApplymentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询主体进件申请单审核结果。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697715"/>。
    /// </summary>
    /// <param name="applymentId">申请单编号（官方 query <c>applyment_id</c>，选填 Integer）：由 <see cref="SubmitApplymentAsync"/> 返回。</param>
    /// <param name="businessCode">业务申请编号（官方 query <c>business_code</c>，选填 string(128)）：商户自有编号。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>审核结果（<c>applyment_state</c> + <c>qrcode_data</c> + 驳回两项），见 <see cref="Apply4SubjectApplymentStateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/apply4subject/applyment?applyment_id=…&amp;business_code=…</c>；
    /// 本域查询是<b>单端点 + query 定位</b>，不像进件域拆成两个 path 端点。</para>
    /// <para><b>两键至少传一个</b>（官方原文：「申请单编号和业务申请编号至少传一个」）：
    /// SDK 侧<b>不做</b>参数校验拦截（官方对两个 query 均标选填，且未定义二者同传时的优先级），
    /// 由调用方保证；同传时官方以 curl 示例形态处理（示例即两键同传）。</para>
    /// <para><b>判据只用 <c>applyment_state</c></b>（<see cref="Apply4SubjectApplymentStates"/>）；
    /// <c>reject_param</c> / <c>reject_reason</c> 仅 <c>REJECTED</c> / <c>FREEZED</c> 返回，
    /// 且一次<b>只指出一个</b>字段（无 <c>audit_detail[]</c>）⇒ 多字段驳回须按轮次逐项修。</para>
    /// <para><b><c>qrcode_data</c> 要 base64 解码成图片</b>（不是链接、不是小程序页面 path），
    /// 出现于 <c>WAITTING_FOR_CONFIRM_CONTACT</c> / <c>WAITTING_FOR_CONFIRM_LEGALPERSON</c> /
    /// <c>PASSED</c> / <c>FREEZED</c> 四态。</para>
    /// <para><b>应答不回显定位键</b>：本页字段表无 <c>business_code</c> / <c>applyment_id</c> ⇒
    /// 轮询多单时须自持编号与状态映射。</para>
    /// </remarks>
    [Get("/v3/apply4subject/applyment")]
    Task<Apply4SubjectApplymentStateResponse> QueryApplymentStateAsync(
        [Query("applyment_id")] string? applymentId = null,
        [Query("business_code")] string? businessCode = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤销主体进件申请单（按业务申请编号）。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697627"/>。
    /// </summary>
    /// <param name="businessCode">业务申请编号（官方 path <c>business_code</c>，string(128)）：商户自定义编号。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/apply4subject/applyment/{business_code}/cancel</c>；
    /// <b>无请求体</b>，成功应答 <b>204 No Content 且无包体</b> ⇒ 返回类型声明为 <c>Task</c>。</para>
    /// <para><b>与 <see cref="CancelApplymentByApplymentIdAsync"/> 是同页的两个定位键变体</b>
    /// （官方本页把两个 path 参数都标选填 ⇒ 二选一）；本仓保留两个方法而非合成一个，
    /// 因为路径段含义不同（编号字符串 vs 申请单号），守卫 A4SJ-B1 分别锁定占位符参数名。</para>
    /// <para><b>撤销后不可复用编号</b>：撤销与「同编号重提」的先后关系官方未给错误码区分，
    /// 稳妥做法是换新 <c>business_code</c> 重新提交。</para>
    /// </remarks>
    [Post("/v3/apply4subject/applyment/{businessCode}/cancel")]
    Task CancelApplymentByBusinessCodeAsync(
        [Path] string businessCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤销主体进件申请单（按申请单编号）。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012697627"/>。
    /// </summary>
    /// <param name="applymentId">申请单编号（官方 path <c>applyment_id</c>，Integer，由 <see cref="SubmitApplymentAsync"/> 返回）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/apply4subject/applyment/{applyment_id}/cancel</c>；
    /// 无请求体，成功应答 <b>204</b>。</para>
    /// <para><b>占位符写法</b>：路由占位符用 C# 参数名 <c>{applymentId}</c>（生成器 <c>HTTPCLIENT013</c> 强制），
    /// 官方 snake_case 不出现在路由里；本仓 <c>[Path]</c> 参数统一 <c>string</c>。</para>
    /// <para><b>两变体的 path 段字面相同</b>（都是单段 + <c>/cancel</c>）⇒ 写错定位键<b>不会</b>得到
    /// 「参数类型不符」，而是「查无此单」语义的 <c>INVALID_REQUEST</c>，故本方法与
    /// <see cref="CancelApplymentByBusinessCodeAsync"/> 必须靠方法名与占位符名区分，不可合并。</para>
    /// </remarks>
    [Post("/v3/apply4subject/applyment/{applymentId}/cancel")]
    Task CancelApplymentByApplymentIdAsync(
        [Path] string applymentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商户开户意愿确认状态。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012467549"/>。
    /// </summary>
    /// <param name="subMchId">特约商户号（官方 path <c>sub_mchid</c>，必填 string(32)）：官方原文标注「放在请求链接 path 中，不用写到 body」。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>授权状态（<b>仅</b> <c>authorize_state</c>），见 <see cref="Apply4SubjectMerchantStateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/apply4subject/applyment/merchants/{sub_mchid}/state</c>；仅 path 参数、无 query。</para>
    /// <para><b>与申请单状态是两个独立维度</b>：本页回答「该<b>特约商户号</b>是否已完成实名认证」
    /// （<see cref="Apply4SubjectAuthorizeStates"/>，2 值），<b>不得</b>用它判进件审核进度，
    /// 也不得用 <see cref="QueryApplymentStateAsync"/> 的结果判授权。</para>
    /// <para><b>本端点是本域唯一以商户号定位的端点</b>：<c>sub_mchid</c> 恒为调用期参数，
    /// <b>不得</b>依赖 <c>WechatPayMerchantConfig.SubMchId</c>（见接口 remarks 的商户形态结论）。</para>
    /// </remarks>
    [Get("/v3/apply4subject/applyment/merchants/{subMchId}/state")]
    Task<Apply4SubjectMerchantStateResponse> QueryMerchantAuthorizeStateAsync(
        [Path] string subMchId,
        CancellationToken cancellationToken = default);
}
