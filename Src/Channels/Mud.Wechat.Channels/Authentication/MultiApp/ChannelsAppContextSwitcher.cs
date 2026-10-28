// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.Abstractions.Authentication.MultiApp;

/// <summary>
/// 微信小店 / 视频号应用上下文切换器默认实现。
/// </summary>
/// <remarks>
/// <para>
/// 继承组件 <see cref="AsyncLocalAppContextSwitcher"/> 复用其 <see cref="AsyncLocal{T}"/> 上下文传播语义
/// （<see cref="IAppContextHolder.Current"/> / <see cref="IAppContextHolder.SwitchTo"/> /
/// <see cref="IAppContextHolder.BeginScope(IMudAppContext)"/>），补齐应用解析与作用域式入口。
/// </para>
/// <para>
/// <b>作用域组合方式（重要）</b>：<see cref="UseAppScope"/> <b>不预先</b>调用
/// <see cref="IAppContextHolder.SwitchTo"/>，而是把解析出的上下文直接交给
/// <see cref="IAppContextHolder.BeginScope(IMudAppContext)"/> 建作用域（后者自行快照进入前上下文并在释放时
/// 还原）。若先 <c>SwitchTo</c> 再 <c>BeginScope</c>，快照值会等于目标应用本身，释放时<b>不会</b>回滚
/// （上下文残留到后续调用，多应用下造成串号）。
/// </para>
/// <para>守卫与组件生成类一致：<c>AppKey.IsValid</c> 格式校验 → 授权判定 → 才解析应用。</para>
/// </remarks>
public sealed class ChannelsAppContextSwitcher : AsyncLocalAppContextSwitcher, IChannelsAppContextSwitcher
{
    private readonly IChannelsAppManager _appManager;
    private readonly IAppAccessAuthorizer? _authorizer;

    /// <summary>创建应用上下文切换器。</summary>
    /// <param name="appManager">应用管理器（解析目标应用上下文）。</param>
    /// <param name="authorizer">应用切换授权器（可选；为 null 表示不做授权判定）。</param>
    public ChannelsAppContextSwitcher(IChannelsAppManager appManager, IAppAccessAuthorizer? authorizer = null)
    {
        _appManager = appManager ?? throw new ArgumentNullException(nameof(appManager));
        _authorizer = authorizer;
    }

    /// <summary>
    /// 切换到指定应用上下文，并返回作用域以在结束时自动恢复进入前的上下文（推荐入口）。
    /// </summary>
    /// <param name="appKey">应用标识。</param>
    /// <returns>释放时恢复之前上下文的作用域对象。建议配合 <c>using</c> 使用。</returns>
    public IDisposable UseAppScope(string appKey) => BeginScope(ResolveAppContext(appKey));

    /// <summary>
    /// 切换到默认应用上下文，并返回作用域以在结束时自动恢复进入前的上下文（推荐入口）。
    /// </summary>
    /// <returns>释放时恢复之前上下文的作用域对象。建议配合 <c>using</c> 使用。</returns>
    public IDisposable UseDefaultAppScope() => BeginScope(_appManager.GetDefaultApp());

    /// <inheritdoc />
    /// <remarks>发布前即定稿的新产品线契约：恒为 <c>ChannelsTokenTypes.AccessToken</c>。</remarks>
    [Obsolete("组件 IAppContextSwitcher.GetTokenAsync 已废弃；请改用 IChannelsAppContext.GetTokenManager(...).GetTokenAsync() 或 ITokenProvider。")]
    public Task<string> GetTokenAsync()
    {
        var context = Current
            ?? throw new InvalidOperationException(
                "当前无应用上下文：请先经 IChannelsAppContextSwitcher.UseAppScope(appKey) 切换。");

        return context.GetTokenManager(ChannelsTokenTypes.AccessToken).GetTokenAsync(CancellationToken.None);
    }

    /// <summary>
    /// 解析目标应用上下文（守卫 + 解析，<b>不切换</b>）。
    /// </summary>
    /// <param name="appKey">应用标识（可能来自外部输入）。</param>
    /// <returns>目标应用上下文实例。</returns>
    private IChannelsAppContext ResolveAppContext(string appKey)
    {
        if (!WechatAppKeyValidator.IsValid(appKey))
        {
            throw new ArgumentException(
                "appKey 格式非法：只能由字母、数字、'.'、'_'、'-' 组成，首字符必须是字母或数字，长度不超过 128。",
                nameof(appKey));
        }

        if (_authorizer != null && !_authorizer.CanSwitchTo(appKey))
        {
            throw new InvalidOperationException($"当前调用主体无权切换到应用 '{appKey}'，切换已被拒绝。");
        }

        return _appManager.GetApp(appKey);
    }
}