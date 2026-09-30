// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication;

/// <summary>
/// 企业微信应用上下文切换接口。
/// </summary>
/// <remarks>
/// <para>提供在不同企业微信应用上下文之间切换的能力。</para>
/// <para>当系统配置了多个企业微信应用时，可通过此接口快速切换当前使用的应用上下文；
/// 第三方/服务商代开发场景可进一步经 <see cref="SetCorp"/> 切换代操作企业
/// （企业级 access_token 的 scope 来源）。</para>
/// </remarks>
public interface IWechatAppContextSwitcher : IAppContextSwitcher
{
    /// <summary>
    /// 切换代开发企业上下文（设置当前异步流的 authCorpId / permanentCode）。
    /// </summary>
    /// <param name="authCorpId">授权方（企业）CorpId。</param>
    /// <param name="permanentCode">该企业的永久授权码（可选；缺省时由
    /// <c>IWechatCorpAuthStore</c> 持久化仓储提供）。</param>
    /// <remarks>
    /// 作用域为当前异步执行流（AsyncLocal），跨异步边界自然隔离，多企业令牌互不串扰。
    /// </remarks>
    void SetCorp(string authCorpId, string? permanentCode = null);
}
