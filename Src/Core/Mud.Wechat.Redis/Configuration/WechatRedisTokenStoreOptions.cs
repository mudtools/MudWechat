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

    /// <summary>
    /// U-2：写入侧启用官方 <b>v2 版本化信封</b>（<c>v2:{expiresInSeconds}|{token}</c> + 显式过期态）。
    /// 默认 <c>false</c> = 维持官方默认形态 <c>{expiresInSeconds}|{token}</c>（逐位零变化）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>两阶段切换纪律</b>：开启前必须先把<b>全部读取实例</b>升级到含 v2 前缀识别的本库版本
    /// （升级即得、无需改配置）；旧读取方读 v2 值必然 miss（方向安全、走刷新路径，但会持续重复刷新）。
    /// <b>禁止「写入方先升级」的逆序切换</b>。
    /// </para>
    /// <para>
    /// 开启时必须同时置 <see cref="AcknowledgeVersionedEnvelopeReadersUpgraded"/>——未确认时
    /// <c>WechatRedisOptions.Validate()</c>（IValidateOptions 管线）与官方存储构造期<b>双重 fail-fast</b>。
    /// 本侧书面结论为<b>暂缓启用写入侧</b>（令牌管理器无「显式过期 vs 缺失」区分需求），
    /// 本开关镜像官方形态、供宿主按自身运维纪律选开。
    /// </para>
    /// </remarks>
    public bool UseVersionedEnvelope { get; set; }

    /// <summary>
    /// U-2：两阶段切换的<b>显式确认</b>——声明「全部读取实例已升级至含 v2 解码的本库版本」。
    /// 仅在 <see cref="UseVersionedEnvelope"/> 为 <c>true</c> 时被要求为 <c>true</c>；默认 <c>false</c>。
    /// </summary>
    public bool AcknowledgeVersionedEnvelopeReadersUpgraded { get; set; }
}
