// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Pay;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「企业支付」模块交易账单域企业自建应用 SDK（交易账单申请）。
/// <para>
/// 官方仅向自建应用开放本域 1 个端点（代开发应用与第三方应用均暂不支持），
/// 声明于本接口（形态对齐 <see cref="IWechatWorkInternalPayFundFlowService"/>）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。
/// 官方权限口径：须先在企微管理端开通对外收款能力，商户号为「由企业微信生成并下发」的二级商户号。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Pay",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkPayTradeBillService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalPayTradeBillService : IWechatWorkPayTradeBillService
{
    /// <summary>
    /// 交易账单申请
    /// <para>申请指定商户号、指定日期的交易账单文件下载信息。</para>
    /// <para>官方业务限制：仅支持三个月内的账单下载申请；返回的下载地址 30 秒内有效；
    /// 下载账单文件时须以响应中的 auth 作为 https 校验的 Authorization 请求头；
    /// 账单文件含明细与汇总四部分，字段以英文逗号分隔。</para>
    /// </summary>
    /// <param name="request">申请请求体（<see cref="GetPayTradeBillRequest"/>：
    /// bill_date（yyyy-MM-dd）/ mchid，bill_type 与 tar_type 可选）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>账单下载信息（hash_type / hash_value / download_url / auth）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98115"/></para>
    /// <para>官方业务限制：代开发应用、第三方应用均暂不支持本接口。</para>
    /// </remarks>
    [Post("/cgi-bin/miniapppay/get_bill")]
    Task<GetPayTradeBillResponse> GetTradeBillAsync(
        [Body] GetPayTradeBillRequest request,
        CancellationToken cancellationToken = default);
}
