// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication.TokenManager;

/// <summary>
/// 企业微信令牌持久化仓储（可选的跨进程共享 / 落盘层）。
/// </summary>
/// <remarks>
/// <para>
/// 定位（Mud.HttpUtils v2.0.9 契约核实）：令牌的进程内缓存、过期判定、并发刷新去重由
/// <see cref="TokenManagerBase"/> 内置（<see cref="ITokenCache{T}"/>，可在管理器构造时注入替换实现）。
/// 本仓储重新定位为<b>可选的持久化 / 跨进程共享层</b>（多实例部署共享令牌或令牌落盘时实现），
/// 默认注册为进程内实现、可不接分布式存储。
/// </para>
/// <para>
/// 启用持久化时，<c>WechatAppTokenManagerBase</c> 经框架桥接器
/// <see cref="TokenStoreBackedTokenCache{T}"/> 将本仓储接入管理器管线
/// （内存镜像 + 异步写穿 + 补偿重放 + 冷启动水合/读穿透），无需自建写穿/恢复叠层。
/// </para>
/// </remarks>
public interface IWechatTokenStore : ITokenStore
{
}

/// <summary>
/// 令牌持久层批量删除能力（可选实现，W1/M10）：退役清库/凭据轮换的 N 次逐键删除收敛为一次批量提交。
/// </summary>
/// <remarks>
/// <para>
/// 能力探测形态（决策 M10）：未实现本接口的 <see cref="IWechatTokenStore"/> 宿主自定义实现<b>零破坏</b>
/// ——管理器经 <c>is IWechatTokenStoreBatchRemove</c> 探测，未命中即回退逐键删除（既有行为，行为等价）。
/// </para>
/// <para>
/// 形态取舍：接口取「调用方算键 + 实现方批删」而非「服务端按前缀过滤」——
/// <c>PurgeAppTokensAsync</c> 按键的<b>中间段</b> appKey 匹配（tokenType 在首段且可变），前缀扫描不适用。
/// 实现方按自身能力 pipeline 化（如 Redis 用 batch/scan）。
/// </para>
/// </remarks>
public interface IWechatTokenStoreBatchRemove : IWechatTokenStore
{
    /// <summary>批量删除指定键，返回实际删除数量。</summary>
    /// <param name="keys">待删除的完整持久层键集合（三段式 <c>{tokenType}:{appKey}:{scopeKey}</c>）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>实际删除的键数量。</returns>
    Task<int> RemoveRangeAsync(IReadOnlyCollection<string> keys, CancellationToken cancellationToken = default);
}
