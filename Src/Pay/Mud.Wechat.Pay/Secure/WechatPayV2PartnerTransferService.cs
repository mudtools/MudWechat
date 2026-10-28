// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.Secure;

/// <summary>
/// <see cref="IWechatPayV2PartnerTransferService"/> 的实现：逐商户证书通道 + 手工签名收发。
/// </summary>
/// <remarks>
/// <para>
/// <b>路径前缀是 <c>/mmpaysptrans/</c></b>（不是零钱族的 <c>/mmpaymkttransfers/</c>），
/// 域名与前两族同为 <see cref="WechatPayV2HttpClientNames.BaseAddress"/>（<c>api.mch</c>）。
/// </para>
/// <para>
/// <b>SDK 不代做加密</b>：卡号与户名的 RSA OAEP 密文、以及查询用的 MD5 均由宿主产出后填入 DTO ⇒
/// 本类只是「路径 + 域名 + DTO」映射，个人金融信息全程不过 SDK 的加解密面，
/// 也就不存在 SDK 侧留痕的风险点。
/// </para>
/// <para><b>不出敏感信息</b>（PAY-B7，本族最敏感）：异常文本不含密钥、<c>sign</c>、
/// <c>enc_bank_no</c> / <c>enc_true_name</c> 及其 MD5 形态。</para>
/// </remarks>
internal sealed class WechatPayV2PartnerTransferService : IWechatPayV2PartnerTransferService
{
    private readonly WechatPayV2SecureInvoker _invoker;

    /// <summary>创建付款到银行卡侧服务（由 DI 解析）。</summary>
    public WechatPayV2PartnerTransferService(WechatPayV2SecureInvoker invoker)
        => _invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));

    /// <inheritdoc />
    public Task<V2PayBankResponse> PayBankAsync(
        V2PayBankRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2PayBankRequest, V2PayBankResponse>(
            "/mmpaysptrans/pay_bank",
            WechatPayV2HttpClientNames.BaseAddress, request, cancellationToken);

    /// <inheritdoc />
    public Task<V2QueryBankResponse> QueryBankAsync(
        V2QueryBankRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2QueryBankRequest, V2QueryBankResponse>(
            "/mmpaysptrans/query_bank",
            WechatPayV2HttpClientNames.BaseAddress, request, cancellationToken);
}
