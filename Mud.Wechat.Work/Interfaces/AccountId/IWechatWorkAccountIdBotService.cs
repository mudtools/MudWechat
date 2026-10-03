// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.AccountId;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「账号ID」域「userid 转换（未明确企业身份场景）」接口族公共 SDK。
/// <para>
/// 将企业主体下的加密 userid 转换成服务商主体下的 open_userid，用于「未明确企业身份场景」
/// （智能机器人场景，97106）。官方路由沿用 <c>userid_to_openuserid</c> 命名，实际转换方向为
/// 企业主体加密 userid → 服务商主体 open_userid（与「已明确企业身份场景」
/// <see cref="IWechatWorkAccountIdService.UserIdToOpenUserIdAsync"/> 的明文 → 密文方向相反）。
/// 本端点以 <c>provider_access_token</c>（服务商凭证）鉴权 —— 与企业级 <c>access_token</c> 端点
/// 分属不同令牌路由键，故独立成族；官方对第三方应用与代开发均开放（服务商主体调用），
/// 端点声明于本公共父接口，第三方/代开发应用类型子接口为空标记
/// （企业自建应用官方无本族端点，不设子接口）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键为 <see cref="WechatTokenTypes.ProviderAccessToken"/>（Query 注入 <c>provider_access_token</c>，
/// 应用服务商的接口调用凭证）；该参数名已在组件 <c>SensitiveUrlRedactor</c> 词表内，无 G7 豁免负担。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>provider_access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkAccountIdBotService
{
    /// <summary>
    /// userid 的转换（未明确企业身份场景）
    /// <para>将企业主体下的加密 userid 转换成服务商主体下的 open_userid（智能机器人场景）。</para>
    /// <para>open_userid 需要在智能机器人的可见范围内；智能机器人所在企业需要安装服务商的
    /// 第三方应用或代开发应用，且传入的 open_userid 需要在已安装应用的可见范围内。</para>
    /// </summary>
    /// <param name="request">转换请求体（<see cref="ServiceUserIdToOpenUserIdRequest"/>；open_userid_list 最多不超过 1000 个）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>智能机器人所在企业 ID（open_corpid）、ID 转换结果列表（items）及无法转换列表（invalid_open_userid_list）。</returns>
    /// <remarks>
    /// <para>本端点与 <see cref="IWechatWorkAccountIdService.UserIdToOpenUserIdAsync"/> 同属官方
    /// 「userid 的转换」文档页（第三方 96516 / 服务商代开发 97106），仅面向智能机器人「未明确企业身份」场景。</para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96516"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97106"/></para>
    /// </remarks>
    [Post("/cgi-bin/service/batch/userid_to_openuserid")]
    Task<ServiceUserIdToOpenUserIdResponse> ServiceUserIdToOpenUserIdAsync(
        [Body] ServiceUserIdToOpenUserIdRequest request,
        CancellationToken cancellationToken = default);
}
