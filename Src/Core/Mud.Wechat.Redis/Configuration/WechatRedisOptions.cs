// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Redis.Configuration;

/// <summary>
/// Mud.Wechat Redis 分布式存储配置（配置节 <see cref="SectionName"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 嵌套 <see cref="Connection"/> / <see cref="Advanced"/>，无扁平旧键回填（WeChat 无历史包袱）。
/// 配置 DTO 禁用 <c>required</c>（配置绑定源生成器以 <c>new T()</c> 构造），校验收敛于 <see cref="Validate()"/>。
/// </para>
/// <para>
/// 安全：<see cref="WechatRedisConnectionOptions.Password"/> 为敏感凭据，
/// <see cref="ToString()"/> 已脱敏，不得把本对象整体写入日志之外的明文输出。
/// </para>
/// </remarks>
public class WechatRedisOptions
{
    /// <summary>配置节名称。</summary>
    public const string SectionName = "WechatRedis";

    /// <summary>键前缀默认值（与 Mud.Feishu.Redis 的 <c>feishu</c> 对称，双 SDK 共库时天然隔离）。</summary>
    public const string DefaultKeyPrefix = "wechat";

    /// <summary>连接配置（服务器地址、凭据、超时、TLS）。</summary>
    public WechatRedisConnectionOptions Connection { get; set; } = new();

    /// <summary>高级连接配置（AllowAdmin、客户端名称）。</summary>
    public WechatRedisAdvancedOptions Advanced { get; set; } = new();

    /// <summary>
    /// 统一键前缀（RD1：四类键 token/corpa/ticket/replay 共享的唯一前缀旋钮）。
    /// 空值由 <see cref="NormalizeKeyPrefix"/> 兜底 <see cref="DefaultKeyPrefix"/>；
    /// <see cref="Validate()"/> 对配置路径做严格校验（非空且不以 <c>*</c> 开头）。
    /// </summary>
    public string KeyPrefix { get; set; } = DefaultKeyPrefix;

    /// <summary>
    /// suite_ticket 存储的 TTL（RD5）。默认 <see cref="TimeSpan.Zero"/> = 不过期（对齐 InMemory 永驻 + 覆盖写保新鲜）；
    /// 可配正值清理死套件残留（建议 ≥ 2× 推送间隔即 ≥ 20 分钟）。
    /// </summary>
    public TimeSpan SuiteTicketTtl { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// 校验配置合法性（注册期 / 启动期 fail-fast）。
    /// </summary>
    /// <exception cref="InvalidOperationException">配置非法。</exception>
    public void Validate()
    {
        var connection = Connection ?? new WechatRedisConnectionOptions();

        var serverAddress = connection.ServerAddress ?? string.Empty;
        if (serverAddress.Length == 0)
        {
            throw new InvalidOperationException("WechatRedis:Connection:ServerAddress 不能为空。");
        }

        var hasPort = serverAddress.Contains(':');
        var hasScheme = serverAddress.StartsWith("redis://", StringComparison.OrdinalIgnoreCase)
                        || serverAddress.StartsWith("rediss://", StringComparison.OrdinalIgnoreCase);
        if (!hasPort && !hasScheme)
        {
            throw new InvalidOperationException(
                "WechatRedis:Connection:ServerAddress 格式非法：须为 host:port / redis:// / rediss:// 形态。");
        }

        if (connection.ConnectTimeout < 1000)
        {
            throw new InvalidOperationException("WechatRedis:Connection:ConnectTimeout 不得小于 1000 毫秒。");
        }

        if (connection.SyncTimeout < 1000)
        {
            throw new InvalidOperationException("WechatRedis:Connection:SyncTimeout 不得小于 1000 毫秒。");
        }

        if (connection.ConnectRetry < 0)
        {
            throw new InvalidOperationException("WechatRedis:Connection:ConnectRetry 不得为负数。");
        }

        if (string.IsNullOrWhiteSpace(KeyPrefix))
        {
            throw new InvalidOperationException(
                $"WechatRedis:KeyPrefix 不能为空（空前缀会使 SCAN 模式退化为全库匹配；缺省值 {DefaultKeyPrefix}）。");
        }

        if (KeyPrefix.StartsWith("*"))
        {
            throw new InvalidOperationException("WechatRedis:KeyPrefix 不能以 '*' 开头（通配符前缀会使 SCAN 匹配所有键）。");
        }

        if (SuiteTicketTtl < TimeSpan.Zero)
        {
            throw new InvalidOperationException("WechatRedis:SuiteTicketTtl 不得为负数（Zero 表示不过期）。");
        }
    }

    /// <summary>
    /// 键前缀规范化（消费点兜底）：null/空白回退 <see cref="DefaultKeyPrefix"/>。
    /// 配置路径由 <see cref="Validate()"/> 严格校验；本方法为直接构造 <see cref="WechatRedisOptions"/>
    ///（未经校验管线）的防御兜底。
    /// </summary>
    internal static string NormalizeKeyPrefix(string? keyPrefix)
        => string.IsNullOrWhiteSpace(keyPrefix) ? DefaultKeyPrefix : keyPrefix.Trim();

    /// <summary>脱敏描述（Password 不落明文）。</summary>
    public override string ToString()
    {
        var connection = Connection ?? new WechatRedisConnectionOptions();
        return $"WechatRedis {{ ServerAddress = {connection.ServerAddress}, KeyPrefix = {KeyPrefix}, "
               + $"SuiteTicketTtl = {SuiteTicketTtl}, AbortOnConnectFail = {connection.AbortOnConnectFail}, Password = *** }}";
    }
}
