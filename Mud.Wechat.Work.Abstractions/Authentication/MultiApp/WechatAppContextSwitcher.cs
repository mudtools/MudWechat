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
/// <b>MT-02 授权</b>：<see cref="UseApp"/> / <see cref="UseAppScope"/> / <see cref="BeginScope(string)"/>
/// 在切换前经 <see cref="IAppAccessAuthorizer"/> 授权（未注册实现时框架默认拒绝，本 SDK 在
/// <c>AddWechatTokenInfrastructure</c> 中注册放行型实现）。
/// </para>
/// <para>
/// <b>Mud.HttpUtils 3.0.0 适配（BC-27）</b>：本类除实现本接口外，还实现
/// <see cref="IAppScopeSwitcher"/>（作用域式推荐入口 <see cref="UseAppScope"/> / <see cref="UseDefaultAppScope"/>）。
/// 三个旧入口（<see cref="UseApp"/> / <see cref="UseDefaultApp"/> / <see cref="BeginScope(string)"/>）
/// 由 <see cref="IWechatAppContextSwitcher"/> 重声明接续，已标记 <c>[Obsolete]</c>，将在下一个大版本移除。
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
    public IMudAppContext UseApp(string appKey)
    {
        var context = ResolveAppContext(appKey);
        SwitchTo(context);
        return context;
    }

    /// <inheritdoc />
    public IMudAppContext UseDefaultApp()
    {
        var context = _appManager.GetDefaultApp();
        SwitchTo(context);
        return context;
    }

    /// <summary>
    /// 切换到指定应用上下文，并返回作用域以在结束时自动恢复进入前的上下文（推荐入口）。
    /// </summary>
    /// <param name="appKey">应用标识。</param>
    /// <returns>释放时恢复之前上下文的作用域对象。建议配合 <c>using</c> 使用。</returns>
    /// <remarks>
    /// <para>
    /// 守卫与框架生成类<b>逐字对齐</b>：<see cref="AppKey.IsValid"/> 格式校验 → <see cref="IAppAccessAuthorizer"/> 授权判定
    /// （授权器为 null 时本 SDK 的语义是「不判定」，见 <see cref="WechatAppContextSwitcher"/> 类注释）。
    /// </para>
    /// <para>
    /// <b>与 <see cref="IAppContextHolder.BeginScope(IMudAppContext)"/> 的组合方式（重要）</b>：本方法<b>不预先</b>调用
    /// <see cref="IAppContextHolder.SwitchTo"/>，而是把解析出的上下文直接交给
    /// <see cref="IAppContextHolder.BeginScope(IMudAppContext)"/> 建立作用域 —— 后者会自行快照进入前的上下文并在释放时还原。
    /// 若先 <c>SwitchTo</c> 再 <c>BeginScope</c>，快照值会等于目标应用本身，释放时便<b>不会</b>回滚
    /// （上下文残留到后续调用）。框架生成类的 <c>UseAppScope</c> 同为「不预先 SwitchTo」写法。
    /// </para>
    /// </remarks>
    public IDisposable UseAppScope(string appKey) => BeginScope(ResolveAppContext(appKey));

    /// <summary>
    /// 切换到默认应用上下文，并返回作用域以在结束时自动恢复进入前的上下文（推荐入口）。
    /// </summary>
    /// <returns>释放时恢复之前上下文的作用域对象。建议配合 <c>using</c> 使用。</returns>
    /// <remarks>
    /// 与框架生成类一致：默认应用路径<b>不做</b> appKey 格式校验与授权判定（无 appKey 输入）。
    /// 组合方式同 <see cref="UseAppScope"/>（不预先 <c>SwitchTo</c>，保证释放时正确回滚）。
    /// </remarks>
    public IDisposable UseDefaultAppScope() => BeginScope(_appManager.GetDefaultApp());

    /// <inheritdoc />
    /// <remarks>
    /// 与 <see cref="UseAppScope"/> 为<b>同一实现</b>（迁移别名）：本方法自 Mud.HttpUtils 3.0.0 起
    /// 委托给推荐入口，二者行为完全一致（含「释放时自动归还上下文」）。
    /// </remarks>
    public IDisposable BeginScope(string appKey) => UseAppScope(appKey);

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
                "当前无应用上下文：请先经 IWechatAppContextSwitcher.UseAppScope(appKey) 切换，或显式传入 appKey。");

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

    /// <summary>
    /// 解析目标应用上下文（<b>守卫 + 解析，不切换</b>）。
    /// </summary>
    /// <param name="appKey">应用标识（可能来自外部输入）。</param>
    /// <returns>目标应用上下文实例。</returns>
    /// <remarks>
    /// 守卫顺序与框架生成类（<c>ConstructorGenerator.GenerateAppKeyGuard</c>）一致：
    /// <b>格式校验 → 授权判定 → 才解析应用</b>，且拒绝路径<b>不得</b>触碰 <c>IWechatAppManager</c>。
    /// <para>
    /// Mud.HttpUtils 3.0.0 迁移补齐：原实现<b>缺少</b> <see cref="AppKey.IsValid"/> 格式校验 ——
    /// appKey 来自外部参数时会未经校验直接进入 <c>GetApp</c> 查询（防御缺口，见改造计划 §3.2）。
    /// 本方法不再负责切换，切换/建作用域由调用方按语义选择（<c>SwitchTo</c> 或 <c>BeginScope</c>）。
    /// </para>
    /// </remarks>
    private IMudAppContext ResolveAppContext(string appKey)
    {
        if (!AppKey.IsValid(appKey))
        {
            throw new ArgumentException(
                "appKey 格式非法：只能由字母、数字、'.'、'_'、'-' 组成，首字符必须是字母或数字，长度不超过 128。",
                nameof(appKey));
        }

        if (_authorizer != null && !_authorizer.CanSwitchTo(appKey))
        {
            throw new InvalidOperationException(
                $"当前调用主体无权切换到应用 '{AppKey.ToSafeText(appKey)}'，切换已被拒绝。");
        }

        return _appManager.GetApp(appKey);
    }
}