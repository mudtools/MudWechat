// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.DataZone;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「数据与智能专区」模块基础接口域第三方应用 SDK。
/// <para>
/// 官方对第三方应用开放与三类应用公共面一致的 12 个端点（继承自
/// <see cref="IWechatWorkDataZoneService"/>），并额外开放 2 个差异端点：
/// 「获取数据与智能专区授权信息」（官方不支持自建应用，服务商代开发亦开放，见
/// <see cref="IWechatWorkProviderDataZoneService"/>）与「获取数据与智能专区文档存档授权信息」
/// （官方仅第三方应用开放）。
/// </para>
/// <para>企业自建应用见 <see cref="IWechatWorkInternalDataZoneService"/>；
/// 服务商代开发见 <see cref="IWechatWorkProviderDataZoneService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。各端点均要求应用具备
/// 「数据与智能专区」权限（文档存档端点要求「数据与智能专区-分析企业文档数据」权限，且需企业管理员二次授权）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "DataZone",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkDataZoneService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartyDataZoneService : IWechatWorkDataZoneService
{
    /// <summary>
    /// 获取数据与智能专区授权信息
    /// <para>获取企业授权的会话版本列表（内部会话 / 内外部会话 / 内外部会话及语音通话）、
    /// 授权存档范围（人员 / 部门 / 标签）、版本状态与生效时间等授权详情。
    /// 实际存档生效人员取决于服务商购买的人数（参见「获取会话存档授权成员列表」）。</para>
    /// <para>官方限制：企业自建应用不支持；应用需具备「数据与智能专区」权限；
    /// 授权变更（授权会话类型、人员范围、会话时长变更）会回调「变更授权通知」事件，
    /// 服务商收到事件后可调用本接口获取授权详情进行比对。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>企业授权的会话版本列表（auth_edition_list：edition / auth_scope / status / begin_time / end_time / msg_duration_days / auth_user_count）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100244"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100237"/></para>
    /// </remarks>
    [Post("/cgi-bin/chatdata/get_corp_auth_info")]
    Task<GetDataZoneCorpAuthInfoResponse> GetCorpAuthInfoAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取数据与智能专区文档存档授权信息
    /// <para>获取企业「文件内容存档」的授权状态（试用中 / 试用已结束）、授权存档范围（人员 / 部门 / 标签）与去重人数。</para>
    /// <para>官方限制：企业自建应用与服务商代开发均不支持，仅第三方应用开放；
    /// 应用需具备「数据与智能专区-分析企业文档数据」权限，且该权限需要企业管理员二次授权。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>授权状态（status）、授权存档范围（auth_scope）、试用起止时间（begin_time / end_time）与去重人数（auth_user_count）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101488"/></para>
    /// </remarks>
    [Post("/cgi-bin/docdata/get_auth_info")]
    Task<GetDocArchiveAuthInfoResponse> GetDocArchiveAuthInfoAsync(
        CancellationToken cancellationToken = default);
}
