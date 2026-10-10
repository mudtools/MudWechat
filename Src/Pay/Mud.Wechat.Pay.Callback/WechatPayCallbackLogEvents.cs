// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Callback;

/// <summary>支付回调日志事件 ID 常量（结构化日志的稳定键）。</summary>
/// <remarks>
/// <b>日志红线（PAY-B7）</b>：本组事件只记录<b>类别、商户键、路由、耗时</b>等元数据；
/// <b>绝不</b>记录 APIv3 密钥、<c>Authorization</c> 头、签名原文、<c>resource.ciphertext</c>
/// 或解密后的报文内容。
/// </remarks>
internal static class WechatPayCallbackLogEvents
{
    /// <summary>验签 / 时效 / 重放 / 解密 / 商户未命中 等验证类失败。</summary>
    public static readonly EventId VerificationFailed = new(5101, nameof(VerificationFailed));

    /// <summary>收到 <c>PUB_KEY_ID_</c> 公钥模式序列号（当前不支持，fail-closed）。</summary>
    public static readonly EventId PublicKeyModeUnsupported = new(5102, nameof(PublicKeyModeUnsupported));

    /// <summary>单个处理器异常被隔离。</summary>
    public static readonly EventId HandlerFailed = new(5103, nameof(HandlerFailed));

    /// <summary>无匹配处理器（已接收、已应答 SUCCESS）。</summary>
    public static readonly EventId Unhandled = new(5104, nameof(Unhandled));

    /// <summary>分发软超时（应答 5xx 触发官方重试）。</summary>
    public static readonly EventId DispatchTimedOut = new(5105, nameof(DispatchTimedOut));

    /// <summary>未预期异常。</summary>
    public static readonly EventId UnhandledError = new(5106, nameof(UnhandledError));
}
