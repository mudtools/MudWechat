// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Identity;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「身份验证」模块二次验证域企业自建应用 SDK：
/// 官方仅向「通讯录同步」或自建应用开放本域端点（获取用户二次验证信息 + 使用二次验证），全部声明于本接口。
/// <para>第三方应用与服务商代开发官方无对应文档，不设子接口；
/// 登录二次验证（<c>/cgi-bin/user/authsucc</c>）已由 <see cref="IWechatWorkUsersService.CompleteSecondaryAuthAsync"/> 承载。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 自建应用调用时，用户需在二次验证范围和应用可见范围内，且验证页面的链接必须填该自建应用的 oauth2 链接。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Identity",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkIdentityTfaService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalIdentityTfaService : IWechatWorkIdentityTfaService
{
    /// <summary>
    /// 获取用户二次验证信息
    /// <para>用户进入二次验证页面后，以企业微信颁发的 code 换取成员身份与二次验证授权码；
    /// code 只能使用一次、5 分钟未被使用自动过期。</para>
    /// </summary>
    /// <param name="request">二次验证信息请求体（<see cref="GetTfaInfoRequest"/>：code）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成员身份与二次验证授权码（userid / tfa_code；tfa_code 有效期五分钟且只能使用一次）。</returns>
    /// <remarks>
    /// <para>取得 tfa_code 后经 <see cref="TfaSuccAsync"/> 标记验证成功、解锁企业微信终端。</para>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99499"/></para>
    /// <para>官方业务限制：仅「通讯录同步」或自建应用可调用；并发限制 20。</para>
    /// </remarks>
    [Post("/cgi-bin/auth/get_tfa_info")]
    Task<GetTfaInfoResponse> GetTfaInfoAsync(
        [Body] GetTfaInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用二次验证
    /// <para>用户完成二次验证后，携带 <see cref="GetTfaInfoAsync"/> 返回的 tfa_code 标记验证成功，
    /// 解锁企业微信终端；tfa_code 五分钟内有效且只能使用一次。</para>
    /// </summary>
    /// <param name="request">二次验证请求体（<see cref="TfaSuccRequest"/>：userid / tfa_code）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>操作结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99500"/></para>
    /// <para>官方业务限制：仅「通讯录同步」或自建应用可调用；并发限制 20。</para>
    /// </remarks>
    [Post("/cgi-bin/user/tfa_succ")]
    Task<WechatWorkResponse> TfaSuccAsync(
        [Body] TfaSuccRequest request,
        CancellationToken cancellationToken = default);
}
