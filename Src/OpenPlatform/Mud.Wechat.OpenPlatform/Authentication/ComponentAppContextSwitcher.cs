// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils;
using Mud.Wechat.OpenPlatform.Abstractions;
using Mud.Wechat.OpenPlatform.Abstractions.Authentication;

namespace Mud.Wechat.OpenPlatform.Authentication;

/// <summary>
/// 开放平台应用上下文切换器实现。
/// </summary>
/// <remarks>
/// <para>
/// 继承组件 <see cref="AsyncLocalAppContextSwitcher"/> 复用其 <see cref="AsyncLocal{T}"/> 上下文传播语义
/// （<see cref="IAppContextHolder.Current"/> / <see cref="IAppContextHolder.SwitchTo"/> /
/// <see cref="IAppContextHolder.BeginScope(IMudAppContext)"/>），补齐授权方作用域入口。
/// </para>
/// <para>
/// <b>作用域组合方式（照抄公众号线）</b>：<see cref="UseAuthorizerScope"/> <b>不预先</b>调用
/// <see cref="IAppContextHolder.SwitchTo"/>，而是把解析出的上下文直接交给
/// <see cref="IAppContextHolder.BeginScope(IMudAppContext)"/> 建作用域（后者自行快照进入前上下文并在释放时
/// 还原）。若先 <c>SwitchTo</c> 再 <c>BeginScope</c>，快照值会等于目标上下文本身，释放时<b>不会</b>回滚
/// （上下文残留到后续调用，多授权方下造成串号）。
/// </para>
/// <para>
/// <b>幂等还原（对齐企微 <c>UseCorpScope</c>）</b>：嵌套进入同一授权方作用域时，内层释放后外层仍在目标作用域
/// ——由组件 <c>BeginScope</c> 的栈式快照语义天然保证。
/// </para>
/// </remarks>
internal sealed class ComponentAppContextSwitcher : AsyncLocalAppContextSwitcher, IComponentAppContextSwitcher
{
    private readonly IOpenPlatformAppManager _appManager;

    /// <summary>创建切换器。</summary>
    /// <param name="appManager">应用管理器（解析目标授权方上下文）。</param>
    public ComponentAppContextSwitcher(IOpenPlatformAppManager appManager)
    {
        _appManager = appManager ?? throw new ArgumentNullException(nameof(appManager));
    }

    /// <inheritdoc />
    /// <remarks>授权方作用域与平台作用域共用本入口：应用键为授权方 <c>appid</c> 时即授权方作用域。</remarks>
    public IDisposable UseAppScope(string appKey) => BeginScope(ResolveAuthorizerContext(appKey));

    /// <inheritdoc />
    /// <remarks>平台自身作用域（<see cref="IOpenPlatformAppManager.PlatformContext"/>）。</remarks>
    public IDisposable UseDefaultAppScope() => BeginScope(_appManager.GetDefaultApp());

    /// <inheritdoc />
    public IDisposable UseAuthorizerScope(string authorizerAppId)
        => BeginScope(ResolveAuthorizerContext(authorizerAppId));

    /// <inheritdoc />
    /// <remarks>发布前即定稿的新产品线契约：恒经当前上下文的令牌管理器取平台令牌。</remarks>
    [Obsolete("组件 IAppContextSwitcher.GetTokenAsync 已废弃；请改用 IOpenPlatformAppContext.GetTokenManager(...).GetTokenAsync() 或 ITokenProvider。")]
    public Task<string> GetTokenAsync()
    {
        var context = Current
            ?? throw new InvalidOperationException(
                "当前无应用上下文：请先经 IComponentAppContextSwitcher.UseAuthorizerScope(authorizerAppId) 或 UseDefaultAppScope() 切换。");

        return context.GetTokenManager(OpenPlatformTokenTypes.ComponentAccessToken)
            .GetTokenAsync(CancellationToken.None);
    }

    /// <summary>
    /// 解析目标授权方上下文（校验 + 解析，<b>不切换</b>）。
    /// </summary>
    /// <param name="authorizerAppId">授权方 <c>appid</c>（可能来自外部输入）。</param>
    /// <returns>目标授权方上下文实例。</returns>
    private IOpenPlatformAppContext ResolveAuthorizerContext(string authorizerAppId)
    {
        if (string.IsNullOrWhiteSpace(authorizerAppId))
        {
            throw new ArgumentException("授权方 appid 不能为空白。", nameof(authorizerAppId));
        }

        if (!WechatAppKeyValidator.IsValid(authorizerAppId))
        {
            throw new ArgumentException(
                "授权方 appid 格式非法：只能由字母、数字、'.'、'_'、'-' 组成，"
                + "首字符必须是字母或数字，长度不超过 128。",
                nameof(authorizerAppId));
        }

        return _appManager.GetApp(authorizerAppId);
    }
}
