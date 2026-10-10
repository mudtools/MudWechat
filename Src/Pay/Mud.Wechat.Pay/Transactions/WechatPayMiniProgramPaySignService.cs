// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Abstractions.Credential;
using Mud.Wechat.Pay.DataModels.Transactions;

namespace Mud.Wechat.Pay.Transactions;

/// <summary>
/// <see cref="IWechatPayMiniProgramPaySignService"/> 默认实现。
/// </summary>
/// <remarks>
/// <para>
/// <b>复用既有密码学，不新造通道</b>：签名走 <see cref="IWechatPaySignatureProviderFactory"/>（按商户缓存，
/// 私钥加载不留副本），签名串走 <see cref="WechatPaySignatureMessages.BuildMiniProgramPaySignMessage"/>，
/// 随机串走 <c>CreateNonce()</c>（密码学随机、32 位 <c>[a-zA-Z0-9]</c>）—— 与传输层签名同源，
/// 避免出现「第二套签名实现」。
/// </para>
/// <para>
/// <b>时间戳口径</b>：官方描述为「标准北京时间（东八区）… 自 1970-01-01 起秒数」，
/// 但 Unix 时间戳本身<b>与时区无关</b>，故取 <c>DateTimeOffset.UtcNow.ToUnixTimeSeconds()</c>
/// 即为所需值（10 位秒级；毫秒级会校验失败，故<b>不</b>用毫秒）。
/// </para>
/// </remarks>
public sealed class WechatPayMiniProgramPaySignService : IWechatPayMiniProgramPaySignService
{
    private readonly IWechatPayMerchantContext _merchantContext;
    private readonly IWechatPaySignatureProviderFactory _signatureProviders;

    /// <summary>创建签名服务。</summary>
    /// <param name="merchantContext">当前商户环境上下文（决定用哪把私钥）。</param>
    /// <param name="signatureProviders">按商户缓存的签名提供器工厂。</param>
    /// <exception cref="ArgumentNullException">任一参数为 <c>null</c>。</exception>
    public WechatPayMiniProgramPaySignService(
        IWechatPayMerchantContext merchantContext,
        IWechatPaySignatureProviderFactory signatureProviders)
    {
        _merchantContext = merchantContext ?? throw new ArgumentNullException(nameof(merchantContext));
        _signatureProviders = signatureProviders ?? throw new ArgumentNullException(nameof(signatureProviders));
    }

    /// <inheritdoc />
    public async Task<WechatPayMiniProgramPaySign> CreatePaySignAsync(
        string appId,
        string prepayId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(appId))
        {
            throw new ArgumentException(
                "调起支付的 appId 不可为空白（必须与下单时传入的 appid 一致）。", nameof(appId));
        }

        // package 的官方格式由组装点统一承担（传裸 prepay_id 会签名不匹配）。
        var package = WechatPaySignatureMessages.BuildPackageValue(prepayId);

        var merchant = _merchantContext.ResolveCurrent();

        var signatureProvider = await _signatureProviders
            .GetOrCreateAsync(merchant, cancellationToken)
            .ConfigureAwait(false);

        // 时间戳保持**字符串**（官方字段是 string(32)）：先数值再格式化会引入文化/分组分隔符风险。
        var timeStamp = WechatPaySignatureProvider.FormatTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        var nonceStr = WechatPaySignatureMessages.CreateNonce();

        var message = WechatPaySignatureMessages.BuildMiniProgramPaySignMessage(
            appId, timeStamp, nonceStr, package);

        var signature = signatureProvider.Sign(message);

        return new WechatPayMiniProgramPaySign
        {
            TimeStamp = timeStamp,
            NonceStr = nonceStr,
            Package = package,
            SignType = WechatPaySignatureMessages.MiniProgramPaySignType,
            PaySign = signature.Signature,
        };
    }
}
