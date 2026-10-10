// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.ExternalContact.AcquisitionComponent;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「客户联系」模块「获客助手组件」域「代支付流水」接口族第三方应用 SDK：
/// 官方仅向第三方应用开放（企业自建应用与服务商代开发均无对应功能），唯一的 1 个端点声明于本接口。
/// </summary>
/// <remarks>
/// <para>
/// 消费 <c>suite_access_token</c>（路由键 <see cref="WechatTokenTypes.SuiteAccessToken"/>，
/// 获客助手组件的应用凭证，Query 注入）；授权企业以请求体 <c>auth_corpid</c> 参数显式指定。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>suite_access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ExternalContact",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkExternalContactAcquisitionComponentBillService))]
[Token(TokenType = WechatTokenTypes.SuiteAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "suite_access_token")]
public interface IWechatWorkThirdPartyExternalContactAcquisitionComponentBillService : IWechatWorkExternalContactAcquisitionComponentBillService
{
    /// <summary>
    /// 获取代支付流水
    /// <para>服务商获取使用量代付流水：企业通过代付产生使用量后，次日可在获客助手中查看代付的订单记录；
    /// 流水起止间隔不能超过 31 天。若需与企业侧使用量统计周期一致，
    /// 请按当天的 0 时 0 分 01 秒到第二天的 0 时 0 分 0 秒的时间戳查询。</para>
    /// <para>记录明细中的获客链接已删除时不返回 link_id，加好友 state 为空时不返回 state。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetAcquisitionBillListRequest"/>：begin_time / end_time / auth_corpid 必填，cursor / limit 可选）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>代支付流水记录列表（bill_list：timestamp / link_id / state / price）与分页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99602"/></para>
    /// </remarks>
    [Post("/cgi-bin/service/customer_acquisition/get_bill_list")]
    Task<GetAcquisitionBillListResponse> GetAcquisitionBillListAsync(
        [Body] GetAcquisitionBillListRequest request,
        CancellationToken cancellationToken = default);
}
