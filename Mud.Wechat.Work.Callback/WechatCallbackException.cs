// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 回调接收失败类别（P2-2 异常细分；宿主可据此映射 HTTP 应答，见回调域方案附录 A）。
/// </summary>
public enum WechatCallbackFailureKind
{
    /// <summary>缺少 msg_signature 参数。</summary>
    MissingSignature,

    /// <summary>msg_signature 不匹配（PushToken 配置不一致或报文被篡改）。</summary>
    InvalidSignature,

    /// <summary>timestamp 缺失或非数字。</summary>
    MissingTimestamp,

    /// <summary>timestamp 超出时效窗口（±300s，疑似重放或时钟偏差）。</summary>
    TimestampOutOfRange,

    /// <summary>nonce 缺失。</summary>
    MissingNonce,

    /// <summary>回调报文未找到 Encrypt 节点。</summary>
    MissingEncrypt,

    /// <summary>疑似重放（一次性指纹已被消费）。</summary>
    ReplaySuspected,

    /// <summary>解密失败（密钥非法 / Base64 非法 / 填充或长度头校验不通过）。</summary>
    DecryptFailed,

    /// <summary>receiveid 与配置的接收方 ID 不一致。</summary>
    ReceiveIdMismatch,

    /// <summary>接收方 ID 未命中注册表（多套件：ToUserName / receiverId 未知）。</summary>
    UnknownReceiver,
}

/// <summary>
/// 回调接收统一异常（P2-2：携带失败类别，供宿主区分失败类别与应答策略）。
/// </summary>
/// <remarks>
/// <para>
/// 继承 <see cref="InvalidOperationException"/>：既有 <c>catch (InvalidOperationException)</c> 块零破坏（决策 D7）。
/// </para>
/// <para>
/// 异常消息不含密钥/密文材料。各 <see cref="Kind"/> 的建议 HTTP 应答见回调域方案附录 A：
/// 验签类 400/403、<see cref="WechatCallbackFailureKind.ReplaySuspected"/> 200 空体（幂等吞掉）、
/// <see cref="WechatCallbackFailureKind.DecryptFailed"/>/<see cref="WechatCallbackFailureKind.ReceiveIdMismatch"/> 500 + 告警、
/// <see cref="WechatCallbackFailureKind.UnknownReceiver"/> 400/403。
/// </para>
/// </remarks>
public class WechatCallbackException : InvalidOperationException
{
    /// <summary>失败类别。</summary>
    public WechatCallbackFailureKind Kind { get; }

    /// <summary>创建回调异常。</summary>
    /// <param name="kind">失败类别。</param>
    /// <param name="message">异常消息（不得含密钥/密文材料）。</param>
    public WechatCallbackException(WechatCallbackFailureKind kind, string message)
        : base(message)
    {
        Kind = kind;
    }

    /// <summary>创建带内部异常的回调异常。</summary>
    /// <param name="kind">失败类别。</param>
    /// <param name="message">异常消息（不得含密钥/密文材料）。</param>
    /// <param name="innerException">内部异常（如 <see cref="FormatException"/> / <see cref="CryptographicException"/>）。</param>
    public WechatCallbackException(WechatCallbackFailureKind kind, string message, Exception innerException)
        : base(message, innerException)
    {
        Kind = kind;
    }
}
