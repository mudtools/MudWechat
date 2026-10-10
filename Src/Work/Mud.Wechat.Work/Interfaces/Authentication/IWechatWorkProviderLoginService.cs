// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「服务商登录授权」域 SDK（获取登录用户信息 <c>/cgi-bin/service/get_login_info</c>，
/// 官方文档 91154；SSO 概述 91127）。
/// <para>
/// 用于服务商在自有网站（或 Web 登录组件 / 扫码登录链接，链接构造见官方 91139）上完成管理员扫码授权后，
/// 以跳转回的 <c>auth_code</c> 换取登录用户信息——与「授权流接口」（get_pre_auth_code / get_permanent_code 等，
/// 见 <see cref="IWechatWorkProviderAuthenticationService"/>）分族承载，互不混用。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：消费服务商主体 <see cref="WechatTokenTypes.ProviderAccessToken"/>
/// （Query 注入 <c>provider_access_token</c>，由 <c>corpid + provider_secret</c> 经
/// <c>get_provider_token</c> 换取，Abstractions 令牌底座已覆盖）。
/// 该令牌凭据来源唯一（服务商主体级），非应用类型子接口、不声明 <c>TokenManagerKey</c> 归属域键。
/// </para>
/// <para>
/// <b>官方契约特例（errcode 缺省成功语义）</b>：官方 91154 载明「因历史原因，调用失败时才返回 errcode，
/// <b>无 errcode 字段视为成功</b>」。本 SDK 响应体直接继承 <see cref="WechatWorkResponse"/>
/// （<c>errcode</c> 为 <c>int</c>、缺省 0 ⇒ 报文缺失 errcode 字段时反序列化保持 0，
/// <c>IsSuccess</c> / 判错出口即按成功处理）——该判定路径由守卫 PL1 锁定，不得改为可空 errcode 或自建判定器。
/// </para>
/// <para>
/// <b>SSO 扫码登录链接构造（官方 91139，按方案默认降级为文档说明、不设 SDK 端点）</b>：
/// 链接本质为页面跳转 URL 拼装（<c>https://open.work.weixin.qq.com/wwopen/sso/qrConnect?appid=CORPID&amp;agentid=AGENTID&amp;redirect_uri=...&amp;state=...</c>），
/// 不产生 qyapi HTTP 请求，交由宿主按官方文档拼装；如后续需要 SDK 承载，再以独立 URL 构造器成员评估。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>provider_access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Authentication",
    TokenManage = nameof(IWechatAppManager))]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkProviderLoginService
{
    /// <summary>
    /// 获取登录用户信息
    /// <para>用户在扫码登录授权（或 Web 登录组件）中确认后，官方以 <c>auth_code</c>（随跳转返回）为凭证，
    /// 本接口换取该次登录的用户身份信息。</para>
    /// </summary>
    /// <param name="request">
    /// 请求体（<see cref="Mud.Wechat.Work.DataModels.ProviderAuthentication.GetLoginInfoRequest"/>：auth_code，
    /// 最长 512 字节）。
    /// </param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>登录用户信息（corpid / userid / name / avatar / gender）。</returns>
    /// <remarks>
    /// <para><b>服务商·获取登录用户信息</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91154"/></para>
    /// <para>
    /// 官方约束：<c>auth_code</c> 最长 512 字节，<b>单次使用、5 分钟内有效</b>，过期作废；
    /// <b>errcode 语义特例</b>——调用失败时才返回 errcode，无 errcode 字段视为成功；
    /// 返回 <c>name</c> 字段自 2020-06-30 起对服务商<b>不再返回真实姓名</b>（返回 userid，历史兼容字段）。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/service/get_login_info")]
    Task<GetLoginInfoResponse> GetLoginInfoAsync(
        [Body] GetLoginInfoRequest request,
        CancellationToken cancellationToken = default);
}
