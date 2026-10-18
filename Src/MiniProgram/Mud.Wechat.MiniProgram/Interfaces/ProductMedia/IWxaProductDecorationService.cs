// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.MiniProgram.DataModels.ProductMedia;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「基础交易工具（原标准版交易组件 V1）」·装修服务域（6 端点：服务启停 / 体验版 /
/// 状态查询 / 发起压测 / 压测状态 / 压测报告；路由族 <c>/product/decoration/*</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>服务商专用面</b>：6 页均面向第三方服务商替小商店装修；状态查询只认当前
/// <c>access_token</c> 对应的服务商与小商店 appid，不能跨服务商复用结果。
/// </para>
/// <para>
/// <b>启停需管理员确认</b>：<c>decoration_service</c> 调用后会下发模板消息给小商店管理员，
/// 管理员确认后方可发布 ⇒ 返回成功不代表已生效，须轮询 <c>check_status</c> 复核。
/// </para>
/// <para>
/// <b>压测配额</b>：每个小商店每月 4 次，压测系统以 200qps 打开首页 ⇒ 配额耗尽返回 10037，
/// 非重试可解。
/// </para>
/// <para>
/// <b>官方页面 JSON 示例畸形（照录）</b>：<c>check_status</c> / <c>query_pressure_test_status</c>
/// 与压测报告三页的回包示例均使用中文弯引号且缺逗号 ⇒ SDK 按参数表建模，示例不可直接解析。
/// </para>
/// <para>
/// <b>调用限制（诚实留档）</b>：本族 6 页官方正文未给端点级频率数字与权限集编号
/// （2026-10-18 逐页取证）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ProductMedia", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaProductDecorationService
{
    /// <summary>
    /// 启用 / 停用装修服务（<c>POST /product/decoration/decoration_service</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/service/decoration/decoration_service.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>service_id</c> + <c>switch_status</c>），见 <see cref="WxaProductDecorationServiceRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>两段式生效</b>：微信下发模板消息给管理员、管理员确认后才发布 ⇒ 本方法只表达「请求已受理」；
    /// 生效判定须轮询 <see cref="CheckDecorationStatusAsync"/>。
    /// </para>
    /// <para><b>状态语义</b>：<c>switch_status</c> 取 1 启用 / 2 停用，不是布尔。</para>
    /// </remarks>
    [Post("/product/decoration/decoration_service")]
    Task<WxaResponse> SetDecorationServiceAsync(
        [Body] WxaProductDecorationServiceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 打开装修体验版（<c>POST /product/decoration/decoration_service_experience</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/service/decoration/decoration_service_experience.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>service_id</c>），见 <see cref="WxaProductDecorationServiceExperienceRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>不影响线上</b>：默认小商店所有相关人员均可打开体验版，体验版无缓存、不影响线上版本
    /// （官方原文）⇒ 可安全重复调用。
    /// </para>
    /// </remarks>
    [Post("/product/decoration/decoration_service_experience")]
    Task<WxaResponse> OpenDecorationServiceExperienceAsync(
        [Body] WxaProductDecorationServiceExperienceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询装修服务状态（<c>POST /product/decoration/wxaproduct_decoration_check_status</c>，官方「查询」类端点仍是 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/service/decoration/decoration_check_status.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>service_id</c>），见 <see cref="WxaProductDecorationCheckStatusRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>status</c>（1 已启用 / 2 未启用），见 <see cref="WxaProductDecorationCheckStatusResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>错误码与业务字段混排（照录）</b>：10027 装修服务不存在 / 10028 不属于该服务商 /
    /// 10030 获取小商店信息失败，均经 <c>errcode</c> 返回。
    /// </para>
    /// <para><b>官方回包示例 JSON 畸形</b>：弯引号 + 缺逗号 ⇒ 按参数表建模。</para>
    /// </remarks>
    [Post("/product/decoration/wxaproduct_decoration_check_status")]
    Task<WxaProductDecorationCheckStatusResponse> CheckDecorationStatusAsync(
        [Body] WxaProductDecorationCheckStatusRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发起压测（<c>POST /product/decoration/wxaproduct_decoration_pressure_test</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/service/decoration/decoration_pressure_test.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>顶层 <c>pressure_id</c>，见 <see cref="WxaProductDecorationPressureTestStartResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>无请求参数</b>：官方请求示例为 <c>{ }</c>、参数表为空 ⇒ 本方法不接收请求 DTO。
    /// </para>
    /// <para>
    /// <b>配额与速率</b>：每小商店每月 4 次，压测以 200qps 打开首页；配额耗尽返回 10037。
    /// </para>
    /// <para>
    /// <b>非幂等</b>：每次成功都占用一次配额并产生新 <c>pressure_id</c> ⇒ 不得自动重试。
    /// </para>
    /// </remarks>
    [Post("/product/decoration/wxaproduct_decoration_pressure_test")]
    Task<WxaProductDecorationPressureTestStartResponse> StartPressureTestAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询压测状态（<c>POST /product/decoration/wxaproduct_decoration_query_pressure_test_status</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/service/decoration/decoration_query_pressure_test_status.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>pressure_id</c>），见 <see cref="WxaProductDecorationPressureTestStatusRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>status</c>（1 排队 / 2 压测中 / 3 失败 / 4 结束），见 <see cref="WxaProductDecorationPressureTestStatusResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>越权保护</b>：只能查询该小程序提交压测返回的 <c>pressure_id</c>，其它 id 按 10007 处理。
    /// </para>
    /// </remarks>
    [Post("/product/decoration/wxaproduct_decoration_query_pressure_test_status")]
    Task<WxaProductDecorationPressureTestStatusResponse> GetPressureTestStatusAsync(
        [Body] WxaProductDecorationPressureTestStatusRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取压测报告（<c>POST /product/decoration/wxaproduct_decoration_get_pressure_test_report</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/service/decoration/decoration_get_pressure_test_report.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>pressure_id</c>），见 <see cref="WxaProductDecorationPressureTestReportRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>白页率 / 耗时 / 打开次数 / 网络明细等，见 <see cref="WxaProductDecorationPressureTestReportResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>仅在压测结束后可取</b>：10003 表示「获取压测报告失败，请检查压测状态」⇒ 须先经
    /// <see cref="GetPressureTestStatusAsync"/> 确认 <c>status = 4</c>。
    /// </para>
    /// <para>
    /// <b>字段拼写错误照录</b>：<c>blankpage_pencent</c>（官方原文，应为 percent）不做更正，
    /// 否则反序列化失配。
    /// </para>
    /// <para><b>官方回包示例 JSON 畸形</b>：弯引号 + 缺逗号 ⇒ 按参数表建模。</para>
    /// </remarks>
    [Post("/product/decoration/wxaproduct_decoration_get_pressure_test_report")]
    Task<WxaProductDecorationPressureTestReportResponse> GetPressureTestReportAsync(
        [Body] WxaProductDecorationPressureTestReportRequest request,
        CancellationToken cancellationToken = default);
}
