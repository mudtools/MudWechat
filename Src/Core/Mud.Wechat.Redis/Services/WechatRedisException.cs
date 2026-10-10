// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using StackExchange.Redis;

namespace Mud.Wechat.Redis.Services;

/// <summary>
/// Redis 存储层异常分类映射（统一出口：Redis 异常家族 → <see cref="WechatRedisException"/>）。
/// </summary>
/// <remarks>
/// <b>3.3.0 层次注意</b>：<see cref="RedisTimeoutException"/> 直接继承 <see cref="TimeoutException"/>
/// 而非 <see cref="RedisException"/>（与 2.x 不同），故包装面须显式并入；否则超时将裸抛、绕过分类。
/// 编程错误（ArgumentNullException 等）与取消（OperationCanceledException）不在包装面内，原样上抛。
/// </remarks>
internal static class WechatRedisErrors
{
    /// <summary>判断异常是否属 Redis 异常家族（应包装为 <see cref="WechatRedisException"/> 上抛）。</summary>
    /// <param name="ex">待判定的异常。</param>
    /// <returns>属 Redis 异常家族返回 <c>true</c>。</returns>
    public static bool ShouldWrap(Exception ex)
        => ex is RedisException or RedisTimeoutException;

    /// <summary>把 Redis 异常映射为带类别的 <see cref="WechatRedisException"/>。</summary>
    /// <param name="operation">操作描述（如「读取令牌」）。</param>
    /// <param name="key">Redis 键名（仅键名，非凭据，可安全入日志/异常消息）。</param>
    /// <param name="ex">原始 Redis 异常。</param>
    /// <returns>可分类异常（InnerException 为原始异常）。</returns>
    public static WechatRedisException Map(string operation, string key, Exception ex)
        => new(
            ex switch
            {
                RedisConnectionException => WechatRedisFailureKind.Connection,
                RedisTimeoutException => WechatRedisFailureKind.Timeout,
                _ => WechatRedisFailureKind.Server,
            },
            $"{operation}失败（key = {key}）：{ex.Message}",
            ex);
}

/// <summary>
/// Redis 存储层失败类别（对齐 Mud.Feishu.Redis 的 <c>FeishuRedisFailureKind</c>，ADR-6 同构）。
/// </summary>
public enum WechatRedisFailureKind
{
    /// <summary>连接不可用（网络中断、DNS 不可达、认证失败等）。可降级场景。</summary>
    Connection,

    /// <summary>操作超时。</summary>
    Timeout,

    /// <summary>服务端错误（配置错误、非法命令等）。</summary>
    Server,

    /// <summary>无效参数（调用方传入非法值，如超长键段）。不应重试。</summary>
    InvalidArgument,
}

/// <summary>
/// Redis 存储层统一异常（对齐 Mud.Feishu.Redis 的 <c>FeishuRedisException</c>）。
/// </summary>
/// <remarks>
/// 继承 <see cref="InvalidOperationException"/> 使既有捕获语义不变；
/// <see cref="FailureKind"/> 供宿主区分「连接不可用（可降级）」与「服务端错误（应上抛）」。
/// <b>安全</b>：异常消息只允许携带键名（appKey/authCorpId/suiteId/SHA1 指纹均非凭据），
/// 不得携带令牌值、suite_ticket、密码或完整 <see cref="WechatCorpAuthorization"/>。
/// </remarks>
public class WechatRedisException : InvalidOperationException
{
    /// <summary>失败类别。</summary>
    public WechatRedisFailureKind FailureKind { get; }

    /// <summary>创建 Redis 存储层异常。</summary>
    /// <param name="failureKind">失败类别。</param>
    /// <param name="message">异常消息（只含键名与操作描述，不得含敏感凭据）。</param>
    /// <param name="innerException">内部异常。</param>
    public WechatRedisException(WechatRedisFailureKind failureKind, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        FailureKind = failureKind;
    }
}
