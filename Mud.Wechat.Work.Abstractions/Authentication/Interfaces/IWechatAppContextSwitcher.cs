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
    /// <para>
    /// <b>P2-1 配对要求</b>：长生命周期执行上下文（如后台任务、常驻队列消费者）在完成代操作后必须配对
    /// <see cref="ClearCorp"/> 或改用 <c>WechatCorpContext.BeginCorpScope(...)</c>（<c>using</c> 语义），
    /// 否则环境企业上下文会残留到后续不相关的调用（造成 scope 误用）。
    /// </para>
    /// </remarks>
    void SetCorp(string authCorpId, string? permanentCode = null);

    /// <summary>
    /// 清除当前异步流的代开发企业上下文（<see cref="SetCorp"/> 的对称重置入口，P2-1）。
    /// </summary>
    /// <remarks>
    /// 清空属性（authCorpId / permanentCode / 归属 AppKey），后续企业级令牌必须重新经
    /// <see cref="SetCorp"/> 或显式 <c>GetTokenAsync(new[]{ authCorpId })</c> 指定 scope。
    /// </remarks>
    void ClearCorp();

    /// <summary>
    /// 异步获取当前应用上下文指定类型的访问令牌。
    /// </summary>
    /// <param name="tokenType">令牌类型（<see cref="WechatTokenTypes"/> 常量；默认 <see cref="WechatTokenTypes.AccessToken"/>）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>令牌字符串。</returns>
    /// <exception cref="InvalidOperationException">当前无应用上下文，或该应用未装配对应令牌管理器。</exception>
    /// <remarks>
    /// <b>P2-3</b>：多套件场景下取套件/服务商令牌必须能指定类型（原无参成员硬编码 <c>WechatTokenTypes.AccessToken</c>
    /// 且丢弃取消令牌）。
    /// </remarks>
    Task<string> GetTokenAsync(string tokenType, CancellationToken cancellationToken = default);
}
