// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.ExternalContact.ResignedInheritance;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「客户联系」模块离职继承域公共 SDK
/// （获取待分配的离职成员列表 / 分配离职成员的客户 / 查询客户接替状态 / 分配离职成员的客户群）。
/// <para>
/// 官方对三类应用开放完全一致的 4 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalExternalContactResignedInheritanceService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyExternalContactResignedInheritanceService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderExternalContactResignedInheritanceService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：自建应用须使用配置到「可调用应用」列表中的 secret 获取的 access_token 调用；
/// 第三方 / 代开发应用须拥有「企业客户权限-&gt;客户联系-&gt;离职分配」权限；
/// 接替成员 / 新群主必须在应用可见范围内。
/// </para>
/// <para>
/// 与在职继承域（<see cref="IWechatWorkExternalContactJobInheritanceService"/>）的区别：
/// 在职继承针对未离职成员（客户群接替路由 <c>groupchat/onjob_transfer</c>），
/// 离职继承针对已离职成员（客户群接替路由 <c>groupchat/transfer</c>），二者路由与前置条件均不同。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkExternalContactResignedInheritanceService
{
    /// <summary>
    /// 获取待分配的离职成员列表
    /// <para>获取所有离职成员的客户列表，再交由「分配离职成员的客户」接口将这些客户重新分配给其他企业成员。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetUnassignedListRequest"/>：page_size / cursor 分页）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>离职成员的客户列表（info[]）、是否最后一条（is_last）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92124"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92273"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96330"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/get_unassigned_list")]
    Task<GetUnassignedListResponse> GetUnassignedListAsync(
        [Body] GetUnassignedListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 分配离职成员的客户
    /// <para>将已离职成员（原跟进成员）的部分或全部客户分配给接替成员，
    /// 发起接替后待 24 小时自动接替；逐客户返回分配结果
    /// （errcode 0 表示成功发起接替，并不代表最终接替成功，最终状态需查询客户接替状态）。</para>
    /// <para>原跟进成员离职时间不能超过 1 年且离职前一年内至少登录过一次企业微信；
    /// 接替成员最近一年内至少登录过一次企业微信、配置了客户联系功能且已激活实名；
    /// external_userid 必须是 handover_userid 的客户。</para>
    /// </summary>
    /// <param name="request">分配请求体（<see cref="ResignedTransferCustomerRequest"/>：external_userid 每次最多 100 个）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>每个客户的分配结果列表（customer[]）。</returns>
    /// <remarks>
    /// <para>原接口「分配在职或离职成员的客户」官方不再更新维护，应使用本接口。</para>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94081"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94100"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96331"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/resigned/transfer_customer")]
    Task<ResignedTransferCustomerResponse> ResignedTransferCustomerAsync(
        [Body] ResignedTransferCustomerRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询客户接替状态
    /// <para>分页查询离职成员的客户分配情况，每页返回不超过 1000 条
    /// （1 - 接替完毕；2 - 等待接替；3 - 客户拒绝；4 - 接替成员客户达到上限）。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetResignedTransferResultRequest"/>：cursor 分页）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>每个客户的接替状态列表（customer[]）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para>原接口「查询客户接替结果」官方不再更新维护，应使用本接口。</para>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94082"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94101"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96333"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/resigned/transfer_result")]
    Task<GetResignedTransferResultResponse> GetResignedTransferResultAsync(
        [Body] GetResignedTransferResultRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 分配离职成员的客户群
    /// <para>将已离职成员为群主的客户群分配给另一个企业成员（新群主）；
    /// 未能继承的群逐个返回在 failed_chat_list 中。</para>
    /// <para>群主离职了才可继承；新群主须配置了客户联系功能、已设置实名、已激活企业微信，
    /// 并在最近一年内至少登录过一次企业微信；旧群主离职时间不能超过 1 年且离职前一年内至少登录过一次企业微信；
    /// 同一个人的群每天最多分配 300 个给新群主。</para>
    /// </summary>
    /// <param name="request">分配请求体（<see cref="ResignedTransferGroupChatRequest"/>：chat_id_list 取值范围 1~100）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>没能成功继承的群列表（failed_chat_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92127"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93242"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96334"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/groupchat/transfer")]
    Task<ResignedTransferGroupChatResponse> ResignedTransferGroupChatAsync(
        [Body] ResignedTransferGroupChatRequest request,
        CancellationToken cancellationToken = default);
}
