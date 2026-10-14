// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Redis.Configuration;

/// <summary>
/// 官方分布式令牌存储（<c>Mud.HttpUtils.Redis</c> 的 <c>RedisTokenStore</c>）在下游的配置映射
/// （A-5 分层共存裁决：通用 <c>ITokenStore</c> 面迁官方包）。
/// </summary>
/// <remarks>
/// <para>
/// <b>键命名空间与本院端口互不干扰</b>：官方键布局
/// <c>{KeyPrefix}:{tokenType}:{appKey}:access|refresh</c>（默认前缀 <see cref="DefaultKeyPrefix"/>），
/// 与本院四类键（<c>{本院前缀}:token|corpa|ticket|replay:…</c>）前缀不同 ⇒ 共库无需数据迁移。
/// </para>
/// <para>
/// <b>为何在下游持镜像 DTO 而非直配官方选项类型</b>：配置绑定源生成器对下游编译单元内的类型
/// 生成绑定代码；镜像 DTO 使 <c>WechatRedis</c> 配置节保持自包含（官方包类型不出现在下游公开配置面），
/// 映射在注册期单点完成（<c>AddWechatRedisCore</c>）。
/// </para>
/// </remarks>
public class WechatRedisTokenStoreOptions
{
    /// <summary>官方令牌存储键前缀默认值（与方案 F-3 钉死的 <c>mud:httputils</c> 一致）。</summary>
    public const string DefaultKeyPrefix = "mud:httputils";

    /// <summary>
    /// 官方存储键前缀（键布局 <c>{KeyPrefix}:{tokenType}:{appKey}:access|refresh</c>）。
    /// 空值由注册期兜底 <see cref="DefaultKeyPrefix"/>（<c>Validate()</c> 对配置路径严格校验）。
    /// </summary>
    public string KeyPrefix { get; set; } = DefaultKeyPrefix;

    /// <summary>
    /// 应用维度命名空间段（多应用隔离）；为空时取官方 <c>shared</c> 共享段。
    /// 真实 appKey 不应取值 <c>shared</c>（会与共享命名空间碰撞）。
    /// </summary>
    public string? AppKey { get; set; }

    /// <summary>
    /// 兜底 TTL：写入侧无过期信息（<c>expiresInSeconds &lt;= 0</c>）时使用；
    /// <c>null</c> = 不设 TTL（Redis 侧永久保留，直至被移除/覆盖）。
    /// 生产环境建议显式配置（防 store 无界滞留令牌）。
    /// </summary>
    public TimeSpan? DefaultTtl { get; set; }

    /// <summary>Redis 数据库编号；<c>-1</c> 表示连接默认库。</summary>
    public int Database { get; set; } = -1;
}
