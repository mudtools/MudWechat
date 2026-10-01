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
/// <remarks>
/// <para>
/// <b>Mud.HttpUtils 3.0.0 适配（BC-27）</b>：上游已从 <c>IAppContextSwitcher</c> 移除
/// <c>UseApp(string)</c> / <c>UseDefaultApp()</c> / <c>BeginScope(string)</c>，
/// 并新增平行的 <see cref="IAppScopeSwitcher"/>（作用域式安全入口）。本接口
/// <b>继承 <see cref="IAppScopeSwitcher"/></b> 并<b>显式重声明</b>三个旧成员（标 <c>[Obsolete]</c>）：
/// 既让下游可直接以 <see cref="IAppScopeSwitcher"/> 变量类型使用推荐面，又保持本 SDK 的对外契约不变
/// （实现类 <c>WechatAppContextSwitcher</c> 已实现这三个成员，无需额外实现代码）。
/// </para>
/// <para>
/// 新代码请使用 <see cref="IAppScopeSwitcher.UseAppScope(string)"/> / <see cref="IAppScopeSwitcher.UseDefaultAppScope"/>：
/// 守卫完全相同，且返回 <see cref="IDisposable"/>、释放时<b>自动归还</b>上下文。
/// 三个旧成员将在下一个大版本移除。
/// </para>
/// </remarks>
public interface IWechatAppContextSwitcher : IAppContextSwitcher, IAppScopeSwitcher
{
    // ───────────── 以下三个成员为「Mud.HttpUtils 3.0.0 适配」新增的接续声明 ─────────────
    // 上游 BC-27 已从 IAppContextSwitcher 移除这三个成员，生成器也不再默认发射它们。
    // 本接口显式重声明以保持 Mud.Wechat 的对外契约不变（实现类 WechatAppContextSwitcher 已实现它们，
    // 无需额外实现代码）；签名必须与上游原签名逐字一致，否则重声明与实现类的隐式实现不再匹配（CS0535）。
    // 新代码请改用 IAppScopeSwitcher.UseAppScope / UseDefaultAppScope（返回 IDisposable，释放时自动归还上下文）。
    // 本组成员将在本 SDK 的下一个大版本随上游一并移除。

    /// <summary>切换到指定应用上下文（<b>无作用域</b>：不会自动归还上下文）。</summary>
    /// <param name="appKey">应用标识。</param>
    /// <returns>切换后的应用上下文实例。</returns>
    [Obsolete("请改用 IAppScopeSwitcher.UseAppScope(appKey)：守卫完全相同，且释放时自动归还上下文。Mud.Wechat 将在下一个大版本移除本成员。")]
    IMudAppContext UseApp(string appKey);

    /// <summary>切换到默认应用上下文（<b>无作用域</b>：不会自动归还上下文）。</summary>
    /// <returns>默认应用上下文实例。</returns>
    [Obsolete("请改用 IAppScopeSwitcher.UseDefaultAppScope()：守卫完全相同，且释放时自动归还上下文。Mud.Wechat 将在下一个大版本移除本成员。")]
    IMudAppContext UseDefaultApp();

    /// <summary>切换到指定应用并以作用域自动归还上下文。</summary>
    /// <param name="appKey">应用标识。</param>
    /// <returns>释放时恢复之前上下文的作用域对象。</returns>
    [Obsolete("请改用 IAppScopeSwitcher.UseAppScope(appKey)：二者为同一实现（逐行等价的别名），且返回类型同为 IDisposable。Mud.Wechat 将在下一个大版本移除本成员。")]
    IDisposable BeginScope(string appKey);

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
