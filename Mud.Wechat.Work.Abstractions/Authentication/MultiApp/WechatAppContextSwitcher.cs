// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication.MultiApp;

/// <summary>
/// 企业微信应用上下文切换器默认实现（R9）。
/// </summary>
/// <remarks>
/// <para>
/// 继承 <see cref="AsyncLocalAppContextSwitcher"/> 复用其 <see cref="AsyncLocal{T}"/> 上下文传播语义
/// （<see cref="IAppContextHolder.Current"/> / <see cref="IAppContextHolder.SwitchTo"/> /
/// <see cref="IAppContextHolder.BeginScope(IMudAppContext)"/>），仅补齐
/// <see cref="IWechatAppContextSwitcher"/> 的应用解析与代开发企业上下文写入。
/// </para>
/// <para>
/// <b>MT-02 授权</b>：<see cref="UseApp"/> / <see cref="BeginScope(string)"/> 在切换前经
/// <see cref="IAppAccessAuthorizer"/> 授权（未注册实现时框架默认拒绝，本 SDK 在
/// <c>AddWechatTokenInfrastructure</c> 中注册放行型实现）。
/// </para>
/// </remarks>
public sealed class WechatAppContextSwitcher : AsyncLocalAppContextSwitcher, IWechatAppContextSwitcher
{
    private readonly IWechatAppManager _appManager;
    private readonly IAppAccessAuthorizer? _authorizer;

    /// <summary>创建应用上下文切换器。</summary>
    /// <param name="appManager">应用管理器（解析目标应用上下文）。</param>
    /// <param name="authorizer">应用切换授权器（可选；为 null 表示不做授权判定）。</param>
    public WechatAppContextSwitcher(IWechatAppManager appManager, IAppAccessAuthorizer? authorizer = null)
    {
        _appManager = appManager ?? throw new ArgumentNullException(nameof(appManager));
        _authorizer = authorizer;
    }

    /// <inheritdoc />
    public IMudAppContext UseApp(string appKey) => SwitchToApp(appKey);

    /// <inheritdoc />
    public IMudAppContext UseDefaultApp()
    {
        var context = _appManager.GetDefaultApp();
        SwitchTo(context);
        return context;
    }

    /// <inheritdoc />
    public IDisposable BeginScope(string appKey) => BeginScope((IMudAppContext)SwitchToApp(appKey));

    /// <inheritdoc />
    public Task<string> GetTokenAsync() => GetTokenAsync(WechatTokenTypes.AccessToken, CancellationToken.None);

    /// <inheritdoc />
    public Task<string> GetTokenAsync(string tokenType, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(tokenType))
        {
            throw new ArgumentNullException(nameof(tokenType));
        }

        var context = Current
            ?? throw new InvalidOperationException(
                "当前无应用上下文：请先经 IWechatAppContextSwitcher.UseApp(appKey) 切换，或显式传入 appKey。");

        // P2-3：按 tokenType 路由（原无参成员硬编码 AccessToken 且丢弃调用方取消令牌）。
        return context.GetTokenManager(tokenType).GetTokenAsync(cancellationToken);
    }

    /// <inheritdoc />
    /// <remarks>
    /// 归属应用取当前环境上下文（<see cref="IMudAppContext.AppKey"/>）：未切换应用时为 null，
    /// 表示不声明归属（<c>CorpTokenManager</c> 归属校验随之跳过）。
    /// </remarks>
    public void SetCorp(string authCorpId, string? permanentCode = null)
        => WechatCorpContext.SetCorp(Current?.AppKey, authCorpId, permanentCode);

    /// <inheritdoc />
    /// <remarks>P2-1：<see cref="SetCorp"/> 的对称重置入口（暴露既有 <see cref="WechatCorpContext.Clear"/>）。</remarks>
    public void ClearCorp() => WechatCorpContext.Clear();

    private IMudAppContext SwitchToApp(string appKey)
    {
        if (_authorizer != null && !_authorizer.CanSwitchTo(appKey))
        {
            throw new InvalidOperationException(
                $"当前调用主体无权切换到应用 '{AppKey.ToSafeText(appKey)}'，切换已被拒绝。");
        }

        var context = _appManager.GetApp(appKey);
        SwitchTo(context);
        return context;
    }
}