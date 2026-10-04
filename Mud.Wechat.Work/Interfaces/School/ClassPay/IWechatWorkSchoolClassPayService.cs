// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.School.ClassPay;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「家校沟通」模块班级收款域公共 SDK。
/// <para>
/// 官方向企业自建应用与第三方应用开放完全一致的 2 个端点（获取学生付款结果、获取订单详情），
/// 全部收敛声明于本接口；服务商代开发官方未开放服务端查询接口（官方代开发文档「班级收款」
/// 仅目录页、无服务端 API），故不声明代开发子接口
/// （形态对齐家校管理配置域「官方仅自建 + 第三方」的开放面收敛模式）：
/// 自建见 <see cref="IWechatWorkInternalSchoolClassPayService"/>，
/// 第三方见 <see cref="IWechatWorkThirdPartySchoolClassPayService"/>。
/// </para>
/// <para>
/// 家校应用域其它功能族：<see cref="IWechatWorkSchoolService"/>（基础域）、
/// <see cref="IWechatWorkSchoolSettingService"/>（家校管理配置域）、
/// <see cref="IWechatWorkSchoolUserService"/>（学生与家长管理域）、
/// <see cref="IWechatWorkSchoolDepartmentService"/>（部门管理域）、
/// <see cref="IWechatWorkSchoolAuthService"/>（网页授权登录域）、
/// <see cref="IWechatWorkSchoolHealthReportService"/>（健康上报域）、
/// <see cref="IWechatWorkSchoolLivingService"/>（上课直播域）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 两类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：官方仅声明「只允许获取由应用本身创建的收款项目详情 / 订单详情」（应用隔离，跨应用不可查）。
/// 发起班级收款为 JS-SDK / 小程序客户端能力，不属于本域服务端 API。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkSchoolClassPayService
{
    /// <summary>
    /// 获取学生付款结果
    /// <para>获取班级收款项目的学生付款结果列表。</para>
    /// <para>官方业务限制：只允许获取由应用本身创建的收款项目详情；
    /// payment_id 由 jssdk 的发起班级收款接口或小程序的发起班级收款接口返回。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ClassPayGetPaymentResultRequest"/>，payment_id 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>收款项目信息（project_name / amount）与学生付款列表（payment_result）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94470"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94553"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/get_payment_result")]
    Task<ClassPayGetPaymentResultResponse> GetPaymentResultAsync(
        [Body] ClassPayGetPaymentResultRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取订单详情
    /// <para>获取班级收款项目指定订单的详情（微信交易单号与交易时间）。</para>
    /// <para>官方业务限制：只允许获取由应用本身创建的收款项目的订单详情；
    /// trade_no 由获取学生付款结果接口返回。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ClassPayGetTradeRequest"/>，payment_id 与 trade_no 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单详情（transaction_id / pay_time）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94471"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94554"/></para>
    /// </remarks>
    [Post("/cgi-bin/school/get_trade")]
    Task<ClassPayGetTradeResponse> GetTradeAsync(
        [Body] ClassPayGetTradeRequest request,
        CancellationToken cancellationToken = default);
}
