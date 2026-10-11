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
/// 开放平台令牌管理器注册表：按令牌类型键路由到对应管理器（errcode 恢复执行器经
/// <see cref="ITokenManagerRegistry"/> 定位失效目标）。
/// </summary>
/// <remarks>
/// <para>
/// <b>作用域正确性</b>：平台令牌键无作用域语义，直接返回平台管理器；授权方令牌键按
/// <b>当前上下文</b>（<see cref="IAppContextHolder.Current"/>，缺省回退平台上下文）解析——
/// 上下文不带授权方作用域时 fail-fast 抛出（对齐 <see cref="OpenPlatformAppContext.GetTokenManager"/> 的错配语义）。
/// </para>
/// <para>
/// 未知键返回 <c>null</c>（组件契约：解析失败由调用方决定回退策略，不 fail-fast 打断恢复链）。
/// </para>
/// </remarks>
internal sealed class OpenPlatformTokenManagerRegistry : ITokenManagerRegistry
{
    private readonly IAppContextHolder _holder;
    private readonly IOpenPlatformAppManager _appManager;

    /// <summary>创建注册表。</summary>
    /// <param name="holder">应用上下文持有器（读取当前作用域）。</param>
    /// <param name="appManager">应用管理器（定位管理器所属上下文）。</param>
    public OpenPlatformTokenManagerRegistry(IAppContextHolder holder, IOpenPlatformAppManager appManager)
    {
        _holder = holder ?? throw new ArgumentNullException(nameof(holder));
        _appManager = appManager ?? throw new ArgumentNullException(nameof(appManager));
    }

    /// <inheritdoc />
    public ITokenManager? Resolve(string tokenManagerKey)
    {
        if (string.Equals(tokenManagerKey, OpenPlatformTokenTypes.ComponentAccessToken, StringComparison.Ordinal))
        {
            return ResolveFrom(_appManager.PlatformContext, tokenManagerKey);
        }

        if (string.Equals(tokenManagerKey, OpenPlatformTokenTypes.AuthorizerAccessToken, StringComparison.Ordinal))
        {
            var context = _holder.Current as IOpenPlatformAppContext ?? _appManager.GetDefaultApp();
            return ResolveFrom(context, tokenManagerKey);
        }

        return null;
    }

    private static ITokenManager? ResolveFrom(IOpenPlatformAppContext context, string tokenManagerKey)
        => context.GetTokenManager(tokenManagerKey) is { } manager ? manager : null;
}
