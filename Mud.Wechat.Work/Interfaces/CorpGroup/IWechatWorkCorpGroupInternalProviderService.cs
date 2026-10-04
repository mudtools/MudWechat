// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.CorpGroup;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「上下游」域<b>企业自建 + 服务商代开发</b>公共 SDK 接口。
/// <para>
/// 承载官方<b>仅向自建应用与服务商代开发开放</b>的 5 个上下游端点（下级/下游企业凭证、小程序 session、
/// 上下游关联客户信息 3 条），并继承公共父接口 <see cref="IWechatWorkCorpGroupService"/> 的
/// 「获取应用共享信息」公共面 —— 合计 6 个端点，与官方对自建/代开发开放的端点集完全一致。
/// </para>
/// <para>
/// 官方第三方应用文档树<b>未开放</b>本接口族任何端点（仅「获取应用共享信息」向第三方开放，见
/// <see cref="IWechatWorkCorpGroupService"/>），因此本接口<b>不由第三方应用类型子接口继承</b>：
/// <see cref="IWechatWorkThirdPartyCorpGroupService"/> 直接继承公共父接口、类型化面仅 1 个端点。
/// </para>
/// <para>
/// 应用类型子接口：<see cref="IWechatWorkInternalCorpGroupService"/>（自建）、
/// <see cref="IWechatWorkProviderCorpGroupService"/>（服务商代开发），二者均为空标记。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>，即上级/上游企业应用的凭证），
/// 由多应用基座按当前应用上下文（AppKey + scope）路由——代开发为授权企业级令牌，
/// 调用前须经 <c>IWechatAppContextSwitcher</c> 切换到目标授权企业作用域。
/// </para>
/// <para>
/// 令牌能力边界：<see cref="TransferMiniProgramSessionAsync"/> 消费的是<b>下级/下游企业</b>的
/// access_token（经 <see cref="GetCorpGroupTokenAsync"/> 获取，SDK 令牌基座不自动缓存该凭证，
/// 由宿主写入令牌存储后切换上下文调用或自行注入）；其余端点均消费上级/上游企业应用自身凭证。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true,
    InheritedFrom = nameof(WechatWorkCorpGroupService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkCorpGroupInternalProviderService : IWechatWorkCorpGroupService
{
    /// <summary>
    /// 获取下级/下游企业的 access_token
    /// <para>以上级/上游企业应用凭证换取已授权的下级/下游企业应用调用凭证（最长 512 字节）。</para>
    /// <para>返回的凭证由调用方自行管理生命周期；可用于调用下级/下游企业侧的接口（如小程序 session 转换）。</para>
    /// </summary>
    /// <param name="request">下级/下游企业凭证请求体（<see cref="GetCorpGroupTokenRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>下级/下游企业调用凭证（access_token + expires_in）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95816"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96873"/></para>
    /// </remarks>
    [Post("/cgi-bin/corpgroup/corp/gettoken")]
    Task<GetCorpGroupTokenResponse> GetCorpGroupTokenAsync(
        [Body] GetCorpGroupTokenRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取下级/下游企业小程序 session
    /// <para>上级/上游企业通过该接口将小程序登录态转换为下级/下游企业的 session。</para>
    /// <para><b>令牌语义</b>：必须使用下级/下游企业的 access_token（经获取下级/下游企业的 access_token 接口获取，
    /// 且该凭证对应的下级/下游企业应用必须是 session_key 对应的上级/上游企业应用分享而来）；
    /// userid 与 session_key 均通过 code2Session 接口获取、均不多于 64 字节。</para>
    /// </summary>
    /// <param name="request">小程序 session 转换请求体（<see cref="TransferMiniProgramSessionRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>下级/下游企业侧的用户 ID 与会话密钥（session_key 敏感，不得记录日志）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95817"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96874"/></para>
    /// </remarks>
    [Post("/cgi-bin/miniprogram/transfer_session")]
    Task<TransferMiniProgramSessionResponse> TransferMiniProgramSessionAsync(
        [Body] TransferMiniProgramSessionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上下游关联客户信息-已添加客户
    /// <para>将微信客户的 unionid 转换为下游企业（或指定企业）的外部联系人 external_userid。</para>
    /// <para>应用须为上下游共享的应用且具有客户联系权限；上游企业须已认证；unionid 与 openid 主体须与当前企业一致，
    /// 且在同一个小程序获取。调用频率：10 万次/小时、48 万次/天、750 万次/月；
    /// 传入有效 mass_call_ticket 可不受该限制（仍受基础频率限制），适用于数据初始化场景。</para>
    /// </summary>
    /// <param name="request">已添加客户转换请求体（<see cref="UnionidToExternalUserIdRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>该 unionid 对应的外部联系人信息列表（corpid + external_userid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95818"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96875"/></para>
    /// </remarks>
    [Post("/cgi-bin/corpgroup/unionid_to_external_userid")]
    Task<UnionidToExternalUserIdResponse> UnionidToExternalUserIdAsync(
        [Body] UnionidToExternalUserIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上下游关联客户信息-未添加客户（unionid 转 pending_id）
    /// <para>品牌方将公众号、小程序粉丝的 unionid 预先转换为 pending_id（临时外部联系人 ID，有效期 90 天、共享应用内唯一）；
    /// 当微信用户成为下游企业客户后，可用 external_userid 转批量 pending_id 接口建立
    /// unionid = pending_id = external_userid 的映射关系。</para>
    /// <para>应用须为上下游共享的自建/代开发应用且具有客户联系权限；unionid 与 openid 主体须认证且与上游企业主体一致；
    /// 调用频率：10 万次/小时、48 万次/天、750 万次/月（按上游企业维度）。</para>
    /// </summary>
    /// <param name="request">unionid 转 pending_id 请求体（<see cref="UnionidToPendingIdRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>对应的 pending_id。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97357"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98040"/></para>
    /// </remarks>
    [Post("/cgi-bin/corpgroup/unionid_to_pending_id")]
    Task<UnionidToPendingIdResponse> UnionidToPendingIdAsync(
        [Body] UnionidToPendingIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上下游关联客户信息-未添加客户（external_userid 批量转 pending_id）
    /// <para>将上游或下游企业外部联系人 id 批量转换为 pending_id（最多同时查询 100 个），
    /// 以打通 unionid = pending_id = external_userid 映射。</para>
    /// <para>上游应用须已调用过 unionid 转 pending_id 接口；该客户的跟进人或其所在客户群群主必须在应用的可见范围之内；
    /// 传入 chat_id 时只检查群主是否在可见范围并忽略该群以外的 external_userid。</para>
    /// </summary>
    /// <param name="request">external_userid 批量转 pending_id 请求体（<see cref="ExternalUserIdToPendingIdRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>转换结果列表（external_userid + pending_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97357"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98040"/></para>
    /// </remarks>
    [Post("/cgi-bin/corpgroup/batch/external_userid_to_pending_id")]
    Task<ExternalUserIdToPendingIdResponse> ExternalUserIdToPendingIdAsync(
        [Body] ExternalUserIdToPendingIdRequest request,
        CancellationToken cancellationToken = default);
}
