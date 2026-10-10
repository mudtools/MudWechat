// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using StackExchange.Redis;

namespace Mud.Wechat.Redis.Services;

/// <summary>
/// Redis 服务器节点选择与取消语义工具（对齐 Mud.Feishu.Redis 的 <c>RedisStoreHelper</c>）。
/// </summary>
internal static class RedisServerHelper
{
    /// <summary>
    /// 将 <see cref="CancellationToken"/> 转换为 StackExchange.Redis 的 <see cref="CommandFlags"/>。
    /// </summary>
    /// <remarks>
    /// StackExchange.Redis 不直接接受 CancellationToken 作为取消信号；本方法恒返回
    /// <see cref="CommandFlags.None"/>（入参不被消费，保留仅为调用点形状统一）。
    /// 实际取消由调用方在命令之间 <c>cancellationToken.ThrowIfCancellationRequested()</c> 实现——
    /// 单次 Redis 命令（含 SCAN 一页、批量删除）不可中断，等待上限由 SyncTimeout 决定。
    /// </remarks>
    /// <param name="cancellationToken">取消令牌（命令之间生效）。</param>
    /// <returns>恒为 <see cref="CommandFlags.None"/>。</returns>
    public static CommandFlags ToCommandFlags(CancellationToken cancellationToken) => CommandFlags.None;

    /// <summary>
    /// 获取可用的 Redis 服务器节点（单节点，优选主节点）。
    /// </summary>
    /// <remarks>
    /// 遍历所有端点选择首个 <c>IsConnected &amp;&amp; !IsReplica</c> 的主节点；
    /// 全部不可用或全为副本时回退首个节点（由调用方处理后续命令异常）。
    /// </remarks>
    /// <param name="redis">连接多路复用器。</param>
    /// <returns>可用的服务器实例。</returns>
    /// <exception cref="InvalidOperationException">连接未配置任何端点。</exception>
    public static IServer GetServer(IConnectionMultiplexer redis)
    {
        var endpoints = redis.GetEndPoints();
        if (endpoints.Length == 0)
        {
            throw new InvalidOperationException("Redis 连接多路复用器未配置任何端点，无法获取服务器实例。");
        }

        foreach (var endpoint in endpoints)
        {
            try
            {
                var server = redis.GetServer(endpoint);
                if (server.IsConnected && !server.IsReplica)
                {
                    return server;
                }
            }
            catch
            {
                // 跳过不可访问的节点，继续尝试下一个。
            }
        }

        return redis.GetServer(endpoints[0]);
    }

    /// <summary>
    /// 获取全部已连接且非副本的 Redis 服务器节点（Cluster 化扫描）。
    /// </summary>
    /// <remarks>
    /// 遍历所有端点返回全部 <c>IsConnected &amp;&amp; !IsReplica</c> 的主节点，用于 SCAN/删除类 API
    /// 在 Cluster 下覆盖全部分片（防漏删/漏列）；无可用主节点时回退首个节点。
    /// </remarks>
    /// <param name="redis">连接多路复用器。</param>
    /// <returns>全部可用主节点的服务器实例集合。</returns>
    /// <exception cref="InvalidOperationException">连接未配置任何端点。</exception>
    public static IEnumerable<IServer> GetServers(IConnectionMultiplexer redis)
    {
        var endpoints = redis.GetEndPoints();
        if (endpoints.Length == 0)
        {
            throw new InvalidOperationException("Redis 连接多路复用器未配置任何端点，无法获取服务器实例。");
        }

        var servers = new List<IServer>();
        foreach (var endpoint in endpoints)
        {
            try
            {
                var server = redis.GetServer(endpoint);
                if (server.IsConnected && !server.IsReplica)
                {
                    servers.Add(server);
                }
            }
            catch
            {
                // 跳过不可访问的节点。
            }
        }

        if (servers.Count == 0)
        {
            servers.Add(redis.GetServer(endpoints[0]));
        }

        return servers;
    }
}
