// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication;

/// <summary>
/// 授权事件协调器：把回调事件（<c>create_auth</c> / <c>reset_permanent_code</c> /
/// <c>change_auth</c> / <c>cancel_auth</c>）编排到授权服务（由主包注册实现）。
/// </summary>
/// <remarks>
/// <para>
/// <b>分层解耦</b>：抽象定义在 Abstractions，实现由主包（<c>AddAuthenticationApi()</c>）注册；
/// 回调包仅依赖 Abstractions，以「可选依赖」方式解析本接口——未安装主包授权模块时返回 <c>null</c>，
/// 回调处理器退化为基础清理行为（不抛异常，首次命中输出一次性告警）。
/// </para>
/// <para>
/// <b>suiteId → appKey 映射</b>：实现方遍历 <see cref="IWechatAppManager.ConfiguredAppKeys"/>，
/// 匹配各自 <see cref="Configuration.WechatAppConfig.SuiteId"/>；多套件命中即处理，未命中记 Warning。
/// </para>
/// <para>
/// <b>R11</b>：<c>create_auth</c> / <c>reset_permanent_code</c> 报文本体不含 <c>AuthCorpId</c>，
/// 授权企业由 <c>auth_code</c> 换码后经 <c>auth_corp_info.corpid</c> 反查。
/// </para>
/// </remarks>
public interface IWechatAuthorizationCoordinator
{
    /// <summary>处理授权成功事件（<c>create_auth</c>）：以一次性 <c>auth_code</c> 换码并落库。</summary>
    /// <param name="suiteId">套件 Id（代开发模板 id 即 suite_id，K2）。</param>
    /// <param name="authCode">一次性临时授权码。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    Task OnAuthorizationSucceededAsync(string suiteId, string authCode, CancellationToken cancellationToken = default);

    /// <summary>处理永久授权码重置事件（<c>reset_permanent_code</c>）：重新换码覆盖既有授权条目。</summary>
    /// <param name="suiteId">套件 Id。</param>
    /// <param name="authCode">一次性临时授权码。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    Task OnPermanentCodeResetAsync(string suiteId, string authCode, CancellationToken cancellationToken = default);

    /// <summary>处理授权变更事件（<c>change_auth</c>）：刷新授权信息落库（本地无该授权时仅告警不抛）。</summary>
    /// <param name="suiteId">套件 Id。</param>
    /// <param name="authCorpId">授权方（企业）CorpId。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    Task OnAuthorizationChangedAsync(string suiteId, string authCorpId, CancellationToken cancellationToken = default);

    /// <summary>处理取消授权事件（<c>cancel_auth</c> / <c>del_auth</c>）：复合键清理该 appKey 的授权并失效其企业令牌。</summary>
    /// <param name="suiteId">套件 Id。</param>
    /// <param name="authCorpId">授权方（企业）CorpId。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    Task OnAuthorizationCanceledAsync(string suiteId, string authCorpId, CancellationToken cancellationToken = default);
}