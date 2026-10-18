// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.Crypto;

/// <summary>国密算法名常量（单一来源；授权类型常量见支付线 <c>WechatPaySignatureAlgorithms</c>）。</summary>
/// <remarks>
/// <para><b>去重裁决（方案 v1 §A.3 #9）</b>：授权类型常量 <c>WECHATPAY2-SM2-WITH-SM3</c> 在支付线与叶层两处定义会导致静默漂移。
/// 本类只定义算法名常量（<c>SM2</c>/<c>SM3</c>/<c>SM4_GCM</c>），授权类型常量由支付线 <c>WechatPaySignatureAlgorithms.Sm2WithSm3</c> 唯一定义。</para>
/// </remarks>
public static class WechatSmAlgorithms
{
    /// <summary>SM2 椭圆曲线公钥密码算法。</summary>
    public const string Sm2 = "SM2";

    /// <summary>SM3 密码杂凑算法（输出 32 字节 / 256 位）。</summary>
    public const string Sm3 = "SM3";

    /// <summary>SM4 分组密码算法配合 GCM 认证加密模式（官方 <c>SM4_GCM</c>）。</summary>
    public const string Sm4Gcm = "SM4_GCM";
}
