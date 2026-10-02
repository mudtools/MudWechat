// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.IO;

namespace Mud.Wechat.Work.Callback.Tests.ContractGuards;

/// <summary>
/// 回调域契约守卫（CB1~CB4，回调域方案 §八）：防止 P0-1 填充互操作、P1-1 指纹闸次序、
/// P1-3 注册期校验、P1-2 URL 验证链路的关键约束回归。
/// </summary>
public class WechatCallbackContractGuards
{
    /// <summary>解决方案根目录（向上查找 Mud.Wechat.slnx）。</summary>
    private static string GetSolutionRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("未找到 Mud.Wechat.slnx");
    }

    private static string ReadCallbackSource(string fileName)
        => File.ReadAllText(Path.Combine(GetSolutionRoot(), "Mud.Wechat.Work.Callback", fileName));

    /// <summary>
    /// 契约守卫 CB1（P0-1）：<c>WechatCallbackCrypto</c> 不得出现 .NET 内置 16 块 PKCS7
    /// （官方报文 pad∈[17..32] 时解密必抛 <c>CryptographicException</c>）；解密必须为
    /// <c>PaddingMode.None</c> + 手工 32 块填充剥离。
    /// </summary>
    [Fact]
    public void CallbackDecrypt_ShouldUseManualPkcs7PaddingOf32Bytes()
    {
        var source = ReadCallbackSource("WechatCallbackCrypto.cs");

        source.Should().NotContain("PaddingMode.PKCS7",
            "CB1：.NET 内置 PKCS7 为 16 字节块校验，官方报文 pad∈[17..32] 时解密必抛（P0-1 回归守卫）");
        source.Should().Contain("PaddingMode.None",
            "CB1：解密/加密必须走 PaddingMode.None + 手工 32 块填充补位/剥离（决策 D1）");
        source.Should().Contain("StripPkcs7Padding",
            "CB1：Decrypt 必须手工剥离 32 块 PKCS7 填充（否则尾部填充并入 receiveid，§4.1.2）");
    }

    /// <summary>
    /// 契约守卫 CB2（P1-1/D2）：接收器源码中 <c>TryMarkAsync</c> 调用位置必须位于
    /// <c>WechatCallbackCrypto.Decrypt</c> 之后——防止「标记先于解密」的 F2 回归（解密失败消耗指纹 → 官方重试被拒）。
    /// </summary>
    [Fact]
    public void CallbackFingerprintMark_ShouldFollowDecrypt()
    {
        var source = ReadCallbackSource("WechatCallbackReceiver.cs");

        var markIndex = source.IndexOf("TryMarkAsync", StringComparison.Ordinal);
        var decryptIndex = source.IndexOf("WechatCallbackCrypto.Decrypt(", StringComparison.Ordinal);

        markIndex.Should().BePositive("CB2：接收器必须调用 TryMarkAsync（P0-2 第二道闸不得被移除）");
        decryptIndex.Should().BePositive("CB2：接收器必须经 WechatCallbackCrypto.Decrypt 解密");
        markIndex.Should().BeGreaterThan(decryptIndex,
            "CB2：指纹标记必须位于解密之后（P1-1/D2：解密失败不消耗指纹，官方重试可重新进入管线）");
    }

    /// <summary>
    /// 契约守卫 CB3（P1-3/D11）：注册表必须含接收方 ID 非空/唯一校验与配置完整性校验（注册期 fail-fast）；
    /// 行为用例见 <c>WechatCallbackServiceCollectionExtensionsTests</c>（重复/空 receiverId 注册抛、缺配置注册期即抛）。
    /// </summary>
    [Fact]
    public void CallbackSuiteRegistration_ShouldFailFastOnInvalidReceiverId()
    {
        var source = ReadCallbackSource("WechatCallbackOptionsRegistry.cs");

        source.Should().Contain("Validate()",
            "CB3：登记回调配置必须先做配置完整性校验（F12：校验前置到注册期）");
        source.Should().Contain("接收方 ID 重复",
            "CB3：注册表必须拒绝重复的接收方 ID（同一 ToUserName 只允许一套回调配置）");
        source.Should().Contain("ContainsKey",
            "CB3：接收方 ID 唯一性必须以注册表键判定");
    }

    /// <summary>
    /// 契约守卫 CB4（P1-2/D5）：URL 验证实现中验签（SHA1 计算/比对）必须位于解密之前，
    /// 且<b>不得调用</b> <c>TryMarkAsync</c>（幂等读不消耗指纹）。
    /// </summary>
    [Fact]
    public void CallbackUrlVerification_ShouldVerifySignatureBeforeDecrypt()
    {
        var source = ReadCallbackSource("WechatCallbackReceiver.cs");

        var verifyStart = source.IndexOf("VerifyUrlCoreAsync", StringComparison.Ordinal);
        verifyStart.Should().BePositive("CB4：接收器必须实现 URL 验证核心（P1-2）");

        var verifierBody = source[verifyStart..];
        var signatureIndex = verifierBody.IndexOf("ComputeSignature", StringComparison.Ordinal);
        var decryptIndex = verifierBody.IndexOf("WechatCallbackCrypto.Decrypt(", StringComparison.Ordinal);

        signatureIndex.Should().BePositive("CB4：URL 验证必须计算并比对签名（echostr 参与签名，同 96238）");
        decryptIndex.Should().BePositive("CB4：URL 验证必须解密 echostr");
        signatureIndex.Should().BeLessThan(decryptIndex,
            "CB4：URL 验证中验签必须位于解密之前（攻击者无 token 不得触达解密）");
        verifierBody.Should().NotContain("TryMarkAsync",
            "CB4：URL 验证是幂等读，不得消耗一次性指纹（D5：去重会造成「验证被上一次消耗」的假失败）");
    }
}
