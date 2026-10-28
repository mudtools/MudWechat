// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication.MultiApp;

/// <summary>
/// 装配期所有权收集表（M1/F1）：登记 DI scope 之外的装配产物，装配中途失败时逆序确定性回收。
/// </summary>
/// <remarks>
/// 令牌管理器由 <c>new</c> 构造、不入 DI scope，且持有组件 <c>TokenManagerBase</c> 的两个维护 Timer
/// （清理 300s / 锁清理 600s，TimerQueue 根持）——装配中途失败若不回收即成永久孤儿（活动 Timer
/// 使 GC 无法回收管理器）。释放顺序与 <see cref="WechatAppContext.Dispose"/> 一致（先装配产物、后 scope）：
/// 管理器持有 scope 解析出的 tokenStore 等引用，须先于 scope 关闭释放。
/// 成功路径经 <see cref="Clear"/> 整体移交所有权（由 <see cref="WechatAppContext"/> 接管 Dispose）。
/// </remarks>
internal sealed class WechatOwnedResourceTracker
{
    private readonly List<IDisposable> _owned = new();
    private readonly ILogger _logger;

    /// <summary>创建所有权收集表。</summary>
    /// <param name="logger">日志器（回收异常逐个隔离记录，不吞原始装配异常）。</param>
    public WechatOwnedResourceTracker(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>登记装配产物；非 <see cref="IDisposable"/> 资源直接透传（零开销）。</summary>
    /// <typeparam name="T">装配产物类型。</typeparam>
    /// <param name="resource">刚构造的装配产物。</param>
    /// <returns>原资源（便于内联包装构造调用）。</returns>
    public T Track<T>(T resource) where T : class
    {
        if (resource is IDisposable disposable)
        {
            _owned.Add(disposable);
        }

        return resource;
    }

    /// <summary>所有权移交成功（上下文已构造）：清空登记表，装配失败兜底不再回收。</summary>
    public void Clear() => _owned.Clear();

    /// <summary>逆序释放全部已登记资源（后建管理器引用先建客户端，逆序天然满足依赖序）。</summary>
    public void DisposeAll()
    {
        for (var i = _owned.Count - 1; i >= 0; i--)
        {
            try
            {
                _owned[i].Dispose();
            }
            catch (Exception ex)
            {
                // 逐个隔离释放异常：不得吞掉/替换原始装配异常（调用方依赖原异常类型与消息）。
                _logger.LogDebug(ex, "装配失败回收孤儿资源异常。");
            }
        }

        _owned.Clear();
    }
}
