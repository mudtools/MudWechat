// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Approval;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「审批」模块审批申请数据域企业自建应用 SDK。
/// <para>
/// 官方对自建应用开放与三类应用公共面一致的 3 个端点（继承自
/// <see cref="IWechatWorkApprovalService"/>），并额外开放 1 个差异端点：
/// 「获取审批数据（旧）」（官方仅自建开放，第三方/代开发文档未提供该端点）。
/// </para>
/// <para>服务商代开发见 <see cref="IWechatWorkProviderApprovalService"/>；
/// 第三方应用见 <see cref="IWechatWorkThirdPartyApprovalService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 自建应用须配置到「审批 - 可调用接口的应用」中；
/// 自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Approval",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkApprovalService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalApprovalService : IWechatWorkApprovalService
{
    /// <summary>
    /// 获取审批数据（旧）
    /// <para>获取公司一段时间内的审批记录（旧版数据格式，含请假/报销模板的结构化数据与自定义模板的 apply_data 数据）。</para>
    /// <para>官方限制：一次拉取调用最多拉取 100 个审批记录，超过 100 条请使用 next_spnum 进行分页拉取；
    /// endtime 需大于 starttime，同时起始时间跨度不要超过 30 天；无法获取管理员已删除的审批单数据；
    /// <b>官方推荐使用新接口「批量获取审批单号」及「获取审批申请详情」，此接口后续将不再维护、逐步下线</b>。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetApprovalDataRequest"/>：starttime / endtime / next_spnum）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>审批记录列表（data：spname / apply_name / approval_name / sp_status / sp_num / leave / expense / comm 等）与分页信息（count / total / next_spnum）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91530"/></para>
    /// <para>官方权限：仅自建应用开放；接口频率限制 600 次/分。</para>
    /// </remarks>
    [Post("/cgi-bin/corp/getapprovaldata")]
    Task<GetApprovalDataResponse> GetApprovalDataAsync(
        [Body] GetApprovalDataRequest request,
        CancellationToken cancellationToken = default);
}
