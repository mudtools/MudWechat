// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.ExternalContact.JobInheritance;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「客户联系」模块在职继承域公共 SDK（分配在职成员的客户 / 查询客户接替状态 / 分配在职成员的客户群）。
/// <para>
/// 官方对三类应用开放完全一致的 3 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalExternalContactJobInheritanceService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyExternalContactJobInheritanceService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderExternalContactJobInheritanceService"/>。
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
/// 第三方 / 代开发应用须拥有「企业客户权限-&gt;客户联系-&gt;在职继承」（客户接替）与
/// 「企业客户权限-&gt;客户联系-&gt;分配在职成员的客户群」（客户群接替）权限；
/// 接替成员 / 新群主必须在应用可见范围内。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkExternalContactJobInheritanceService
{
    /// <summary>
    /// 分配在职成员的客户
    /// <para>将原跟进成员（在职）的部分或全部客户分配给接替成员，发起接替后待 24 小时自动接替；
    /// 逐客户返回分配结果（errcode 0 表示成功发起接替，并不代表最终接替成功，最终状态需查询客户接替状态）。</para>
    /// <para>接替成员须在应用可见范围内、配置了客户联系功能且已激活实名；
    /// 原跟进成员和接替成员在最近一年内需登录过至少一次企业微信；
    /// 90 个自然日内在职成员的每位客户仅可被转接 2 次。</para>
    /// </summary>
    /// <param name="request">分配请求体（<see cref="TransferCustomerRequest"/>：external_userid 每次最多 100 个）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>每个客户的分配结果列表（customer[]）。</returns>
    /// <remarks>
    /// <para>原接口「分配在职或离职成员的客户」官方不再更新维护，应使用本接口。</para>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92125"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94096"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96325"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/transfer_customer")]
    Task<TransferCustomerResponse> TransferCustomerAsync(
        [Body] TransferCustomerRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询客户接替状态
    /// <para>分页查询原跟进成员分配给接替成员的客户的接替状态
    /// （1 - 接替完毕；2 - 等待接替；3 - 客户拒绝；4 - 接替成员客户达到上限），每页最多 1000 条。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetTransferResultRequest"/>：cursor 分页）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>每个客户的接替状态列表（customer[]）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para>原接口「查询客户接替结果」官方不再更新维护，应使用本接口。</para>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94088"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94097"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96327"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/transfer_result")]
    Task<GetTransferResultResponse> GetTransferResultAsync(
        [Body] GetTransferResultRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 分配在职成员的客户群
    /// <para>将在职成员为群主的客户群分配给新群主；未能继承的群逐个返回在 failed_chat_list 中。</para>
    /// <para>新群主必须是配置了客户联系功能的成员、已设置实名且已激活企业微信；
    /// 群主必须在应用的可见范围内；同一个人的群每天最多分配 300 个给新群主；
    /// 90 个自然日内在职成员的每个客户群仅可被转接 2 次。</para>
    /// </summary>
    /// <param name="request">分配请求体（<see cref="TransferGroupChatRequest"/>：chat_id_list 取值范围 1~100）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>没能成功继承的群列表（failed_chat_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95703"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95742"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96328"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/groupchat/onjob_transfer")]
    Task<TransferGroupChatResponse> TransferGroupChatAsync(
        [Body] TransferGroupChatRequest request,
        CancellationToken cancellationToken = default);
}
