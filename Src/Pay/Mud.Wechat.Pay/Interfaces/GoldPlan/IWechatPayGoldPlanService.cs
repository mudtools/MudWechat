// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.GoldPlan;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「点金计划（商家小票与广告位管理）」域 SDK（5 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 5 支中<b>5 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 5 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>无应答体支的返回形态</b>：本域 3 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 <see cref="Task"/>，不建响应类型。</para>
/// <para><b>五支官方全部只印服务商页，身份是【普通服务商】+【渠道商】双标签</b>（正文原文「支持商户：【普通服务商】 【渠道商】」） ⇒ 两种身份共用同一契约、同一接口，<b>不得</b>按身份拆面或补建商户页。</para>
/// <para><b>动词是官方反直觉形态，照页不得「纠正」</b>：关闭广告展示用 <b>POST</b>（<c>close-advertising-show</c>）、 开通广告展示用 <b>PATCH</b>（<c>open-advertising-show</c>）、同业过滤标签管理用 <b>POST</b>（<c>set-advertising-industry-filter</c>）， 两支状态切换也都是 POST ⇒ 不得按「开关对称就该同动词」统一。</para>
/// <para><b>三支无应答包体</b>（广告关闭 / 广告开通 / 同业标签管理，官方正常示例 204 No Content）⇒ 返回裸 <see cref="Task"/>； 两支状态切换（商家小票、点金计划开关）应答表同为单键 <c>sub_mchid</c> 且必填口径一致 ⇒ 共用 <see cref="GoldPlanStatusResponse"/>。</para>
/// <para><b>同业过滤标签是「覆盖」不是「追加」</b>：官方原文「如已设置同业过滤标签，再次请求传入，视为新增，<b>将覆盖原有</b>同业标签配置」， 管理支 <c>advertising_industry_filters</c> <b>必填</b>、最少 1 个最多 3 个，开通支同字段<b>选填</b> ⇒ 两支必填口径不同， 不得共用请求类型，也不得在 SDK 侧做「合并已有标签」的假象。</para>
/// <para><b>枚举值域 35 项、官方给值不给类型</b>（<c>E_COMMERCE</c> 综合电商平台 … <c>HOME_DIGITAL</c> 数码家电）⇒ 标签列表以 <c>List&lt;string&gt;</c> 承载并在注释里照页列全，<b>不建</b> C# 枚举（官方扩充值即破坏契约与序列化）。</para>
/// <para><b>前置与风控态写在业务码里</b>：官方码含「服务商未开通点金计划」「服务商<b>未配置商家小票链接</b>」「服务商<b>被处罚中</b>」 「特约商户被处罚中」「命中频率限制」「商户类型不合法 / 商户关系错误」⇒ 调用顺序是先开点金计划、再配小票链接、最后管广告与标签； 五支官方<b>均未印</b>端点级 QPS 数字。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "GoldPlan", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayGoldPlanService
{
    /// <summary>
    /// 商家小票管理。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012473788"/>（【普通服务商】【渠道商】）。
    /// </summary>
    /// <param name="request">商家小票管理请求体，字段见 <see cref="GoldPlanCustomPageStatusChangeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>商家小票管理，见 <see cref="GoldPlanStatusResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/goldplan/merchants/changecustompagestatus</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】【渠道商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="GoldPlanStatusResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：服务商未开通点金计划 / 400 INVALID_REQUEST：服务商未配置商家小票链接 / 400 INVALID_REQUEST：服务商被处罚中 / 400 INVALID_REQUEST：特约商户被处罚中 / 400 INVALID_REQUEST：操作失败，请稍后再试 / 400 MCH_NOT_EXISTS：特约商户号不存在。</para>
    /// </remarks>
    [Post("/v3/goldplan/merchants/changecustompagestatus")]
    Task<GoldPlanStatusResponse> ChangeGoldPlanCustomPageStatusAsync(
        [Body] GoldPlanCustomPageStatusChangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 点金计划管理。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012473796"/>（【普通服务商】【渠道商】）。
    /// </summary>
    /// <param name="request">点金计划管理请求体，字段见 <see cref="GoldPlanPlanStatusChangeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>点金计划管理，见 <see cref="GoldPlanStatusResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/goldplan/merchants/changegoldplanstatus</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】【渠道商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="GoldPlanStatusResponse"/>（官方应答表逐字段建模，与 <c>POST /goldplan/merchants/changecustompagestatus</c> 应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：服务商未开通点金计划 / 400 INVALID_REQUEST：服务商被处罚中 / 400 INVALID_REQUEST：特约商户被处罚中 / 400 INVALID_REQUEST：操作失败 / 400 MCH_NOT_EXISTS：特约商户号不存在。</para>
    /// </remarks>
    [Post("/v3/goldplan/merchants/changegoldplanstatus")]
    Task<GoldPlanStatusResponse> ChangeGoldPlanStatusAsync(
        [Body] GoldPlanPlanStatusChangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 关闭广告展示。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012473781"/>（【普通服务商】【渠道商】）。
    /// </summary>
    /// <param name="request">关闭广告展示请求体，字段见 <see cref="GoldPlanAdvertisingShowCloseRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段（官方应答节原文「无应答包体」/「无参数返回」，正常示例状态码 <b>204 No Content</b>） ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/goldplan/merchants/close-advertising-show</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】【渠道商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：调用方非普通服务商/渠道商 / 400 INVALID_REQUEST：服务商未开通点金计划 / 400 INVALID_REQUEST：服务商被处罚中 / 400 INVALID_REQUEST：特约商户未开通点金计划 / 400 INVALID_REQUEST：特约商户被处罚中 / 400 INVALID_REQUEST：操作失败 / 400 INVALID_REQUEST：特约商户信息逻辑错误 / 400 INVALID_REQUEST：命中频率限制 / 400 INVALID_REQUEST：商户类型不合法 / 400 INVALID_REQUEST：商户关系错误 / 400 MCH_NOT_EXISTS：特约商户号不存在。</para>
    /// </remarks>
    [Post("/v3/goldplan/merchants/close-advertising-show")]
    Task CloseGoldPlanAdvertisingShowAsync(
        [Body] GoldPlanAdvertisingShowCloseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 开通广告展示。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012473794"/>（【普通服务商】【渠道商】）。
    /// </summary>
    /// <param name="request">开通广告展示请求体，字段见 <see cref="GoldPlanAdvertisingShowOpenRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段（官方应答节原文「无应答包体」/「无参数返回」，正常示例状态码 <b>204 No Content</b>） ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PATCH</b> <c>/v3/goldplan/merchants/open-advertising-show</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】【渠道商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：调用方非普通服务商/渠道商 / 400 INVALID_REQUEST：服务商未开通点金计划 / 400 INVALID_REQUEST：服务商被处罚中 / 400 INVALID_REQUEST：特约商户未开通点金计划 / 400 INVALID_REQUEST：特约商户被处罚中 / 400 INVALID_REQUEST：操作失败 / 400 INVALID_REQUEST：特约商户信息逻辑错误 / 400 INVALID_REQUEST：命中频率限制 / 400 INVALID_REQUEST：商户类型不合法 / 400 INVALID_REQUEST：商户关系错误 / 400 MCH_NOT_EXISTS：特约商户号不存在 / 500 SYSTEM ERROR：系统繁忙，请稍后重试。</para>
    /// </remarks>
    [Patch("/v3/goldplan/merchants/open-advertising-show")]
    Task OpenGoldPlanAdvertisingShowAsync(
        [Body] GoldPlanAdvertisingShowOpenRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 同业过滤标签管理。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012473784"/>（【普通服务商】【渠道商】）。
    /// </summary>
    /// <param name="request">同业过滤标签管理请求体，字段见 <see cref="GoldPlanIndustryFilterSetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段（官方应答节原文「无应答包体」/「无参数返回」，正常示例状态码 <b>204 No Content</b>） ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/goldplan/merchants/set-advertising-industry-filter</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】【渠道商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：调用方非普通服务商/渠道商 / 400 INVALID_REQUEST：服务商未开通点金计划 / 400 INVALID_REQUEST：服务商被处罚中 / 400 INVALID_REQUEST：特约商户未开通点金计划 / 400 INVALID_REQUEST：特约商户被处罚中 / 400 INVALID_REQUEST：操作失败 / 400 MCH_NOT_EXISTS：特约商户号不存在。</para>
    /// </remarks>
    [Post("/v3/goldplan/merchants/set-advertising-industry-filter")]
    Task SetGoldPlanAdvertisingIndustryFilterAsync(
        [Body] GoldPlanIndustryFilterSetRequest request,
        CancellationToken cancellationToken = default);
}
