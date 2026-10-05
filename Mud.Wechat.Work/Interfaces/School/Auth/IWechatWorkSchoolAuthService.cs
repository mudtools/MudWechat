// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.School;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「家校沟通」模块网页授权登录域<b>企业自建 + 服务商代开发</b>公共 SDK 接口。
/// <para>
/// 官方网页授权登录对自建与代开发开放完全一致的 1 个端点（获取家校访问用户身份，同路由同契约、
/// 以 <c>access_token</c> 鉴权），收敛声明于本接口；
/// 应用类型子接口均为零差异端点空标记：自建见 <see cref="IWechatWorkInternalSchoolAuthService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderSchoolAuthService"/>。
/// </para>
/// <para>
/// 架构决策（单一所有者）：「获取访问用户身份」（<c>/cgi-bin/auth/getuserinfo</c>）曾在本接口重复声明，
/// 因与「身份验证」域 <see cref="IWechatWorkIdentityService"/> 构成同路由重复声明、且平行 DTO 家族互为同构，
/// 现已收敛为「身份验证」域单一所有者声明，家校场景请改用
/// <see cref="IWechatWorkIdentityService.GetUserInfoAsync"/>。
/// </para>
/// <para>
/// 官方对<b>第三方应用</b>开放的是独立路由（<c>/cgi-bin/service/getuserinfo3rd</c>、
/// <c>/cgi-bin/service/school/getuserinfo3rd</c>）且以 <c>suite_access_token</c> 鉴权——
/// 令牌路由键不同，故第三方子接口 <see cref="IWechatWorkThirdPartySchoolAuthService"/>
/// <b>不继承本接口</b>、独立声明 2 个端点（形态对齐上下游域
/// <see cref="IWechatWorkCorpGroupInternalProviderService"/>：差异面独立成族）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 官方业务限制：code 最大 512 字节、每次授权的 code 不同、只能使用一次、5 分钟未被使用自动过期；
/// 跳转的域名须完全匹配该 access_token 对应应用的可信域名，否则返回 50001 错误。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkSchoolAuthService
{
    /// <summary>
    /// 获取家校访问用户身份（code 换家长/学生身份，官方即 GET）
    /// <para>根据网页授权回调携带的 code 获取访问用户在家校通讯录中的身份。</para>
    /// <para>官方响应按用户身份二选一：学校家长返回 parent_userid；
    /// 学校学生返回 student_userid；两形态均携带 DeviceId（官方字段名为 PascalCase 的 DeviceId）。</para>
    /// <para>官方业务限制：code 最大 512 字节、每次授权的 code 不同、只能使用一次、
    /// 5 分钟未被使用自动过期；跳转的域名须完全匹配该 access_token 对应应用的可信域名，否则返回 50001 错误。</para>
    /// </summary>
    /// <param name="code">通过成员授权获取到的 code（最大 512 字节，只能使用一次，
    /// 5 分钟未被使用自动过期）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>家校访问用户身份（parent_userid / student_userid 二形态之一 + DeviceId）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95791"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96715"/></para>
    /// <para><b>第三方应用</b>走独立路由与 suite_access_token，见
    /// <see cref="IWechatWorkThirdPartySchoolAuthService"/>（官方文档 95790）。</para>
    /// </remarks>
    [Get("/cgi-bin/school/getuserinfo")]
    Task<SchoolAuthSchoolUserInfoResponse> GetSchoolUserInfoAsync(
        [Query("code")] string code,
        CancellationToken cancellationToken = default);
}
