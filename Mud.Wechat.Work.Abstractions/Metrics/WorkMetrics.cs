// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics.Metrics;
using Mud.Wechat.Abstractions.Observability;

namespace Mud.Wechat.Work.Abstractions.Metrics;

/// <summary>
/// 企业微信产品线性能指标源。
/// </summary>
/// <remarks>
/// <para>
/// <see cref="MeterName"/> = <c>Mud.Wechat.Work</c>，指标名不带产品线前缀（产品线已由 Meter 名 +
/// <see cref="WechatActivitySource.Tags.Product"/> 双重编码，避免三重冗余）。
/// </para>
/// <para>
/// HTTP 请求指标由 <c>Mud.HttpUtils.MudHttpMeter</c> 自动采集（<c>mud.http.requests</c> / <c>mud.http.request.duration</c>）。
/// Token 刷新指标由 <c>Mud.HttpUtils.TokenManagerBase</c> 自动采集（<c>mud.token.refresh</c> / <c>mud.token.refresh.duration</c>）。
/// 本类只定义 WeChat 特有指标（回调接收、事件分发、令牌失效恢复、Redis 存储、授权编排）。
/// </para>
/// </remarks>
public static class WorkMetrics
{
    /// <summary>
    /// Meter 名称，遵循 OTel 命名约定（产品线段）。
    /// </summary>
    public const string MeterName = "Mud.Wechat.Work";

    /// <summary>
    /// Meter 版本（与 <see cref="WechatActivitySource.Version"/> 一致）。
    /// </summary>
    public const string Version = WechatActivitySource.Version;

    /// <summary>
    /// 静态 Meter 实例。
    /// </summary>
    public static readonly Meter Instance = new(MeterName, Version);

    // ── 回调接收指标 ──

    /// <summary>
    /// 回调入站计数（维度：app_key, channel, outcome）。
    /// </summary>
    public static readonly Counter<long> CallbackRequestCount = Instance.CreateCounter<long>(
        "callback.request",
        unit: "{request}",
        description: "企业微信回调入站计数（含 echo / receive / bot）");

    /// <summary>
    /// 回调全程耗时直方图（毫秒，维度：app_key, channel）。
    /// </summary>
    public static readonly Histogram<double> CallbackRequestDuration = Instance.CreateHistogram<double>(
        "callback.request.duration",
        unit: "ms",
        description: "企业微信回调全程耗时分布");

    /// <summary>
    /// SHA1 抗重放指纹命中/未命中计数（维度：app_key, dedup_type, outcome）。
    /// </summary>
    public static readonly Counter<long> CallbackDeduplicationCount = Instance.CreateCounter<long>(
        "callback.deduplication",
        unit: "{operation}",
        description: "回调抗重放指纹命中/未命中计数");

    // ── 事件分发指标 ──

    /// <summary>
    /// 事件分发次数（维度：app_key, event_type, handler_type, outcome）。
    /// </summary>
    public static readonly Counter<long> EventHandlingCount = Instance.CreateCounter<long>(
        "event.handling",
        unit: "{event}",
        description: "企业微信事件分发次数");

    /// <summary>
    /// 事件处理耗时直方图（毫秒，维度：app_key, event_type）。
    /// </summary>
    public static readonly Histogram<double> EventHandlingDuration = Instance.CreateHistogram<double>(
        "event.handling.duration",
        unit: "ms",
        description: "企业微信事件处理耗时分布");

    // ── 令牌/凭据指标 ──

    /// <summary>
    /// errcode 失效判定 → 令牌失效恢复次数（维度：app_key, token_owner, outcome）。
    /// </summary>
    public static readonly Counter<long> TokenInvalidationCount = Instance.CreateCounter<long>(
        "token.invalidation",
        unit: "{operation}",
        description: "令牌失效判定与恢复次数");

    // ── Redis 存储指标 ──

    /// <summary>
    /// Redis 存储操作计数（维度：app_key, command, outcome）。
    /// </summary>
    public static readonly Counter<long> RedisOperationCount = Instance.CreateCounter<long>(
        "redis.operation",
        unit: "{operation}",
        description: "Redis 存储操作计数（含失败分类）");

    /// <summary>
    /// Redis 操作耗时直方图（毫秒，维度：app_key, command）。
    /// </summary>
    public static readonly Histogram<double> RedisOperationDuration = Instance.CreateHistogram<double>(
        "redis.operation.duration",
        unit: "ms",
        description: "Redis 存储操作耗时分布");

    // ── 授权编排指标 ──

    /// <summary>
    /// 授权换码/刷新/撤销次数（维度：app_key, outcome）。
    /// </summary>
    public static readonly Counter<long> AuthorizationExchangeCount = Instance.CreateCounter<long>(
        "authorization.exchange",
        unit: "{operation}",
        description: "授权换码/刷新/撤销次数");

    // ── 标签常量 ──

    /// <summary>
    /// 企业微信产品线特有标签常量集合。
    /// </summary>
    public static class Tags
    {
        /// <summary>回调通道（app / suite / bot）</summary>
        public const string Channel = "wechat.work.channel";

        /// <summary>事件类型（EventTypeKey）</summary>
        public const string EventType = "wechat.work.event_type";

        /// <summary>事件处理器类型名</summary>
        public const string HandlerType = "wechat.work.handler_type";

        /// <summary>去重类型</summary>
        public const string DedupType = "wechat.work.dedup_type";

        /// <summary>令牌归属域（internal / corp / suite + provider）</summary>
        public const string TokenOwner = "wechat.work.token_owner";

        /// <summary>Redis 命令/操作名</summary>
        public const string RedisCommand = "wechat.work.redis_command";
    }

    // ── 受控枚举 ──

    /// <summary>
    /// 令牌归属域取值（对齐 <see cref="WechatTokenManagerKeys"/> 的归属域）。
    /// </summary>
    public static class TokenOwners
    {
        /// <summary>自建应用令牌</summary>
        public const string Internal = "internal";

        /// <summary>企业级令牌（代开发 gettoken）</summary>
        public const string Corp = "corp";

        /// <summary>套件令牌</summary>
        public const string Suite = "suite";

        /// <summary>服务商令牌</summary>
        public const string Provider = "provider";
    }

    /// <summary>
    /// Redis 命令名取值（受控枚举）。
    /// </summary>
    public static class RedisCommands
    {
        /// <summary>令牌：读取</summary>
        public const string TokenGet = "token_get";

        /// <summary>令牌：写入</summary>
        public const string TokenSet = "token_set";

        /// <summary>令牌：移除</summary>
        public const string TokenRemove = "token_remove";

        /// <summary>企业授权：读取</summary>
        public const string CorpStoreGet = "corp_store_get";

        /// <summary>企业授权：写入</summary>
        public const string CorpStoreSet = "corp_store_set";

        /// <summary>企业授权：删除</summary>
        public const string CorpStoreRemove = "corp_store_remove";

        /// <summary>套件票据：读取</summary>
        public const string TicketGet = "ticket_get";

        /// <summary>套件票据：写入</summary>
        public const string TicketSet = "ticket_set";

        /// <summary>抗重放：尝试标记</summary>
        public const string ReplayTryMark = "replay_trymark";

        /// <summary>抗重放：标记完成</summary>
        public const string ReplayMark = "replay_mark";
    }

    /// <summary>
    /// Redis 操作结果取值（受控枚举，对齐 <c>WechatRedisErrors</c> 的 <c>ShouldWrap</c> 失败分类）。
    /// </summary>
    /// <remarks>
    /// 注意 SE.Redis 3.3.0 的 <c>RedisTimeoutException</c> 继承 <c>TimeoutException</c> 而非
    /// <c>RedisException</c>，裸 <c>catch (RedisException)</c> 会漏捕获超时。
    /// </remarks>
    public static class RedisOutcomes
    {
        /// <summary>操作成功</summary>
        public const string Success = "success";

        /// <summary>操作超时（RedisTimeoutException）</summary>
        public const string Timeout = "timeout";

        /// <summary>连接不可用</summary>
        public const string Connection = "connection";

        /// <summary>服务端错误</summary>
        public const string Server = "server";

        /// <summary>调用方参数非法</summary>
        public const string InvalidArgument = "invalid_argument";
    }

    /// <summary>
    /// 回调通道取值。
    /// </summary>
    public static class Channels
    {
        /// <summary>应用数据通道（XML 回调）</summary>
        public const string App = "app";

        /// <summary>套件指令通道（XML 回调）</summary>
        public const string Suite = "suite";

        /// <summary>智能机器人通道（JSON 回调）</summary>
        public const string Bot = "bot";
    }
}
