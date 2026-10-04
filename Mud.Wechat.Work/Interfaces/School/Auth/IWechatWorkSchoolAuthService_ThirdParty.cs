// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.School;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「家校沟通」模块网页授权登录域第三方应用 SDK。
/// <para>
/// 官方对第三方应用开放的是<b>独立路由</b>的 2 个端点（获取访问用户身份
/// <c>/cgi-bin/service/getuserinfo3rd</c> + 获取家校访问用户身份
/// <c>/cgi-bin/service/school/getuserinfo3rd</c>），且以 <c>suite_access_token</c>（套件级凭证）鉴权——
/// 与企业自建/代开发的 <c>access_token</c> 端点（见 <see cref="IWechatWorkSchoolAuthService"/>）
/// 分属不同令牌路由键，故本接口<b>不继承</b>该公共父接口、独立声明 2 个端点
/// （形态对齐账号ID域群 ID 升级族 <see cref="IWechatWorkAccountIdChatIdUpgradeService"/>：
/// 一接口族一令牌路由键）。
/// </para>
/// <para>注意：<c>suite_access_token</c> 为套件级凭证、无企业 scope，不经由令牌作用域机制表达。</para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键为 <see cref="WechatTokenTypes.SuiteAccessToken"/>（Query 注入 <c>suite_access_token</c>，
/// 参数名已在组件 <c>SensitiveUrlRedactor</c> 词表内，无 G7 豁免负担）。
/// </para>
/// <para>
/// 官方业务限制：code 最大 512 字节、每次授权的 code 不同、只能使用一次、
/// 5 分钟未被使用自动过期；跳转的域名须完全匹配应用的可信域名，否则返回 50001 错误。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>suite_access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "School", TokenManage = nameof(IWechatAppManager))]
[Token(TokenType = WechatTokenTypes.SuiteAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "suite_access_token")]
public interface IWechatWorkThirdPartySchoolAuthService
{
    /// <summary>
    /// 获取访问用户身份（code 换用户身份，官方即 GET）
    /// <para>根据网页授权回调携带的 code 获取访问用户身份（第三方应用版本）。</para>
    /// <para>官方响应按用户身份三选一：用户属于某个企业返回 CorpId + UserId（+ DeviceId）；
    /// 用户为学校家长返回 CorpId（兼容旧版）+ external_userid（兼容旧版）+ parents 家长列表
    /// （同一家长微信可在多个学校各有一条记录，建议用 parents 字段）；
    /// 用户不属于任何企业返回 OpenId（对当前服务商唯一）。</para>
    /// <para>官方业务限制：code 最大 512 字节、每次授权的 code 不同、只能使用一次、
    /// 5 分钟未被使用自动过期；跳转的域名须完全匹配应用的可信域名，否则返回 50001 错误。
    /// 官方响应字段名为 PascalCase 的 CorpId/UserId/DeviceId/OpenId，照抄不纠正。</para>
    /// </summary>
    /// <param name="code">通过成员授权获取到的 code（最大 512 字节，只能使用一次，
    /// 5 分钟未被使用自动过期）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>访问用户身份（CorpId+UserId / CorpId+external_userid+parents / OpenId 三形态之一）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91711"/></para>
    /// <para>企业自建/服务商代开发走 <c>/cgi-bin/auth/getuserinfo</c>（access_token），
    /// 见 <see cref="IWechatWorkSchoolAuthService"/>（官方文档 91707/96712）。</para>
    /// </remarks>
    [Get("/cgi-bin/service/getuserinfo3rd")]
    Task<SchoolAuthThirdPartyUserInfoResponse> GetUserInfoAsync(
        [Query("code")] string code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取家校访问用户身份（code 换家长/学生身份，官方即 GET）
    /// <para>根据网页授权回调携带的 code 获取访问用户在家校通讯录中的身份（第三方应用版本）。</para>
    /// <para>官方响应按用户身份二选一：学校家长返回 parents 家长列表（corpid + parent_userid）；
    /// 学校学生返回 students 学生列表（corpid + student_userid）；两形态均携带 DeviceId。</para>
    /// <para>官方业务限制：code 最大 512 字节、每次授权的 code 不同、只能使用一次、
    /// 5 分钟未被使用自动过期；跳转的域名须完全匹配应用的可信域名，否则返回 50001 错误。</para>
    /// </summary>
    /// <param name="code">通过成员授权获取到的 code（最大 512 字节，只能使用一次，
    /// 5 分钟未被使用自动过期）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>家校访问用户身份（parents / students 二形态之一 + DeviceId）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95790"/></para>
    /// <para>企业自建/服务商代开发走 <c>/cgi-bin/school/getuserinfo</c>（access_token），
    /// 见 <see cref="IWechatWorkSchoolAuthService"/>（官方文档 95791/96715）。</para>
    /// </remarks>
    [Get("/cgi-bin/service/school/getuserinfo3rd")]
    Task<SchoolAuthThirdPartySchoolUserInfoResponse> GetSchoolUserInfoAsync(
        [Query("code")] string code,
        CancellationToken cancellationToken = default);
}
