// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Approval;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「审批」模块审批流程引擎域公共 SDK（查询审批单当前状态，单端点收敛面）。
/// <para>
/// 审批流程引擎可将审批流程相关功能嵌入到自建应用/第三方应用中：开发者可在应用内通过 JS-SDK 发起审批申请
/// （thirdNo 为开发者自定义审批单号），系统按配置的审批流程自动通知相关人员进行审批操作，
/// 每次审批状态变化通过「审批状态变化通知回调」（open_approval_change 事件）推送开发者；
/// 本接口族承载其唯一的 HTTP 查询端点——按 thirdNo 主动查询审批单当前状态。
/// 官方对三类应用开放一致的本端点，收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 企业自建应用见 <see cref="IWechatWorkInternalApprovalEngineService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderApprovalEngineService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyApprovalEngineService"/>。
/// 注意：审批流程引擎的审批单据体系（thirdNo / OpenTemplateId）与企业微信「审批应用」的单据体系（sp_no / template_id）相互独立。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）——
/// 自建应用消费应用自身 access_token；第三方/服务商代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。官方权限口径：自建应用须配置到
/// 「审批 - 可调用接口的应用」中；代开发应用与第三方应用须具有「审批」权限。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkApprovalEngineService
{
    /// <summary>
    /// 查询审批单当前状态
    /// <para>开发者按发起申请时自定义的审批单号（thirdNo）主动查询审批流程引擎审批单的当前审批状态。</para>
    /// <para>官方限制：审批状态变化的实时感知应使用「审批状态变化通知回调」（open_approval_change 事件），
    /// 本端点用于主动查询兜底。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetOpenApprovalDataRequest"/>：thirdNo 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>审批单当前状态（data：ThirdNo / OpenTemplateId / OpenSpName / OpenSpstatus / ApplyTime / ApprovalNodes / NotifyNodes / ApproverStep）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90269"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93798"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97114"/></para>
    /// <para>官方权限：自建应用须配置到「审批 - 可调用接口的应用」中；代开发应用与第三方应用须具有「审批」权限。</para>
    /// </remarks>
    [Post("/cgi-bin/corp/getopenapprovaldata")]
    Task<GetOpenApprovalDataResponse> GetOpenApprovalDataAsync(
        [Body] GetOpenApprovalDataRequest request,
        CancellationToken cancellationToken = default);
}
