// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.MerchantService;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「商户消费者投诉」域 SDK（<b>11 端点</b>：通知地址 CRUD + 投诉列表 / 详情 / 协商历史 + 回复 / 完成 / 退款审批 + 即时服务）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（2026-10-14 十三页逐字段核验）：通知地址创建
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458679"/>、更新
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012459282"/>、查询
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012459014"/>、删除
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012460452"/>；
/// 投诉列表 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533431"/>、详情
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458106"/>、协商历史
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467250"/>；
/// 回复 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467254"/>、完成处理
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467255"/>、退款审批
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467256"/>、即时服务
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>。</para>
/// <para><b>本域是支付线唯一同时含 POST/PUT/GET/DELETE 四动词的域</b>：通知地址四支共用同一路由
/// <c>/v3/merchant-service/complaint-notifications</c>，其中 POST/PUT 请求体相同（守卫 MC-B6），
/// GET/POST/PUT 应答相同（守卫 MC-B6），DELETE 为 204 无包体。</para>
/// <para><b>写支返回形态分裂</b>：回复 / 完成 / 退款审批三支为 204 无包体 ⇒ 裸 <see cref="Task"/>；
/// 即时服务支为 200 + JSON（回 <c>log_id</c>）⇒ 有返回类型。与门店域的 PATCH 204 同理，
/// 不能把 204 当「成功即生效」——本域的状态闭环靠查询支读 <c>complaint_state</c>。</para>
/// <para><b>图片通道独立</b>：上传与拉取两支走手工通道（非声明式），见 <see cref="Mud.Wechat.Pay.MerchantService.IWechatPayComplaintMediaService"/>，
/// 因为上传是 multipart、拉取是二进制流，无法用 <c>[Post]</c>/<c>[Get]</c> 表达。</para>
/// <para><b>客户端</b>：走支付专用命名客户端 <c>wechat-pay</c>（BaseAddress 恒为
/// <c>https://api.mch.weixin.qq.com</c>）；本域<b>零 <c>[Token]</c></b>（守卫 PAY-B1）。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "MerchantService", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayMerchantService
{
    // ===== 通知地址 CRUD（4 端点）=====

    /// <summary>
    /// 「创建投诉通知回调地址」：POST <c>/v3/merchant-service/complaint-notifications</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458679"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="MerchantServiceComplaintNotificationUrlRequest"/>（仅 <c>url</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>。</param>
    /// <returns>应答体，见 <see cref="MerchantServiceComplaintNotificationResponse"/>（<c>mchid</c> + <c>url</c>）。</returns>
    [Post("/v3/merchant-service/complaint-notifications")]
    Task<MerchantServiceComplaintNotificationResponse> CreateComplaintNotificationAsync(
        MerchantServiceComplaintNotificationUrlRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「更新投诉通知回调地址」：PUT <c>/v3/merchant-service/complaint-notifications</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012459282"/>。
    /// </summary>
    /// <param name="request">请求体，与创建支<b>同一类型</b>，见 <see cref="MerchantServiceComplaintNotificationUrlRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>。</param>
    /// <returns>应答体，与创建/查询支<b>同一类型</b>，见 <see cref="MerchantServiceComplaintNotificationResponse"/>。</returns>
    /// <remarks><b>请求体与创建支完全一致</b>（守卫 MC-B6）：仅 <c>url string(255)</c> 必填。</remarks>
    [Put("/v3/merchant-service/complaint-notifications")]
    Task<MerchantServiceComplaintNotificationResponse> UpdateComplaintNotificationAsync(
        MerchantServiceComplaintNotificationUrlRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询投诉通知回调地址」：GET <c>/v3/merchant-service/complaint-notifications</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012459014"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>。</param>
    /// <returns>应答体，与创建/更新支<b>同一类型</b>，见 <see cref="MerchantServiceComplaintNotificationResponse"/>。</returns>
    [Get("/v3/merchant-service/complaint-notifications")]
    Task<MerchantServiceComplaintNotificationResponse> GetComplaintNotificationAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「删除投诉通知回调地址」：DELETE <c>/v3/merchant-service/complaint-notifications</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012460452"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>。</param>
    /// <returns>无返回内容（204 无包体）。</returns>
    [Delete("/v3/merchant-service/complaint-notifications")]
    Task DeleteComplaintNotificationAsync(CancellationToken cancellationToken = default);

    // ===== 投诉查询（3 端点）=====

    /// <summary>
    /// 「查询投诉单列表」：GET <c>/v3/merchant-service/complaints-v2</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533431"/>。
    /// </summary>
    /// <param name="limit">分页大小（query <c>limit</c>，选填 integer）：范围 [1,50]。</param>
    /// <param name="offset">分页开始位置（query <c>offset</c>，选填 integer）：从 0 计数。</param>
    /// <param name="beginDate">开始日期（query <c>begin_date</c>，<b>必填</b> string(10)）：格式 yyyy-MM-DD，跨度 ≤30 天。</param>
    /// <param name="endDate">结束日期（query <c>end_date</c>，<b>必填</b> string(10)）：格式 yyyy-MM-DD。</param>
    /// <param name="complaintedMchId">被诉商户号（query <c>complainted_mchid</c>，选填 string(64)）：服务商查指定子商户时传。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>。</param>
    /// <returns>列表应答，见 <see cref="MerchantServiceComplaintListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b>，query 参数。<c>total_count</c> 仅 <c>offset=0</c> 时返回。</para>
    /// <para><b>本域 <c>payer_openid</c> 不在列表里</b>：只有详情支返回 ⇒ 需要 openid 做客服触达时须逐单调详情（守卫 MC-B6）。</para>
    /// </remarks>
    [Get("/v3/merchant-service/complaints-v2")]
    Task<MerchantServiceComplaintListResponse> ListComplaintsAsync(
        [Query("limit")] long? limit = null,
        [Query("offset")] long? offset = null,
        [Query("begin_date")] string? beginDate = null,
        [Query("end_date")] string? endDate = null,
        [Query("complainted_mchid")] string? complaintedMchId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询投诉单详情」：GET <c>/v3/merchant-service/complaints-v2/{complaint_id}</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458106"/>。
    /// </summary>
    /// <param name="complaintId">投诉单号（path <c>complaint_id</c>，必填 string(64)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>。</param>
    /// <returns>详情应答，见 <see cref="MerchantServiceComplaintDetail"/>（比列表多 <c>payer_openid</c> 等键）。</returns>
    [Get("/v3/merchant-service/complaints-v2/{complaintId}")]
    Task<MerchantServiceComplaintDetail> GetComplaintDetailAsync(
        [Path] string complaintId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询投诉单协商历史」：GET <c>/v3/merchant-service/complaints-v2/{complaint_id}/negotiation-historys</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467250"/>。
    /// </summary>
    /// <param name="complaintId">投诉单号（path <c>complaint_id</c>，必填 string(64)）。</param>
    /// <param name="limit">分页大小（query <c>limit</c>，选填 integer）：范围 [1,300]（上限高于列表支）。</param>
    /// <param name="offset">分页开始位置（query <c>offset</c>，选填 integer）：从 0 计数。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>。</param>
    /// <returns>协商历史应答，见 <see cref="MerchantServiceNegotiationHistoryResponse"/>。</returns>
    /// <remarks><b>本域 <c>limit</c> 上界不同</b>：列表支 50、协商历史支 300（守卫 MC-B1 锁定该差异）。</remarks>
    [Get("/v3/merchant-service/complaints-v2/{complaintId}/negotiation-historys")]
    Task<MerchantServiceNegotiationHistoryResponse> GetNegotiationHistoryAsync(
        [Path] string complaintId,
        [Query("limit")] long? limit = null,
        [Query("offset")] long? offset = null,
        CancellationToken cancellationToken = default);

    // ===== 投诉处理写支（4 端点）=====

    /// <summary>
    /// 「回复投诉单」：POST <c>/v3/merchant-service/complaints-v2/{complaint_id}/response</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467254"/>。
    /// </summary>
    /// <param name="complaintId">投诉单号（path <c>complaint_id</c>，必填 string(64)）。</param>
    /// <param name="request">请求体，见 <see cref="MerchantServiceComplaintResponseRequest"/>（含文字链 / 小程序跳转信息）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>。</param>
    /// <returns>无返回内容（204 无包体）。</returns>
    /// <remarks><b>首次回复即改变状态</b>：待处理 → 处理中（官方原文），但受理 ≠ 可用，仍须经查询支确认。</remarks>
    [Post("/v3/merchant-service/complaints-v2/{complaintId}/response")]
    Task RespondToComplaintAsync(
        [Path] string complaintId,
        MerchantServiceComplaintResponseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「标记投诉单处理完成」：POST <c>/v3/merchant-service/complaints-v2/{complaint_id}/complete</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467255"/>。
    /// </summary>
    /// <param name="complaintId">投诉单号（path <c>complaint_id</c>，必填 string(64)）。</param>
    /// <param name="request">请求体，见 <see cref="MerchantServiceComplaintCompleteRequest"/>（仅 <c>complainted_mchid</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>。</param>
    /// <returns>无返回内容（204 无包体）。</returns>
    /// <remarks><b>调用后即对用户下发满意度评价</b>（官方原文），无法撤回；重复投诉率是考核指标之一。</remarks>
    [Post("/v3/merchant-service/complaints-v2/{complaintId}/complete")]
    Task CompleteComplaintAsync(
        [Path] string complaintId,
        MerchantServiceComplaintCompleteRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「反馈退款审批结果」：POST <c>/v3/merchant-service/complaints-v2/{complaint_id}/update-refund-progress</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467256"/>。
    /// </summary>
    /// <param name="complaintId">投诉单号（path <c>complaint_id</c>，必填 string(64)）。</param>
    /// <param name="request">请求体，见 <see cref="MerchantServiceRefundProgressUpdateRequest"/>（<b>无</b> <c>complainted_mchid</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>。</param>
    /// <returns>无返回内容（204 无包体）。</returns>
    /// <remarks>
    /// <para><b>同意不落退款</b>：<c>APPROVE</c> 后必须额外调退款接口发起原路退款，到账后状态才自动扭转为处理完成；
    /// <c>REJECT</c> 则当场扭转。</para>
    /// <para><b>本支请求体没有 <c>complainted_mchid</c></b>：与同族其他写支不同（守卫 MC-B3 锁定该差异）。</para>
    /// </remarks>
    [Post("/v3/merchant-service/complaints-v2/{complaintId}/update-refund-progress")]
    Task UpdateRefundProgressAsync(
        [Path] string complaintId,
        MerchantServiceRefundProgressUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「回复需要即时服务的投诉单」：POST <c>/v3/merchant-service/complaints-v2/{complaint_id}/response-immediate-service</c>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017151596"/>。
    /// </summary>
    /// <param name="complaintId">投诉单号（path <c>complaint_id</c>，必填 string(64)）。</param>
    /// <param name="request">请求体，见 <see cref="MerchantServiceImmediateServiceRequest"/>（新版消息格式 + 幂等 ID 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>。</param>
    /// <returns>应答体，见 <see cref="MerchantServiceImmediateServiceResponse"/>（回 <c>log_id</c>）。</returns>
    /// <remarks>
    /// <para><b>官方适用对象</b>：列表返回的 <c>need_immediate_service=true</c> 的投诉单（2026 年新页）。</para>
    /// <para><b><c>idempotent_id</c> 必填</b>：官方 string(128)，「相同 idempotent_id 的请求只会被处理一次」，留空即 <c>PARAM_ERROR</c>。</para>
    /// <para><b>本域唯一返回包体的写支</b>：其余三支均为 204，只有这一支能拿到操作流水号，用于与协商历史条目对齐。</para>
    /// </remarks>
    [Post("/v3/merchant-service/complaints-v2/{complaintId}/response-immediate-service")]
    Task<MerchantServiceImmediateServiceResponse> RespondToImmediateServiceAsync(
        [Path] string complaintId,
        MerchantServiceImmediateServiceRequest request,
        CancellationToken cancellationToken = default);
}
