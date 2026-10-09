// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Certificates;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台证书」域 SDK（APIv3 获取平台证书列表，1 端点）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（普通商户文档中心，2026-10-09 逐页核验）：
/// 下载平台证书 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012551764"/>
/// （概览 <c>4024350132</c>；页面更新时间 2024.09.13）。</para>
/// <para><b>用途</b>：应答 / 回调验签需要<b>平台证书公钥</b>。本端点用于<b>定时拉取并缓存</b>平台证书，
/// 以处理 <c>Wechatpay-Serial</c> 轮换（未知 serial ⇒ 触发一次刷新后仍失败即拒，<b>不得刷新失败即放行</b>，
/// 见红线 §7.3）。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）：形态与
/// <see cref="IWechatPayTransactionsService"/> 一致，详见其 remarks。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Certificates", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayCertificatesService
{
    /// <summary>
    /// 获取平台证书列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012551764"/>
    /// （官方接口名 <c>DirectAPIv3Certificates</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>平台证书列表（<c>data[]</c>，证书内容为 AES-256-GCM 密文），见 <see cref="PlatformCertificatesResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/certificates</c>；<b>无 query / path 参数</b>；
    /// 必带 <c>Accept: application/json</c>。支持商户类型：<b>普通商户</b>。</para>
    /// <para><b>频率限制</b>：单个商户号 <b>1000 次/s</b>（官方原文）。仍应<b>定时低频拉取</b>并本地缓存，
    /// 逐请求拉取会带来无谓往返。</para>
    /// <para><b>解密</b>：应答中的证书为密文（<c>encrypt_certificate</c>），须用 APIv3 密钥经
    /// <c>WechatPayAesGcmCodec</c> 解密为 PEM 明文后方可构造 X509 证书验签。</para>
    /// </remarks>
    [Get("/v3/certificates")]
    Task<PlatformCertificatesResponse> GetPlatformCertificatesAsync(
        CancellationToken cancellationToken = default);
}
