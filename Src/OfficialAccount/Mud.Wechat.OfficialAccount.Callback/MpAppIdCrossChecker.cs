// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.OfficialAccount.Callback;

/// <summary>
/// 回调 <c>AppId</c> 与多应用基座配置的<b>惰性可选</b>交叉校验（方案 §4 要点 1）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何惰性可选</b>：回调包必须对「仅接回调、不调 API」的宿主自足 —— 启动期强依赖 <c>IMpAppManager</c>
/// 会破坏该自足性并引入注册顺序耦合。故经 <c>IServiceProvider.GetService&lt;&gt;()</c> 惰性解析
/// （与 <c>IWechatAuthorizationCoordinator</c> 既有先例同构），未注册即整体跳过。
/// </para>
/// <para>
/// <b>为何首次请求期校验且不 fail-fast</b>：不一致只可能是「宿主配错」或「同一公众号被两条配置引用」，
/// 且回调凭据（<c>MpCallbackOptions.Apps</c>）是回调链路的**权威来源**；故记 <c>Error</c> 后
/// **按回调配置继续**（拒绝请求会让回调彻底不可用，且回调-only 宿主无从修复该告警）。
/// 结论按 appKey <b>缓存一次</b>，避免每包一次字典查找与日志风暴。
/// </para>
/// </remarks>
internal sealed class MpAppIdCrossChecker
{
    private readonly IServiceProvider _services;
    private readonly ILogger _logger;
    private readonly ConcurrentDictionary<string, byte> _checkedKeys = new(StringComparer.Ordinal);
    private readonly object _resolveLock = new();
    private IMpAppManager? _manager;
    private bool _resolved;

    internal MpAppIdCrossChecker(IServiceProvider services, ILogger logger)
    {
        _services = services;
        _logger = logger;
    }

    /// <summary>对指定应用键做一次（幂等）AppId 一致性校验。</summary>
    /// <param name="appKey">回调路由应用键。</param>
    /// <param name="callbackAppId">回调配置声明的 AppId。</param>
    internal void Check(string appKey, string callbackAppId)
    {
        if (string.IsNullOrEmpty(appKey) || !_checkedKeys.TryAdd(appKey, 0))
        {
            return;
        }

        var manager = ResolveManager();
        if (manager == null)
        {
            return;
        }

        try
        {
            if (!manager.TryGetConfig(appKey, out var config) || config == null)
            {
                return;
            }

            var configuredAppId = config.AppId;
            if (string.IsNullOrEmpty(configuredAppId)
                || string.Equals(configuredAppId, callbackAppId, StringComparison.Ordinal))
            {
                return;
            }

            _logger.LogError(
                "回调 AppId 与多应用基座配置不一致：appKey {AppKey} 的回调配置 AppId = {CallbackAppId}，" +
                "而 MpAppConfig.AppId = {ConfiguredAppId}。回调链路按**回调配置**继续处理（回调配置是回调链路的权威来源）；" +
                "请修正其中一处，否则解密后的 appid 校验可能与非本应用的消息混流。",
                appKey, callbackAppId, configuredAppId);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // 交叉校验是**加固**而非安全闸：任何异常都不得影响回调接收（主闸是 appid 解密比对本身）。
            _logger.LogWarning(ex, "回调 AppId 交叉校验失败（已忽略，不影响回调处理）。appKey: {AppKey}", appKey);
        }
    }

    private IMpAppManager? ResolveManager()
    {
        if (_resolved)
        {
            return _manager;
        }

        lock (_resolveLock)
        {
            if (_resolved)
            {
                return _manager;
            }

            try
            {
                _manager = _services.GetService(typeof(IMpAppManager)) as IMpAppManager;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _manager = null;
                _logger.LogWarning(ex, "多应用基座（IMpAppManager）解析失败，跳过回调 AppId 交叉校验。");
            }

            _resolved = true;
            if (_manager == null)
            {
                _logger.LogDebug("未注册多应用基座（IMpAppManager），跳过回调 AppId 交叉校验（回调-only 宿主为合法形态）。");
            }

            return _manager;
        }
    }
}
