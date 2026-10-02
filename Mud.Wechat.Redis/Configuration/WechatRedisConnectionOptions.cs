// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Redis.Configuration;

/// <summary>
/// Redis 连接配置（映射 <c>StackExchange.Redis.ConfigurationOptions</c> 对应字段）。
/// </summary>
public class WechatRedisConnectionOptions
{
    /// <summary>服务器地址。支持 <c>host:port</c> / <c>redis://</c> / <c>rediss://</c>；
    /// <c>rediss://</c> 前缀在 <c>RedisConnectionFactory.Build</c> 中显式推导 TLS
    /// （StackExchange.Redis 3.3.0 的 <c>Parse</c> 不会因 scheme 自动启用 TLS）。</summary>
    public string ServerAddress { get; set; } = "localhost:6379";

    /// <summary>认证密码（敏感凭据，不得写入日志；配置对象 <c>ToString()</c> 已脱敏）。</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>是否启用 TLS（与 <c>rediss://</c> scheme 推导取或）。</summary>
    public bool Ssl { get; set; }

    /// <summary>默认数据库编号（null = 使用服务端默认 db0）。</summary>
    public int? DefaultDatabase { get; set; }

    /// <summary>连接超时（毫秒）。校验下限 1000。</summary>
    public int ConnectTimeout { get; set; } = 5000;

    /// <summary>同步操作超时（毫秒）。校验下限 1000。</summary>
    public int SyncTimeout { get; set; } = 5000;

    /// <summary>连接重试次数。校验下限 0。</summary>
    public int ConnectRetry { get; set; } = 3;

    /// <summary>
    /// 连接不可用时是否终止启动（fail-fast）。默认 <c>true</c>；
    /// 置 <c>false</c> 时启动预热仅告警，交由 StackExchange.Redis 自动重连。
    /// </summary>
    public bool AbortOnConnectFail { get; set; } = true;
}

/// <summary>
/// Redis 高级连接配置。
/// </summary>
public class WechatRedisAdvancedOptions
{
    /// <summary>是否允许管理类命令（SCAN 的 <c>KeysAsync</c> 不需要；仅为运维扩展预留）。默认 <c>false</c>。</summary>
    public bool AllowAdmin { get; set; }

    /// <summary>客户端名称（null 兜底 <c>Wechat-Redis-{MachineName}</c>，便于服务端识别连接来源）。</summary>
    public string? ClientName { get; set; }
}
