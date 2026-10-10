// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// 支付线签名提供器：**RSA-SHA256 签名与验签**（方案 v2 §2.3「既有签名通道范式的第三种算法」）。
/// </summary>
/// <remarks>
/// <para>
/// 形状对齐组件 <c>IHmacSignatureProvider</c>（<c>Generate</c>/<c>Verify</c> 双方法、可替换实现），
/// 但**算法与凭据形态完全不同**——这是守卫 <b>PAY-B1</b> 的立论基础：
/// </para>
/// <list type="bullet">
/// <item>组件钩子是「<c>string secretKey</c>（对称共享密钥）→ 单个 header 标量」，默认 <c>HMACSHA256</c>；</item>
/// <item>本接口是「商户 <b>RSA-2048 私钥</b>签名 + 平台证书公钥验签」，且签名输入含请求体（逐请求计算）。</item>
/// </list>
/// <para>
/// <b>本接口不参与 <c>[Token]</c> 注入</b>：签名发生在 HTTP 传输层，不是接口属性；
/// <c>TokenInjectionMode</c> 无 RSA 形态 ⇒ 支付接口一律只声明 <c>[HttpClientApi]</c>。
/// </para>
/// <para>
/// 实现为**同步**方法：BCL 的 <c>RSA.SignData</c>/<c>VerifyData</c> 是 CPU 内的纯内存运算、
 /// 无 IO 与网络等待，包一层 <c>Task</c> 只会制造假异步（分配 + 状态机）且对 AOT 无益。
/// </para>
/// </remarks>
public interface IWechatPaySignatureProvider
{
    /// <summary>用商户私钥对 <paramref name="message"/> 做 RSA-SHA256 签名。</summary>
    /// <param name="message">由 <see cref="WechatPaySignatureMessages.BuildRequestMessage"/> 产出的签名串。</param>
    /// <returns>Base64 签名与商户证书序列号。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> 为 <c>null</c>。</exception>
    /// <exception cref="InvalidOperationException">未配置商户私钥或商户证书序列号。</exception>
    WechatPaySignatureResult Sign(string message);

    /// <summary>
    /// 用平台证书公钥验签。**fail-closed**：序列号未知、签名非法 Base64、证书缺失、算法失败，一律返回 <c>false</c>，
    /// 不抛异常、不区分失败原因（原因只进日志由调用方决定，且**不得**回显签名原文）。
    /// </summary>
    /// <param name="serialNumber"><c>Wechatpay-Serial</c> 原文（平台证书序列号）。</param>
    /// <param name="message">由 <see cref="WechatPaySignatureMessages.BuildVerifyMessage"/> 产出的验签串。</param>
    /// <param name="signature">Base64 签名（<c>Wechatpay-Signature</c>）。</param>
    /// <returns><c>true</c> 表示签名有效；否则 <c>false</c>。</returns>
    bool Verify(string? serialNumber, string? message, string? signature);
}
