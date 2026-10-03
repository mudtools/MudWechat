// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.AccountId;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「账号ID」域「corpid 转换」接口族公共 SDK。
/// <para>
/// 将企业主体的 corpid 转换为服务商的密文 corpid（open_corpid）。官方对<b>第三方应用与服务商代开发</b>
/// 开放完全一致的 1 个端点（97061/97105），以 <c>provider_access_token</c>（服务商凭证）鉴权 ——
/// 与企业级 <c>access_token</c> 端点分属不同令牌路由键，一接口族一令牌路由键，故独立成族；
/// 全部端点声明于本公共父接口，第三方/代开发应用类型子接口为空标记（企业自建应用官方无本族端点，不设子接口）。
/// </para>
/// <para>
/// corpid 密文大小写敏感，使用时不能再转为纯小写或纯大写。
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
public interface IWechatWorkAccountIdCorpidService
{
    /// <summary>
    /// corpid 转换
    /// <para>将企业主体的 corpid 转换为服务商的密文 corpid；仅限第三方服务商，
    /// 转换已获授权企业的 corpid（或待迁移的自建应用企业 corpid）。</para>
    /// </summary>
    /// <param name="request">转换请求体（<see cref="CorpidToOpenCorpidRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>该服务商第三方应用下的企业 ID（open_corpid）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97061"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97105"/></para>
    /// </remarks>
    [Post("/cgi-bin/service/corpid_to_opencorpid")]
    Task<CorpidToOpenCorpidResponse> CorpidToOpenCorpidAsync(
        [Body] CorpidToOpenCorpidRequest request,
        CancellationToken cancellationToken = default);
}
