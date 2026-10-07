// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.Callback;

/// <summary>
/// 微信系回调接收失败类别（企业微信 / 公众号共用；宿主可据此映射 HTTP 应答，见各产品线回调域方案附录）。
/// </summary>
/// <remarks>
/// 本枚举为<b>叶层中立面</b>：取值语义与产品线无关（缺参 / 验签不匹配 / 时效越界 / 指纹重放 / 解密失败 /
/// 接收方不匹配 / 路由未命中），故不按产品线复制（复制会导致宿主 <c>catch</c> 分支与失败类别映射漂移）。
/// </remarks>
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

    /// <summary>回调报文未找到 Encrypt 节点（明文报文 / 密文缺失）。</summary>
    MissingEncrypt,

    /// <summary>
    /// 安全模式下收到<b>明文</b>报文（公众号三模式语义：安全模式须为纯密文，明文一律拒收）。
    /// </summary>
    /// <remarks>企业微信恒为密文，不会产生本类别。</remarks>
    PlainTextRejected,

    /// <summary>疑似重放（一次性指纹已被消费）。</summary>
    ReplaySuspected,

    /// <summary>解密失败（密钥非法 / Base64 非法 / 填充或长度头校验不通过）。</summary>
    DecryptFailed,

    /// <summary>receiveid 与配置的接收方 ID 不一致。</summary>
    ReceiveIdMismatch,

    /// <summary>回调路由未命中任何已登记的应用键（既无精确键也无通配键；多应用路由 fail-closed）。</summary>
    UnknownReceiver,
}

/// <summary>
/// 微信系回调接收统一异常（携带失败类别，供宿主区分失败类别与应答策略）。
/// </summary>
/// <remarks>
/// <para>
/// 继承 <see cref="InvalidOperationException"/>：既有 <c>catch (InvalidOperationException)</c> 块零破坏（决策 D7）。
/// </para>
/// <para>
/// 异常消息不含密钥/密文材料。产品线应答映射：企微验签类 403、
/// <see cref="WechatCallbackFailureKind.DecryptFailed"/>/<see cref="WechatCallbackFailureKind.ReceiveIdMismatch"/> 500 + 告警；
/// 公众号验签类 403 + 空体、<see cref="WechatCallbackFailureKind.PlainTextRejected"/> 403（安全模式拒明文）。
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
    /// <param name="innerException">内部异常（如 <see cref="FormatException"/> / <see cref="System.Security.Cryptography.CryptographicException"/>）。</param>
    public WechatCallbackException(WechatCallbackFailureKind kind, string message, Exception innerException)
        : base(message, innerException)
    {
        Kind = kind;
    }
}
