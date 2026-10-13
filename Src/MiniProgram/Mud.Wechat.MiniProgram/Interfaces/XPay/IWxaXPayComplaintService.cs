// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「虚拟支付 · 支付投诉」域 SDK（7 端点：获取投诉列表 / 获取投诉详情 / 获取协商历史 /
/// 回复用户 / 完成投诉处理 / 获取投诉图片签名头部 / 商户被管控原因查询）。
/// </summary>
/// <remarks>
/// <para>
/// 域统一形态（POST only、不支持云调用、权限集 157、<c>pay_sig</c> 落 Query 的凭据处置、敏感值口径、
/// 错误码表三分与 <c>env</c> 的三类语义）见 <see cref="IWxaXPayCurrencyService"/>，本文件只写本族特有事实。
/// </para>
/// <para>
/// <b>本族 7 端点全部要求 <c>pay_sig</c>，且全部是「签名校验用 env」</b>（官方原文「仅作为签名校验，
/// 查询的结果都是正式环境的」）⇒ 投诉与管控数据<b>只有现网一份</b>，沙箱值不会切出另一套数据。
/// </para>
/// <para>
/// <b>本族是敏感值最重的一章</b>：投诉人手机号 <c>payer_phone</c> 与 <c>payer_openid</c> 属个人信息，
/// 凭证图片地址、<c>sign</c> 与 <c>cos_url</c>（官方明示 30 分钟有效）属访问凭据 ⇒
/// <b>严禁</b>写进日志、遥测或异常消息（与 <c>permanent_code</c> 同级处置）。
/// </para>
/// <para>
/// <b>处理动作有官方优先级依据</b>：<c>problem_type = REFUND</c>（申请退款）官方原文「是需要最高优先处理的单据」，
/// <c>user_tag_list</c> 含 <c>HIGH_RISK</c> 时原文「请按照运营要求优先妥善处理」⇒ 队列排序应读这两支而非投诉时间。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "XPay", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaXPayComplaintService
{
    /// <summary>
    /// 获取投诉列表（<c>POST /xpay/get_complaint_list</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_get_complaint_list.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>env</c> / <c>begin_date</c> / <c>end_date</c> / <c>offset</c> / <c>limit</c> 五支全必填），见 <see cref="WxaXPayGetComplaintListRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="signature">
    /// 用户态签名（官方 Query 参数 <c>signature</c>）。
    /// <b>官方本页自相矛盾</b>（与 <c>refund_order</c> 同形）：「注意事项」原文写「使用用户态签名与支付签名」，
    /// 而「查询参数」表只列 <c>access_token</c> 与 <c>pay_sig</c> 两支 ⇒ 本 SDK 以<b>参数表为准</b>定 <c>pay_sig</c> 必填、
    /// 以<b>注意事项为准</b>开放可选 <c>signature</c>（默认不发送）。该取舍与 <c>refund_order</c> 一并由守卫 XP3 登记。
    /// </param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>总数 + 投诉单列表（列表元素与详情端点同形共用），见 <see cref="WxaXPayGetComplaintListResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c>；<b>时间窗与分页五支全部必填</b>（本域必填面最严的查询页之一），
    /// 且日期格式是 <c>yyyy-mm-dd</c> 字符串 ⇒ 与账单侧 <c>YYYYMMDD</c> 数字不同形，两支不可互喂。
    /// </para>
    /// <para>
    /// <b>翻页游标是 <c>offset</c> 且官方原文「从0开始」</b> ⇒ 处理过程中列表本身会因新投诉插入而位移，
    /// 按 <c>offset</c> 批处理须以 <c>complaint_id</c> 去重。
    /// </para>
    /// <para>
    /// <b>两支「待办」信号官方单列在响应里</b>：<c>incoming_user_response</c>（有待回复用户留言）与
    /// <c>complaint_state</c>（PENDING / PROCESSING / PROCESSED，取值写在说明列、无枚举表）⇒ SLA 看板须两支一起看。
    /// </para>
    /// <para><b>敏感值</b>：<c>payer_phone</c> / <c>payer_openid</c> / 凭证图片地址 ⇒ 不得进日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/xpay/get_complaint_list")]
    Task<WxaXPayGetComplaintListResponse> GetComplaintListAsync(
        [Body] WxaXPayGetComplaintListRequest request,
        [Query("pay_sig")] string paySig,
        [Query("signature")] string? signature = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取投诉详情（<c>POST /xpay/get_complaint_detail</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_get_complaint_detail.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>env</c> / <c>complaint_id</c> 必填），见 <see cref="WxaXPayGetComplaintDetailRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>单个投诉单（与列表元素共用类型），见 <see cref="WxaXPayGetComplaintDetailResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c>，以 <c>complaint_id</c> 定位（该值官方注明来自
    /// <see cref="GetComplaintListAsync"/>）。
    /// </para>
    /// <para>
    /// <b>列表与详情的子表字段名集逐字段相同</b> ⇒ 共用 <see cref="WxaXPayComplaint"/> 类型（由 <c>sameAs</c> 取证、
    /// 守卫 XP5 锁定）；差异只在官方类型列的详略标注（列表侧把三支嵌套标 <c>objarray</c>、详情侧标 <c>array</c>，
    /// 而示例证明都是对象数组）。
    /// </para>
    /// <para><b>详情比列表多不出新信息</b>：本页返回体就是单个投诉对象 ⇒ 不要用「详情」当补字段的手段，缺字段是官方没有。</para>
    /// </remarks>
    [Post("/xpay/get_complaint_detail")]
    Task<WxaXPayGetComplaintDetailResponse> GetComplaintDetailAsync(
        [Body] WxaXPayGetComplaintDetailRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取协商历史（<c>POST /xpay/get_negotiation_history</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_get_negotiation_history.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>env</c> / <c>complaint_id</c> / <c>offset</c> / <c>limit</c> 全必填），见 <see cref="WxaXPayGetNegotiationHistoryRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>流水总数 + 逐条协商记录（<c>operate_type</c> 取值字典出处），见 <see cref="WxaXPayGetNegotiationHistoryResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c>；<b>本端点是本域唯一给出操作类型枚举表的一页</b>
    /// （<see cref="WxaXPayOperateTypes"/> 20 值，含两处官方 typo 照录）。
    /// </para>
    /// <para>
    /// <b>六值是系统通知而非人发言</b>（三支 <c>*_SYSTEM_MESSAGE</c> 加撤诉/确认两支）⇒ 做「商户是否已回复」判据时
    /// 须排除系统通知支，否则官方自动文案会被读成处理动作。
    /// </para>
    /// <para>
    /// <b>两值仅存在于历史投诉单</b>（官方原文对 <c>USER_REVOKE_COMPLAINT</c> 与 <c>USER_COMFIRM_COMPLAINT</c> 的说明）⇒
    /// <b>不得</b>据本页取值集反推投诉列表的状态机全集。
    /// </para>
    /// </remarks>
    [Post("/xpay/get_negotiation_history")]
    Task<WxaXPayGetNegotiationHistoryResponse> GetNegotiationHistoryAsync(
        [Body] WxaXPayGetNegotiationHistoryRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 回复用户（<c>POST /xpay/response_complaint</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_response_complaint.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>env</c> / <c>complaint_id</c> / <c>response_content</c> / <c>response_images</c> 四支全必填），见 <see cref="WxaXPayResponseComplaintRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方应答体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 直接返回 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c>；<b><c>response_images</c> 官方必填</b>且元素是
    /// <c>upload_vp_file</c> 返回的 <c>file_id</c>（原文「传upload_vp_file接口返回的file_id」）⇒
    /// 纯文字回复也须传<b>空数组</b>而不是省略该字段。
    /// </para>
    /// <para>
    /// <b>回复内容受内容审核</b>：撞 <c>268490007</c>（图片或文字存在敏感内容）时官方不会部分生效 ⇒ 须改文案重发。
    /// </para>
    /// <para><b>回复不等于结单</b>：状态推进须走 <see cref="CompleteComplaintAsync"/>，否则投诉单停留在处理中并继续计入 <c>incoming_user_response</c> 面。</para>
    /// </remarks>
    [Post("/xpay/response_complaint")]
    Task<WxaResponse> ResponseComplaintAsync(
        [Body] WxaXPayResponseComplaintRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 完成投诉处理（<c>POST /xpay/complete_complaint</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_complete_complaint.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>env</c> / <c>complaint_id</c> 必填），见 <see cref="WxaXPayCompleteComplaintRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方应答体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 直接返回 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c>，请求面只有定位键两支。
    /// </para>
    /// <para>
    /// <b>结单不等于退款</b>：本端点只把投诉单置为处理完成，资金动作须走
    /// <c>IWxaXPayOrderService.RefundOrderAsync</c> ⇒ 对 <c>problem_type = REFUND</c> 的单据，
    /// 官方口径是「需最高优先处理」，顺序应为先处理资金再结单。
    /// </para>
    /// <para>
    /// <b>结单是终态动作</b>：<c>complaint_state</c> 一旦推进到 <c>PROCESSED</c>，同单再回复的官方语义本页未给 ⇒
    /// <b>不得</b>推断「结单后仍可继续协商」，需重开时以官方正文或错误码为准。
    /// </para>
    /// </remarks>
    [Post("/xpay/complete_complaint")]
    Task<WxaResponse> CompleteComplaintAsync(
        [Body] WxaXPayCompleteComplaintRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取微信支付反馈投诉图片的签名头部（<c>POST /xpay/get_upload_file_sign</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_get_upload_file_sign.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>env</c> / <c>wxpay_url</c> / <c>convert_cos</c> / <c>complaint_id</c> 四支全必填），见 <see cref="WxaXPayGetUploadFileSignRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>Authorization</c> 头部值 + <c>convert_cos</c> 为真时的临时转存地址，见 <see cref="WxaXPayGetUploadFileSignResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c>；<b>本端点只签发凭证、不代下载</b> —— 官方原文要求下载微信支付图片时
    /// 必须同时带三个头部：<c>Authorization</c>（取本响应 <c>sign</c>）、<c>Accept: application/json</c>、
    /// 非空 <c>User-Agent</c>，缺任一支官方报错 ⇒ 下载动作归宿主。
    /// </para>
    /// <para>
    /// <b><c>wxpay_url</c> 有官方限定的地址形态</b>（原文示例
    /// <c>https://api.mch.weixin.qq.com/v3/merchant-service/images/{xxxxxx}</c>）⇒ 传小程序侧地址不在本页契约内。
    /// </para>
    /// <para><b><c>cos_url</c> 仅当 <c>convert_cos</c> 为 true 才有意义，且官方明示 30 分钟有效</b> ⇒ 不得持久化复用。</para>
    /// <para><b>敏感值</b>：<c>sign</c> 是访问凭证 ⇒ 不得进日志、遥测或异常消息（本响应两支都属凭据形态）。</para>
    /// </remarks>
    [Post("/xpay/get_upload_file_sign")]
    Task<WxaXPayGetUploadFileSignResponse> GetUploadFileSignAsync(
        [Body] WxaXPayGetUploadFileSignRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商户被管控原因查询（<c>POST /xpay/query_punishment_reasons</c>，不支持云调用，权限集 157；
    /// <b>官方明写无特殊错误码</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_punishment_reasons.html"/>。
    /// </summary>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>管控能力清单 + 逐条被管控原因与解脱路径，见 <see cref="WxaXPayQueryPunishmentReasonsResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>官方请求参数表字段数为 0</b>（本域唯一，且无 <c>env</c> 支）：本端点只定位当前商户自身 ⇒
    /// 不设请求 DTO、方法无 <c>[Body]</c> 参数，与本线短剧授权端点同一「空请求体」形态。
    /// </para>
    /// <para>
    /// <b>两支能力面都要读</b>：<c>limited_functions</c> 是标准枚举能力名列表、<c>other_limited_functions</c>
    /// 是未被枚举覆盖的自由文本（官方为该场景单设字段）⇒ 只读前者会漏掉部分管控能力。
    /// </para>
    /// <para>
    /// <b><c>limitation_case_id</c> 是与管控流水通知 <c>business_code</c> 关联的官方 join 键</b>（原文说明）⇒
    /// 收到管控通知后应回本页取解脱路径，而非直接改代码。
    /// </para>
    /// <para>
    /// <b>两支时间不可互替</b>：<c>limitation_start_date</c> 仅延迟管控时返回（预计开始），实际生效时刻看
    /// <c>limitation_date</c>。
    /// </para>
    /// <para><b>官方明写本页无特殊错误码</b>，且三支枚举语义字段（原因类型 / 解脱方式 / 管控生效方式）官方无枚举表⇒ 按 XP5 口径不建常量类。</para>
    /// </remarks>
    [Post("/xpay/query_punishment_reasons")]
    Task<WxaXPayQueryPunishmentReasonsResponse> QueryPunishmentReasonsAsync(
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);
}
