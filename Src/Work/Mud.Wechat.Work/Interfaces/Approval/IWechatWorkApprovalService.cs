// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Approval;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「审批」模块审批申请数据域公共 SDK
/// （提交审批申请 + 批量获取审批单号 + 获取审批申请详情，三端点收敛面）。
/// <para>
/// 官方对三类应用开放一致的 3 个端点，全部收敛声明于本接口；应用类型子接口承载官方开放面差异端点：
/// 企业自建应用见 <see cref="IWechatWorkInternalApprovalService"/>（额外开放「获取审批数据（旧）」），
/// 服务商代开发见 <see cref="IWechatWorkProviderApprovalService"/>（零差异端点空标记），
/// 第三方应用见 <see cref="IWechatWorkThirdPartyApprovalService"/>（零差异端点空标记）。
/// 审批模板族见 <see cref="IWechatWorkApprovalTemplateService"/>、假期管理族见 <see cref="IWechatWorkVacationService"/>、
/// 审批流程引擎族见 <see cref="IWechatWorkApprovalEngineService"/> 接口族。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）——
/// 自建应用消费应用自身 access_token；第三方/服务商代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。官方权限口径：自建应用须配置到
/// 「审批 - 可调用接口的应用」中；代开发应用与第三方应用须具有「审批」权限。
/// 自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkApprovalService
{
    /// <summary>
    /// 提交审批申请
    /// <para>调用接口以申请人在企业微信「审批应用」内提交审批申请（按 template_id 对应的审批模板提单，
    /// 可通过 process 指定审批流程或复用模板在管理后台配置的审批流程）。</para>
    /// <para>官方限制：接口频率限制 600 次/分钟；当模板的控件为必填属性时，表单中对应的控件必须有值；
    /// 暂不支持通过接口提交【打卡补卡】【调班】模板审批单。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ApplyApprovalRequest"/>：creator_userid / template_id / use_template_approver / choose_department / process / apply_data / summary_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>表单提交成功后返回的表单编号（sp_no）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91853"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92632"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96507"/></para>
    /// <para>官方权限：自建应用须配置到「审批 - 可调用接口的应用」中；代开发应用与第三方应用须具有「审批」权限。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/applyevent")]
    Task<ApplyApprovalResponse> ApplyApprovalAsync(
        [Body] ApplyApprovalRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量获取审批单号
    /// <para>获取企业一段时间内企业微信「审批应用」单据的审批编号，支持按模板类型、申请人、部门、申请单审批状态等条件筛选。</para>
    /// <para>官方限制：一次拉取调用最多拉取 100 个审批记录，可通过多次拉取的方式满足需求，调用频率不可超过 600 次/分；
    /// endtime 需大于 starttime，起始时间跨度不能超过 31 天；自建应用仅允许获取应用可见范围内申请人提交的表单；
    /// 老的分页游标字段 cursor 和 next_cursor 待废弃，请使用新字段 new_cursor 和 new_next_cursor。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="BatchGetApprovalNumbersRequest"/>：starttime / endtime / new_cursor / size / filters）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>审批单号列表（sp_no_list）与后续请求查询的游标（new_next_cursor，缺失时表示已拉取完）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91816"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94603"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96509"/></para>
    /// <para>官方权限：自建应用须配置到「审批 - 可调用接口的应用」中；代开发应用与第三方应用须具有「审批」权限。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/getapprovalinfo")]
    Task<BatchGetApprovalNumbersResponse> BatchGetApprovalNumbersAsync(
        [Body] BatchGetApprovalNumbersRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取审批申请详情
    /// <para>按审批单编号获取审批申请详情（申请单状态、审批流程记录、抄送信息、申请数据控件值、备注信息与流程列表）。</para>
    /// <para>官方限制：接口频率限制 600 次/分钟。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetApprovalDetailRequest"/>：sp_no 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>审批申请详情（info：sp_no / sp_name / sp_status / template_id / apply_time / applyer / sp_record / notifyer / apply_data / comments / process_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91983"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92634"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96510"/></para>
    /// <para>官方权限：自建应用须配置到「审批 - 可调用接口的应用」中；代开发应用与第三方应用须具有「审批」权限。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/getapprovaldetail")]
    Task<GetApprovalDetailResponse> GetApprovalDetailAsync(
        [Body] GetApprovalDetailRequest request,
        CancellationToken cancellationToken = default);
}
