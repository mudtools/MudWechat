// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.MerchantAlterApply;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「商户资料变更（主体 / 超级管理员 / 商家简称）」域 SDK（<b>12 端点</b>：三族各「提交 + 两个查询 + 撤销」）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（2026-10-14 十六页逐字段核验，全部落在 <c>/v3/mchalterapply/</c> 前缀下）：
/// 主体族 <see href="https://pay.weixin.qq.com/doc/v3/partner/4014090649"/>（提交）/
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4014090655"/>（按申请单号查）/
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4014090667"/>（按业务单号查）/
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4014090672"/>（撤销）；
/// 超级管理员族 <see href="https://pay.weixin.qq.com/doc/v3/partner/4015617906"/> /
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015617911"/> /
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015617915"/> /
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015617919"/>；
/// 商家简称族 <see href="https://pay.weixin.qq.com/doc/v3/partner/4016166071"/> /
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4016166074"/> /
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4016166085"/> /
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4016166090"/>。</para>
/// <para><b>三族是三条独立路由族，不是「一套模板三次调用」</b>：<c>mchsubjectalterapplyment</c> /
/// <c>mchcontactalterapplyment</c> / <c>merchantnamealterapplyment</c>，且<b>定位键分族</b>——
/// 主体与超管用 <c>merchant_code</c>（服务商进件的特约商户号），简称族用 <c>sub_mchid</c>
/// ⇒ 「按业务单号查」一支的路径段名同为 <c>/merchant/{…}/out-request-no/{…}</c>，
/// 但大括号里的参数<b>不同名</b>（守卫 MAA-B1 逐支锁路径与参数位置）。</para>
/// <para><b>缺口清单的出处错配已在方案 §2.7 一-1 登记</b>：清单用主体族四个 docId 同时承载联系人族四行，
/// 逐页核得两族各有独立页面 ⇒ <b>八行是八个页面</b>，联系人族现行路由是
/// <c>mchcontactalterapplyment</c>（不是转述的 <c>contactalter</c> 形态）。</para>
/// <para><b>共用与分建的裁定（逐类型照页，不是按族一刀切）</b>：
/// 三族「提交」应答表相同 ⇒ 共用 <see cref="MerchantAlterApplySubmitResponse"/>；
/// 超管族与简称族「撤销」应答表相同 ⇒ 共用 <see cref="MerchantAlterApplyRevokeResponse"/>，
/// 而主体族撤销回 <c>{result}</c>（<c>SUCCESS</c> / <c>FAIL</c>）⇒ 分建
/// <see cref="MerchantAlterApplySubjectRevokeResponse"/>；
/// 三族「查询」应答<b>各自一型</b>——主体族与超管族字段名与长度逐字相同，但 <c>state</c> 值清单不同
/// （6 值含 <c>EDITTING</c> vs 5 值），简称族另有 <c>sub_mchid</c> 且<b>无</b> <c>audit_reject_detail</c>
/// ⇒ 按「表不同则分建」（守卫 MAA-B6 三个数都断言）。</para>
/// <para><b>提交 ≠ 生效</b>：三族都是<b>审核流</b>，受理只回 <c>apply_id</c>；
/// 唯一闭环路径是轮询查询支读 <c>state</c> 到 <see cref="MerchantAlterApplySubjectStates.Finished"/>。
/// <see cref="MerchantAlterApplySubjectStates.Modifing"/> 档表示「系统改资料中」，此时旧资料仍生效；
/// <see cref="MerchantAlterApplySubjectStates.Editting"/>（官方三 T 误拼）是「提交发生错误」，
/// 官方处置建议是<b>用同一 <c>out_request_no</c> 重提</b>而不是换新号（换号即第二笔申请单）。</para>
/// <para><b>敏感字段两支有加密头表行</b>：主体族提交（<c>legal_person_info</c> / <c>ubo_info_list[]</c> 的
/// 姓名/证件号/住址）与超管族提交（姓名/证件号/手机号）把 <c>Wechatpay-Serial</c> 列为<b>请求头参数表行</b>（必填），
/// 简称族提交则<b>无该表行且全明文</b> ⇒ 三支请求 DTO 的加密要求不同，不得按「变更族都要加密」处理
/// （守卫 MAA-B5 锁定该分布）。密文仍是 string，DTO 只承载密文，加密由宿主与传输层协同。</para>
/// <para><b>官方拼写与长度陷阱照页</b>：<c>APPLYMENT_STATE_EDITTING</c>（三 T）与
/// <c>APPLYMENT_STATE_MODIFING</c>（单 F）；<c>out_request_no</c> 在提交侧是 string(64)（主体）/
/// string(124)（超管、简称），在<b>查询应答</b>侧一律 string(64)——同域两页长度不一致，官方原文如此；
/// <c>apply_id</c> 在「按申请单号查」的 PATH 是 string(32)（主体族）/ string(64)（另两族），
/// 在应答侧一律 string(64) ⇒ 本 SDK 的 path 参数按线内惯例统一声明为 <c>string</c>，守卫锁的是位置不是 CLR 宽度。</para>
/// <para><b>规模与格式限制</b>（官方散文口径，SDK 不校验）：主体族六个资料对象<b>全标选填</b>但条件必填
/// （按 <c>alter_scope</c> 与主体类型分派，条件组合八种）；<c>ubo_info_list</c> 最多 4 人（法人已是受益人时 3 人）；
/// 补充材料五组各最多 5 张；<c>company_address</c> 4-512 字符；日期 <c>yyyy-MM-dd</c> 且可填关键字「长期」；
/// 一个商户同一时间只允许一笔流程中的申请单。</para>
/// <para><b>三族十二页均无端点级频率数字</b>（两轮复核一致），只有 429 <c>FREQUENCY_LIMITED</c> 公共错误码
/// ⇒ 注释不凭第三方 SDK 补数。<b>业务错误码</b>：400 <c>PARAM_ERROR</c> / 400 <c>INVALID_REQUEST</c> /
/// 401 <c>SIGN_ERROR</c> / 500 <c>SYSTEM_ERROR</c>，原样透传至
/// <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/>。</para>
/// <para><b>本章不含「特约商户进件」与「主体进件」</b>：<c>/v3/applyment4sub/applyment</c>
/// 落 <see cref="IWechatPayApply4SubMerchantService"/>、<c>/v3/applyment4subject/applyment</c> 落
/// <see cref="IWechatPayApply4SubjectService"/>。三族看着同类，逐字比对后只有
/// <c>finance_institution_info</c> 一个嵌套对象同名（<c>licence</c> vs <c>license</c> 拼写分家、
/// 有效期单字段 <c>*_valid_date</c> vs 双字段 <c>*_period_begin</c>/<c>*_period_end</c>、
/// 法人证件 <c>identification_*</c> vs <c>card_*</c>、UBO 列表字段名全不同）⇒ <b>跨域零复用</b>。</para>
/// <para><b>客户端</b>：走支付专用命名客户端 <c>wechat-pay</c>（BaseAddress 恒为
/// <c>https://api.mch.weixin.qq.com</c>）；商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，
/// 报文里的 <c>merchant_code</c> / <c>sub_mchid</c> 是<b>参数</b>而非作用域；本域<b>零 <c>[Token]</c></b>（守卫 PAY-B1）。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{"code","message"}</c>，
/// 加上后错误体原样落到 <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/> 与
/// <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Message"/>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "MerchantAlterApply", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayMerchantAlterApplyService
{
    /// <summary>
    /// 「提交主体信息变更申请单」：一次提交营业执照 / 登记证书 / 金融机构许可证 / 法人 / 受益人 / 补充材料六棵子树，
    /// 受理只回 <c>apply_id</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090649"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="MerchantAlterApplySubjectSubmitRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>微信支付申请单号，见 <see cref="MerchantAlterApplySubmitResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/mchalterapply/mchsubjectalterapplyment</c>；
    /// 支持商户【普通服务商】【平台商户】；无 path / query 参数。</para>
    /// <para><b>敏感字段须加密</b>：法人 / 受益人的姓名、证件号、住址须以微信支付公钥（推荐）或平台证书加密，
    /// 该页把 <c>Wechatpay-Serial</c> 列为<b>请求头表行必填</b>。</para>
    /// <para><b>受理 ≠ 通过</b>：须轮询 <see cref="QuerySubjectAlterApplyByApplyIdAsync"/>；
    /// 被驳回后用<b>同一</b> <c>out_request_no</c> 重提是覆盖原申请单，换新号会产生第二笔单。</para>
    /// </remarks>
    [Post("/v3/mchalterapply/mchsubjectalterapplyment")]
    Task<MerchantAlterApplySubmitResponse> SubmitSubjectAlterApplyAsync(
        MerchantAlterApplySubjectSubmitRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询主体变更申请单状态（按申请单号）」：官方 PATH <c>apply_id</c> 是 string(32)。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090655"/>。
    /// </summary>
    /// <param name="applyId">微信支付申请单号（官方 path <c>apply_id</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>状态与驳回原因，见 <see cref="MerchantAlterApplySubjectQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/mchalterapply/mchsubjectalterapplyment/{apply_id}</c>；无包体。</para>
    /// <para><b>与「按业务单号查」两支应答表逐字段相同</b> ⇒ 共用应答类型；<c>state</c> 本族 6 值。</para>
    /// <para><b>驳回详情分两层</b>：<c>audit_reject_reason</c> 是总体原因、<c>audit_reject_detail</c>
    /// 是按报文路径逐项列出的明细（<c>param_name</c> 形如 <c>business_license_info.merchant_name</c>），
    /// 官方只在 <c>REJECTED</c> 状态返回二者 ⇒ 只读总体原因会漏掉可定位的字段名。</para>
    /// </remarks>
    [Get("/v3/mchalterapply/mchsubjectalterapplyment/{applyId}")]
    Task<MerchantAlterApplySubjectQueryResponse> QuerySubjectAlterApplyByApplyIdAsync(
        [Path] string applyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询主体变更申请单状态（按商户号 + 业务申请编号）」：与 <see cref="QuerySubjectAlterApplyByApplyIdAsync"/> 同一应答类型。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090667"/>。
    /// </summary>
    /// <param name="merchantCode">商户号（官方 path <c>merchant_code</c>，必填 string(32)）：服务商负责进件的特约商户号。</param>
    /// <param name="outRequestNo">业务申请编号（官方 path <c>out_request_no</c>，必填 string(64)）：商家自定义，官方要求 UrlEncode。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与按申请单号查<b>同一应答类型</b>，见 <see cref="MerchantAlterApplySubjectQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b>
    /// <c>/v3/mchalterapply/mchsubjectalterapplyment/merchant/{merchant_code}/out-request-no/{out_request_no}</c>。</para>
    /// <para><b>本支存在的意义是「申请单号丢了也能查」</b>：受理应答若未落库，商家侧只剩自定义的
    /// <c>out_request_no</c> ⇒ 两支都要接，缺一支就没有恢复路径。</para>
    /// <para><b>官方要求路径参数做 UrlEncode</b>（同域各 GET 支一致）；本 SDK 的路径填充沿用传输层既有编码口径。</para>
    /// </remarks>
    [Get("/v3/mchalterapply/mchsubjectalterapplyment/merchant/{merchantCode}/out-request-no/{outRequestNo}")]
    Task<MerchantAlterApplySubjectQueryResponse> QuerySubjectAlterApplyByOutRequestNoAsync(
        [Path] string merchantCode,
        [Path] string outRequestNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「撤销主体变更申请单」：成功应答是 <b>200 + <c>{result}</c></b>（<c>SUCCESS</c> / <c>FAIL</c>），不是 204。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014090672"/>。
    /// </summary>
    /// <param name="applyId">微信支付申请单号（官方 path <c>apply_id</c>，必填 string(64)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>撤销结果字符串，见 <see cref="MerchantAlterApplySubjectRevokeResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/mchalterapply/mchsubjectalterapplyment/{apply_id}/revoke</c>；无包体。</para>
    /// <para><b>只有编辑中 / 审核中 / 已驳回三档可撤销</b>（官方正文口径），已完成或已作废的单不可撤销。</para>
    /// <para><b>撤销失败也是 HTTP 200</b>：<c>result = FAIL</c> 是业务结论而非传输错误 ⇒
    /// 判 <c>IsSuccess</c> 通过后再判 <c>result</c>，只看 HTTP 码会把失败当成功（守卫 MAA-B1）。</para>
    /// <para><b>本族应答与另两族分建</b>：超管族与简称族的撤销回 <c>{apply_id, state}</c>
    /// （见 <see cref="RevokeContactAlterApplyAsync"/>）。</para>
    /// </remarks>
    [Post("/v3/mchalterapply/mchsubjectalterapplyment/{applyId}/revoke")]
    Task<MerchantAlterApplySubjectRevokeResponse> RevokeSubjectAlterApplyAsync(
        [Path] string applyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「提交超级管理员信息变更申请单」：只提交一个 <c>contact_info</c> 对象，姓名 / 证件号 / 手机号须密文。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015617906"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="MerchantAlterApplyContactSubmitRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>微信支付申请单号，见 <see cref="MerchantAlterApplySubmitResponse"/>（与主体族共用）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/mchalterapply/mchcontactalterapplyment</c>；
    /// 支持商户【普通服务商】；无 path / query 参数。</para>
    /// <para><b><c>Wechatpay-Serial</c> 是请求头表行必填</b>：<c>contact_name</c>、
    /// <c>contact_id_card_number</c>、<c>mobile_phone</c> 三个字段官方逐字段注明「需进行加密处理」。</para>
    /// <para><b><c>out_request_no</c> 官方长度 string(124)</b>（主体族是 64）⇒ 两族请求类型不共用。</para>
    /// </remarks>
    [Post("/v3/mchalterapply/mchcontactalterapplyment")]
    Task<MerchantAlterApplySubmitResponse> SubmitContactAlterApplyAsync(
        MerchantAlterApplyContactSubmitRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询超级管理员变更申请单状态（按申请单号）」：官方 PATH <c>apply_id</c> 是 string(64)。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015617911"/>。
    /// </summary>
    /// <param name="applyId">微信支付申请单号（官方 path <c>apply_id</c>，必填 string(64)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>状态与驳回原因，见 <see cref="MerchantAlterApplyContactQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/mchalterapply/mchcontactalterapplyment/{apply_id}</c>；无包体。</para>
    /// <para><b>与主体族查询应答分建</b>：字段名与长度两页相同，唯一差异是 <c>state</c> 值清单
    /// （本族 5 值、<b>无</b> <c>APPLYMENT_STATE_EDITTING</c>）⇒ 值集属字段表的一部分，按「表不同则分建」。</para>
    /// </remarks>
    [Get("/v3/mchalterapply/mchcontactalterapplyment/{applyId}")]
    Task<MerchantAlterApplyContactQueryResponse> QueryContactAlterApplyByApplyIdAsync(
        [Path] string applyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询超级管理员变更申请单状态（按商户号 + 业务申请编号）」：与 <see cref="QueryContactAlterApplyByApplyIdAsync"/> 同一应答类型。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015617915"/>。
    /// </summary>
    /// <param name="merchantCode">商户号（官方 path <c>merchant_code</c>，必填 string(32)）。</param>
    /// <param name="outRequestNo">业务申请编号（官方 path <c>out_request_no</c>，必填 string(64)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与按申请单号查<b>同一应答类型</b>，见 <see cref="MerchantAlterApplyContactQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b>
    /// <c>/v3/mchalterapply/mchcontactalterapplyment/merchant/{merchant_code}/out-request-no/{out_request_no}</c>。</para>
    /// <para><b>路径形态与主体族逐字符同构</b>（只有大括号参数名与族前缀不同）⇒ 守卫 MAA-B1 逐族锁完整路径，
    /// 不接受「一条模板生成三族」的写法。</para>
    /// </remarks>
    [Get("/v3/mchalterapply/mchcontactalterapplyment/merchant/{merchantCode}/out-request-no/{outRequestNo}")]
    Task<MerchantAlterApplyContactQueryResponse> QueryContactAlterApplyByOutRequestNoAsync(
        [Path] string merchantCode,
        [Path] string outRequestNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「撤销超级管理员变更申请单」：应答是 <b>200 + <c>{apply_id, state}</c></b>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015617919"/>。
    /// </summary>
    /// <param name="applyId">微信支付申请单号（官方 path <c>apply_id</c>，必填 string(64)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>申请单号与撤销后状态，见 <see cref="MerchantAlterApplyRevokeResponse"/>（与简称族共用）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/mchalterapply/mchcontactalterapplyment/{apply_id}/revoke</c>；无包体。</para>
    /// <para><b>应答两字段官方均标选填 string</b>（无长度、无值域表行）⇒ 本 SDK 按 string 承载，
    /// 状态判读以查询支为准。</para>
    /// </remarks>
    [Post("/v3/mchalterapply/mchcontactalterapplyment/{applyId}/revoke")]
    Task<MerchantAlterApplyRevokeResponse> RevokeContactAlterApplyAsync(
        [Path] string applyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「提交商家简称变更申请单」：三字段全明文，本支<b>不要求</b> <c>Wechatpay-Serial</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016166071"/>。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="MerchantAlterApplyMerchantNameSubmitRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>微信支付申请单号，见 <see cref="MerchantAlterApplySubmitResponse"/>（三族共用）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/mchalterapply/merchantnamealterapplyment</c>；
    /// 支持商户【普通服务商】【平台商户】。</para>
    /// <para><b>定位键是 <c>sub_mchid</c>（不是 <c>merchant_code</c>）</b>：本族全用特约商户号，
    /// 照抄另两族的字段名会直接 400 <c>PARAM_ERROR</c>。</para>
    /// <para><b>无加密头表行</b>：本族页面无任何「须加密」字段 ⇒ 不得给简称支配 <c>Wechatpay-Serial</c>。</para>
    /// </remarks>
    [Post("/v3/mchalterapply/merchantnamealterapplyment")]
    Task<MerchantAlterApplySubmitResponse> SubmitMerchantNameAlterApplyAsync(
        MerchantAlterApplyMerchantNameSubmitRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询商家简称变更申请单状态（按申请单号）」：应答<b>不含</b> <c>audit_reject_detail</c>，只有总体原因。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016166074"/>。
    /// </summary>
    /// <param name="applyId">微信支付申请单号（官方 path <c>apply_id</c>，必填 string(64)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>五字段状态应答，见 <see cref="MerchantAlterApplyMerchantNameQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/mchalterapply/merchantnamealterapplyment/{apply_id}</c>；无包体。</para>
    /// <para><b>驳回原因只有一层</b>：本族应答表没有 <c>audit_reject_detail</c> 行 ⇒ 复用主体族应答类型
    /// 会多出一个永不为真的字段（守卫 MAA-B6 断言本域三族查询应答是三型）。</para>
    /// </remarks>
    [Get("/v3/mchalterapply/merchantnamealterapplyment/{applyId}")]
    Task<MerchantAlterApplyMerchantNameQueryResponse> QueryMerchantNameAlterApplyByApplyIdAsync(
        [Path] string applyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询商家简称变更申请单状态（按特约商户号 + 业务申请编号）」：路径段名同为 <c>merchant/</c>，但参数是 <c>sub_mchid</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016166085"/>。
    /// </summary>
    /// <param name="subMchId">特约商户号（官方 path <c>sub_mchid</c>，必填 string(32)）。</param>
    /// <param name="outRequestNo">业务申请编号（官方 path <c>out_request_no</c>，必填 string(64)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与按申请单号查<b>同一应答类型</b>，见 <see cref="MerchantAlterApplyMerchantNameQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b>
    /// <c>/v3/mchalterapply/merchantnamealterapplyment/merchant/{sub_mchid}/out-request-no/{out_request_no}</c>。</para>
    /// <para><b>字面段 <c>merchant</c> 里装的是特约商户号</b>：官方路径模板与本族参数表不同名，
    /// 属文档形态而非契约 ⇒ 方法参数按<b>参数表</b>命名为 <c>subMchId</c>（守卫 MAA-B1 锁路径字面量与参数顺序）。</para>
    /// </remarks>
    [Get("/v3/mchalterapply/merchantnamealterapplyment/merchant/{subMchId}/out-request-no/{outRequestNo}")]
    Task<MerchantAlterApplyMerchantNameQueryResponse> QueryMerchantNameAlterApplyByOutRequestNoAsync(
        [Path] string subMchId,
        [Path] string outRequestNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「撤销商家简称变更申请单」：与 <see cref="RevokeContactAlterApplyAsync"/> 同一应答类型（<c>{apply_id, state}</c>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016166090"/>。
    /// </summary>
    /// <param name="applyId">微信支付申请单号（官方 path <c>apply_id</c>，必填 string(64)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>申请单号与撤销后状态，见 <see cref="MerchantAlterApplyRevokeResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/mchalterapply/merchantnamealterapplyment/{apply_id}/revoke</c>；无包体。</para>
    /// <para><b>该页 <c>state</c> 描述只印 <c>APPLYMENT_STATE_CANCELED</c> 一值</b>（且描述首句误写成
    /// 「查询申请单状态」）⇒ 视为文档特例，不据此为本族另建窄枚举。</para>
    /// </remarks>
    [Post("/v3/mchalterapply/merchantnamealterapplyment/{applyId}/revoke")]
    Task<MerchantAlterApplyRevokeResponse> RevokeMerchantNameAlterApplyAsync(
        [Path] string applyId,
        CancellationToken cancellationToken = default);
}
