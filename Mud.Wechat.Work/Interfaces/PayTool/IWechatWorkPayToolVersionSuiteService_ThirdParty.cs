// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.PayTool;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「收银台」模块应用版本付费域第三方应用 SDK
/// （获取订单列表 / 获取订单详情 / 延长试用期）。
/// <para>官方仅在第三方应用开发文档树开放本族端点，全部 3 个端点声明于本接口；
/// 企业自建应用与服务商代开发官方不开放，不设对应子接口。</para>
/// <para>官方「获取企业永久授权码 / 获取企业授权信息」与授权流接口族为同一端点，已在
/// <see cref="IWechatWorkProviderAuthenticationService"/> 承载，本接口不重复声明。</para>
/// </summary>
/// <remarks>
/// <para>消费服务商套件级 suite_access_token（路由键 <see cref="WechatTokenTypes.SuiteAccessToken"/>）。</para>
/// <para>官方约束：订单事件的回调通知（应用版本回调通知族：下单成功 / 改单 / 支付成功 / 退款 /
/// 应用版本变更 / 取消订单）经<b>指令回调 URL</b> 推送到套件通道，回调事件键见
/// <c>WechatCallbackEventTypes</c>。</para>
/// <para>官方页面未给出本族端点的独立频率限制，走官方全局访问频率限制。</para>
/// <para>MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（suite_access_token），无法改用 Header。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "PayTool",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkPayToolVersionSuiteService))]
[Token(TokenType = WechatTokenTypes.SuiteAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "suite_access_token")]
public interface IWechatWorkThirdPartyPayToolVersionSuiteService : IWechatWorkPayToolVersionSuiteService
{
    /// <summary>
    /// 获取订单列表
    /// <para>查询指定时间范围内的应用版本付费订单列表。</para>
    /// <para>官方约束：start_time / end_time 为官方必填（UNIX 时间戳）；
    /// test_mode 指定拉取正式或测试授权的订单，默认值 0（0-正式授权，1-测试授权）。</para>
    /// <para>官方说明：本端点无分页游标，一次返回时间窗内的订单列表。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetPayToolVersionOrderListRequest"/>：start_time 起始时间 /
    /// end_time 终止时间 / test_mode 正式或测试授权订单）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单列表（order_list）。
    /// <para>列表项含 orderid / order_status / order_type / paid_corpid / operator_id / suiteid / appid /
    /// edition_id / edition_name / price / user_count / order_period / order_time / paid_time /
    /// begin_time / end_time / order_from / operator_corpid / service_share_amount /
    /// platform_share_amount / dealer_share_amount / dealer_corp_info。</para></returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91910">path 91910 获取订单列表</see></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/service/get_order_list")]
    Task<GetPayToolVersionOrderListResponse> GetVersionOrderListAsync(
        [Body] GetPayToolVersionOrderListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取订单详情
    /// <para>查询指定订单的详情；订单字段平铺在响应根级（与订单列表元素同构）。</para>
    /// <para>官方契约陷阱：请求参数名为全小写 <c>orderid</c>（非 <c>order_id</c>），本 SDK 照抄官方原文。</para>
    /// <para>官方说明：改单（服务商管理员修改订单价格）会产生<b>新的订单号</b>，
    /// 服务商须用新的订单号来查询订单详情以及关联授权应用。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetPayToolVersionOrderDetailRequest"/>：orderid 订单号）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单详情（订单字段平铺于响应根级，见 <see cref="PayToolVersionOrder"/>）。
    /// <para>官方口径：operator_id 在服务商代下单、服务商下的免支付订单等情形没有该字段；
    /// dealer_corp_info 仅当有渠道商报备后才会有此字段。</para></returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91909">path 91909 获取订单详情</see></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/service/get_order")]
    Task<GetPayToolVersionOrderDetailResponse> GetVersionOrderDetailAsync(
        [Body] GetPayToolVersionOrderDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 延长试用期
    /// <para>延长指定购买方企业的试用有效期。</para>
    /// <para>官方约束：buyer_corpid / prolong_days 为官方必填；appid 仅旧套件需要填。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ProlongPayToolVersionTrialRequest"/>：buyer_corpid 购买方 corpid /
    /// prolong_days 延长天数 / appid 套件应用 id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>延长后的试用到期时间（try_end_time，UNIX 时间戳）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91913">path 91913 延长试用期</see></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/service/prolong_try")]
    Task<ProlongPayToolVersionTrialResponse> ProlongTrialAsync(
        [Body] ProlongPayToolVersionTrialRequest request,
        CancellationToken cancellationToken = default);
}