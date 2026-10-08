// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Pay;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「企业支付」模块资金流水域企业自建应用 SDK（获取资金流水）。
/// <para>
/// 官方仅向自建应用开放本域 1 个端点（代开发应用与第三方应用均暂不支持），
/// 声明于本接口（形态对齐 <see cref="IWechatWorkInternalKfKnowledgeService"/>）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 自建应用须配置到「对外收款 - 可调用接口的应用」中；仅返回在企业微信开通的商户号资金流水。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Pay",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkPayFundFlowService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalPayFundFlowService : IWechatWorkPayFundFlowService
{
    /// <summary>
    /// 获取资金流水
    /// <para>获取企业微信开通商户号的资金流水记录（动账明细）。</para>
    /// <para>官方业务限制：可拉取不早于 2022 年 12 月 1 日的记录（最长保留 3 年）；
    /// 起止时间间隔不能超过 31 天；当日流水在次日上午 11 点后生成；
    /// 会过滤操作人不在应用可见范围内的记录，返回条数可能小于 limit。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetPayFundFlowRequest"/>：
    /// begin_time / end_time（间隔 ≤ 31 天）/ mch_id / cursor / limit（≤ 200））。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>资金流水记录列表（fund_flow_list）与分页游标（next_cursor，无更多数据时不返回）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98100"/></para>
    /// <para>官方业务限制：代开发应用、第三方应用均暂不支持本接口。</para>
    /// </remarks>
    [Post("/cgi-bin/externalpay/get_fund_flow")]
    Task<GetPayFundFlowResponse> GetFundFlowAsync(
        [Body] GetPayFundFlowRequest request,
        CancellationToken cancellationToken = default);
}
