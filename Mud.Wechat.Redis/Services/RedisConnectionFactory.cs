// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using StackExchange.Redis;

namespace Mud.Wechat.Redis.Services;

/// <summary>
/// 连接选项装配单一出口：<see cref="WechatRedisOptions"/> → <see cref="ConfigurationOptions"/>（可单测）。
/// </summary>
internal static class RedisConnectionFactory
{
    /// <summary>TLS scheme 前缀（<c>rediss://</c>）。</summary>
    internal const string RedissScheme = "rediss://";

    /// <summary>
    /// 由配置装配 <see cref="ConfigurationOptions"/>。
    /// </summary>
    /// <param name="options">模块配置。</param>
    /// <returns>连接选项。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> 为 null。</exception>
    public static ConfigurationOptions Build(WechatRedisOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        var connection = options.Connection ?? new WechatRedisConnectionOptions();
        var advanced = options.Advanced ?? new WechatRedisAdvancedOptions();

        var config = ConfigurationOptions.Parse(connection.ServerAddress);
        config.ConnectTimeout = connection.ConnectTimeout;
        config.SyncTimeout = connection.SyncTimeout;

        // R2-26：scheme 推导必须显式——实测 StackExchange.Redis 3.3.0 的 Parse
        // 不会因 rediss:// 前缀自动置 Ssl，须在此显式取或。
        config.Ssl = config.Ssl || connection.Ssl || RequiresSsl(connection.ServerAddress);
        config.Password = string.IsNullOrEmpty(connection.Password) ? config.Password : connection.Password;
        config.AllowAdmin = advanced.AllowAdmin;
        config.AbortOnConnectFail = connection.AbortOnConnectFail;
        config.ConnectRetry = connection.ConnectRetry;
        config.DefaultDatabase = connection.DefaultDatabase;
        config.ClientName = advanced.ClientName ?? $"Wechat-Redis-{Environment.MachineName}";

        return config;
    }

    /// <summary>判断服务器地址是否要求 TLS（<c>rediss://</c> scheme）。</summary>
    /// <param name="serverAddress">服务器地址。</param>
    /// <returns>要求 TLS 返回 <c>true</c>。</returns>
    public static bool RequiresSsl(string? serverAddress)
        => serverAddress is { Length: > 0 }
           && serverAddress.StartsWith(RedissScheme, StringComparison.OrdinalIgnoreCase);
}
