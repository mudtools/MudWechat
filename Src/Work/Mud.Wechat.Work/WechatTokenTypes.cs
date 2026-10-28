// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions;

/// <summary>
/// 企业微信官方令牌类型常量：与官方报文 Query 参数名一一对应的业务令牌类型键
/// （字符串键，与 <c>FeishuTokenTypes</c> 同模式）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本类键值的语义是「官方报文参数名契约」</b>：每个常量决定生成客户端注入报文的
/// Query 参数名——<see cref="AccessToken"/> → <c>access_token</c>、
/// <see cref="ProviderAccessToken"/> → <c>provider_access_token</c>、
/// <see cref="SuiteAccessToken"/> → <c>suite_access_token</c>，映射由官方契约锁定，
/// 契约守卫 <c>WechatTokenOwnerContractGuards</c>（TO1）断言不漂移。
/// <b>不得按应用类型拆分</b>：拆分会改变注入报文的参数名（如把第三方应用消费的业务端点键
/// 拆成「第三方专属键」会注入 <c>provider_access_token</c> 之类的错误参数），服务端直接拒绝。
/// </para>
/// <para>
/// <b>「同一业务令牌类型在不同应用类型下凭据来源不同」的差异不由本类承载</b>，
/// 由 <see cref="WechatTokenManagerKeys"/>（凭据归属域，<c>@Internal</c> / <c>@Corp</c>）承载：
/// <see cref="AccessToken"/> 同时服务「自建应用自身令牌」与「授权企业级令牌」两种凭据来源，
/// 接口声明面经归属域后缀消歧，运行期由 <c>WechatAppContext.GetTokenManager</c> 按应用类型
/// 路由到对应令牌管理器；企业级令牌以 scope（authCorpId）区分缓存（见 <c>CorpTokenManager</c>）。
/// </para>
/// <para>
/// 不要复用 Mud.HttpUtils 的通用 <see cref="T:Mud.HttpUtils.TokenTypes"/> 字符串常量
/// （如 "AccessToken"，其语义与企业微信不符且会污染共享令牌注册表）。
/// <see cref="T:Mud.HttpUtils.Attributes.TokenAttribute"/> 的 TokenType 为 <c>string</c>，
/// 平台 SDK 自建常量类即可，无需对组件做任何扩展。
/// </para>
/// <para>
/// 键值采用 <c>"Wechat."</c> 前缀命名空间，避免与通用 <c>TokenTypes.AccessToken</c>（"AccessToken"）
/// 在共享令牌注册表中冲突。未显式声明 <c>TokenManagerKey</c> 的接口（套件级 / 服务商级管理端点），
/// 本类键值直接充当令牌管理器查找键（上游生成器回退链：TokenManagerKey → TokenType → 构造参数），
/// 亦为令牌缓存键三段式 <c>{tokenType}:{appKey}:{scopeKey}</c> 的第一段。
/// </para>
/// </remarks>
public static class WechatTokenTypes
{
    /// <summary>企业内访问令牌（自建应用 / 授权企业级，access_token）。</summary>
    public const string AccessToken = "Wechat.AccessToken";

    /// <summary>服务商访问令牌（provider_access_token）。</summary>
    public const string ProviderAccessToken = "Wechat.ProviderAccessToken";

    /// <summary>第三方/服务商套件令牌（suite_access_token）。</summary>
    public const string SuiteAccessToken = "Wechat.SuiteAccessToken";
}
