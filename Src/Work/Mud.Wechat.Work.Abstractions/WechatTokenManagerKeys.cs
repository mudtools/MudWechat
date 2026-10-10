// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions;

/// <summary>
/// 令牌管理器查找键常量：在 <see cref="WechatTokenTypes"/>（业务令牌类型）之上补「凭据归属域」维度。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么需要第二个维度</b>：<see cref="WechatTokenTypes.AccessToken"/> 同时服务「自建应用自身令牌」
/// 与「授权企业级令牌」两种凭据来源，运行期由 <c>WechatAppConfig.AppType</c> 路由。二者在声明面上
/// 完全等价，导致「把第三方应用类型子接口注入到自建应用上」这类错配<b>编译期与运行期均无法发现</b>
/// （会用自建令牌把请求正常发出去，只有使用方的心智模型是错的）。
/// </para>
/// <para>
/// <b>不得改 <see cref="WechatTokenTypes"/></b>：该常量决定注入到报文的参数名（<c>access_token</c>），
/// 由企业微信官方契约锁定，按应用类型拆分会注入 <c>provider_access_token</c> 之类的错误参数名。
/// 因此改用 <c>TokenAttribute.TokenManagerKey</c>——框架专为「解耦业务概念（TokenType）与技术查找键」而设，
/// 且生成器会把接口级键贯通到请求期的 <c>IMudAppContext.GetTokenManager(key)</c> 咽喉点（含恢复链路）。
/// </para>
/// <para>
/// <b>键形如</b> <c>{TokenType}@{Owner}</c>，<c>Owner</c> 取值见 <c>WechatTokenOwner</c>；
/// 未携带归属域后缀的旧键（公共父接口、宿主直调）<b>不校验</b>，保持既有语义。
/// 该键仅用于「路由 + 校验」，<b>不进入</b>令牌缓存键（<c>{tokenType}:{appKey}:{scopeKey}</c>）与
/// 失效端口语义。
/// </para>
/// <para>
/// <b>落位规则</b>：应用类型子接口的 <c>[Token]</c> 必须声明本类常量；公共父接口不声明。
/// 仅 <see cref="WechatTokenTypes.AccessToken"/> 因「自建 / 企业级」双凭据来源需要消歧——
/// <see cref="WechatTokenTypes.SuiteAccessToken"/> / <see cref="WechatTokenTypes.ProviderAccessToken"/>
/// 凭据来源唯一、无二义性，对应管理端点接口不使用归属域后缀。
/// 全仓一致性由契约守卫 <c>WechatTokenOwnerContractGuards</c> 锁定。
/// </para>
/// <para>
/// <b>永不增设 <c>@Provider</c> / <c>@Suite</c> 归属域键（设计定夺见 .docs 定夺二）</b>：
/// 归属域键解决的是 <see cref="WechatTokenTypes.AccessToken"/> 的「双凭据来源不可区分」问题；
/// <c>provider_access_token</c> 凭据来源唯一（服务商主体）、第三方与代开发应用都装配
/// <c>ProviderTokenManager</c>（套件族同理），族内不存在错配空间 ⇒ 归属域键对该族
/// <b>无新增拦截能力</b>，仅剩错误信息措辞层面的边际差异，纯为对称性付费。
/// 「把 provider/suite 族接口注入到自建应用上下文」的错配防线由
/// <c>WechatAppContext.GetTokenManager</c> 按 <see cref="WechatTokenTypes"/> 分派的
/// 「未装配即抛」承担（fail-fast，错误信息已指明成因）。
/// 重开议题的判据（需同时满足）：① 官方语义变化——provider_access_token 出现第二种凭据来源
/// 或按应用类型的开放面分裂；② 运行期防线不足被实证——真实故障或审计案例证明
/// <c>GetTokenManager</c> 的「未装配即抛」不足以拦截某类错配。
/// </para>
/// </remarks>
public static class WechatTokenManagerKeys
{
    /// <summary>
    /// 自建应用自身 <c>access_token</c>（<c>corpid</c> + <c>corpsecret</c> 换取）；
    /// 仅 <c>WechatAppType.Internal</c> 应用可用。
    /// </summary>
    /// <remarks>
    /// 对应令牌管理器：<c>InternalAppTokenManager</c>。用于全部 <c>*_Internal</c> /
    /// <c>IWechatWorkInternal*</c> 应用类型子接口。
    /// </remarks>
    public const string InternalAccessToken = "Wechat.AccessToken@Internal";

    /// <summary>
    /// 授权企业级 <c>access_token</c>（第三方 <c>get_corp_token</c> / 代开发
    /// <c>gettoken(corpsecret = permanent_code)</c> 换取，scope = <c>authCorpId</c>）；
    /// 仅 <c>WechatAppType.ThirdParty</c> / <c>WechatAppType.Provider</c> 应用可用。
    /// </summary>
    /// <remarks>
    /// 对应令牌管理器：<c>CorpTokenManager</c>。用于全部 <c>*_ThirdParty</c> /
    /// <c>*_Provider</c> / <c>IWechatWorkThirdParty*</c> / <c>IWechatWorkProvider*</c>
    /// 应用类型子接口；调用前须经 <c>IWechatAppContextSwitcher</c> 切换到目标授权企业作用域。
    /// </remarks>
    public const string CorpAccessToken = "Wechat.AccessToken@Corp";
}
