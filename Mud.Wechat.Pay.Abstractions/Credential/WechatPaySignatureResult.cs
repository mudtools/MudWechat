// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>支付签名结果：Base64 签名 + 商户证书序列号。</summary>
/// <remarks>
/// 两个字段**都不敏感**（签名与序列号本就会明文进 <c>Authorization</c> 头），
/// 因此可安全参与日志与诊断；<b>私钥 / APIv3 密钥 / <c>Authorization</c> 头整体</b>不得入日志（PAY-B7）。
/// </remarks>
public readonly struct WechatPaySignatureResult : IEquatable<WechatPaySignatureResult>
{
    /// <summary>初始化签名结果。</summary>
    /// <param name="signature">Base64 签名。</param>
    /// <param name="serialNumber">商户 API 证书序列号。</param>
    public WechatPaySignatureResult(string signature, string serialNumber)
    {
        Signature = signature ?? throw new ArgumentNullException(nameof(signature));
        SerialNumber = serialNumber ?? throw new ArgumentNullException(nameof(serialNumber));
    }

    /// <summary>Base64 编码的 RSA-SHA256 签名。</summary>
    public string Signature { get; }

    /// <summary>商户 API 证书序列号（写入 <c>Authorization</c> 的 <c>serial_no</c>）。</summary>
    public string SerialNumber { get; }

    /// <inheritdoc />
    public bool Equals(WechatPaySignatureResult other)
        => Signature == other.Signature && SerialNumber == other.SerialNumber;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is WechatPaySignatureResult other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Signature, SerialNumber);

    /// <summary>相等比较。</summary>
    public static bool operator ==(WechatPaySignatureResult left, WechatPaySignatureResult right) => left.Equals(right);

    /// <summary>不等比较。</summary>
    public static bool operator !=(WechatPaySignatureResult left, WechatPaySignatureResult right) => !left.Equals(right);
}
