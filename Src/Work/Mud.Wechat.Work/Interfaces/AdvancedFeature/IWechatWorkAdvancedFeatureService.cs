// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.AdvancedFeature;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「高级功能（成员申请单）」域公共父接口（官方 99880 / 99883 / 99885）。
/// <para>
/// 企业成员向管理员申请「高级账号」（邮件 / 文档 / 微盘 / 会议）的申请单面：
/// 通过 API 设置申请流程、批量拉取申请单 id、获取申请单详情，共 3 支端点。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// <b>应用类型开放面（官方逐页明文标注）</b>：三支端点的应用类型表均为
/// 「自建应用：配置到『高级功能 - 成员申请管理 - 通过 api 设置申请流程』中；
/// 代开发应用：暂不支持；第三方应用：暂不支持」⇒ 本域<b>只落一个自建子接口</b>
/// <see cref="IWechatWorkInternalAdvancedFeatureService"/>，第三方 / 代开发形态<b>零端点</b>，
/// 由本域契约守卫锁定不得回潮。
/// </para>
/// <para>
/// 令牌：本域走企业自建应用 <c>access_token</c>（Query，官方契约），归属域键由自建子接口声明
/// （<c>WechatTokenManagerKeys.InternalAccessToken</c>，TO 系列锁定）。
/// </para>
/// <para>
/// <b>官方反直觉点</b>：① 三支端点<b>全部为 POST</b>，含两支查询接口；
/// ② 设置面的 <c>approval_status</c> 值域为 1/2/3/101，查询面的 <c>approval_status</c>
/// 值域为 0/1/2/3/4/5（多了「管理员驳回 / 管理员已分配 / 历史无流程记录」）⇒ 两套状态<b>不同名同义</b>，
/// SDK 不归一、照抄各自原文；
/// ③ 本域 <c>has_more</c> 为布尔，而「高级接口订单列表」的 <c>has_more</c> 为整型 0/1。
/// </para>
/// <para>
/// <b>个人信息约束</b>：<c>userid</c> / <c>applicant</c> / <c>current_approvers</c> /
/// <c>completed_approvers</c> 为成员身份，<c>request_reason</c> 为成员填写的自由文本申请理由，
/// 均不得写入日志、遥测或异常消息。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkAdvancedFeatureService
{
    /// <summary>
    /// 设置高级功能申请单审批信息
    /// <para>自建应用通过本接口把「成员申请单」的审批流程托管给自身审批系统：写入审批状态、
    /// 跳转链接与审批节点列表。</para>
    /// <para>
    /// <b>官方必填</b>：<c>apply_id</c>、<c>approval_id</c>、<c>approval_status</c>、<c>approval_url</c>
    /// （须以 <c>http://</c> 或 <c>https://</c> 开头）、<c>process_list.node_list</c>。
    /// </para>
    /// <para>
    /// <b>覆盖式写入（危险操作）</b>：官方明确「如果需要变更审批节点信息，需要<b>全量</b>节点都传入」
    /// ⇒ 只传部分节点等于删除其余节点；且审批 id 与申请 id 一一对应、<b>不可改变</b>。
    /// </para>
    /// <para>
    /// <b>官方节点约束</b>：节点状态值域 1 审批中 / 2 已驳回 / 3 已同意 / 101 已撤销 / 102 未到流程；
    /// 多人审批方式 1 会签 / 2 或签 / 3 依次审批；待处理人与已处理人列表各最多 100 个且
    /// <b>不可同时为空</b>。
    /// </para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetAdvancedFeatureApprovalDetailRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅含 errcode / errmsg（官方无业务负载，故不新建空响应 DTO）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99880"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/advanced_feature/set_approval_detail")]
    Task<WechatWorkResponse> SetApprovalDetailAsync(
        [Body] SetAdvancedFeatureApprovalDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量获取高级功能申请单 id
    /// <para>按成员与应用类型批量拉取该成员的高级功能申请单 id 列表，供后续逐个取详情。</para>
    /// <para>
    /// <b>官方必填</b>：<c>business_type</c>（1 邮件 / 2 文档 / 3 微盘 / 4 会议）、<c>userid</c>。
    /// </para>
    /// <para>
    /// <b>官方分页陷阱</b>：<c>limit</c> 默认 100、最大 200，且<b>不保证</b>每次返回条数刚好等于 limit
    /// ⇒ 必须用 <c>has_more</c>（本域为<b>布尔</b>）判断是否继续请求、并带上 <c>next_cursor</c>；
    /// <c>req_type</c> 0 所有 / 1 仅 api 单 / 2 非 api 申请单，默认 0。
    /// </para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetAdvancedFeatureApplyIdListRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>申请单 id 列表（next_cursor / apply_id_list / has_more）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99883"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/advanced_feature/get_apply_id_list")]
    Task<GetAdvancedFeatureApplyIdListResponse> GetApplyIdListAsync(
        [Body] GetAdvancedFeatureApplyIdListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取高级功能申请单详情
    /// <para>按 <c>apply_id</c> 取申请单的申请成员、类型、状态、审批 id、跳转链接与申请原因。</para>
    /// <para>
    /// <b>官方状态值域与设置面不同</b>：此处 <c>approval_status</c> 为 0 历史申请记录（无流程信息）、
    /// 1 审批人审批中、2 审批人驳回、3 审批人已同意、4 管理员驳回、5 管理员已分配；
    /// <c>approval_type</c> 为 0 默认申请 / 1 自定义审批 / 2 企业 openapi。
    /// </para>
    /// <para><c>request_reason</c> 为成员填写的自由文本，SDK 不解析、不改写，宿主不得落日志。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetAdvancedFeatureApprovalInfoRequest"/>：apply_id 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>申请单详情（approval_info）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99885"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/advanced_feature/get_approval_info")]
    Task<GetAdvancedFeatureApprovalInfoResponse> GetApprovalInfoAsync(
        [Body] GetAdvancedFeatureApprovalInfoRequest request,
        CancellationToken cancellationToken = default);
}
