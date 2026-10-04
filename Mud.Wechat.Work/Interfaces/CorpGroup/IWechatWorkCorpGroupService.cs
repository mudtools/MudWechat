// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.CorpGroup;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「上下游」域<b>三类应用公共面</b> SDK 接口。
/// <para>
/// 官方仅对<b>企业自建应用、第三方应用、服务商代开发</b>一致开放「获取应用共享信息」1 个端点
/// （第三方文档号 95324，与自建/代开发同路由同契约），故本公共父接口只承载该端点；
/// 其余 5 个端点官方<b>仅向自建与服务商代开发开放</b>，已下沉至
/// <see cref="IWechatWorkCorpGroupInternalProviderService"/>。
/// </para>
/// <para>
/// 应用类型子接口：<see cref="IWechatWorkThirdPartyCorpGroupService"/>（第三方，唯一空标记，直接继承本父接口）；
/// 自建（<see cref="IWechatWorkInternalCorpGroupService"/>）与服务商代开发
/// （<see cref="IWechatWorkProviderCorpGroupService"/>）则继承
/// <see cref="IWechatWorkCorpGroupInternalProviderService"/>，类型化面为官方开放的完整 6 端点。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 落位与形态对齐 <see cref="IWechatWorkTagsService"/>：令牌路由键为
/// <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>，即上级/上游企业应用的凭证），
/// 由多应用基座按当前应用上下文（AppKey + scope）路由——代开发为授权企业级令牌，
/// 调用前须经 <c>IWechatAppContextSwitcher</c> 切换到目标授权企业作用域。
/// </para>
/// <para>
/// 令牌能力边界：<see cref="IWechatWorkCorpGroupInternalProviderService.TransferMiniProgramSessionAsync"/>
/// 消费的是<b>下级/下游企业</b>的 access_token（经
/// <see cref="IWechatWorkCorpGroupInternalProviderService.GetCorpGroupTokenAsync"/> 获取，SDK 令牌基座不自动缓存该凭证，
/// 由宿主写入令牌存储后切换上下文调用或自行注入）；其余端点均消费上级/上游企业应用自身凭证。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkCorpGroupService
{
    /// <summary>
    /// 获取应用共享信息
    /// <para>拉取上级/上游企业与下级/下游企业之间的应用共享信息（corp_list）。</para>
    /// <para>limit 最大值 100，默认或 0 表示拉取全量；建议分页拉取（cursor 游标）或指定 corpid 拉取。</para>
    /// <para>官方权限说明「自建应用和第三方应用」：本端点是本域唯一向第三方应用开放的端点（第三方文档号 95324，
    /// 与自建/代开发同路由同契约）；其余 5 个端点官方无第三方文档，见
    /// <see cref="IWechatWorkCorpGroupInternalProviderService"/>。</para>
    /// </summary>
    /// <param name="request">应用共享信息请求体（<see cref="ListAppShareInfoRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>应用共享信息列表（ending 分页终止标志 + next_cursor 游标）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95813"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95324"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96872"/></para>
    /// <para><b>「企业互联」文档树</b>（同路由同契约的旧编号）：<see href="https://developer.work.weixin.qq.com/document/path/93403"/>（自建）、
    /// <see href="https://developer.work.weixin.qq.com/document/path/93405"/>（第三方）、
    /// <see href="https://developer.work.weixin.qq.com/document/path/96816"/>（代开发）。</para>
    /// </remarks>
    [Post("/cgi-bin/corpgroup/corp/list_app_share_info")]
    Task<ListAppShareInfoResponse> ListAppShareInfoAsync(
        [Body] ListAppShareInfoRequest request,
        CancellationToken cancellationToken = default);
}
