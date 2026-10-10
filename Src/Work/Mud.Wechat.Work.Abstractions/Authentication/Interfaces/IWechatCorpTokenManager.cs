// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication;

/// <summary>
/// 授权企业 access_token 令牌管理器（路由键 <see cref="WechatTokenTypes.AccessToken"/>，企业级）。
/// </summary>
/// <remarks>
/// <para>
/// 「一企一份」令牌：以 <b>scope（authCorpId）</b>隔离缓存条目（框架 scope 机制），
/// 多企业令牌互不串扰。获取方式二选一：
/// ① 业务侧经 <see cref="IWechatAppContextSwitcher.SetCorp"/> 切换代开发企业上下文后
/// 以无参 <c>GetTokenAsync()</c> 获取（由环境上下文解析 authCorpId）；
/// ② 显式以 scopes 传入 <c>GetTokenAsync(new[] { authCorpId })</c>。
/// </para>
/// <para>
/// 与自建应用管理器在注册表中以不同的 TokenManagerKey 键位区分
/// （企业级管理器仅出现在第三方/服务商应用的上下文中）。
/// </para>
/// </remarks>
public interface IWechatCorpTokenManager : ITokenManager
{
}
