// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.MarketingPartnerDeliveryPlan;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「品牌投放计划」域 SDK（5 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径；同时有【普通商户】与【普通服务商】两页的端点，两页字段表经逐页指纹比对<b>完全同形</b>
/// ⇒ <b>一面一方法 + 双页文档引用</b>，只有字段表存在差异的端点才按「同路由多方法」分建。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>分页口径不统一</b>：本域既有 <c>offset</c>/<c>limit</c>（整型页码）也有 <c>page_size</c> + <c>offset</c> 形态，
/// 一律<b>照页</b>，不得互相套用；应答的 <c>total_count</c> 官方多为选填（<c>offset=0</c> 或结果为空时才回）。</para>
/// <para><b>PATCH 与 PUT</b>：批次/活动的改预算与改状态走 <b>PATCH</b>，终止走 <b>POST .../terminate</b>
/// ⇒ 「写操作动词」在本域不统一，照页不得合并。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "MarketingPartnerDeliveryPlan", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayMarketingPartnerDeliveryPlanService
{
    /// <summary>
    /// 设置投放计划回调地址。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016184598"/>（普通服务商）。
    /// </summary>
    /// <param name="spMchid">【服务商商户号】 是由微信支付系统生成并分配给每个服务商的唯一标识符，具体请参考 服务商模式开发必要参数说明。（官方 path <c>sp_mchid</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="request">设置投放计划回调地址请求体，字段见 <see cref="DeliveryPlanNotifyUrlSetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>设置投放计划回调地址，见 <see cref="DeliveryPlanNotifyUrlSetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/delivery-plan/{sp_mchid}/notify-url</c>；path <c>sp_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="DeliveryPlanNotifyUrlSetResponse"/>（官方应答表与请求表同形，但应答须承载 <c>code</c>/<c>message</c> ⇒ 独立类型，不复用请求类）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/delivery-plan/{spMchid}/notify-url")]
    Task<DeliveryPlanNotifyUrlSetResponse> SetDeliveryPlanNotifyUrlAsync(
        [Path] string spMchid,
        [Body] DeliveryPlanNotifyUrlSetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建投放计划。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016184554"/>（普通服务商）。
    /// </summary>
    /// <param name="request">创建投放计划请求体，字段见 <see cref="DeliveryPlanPlanCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>创建投放计划，见 <see cref="DeliveryPlanPlanCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/delivery-plan/delivery-plans</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="DeliveryPlanPlanCreateResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/delivery-plan/delivery-plans")]
    Task<DeliveryPlanPlanCreateResponse> CreateDeliveryPlanAsync(
        [Body] DeliveryPlanPlanCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询投放计划列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016184563"/>（普通服务商）。
    /// </summary>
    /// <param name="brandId">【品牌ID】 创建投放计划的品牌ID，可登录 合作伙伴平台 ，在“品牌经营中心-合作品牌管理”页面查看已授权的brand_id（官方 path <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="pageSize">【分页大小】 批量查询下每页的数量，最大不超过50。不填默认为10。（官方 query <c>page_size</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="offset">【offset】 该次请求资源的起始位置。第一页从0开始（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="planState">【投放计划状态】 不填写表示查询所有状态的投放计划，填写表示查询特定状态的投放计划。已终止、已过期是终态。。可选取值：`CREATED` 创建成功 / `TERMINATED` 已终止 / `EXPIRED` 已过期 / `DELIVERING` 投放中 / `PAUSED` 已暂停（官方 query <c>plan_state</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="auditState">【投放计划审核状态】 不填写表示查询所有审核状态的投放计划，填写则表示查询特定状态的投放计划。。可选取值：`AUDIT_INITIAL` 初始化 / `AUDIT_PROCESSING` 审批中 / `AUDIT_PASSED` 审批通过 / `AUDIT_REJECTED` 审批拒绝（官方 query <c>audit_state</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="planId">【投放计划ID】 投放计划ID，如果填入该字段，则只返回该ID对应的投放计划（官方 query <c>plan_id</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询投放计划列表，见 <see cref="DeliveryPlanPlanListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/partner/delivery-plan/delivery-plans/{brand_id}/delivery-plans</c>；path <c>brand_id</c>，query <c>page_size</c> / <c>offset</c> / <c>plan_state</c> / <c>audit_state</c> / <c>plan_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// </remarks>
    [Get("/v3/marketing/partner/delivery-plan/delivery-plans/{brandId}/delivery-plans")]
    Task<DeliveryPlanPlanListResponse> QueryDeliveryPlansAsync(
        [Path] string brandId,
        [Query("page_size")] int? pageSize = null,
        [Query("offset")] int? offset = null,
        [Query("plan_state")] string? planState = null,
        [Query("audit_state")] string? auditState = null,
        [Query("plan_id")] string? planId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改投放计划。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016184594"/>（普通服务商）。
    /// </summary>
    /// <param name="planId">【投放计划ID】 创建生成的投放计划 ID（官方 path <c>plan_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">修改投放计划请求体，字段见 <see cref="DeliveryPlanPlanUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>修改投放计划，见 <see cref="DeliveryPlanPlanUpdateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PATCH</b> <c>/v3/marketing/partner/delivery-plan/delivery-plans/{plan_id}</c>；path <c>plan_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="DeliveryPlanPlanUpdateResponse"/>（官方应答表逐字段建模，嵌套子树以独立类型承载）。</para>
    /// </remarks>
    [Patch("/v3/marketing/partner/delivery-plan/delivery-plans/{planId}")]
    Task<DeliveryPlanPlanUpdateResponse> UpdateDeliveryPlanAsync(
        [Path] string planId,
        [Body] DeliveryPlanPlanUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 终止投放计划。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016184572"/>（普通服务商）。
    /// </summary>
    /// <param name="planId">【投放计划ID】 投放计划ID（官方 path <c>plan_id</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段 ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/partner/delivery-plan/delivery-plans/{plan_id}/terminate</c>；path <c>plan_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// </remarks>
    [Post("/v3/marketing/partner/delivery-plan/delivery-plans/{planId}/terminate")]
    Task TerminateDeliveryPlanAsync(
        [Path] string planId,
        CancellationToken cancellationToken = default);
}
