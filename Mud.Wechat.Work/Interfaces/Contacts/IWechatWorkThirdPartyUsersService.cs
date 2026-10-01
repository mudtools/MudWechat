// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Contracts.Users;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信通讯录「成员管理」域第三方应用（Suite）SDK：除继承自 <see cref="IWechatWorkUsersService"/> 的公共读取端点外，
/// 本接口提供第三方应用的通讯录写入端点（创建/更新/删除/批量删除成员，须第三方通讯录应用）、邀请成员，
/// 以及成员授权模式专属端点（成员授权列表、授权状态查询、选人 ticket 换取用户）。
/// <para>自建应用见 <see cref="IWechatWorkInternalUsersService"/>；服务商代开发见 <see cref="IWechatWorkProviderUsersService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（经 <c>get_corp_token</c> 以 <c>permanent_code</c> 换取，路由键
/// <see cref="WechatTokenTypes.AccessToken"/>，scope = authCorpId）——调用前须经
/// <c>IWechatAppContextSwitcher</c> 切换到目标授权企业作用域。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Contact",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkUsersService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartyUsersService : IWechatWorkUsersService
{
    /// <summary>
    /// 创建成员
    /// <para>向授权企业通讯录写入新成员。userid 与 name 必填，mobile/email 不能同时为空。</para>
    /// <para>官方权限口径：「仅通讯录同步助手或第三方通讯录应用可调用」——第三方须为通讯录应用且拥有通讯录写权限。</para>
    /// </summary>
    /// <param name="request">成员请求体（<see cref="CreateUserRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>创建结果（含因填写不存在部门而自动新建的部门列表）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90331"/></para>
    /// </remarks>
    [Post("/cgi-bin/user/create")]
    Task<CreateUserResponse> CreateUserAsync(
        [Body] CreateUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新成员
    /// <para>更新授权企业通讯录中的既有成员。除 userid 外全部字段可选，未传入的字段不更新。</para>
    /// <para>官方权限口径：「仅通讯录同步助手或第三方通讯录应用可调用」。</para>
    /// </summary>
    /// <param name="request">成员请求体（<see cref="UpdateUserRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90333"/></para>
    /// </remarks>
    [Post("/cgi-bin/user/update")]
    Task<WechatWorkResponse> UpdateUserAsync(
        [Body] UpdateUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除成员
    /// <para>从授权企业通讯录中删除成员。若是绑定了腾讯企业邮，则会同时删除邮箱账号。</para>
    /// </summary>
    /// <param name="userid">成员 UserID，对应管理端的账号。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90334"/></para>
    /// </remarks>
    [Get("/cgi-bin/user/delete")]
    Task<WechatWorkResponse> DeleteUserAsync(
        [Query("userid")] string userid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除成员
    /// <para>批量从授权企业通讯录中删除成员。若存在无效 UserID，直接返回错误（整批失败）。</para>
    /// </summary>
    /// <param name="request">请求体（useridlist：最多 200 个成员 UserID）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90335"/></para>
    /// </remarks>
    [Post("/cgi-bin/user/batchdelete")]
    Task<WechatWorkResponse> BatchDeleteUsersAsync(
        [Body] BatchDeleteUsersRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 邀请成员
    /// <para>邀请成员（企业成员或未安装企业微信的成员）使用企业微信，user/party/tag 三者不能同时为空。第三方仅通讯录应用可调用。</para>
    /// <para>与自建应用版本（<see cref="IWechatWorkInternalUsersService.InviteMembersAsync"/>）路由相同、契约一致，
    /// 但令牌作用域不同（授权企业级），故按应用类型分别声明。</para>
    /// </summary>
    /// <param name="request">请求体（user 成员 ID 列表 ≤ 1000；party 部门 ID 列表 ≤ 100；tag 标签 ID 列表 ≤ 100）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>邀请结果（invaliduser/invalidparty/invalidtag 非法列表）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91127"/></para>
    /// </remarks>
    [Post("/cgi-bin/batch/invite")]
    Task<InviteMembersResponse> InviteMembersAsync(
        [Body] InviteMembersRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取成员授权列表
    /// <para>当企业当前授权模式为「成员授权」时，分页获取已授权成员的 open_userid 列表。</para>
    /// </summary>
    /// <param name="request">分页请求体（cursor 首次不填；limit 默认与最大值均为 1000）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成员授权列表（member_auth_list）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94513"/></para>
    /// </remarks>
    [Post("/cgi-bin/user/list_member_auth")]
    Task<ListMemberAuthResponse> ListMemberAuthAsync(
        [Body] ListMemberAuthRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询成员用户是否已授权
    /// <para>当企业当前授权模式为「成员授权」时，查询指定成员是否已授权。</para>
    /// </summary>
    /// <param name="request">请求体（open_userid：企业成员的全局唯一标识）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>授权状态（is_member_auth）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94514"/></para>
    /// </remarks>
    [Post("/cgi-bin/user/check_member_auth")]
    Task<CheckMemberAuthResponse> CheckMemberAuthAsync(
        [Body] CheckMemberAuthRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取选人 ticket 对应的用户
    /// <para>当企业以「成员授权」方式安装第三方应用时，成员经选人 JS-SDK 选择通讯录后，以 selectedTicket 换取对应成员的 open_userid 列表。</para>
    /// </summary>
    /// <param name="request">请求体（selected_ticket：选人 JS-SDK 返回的票据）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>选人结果（操作者与可见范围内外 open_userid 列表、总人数）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94894"/></para>
    /// </remarks>
    [Post("/cgi-bin/user/list_selected_ticket_user")]
    Task<ListSelectedTicketUserResponse> ListSelectedTicketUsersAsync(
        [Body] ListSelectedTicketUserRequest request,
        CancellationToken cancellationToken = default);
}
