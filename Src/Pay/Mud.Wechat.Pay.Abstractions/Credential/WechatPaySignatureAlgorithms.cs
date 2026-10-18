// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Abstractions.Crypto;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>支付线签名算法与授权类型常量（单一来源，叶层 <see cref="WechatSmAlgorithms"/> 引用此处的授权类型）。</summary>
/// <remarks>
/// <para><b>去重裁决（方案 v1 §A.3 #9）</b>：授权类型常量 <c>WECHATPAY2-SM2-WITH-SM3</c> 在支付线与叶层两处定义会导致静默漂移。
/// 本类作为支付线的权威来源，叶层 <see cref="WechatSmAlgorithms.Sm2WithSm3"/> 引用本类的 <see cref="Sm2WithSm3AuthorizationType"/> 保持单一来源。</para>
/// </remarks>
public static class WechatPaySignatureAlgorithms
{
    /// <summary>RSA-SHA256 授权类型标识（官方 <c>WECHATPAY2-SHA256-RSA2048</c>）。</summary>
    public const string Sha256Rsa2048 = "WECHATPAY2-SHA256-RSA2048";

    /// <summary>SM2-SM3 授权类型标识（官方 <c>WECHATPAY2-SM2-WITH-SM3</c>）。</summary>
    /// <remarks>此常量同时是 <c>Wechatpay-Signature-Type</c> 应答头的合法取值之一。</remarks>
    public const string Sm2WithSm3 = "WECHATPAY2-SM2-WITH-SM3";

    /// <summary><c>Wechatpay-Signature-Type</c> 应答头名称（国密链路新增）。</summary>
    public const string SignatureTypeHeader = "Wechatpay-Signature-Type";
}
