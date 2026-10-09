// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Abstractions.Exceptions;

namespace Mud.Wechat.Pay.Download;

/// <summary>
/// 微信支付账单文件下载通道（<c>GET /v3/billdownload/file</c>，由申请账单应答的
/// <c>download_url</c> 驱动）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791909"/>
/// （同内容另有 <c>4013071238</c> 入口；2026-10-09 核验）。
/// </para>
/// <para>
/// <b>为何独立于声明式端点</b>：账单下载返回的<b>不是 JSON</b>（明文 CSV / GZIP 流），
/// 且 URL 由上一接口动态给出 —— 声明式 <c>[Get]</c> 端点的固定路由与 JSON 反序列化都不适用。
/// 形态参照公众号线 <c>IMpMediaDownloadService</c>（<c>SendRawAsync</c> 原始响应直读 + Content-Type 分流）。
/// </para>
/// <para>
/// <b>失败分类与公众号的根本差异</b>：支付<b>无 <c>access_token</c></b>，<b>不存在</b>「令牌失效 → 刷新 → 重试」
/// 自愈链路。失败一律按官方字符串 <c>code</c> 分类（<c>WechatPayErrorCodes</c>），是否重试由宿主决定。
/// </para>
/// <para>
/// <b>签名与 SSRF</b>：下载请求经支付专用 <c>IWechatPayHttpClient</c> 发出，由传输层
/// <c>WechatPayAuthorizationHandler</c> 自动补 <c>Authorization</c>；域名仅允许官方两个接入点
/// （<c>api.mch.weixin.qq.com</c> / <c>api2.mch.weixin.qq.com</c>），其余一律 fail-fast。
/// </para>
/// </remarks>
public interface IWechatPayBillDownloadService
{
    /// <summary>
    /// 下载账单文件。
    /// </summary>
    /// <param name="downloadUrl">
    /// 账单下载地址（由「申请交易账单」/「申请资金账单」应答的 <c>download_url</c> 提供，
    /// <b>5 分钟内有效</b>且<b>只可下载一次</b>）。
    /// </param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>账单内容流（明文 CSV 或 GZIP，取决于申请时的 <c>tar_type</c>），见 <see cref="WechatPayBillDownloadResult"/>。</returns>
    /// <exception cref="WechatPayException">官方返回 JSON 错误体（含 <c>code</c> / <c>message</c>）时抛出。</exception>
    Task<WechatPayBillDownloadResult> DownloadBillAsync(
        string downloadUrl,
        CancellationToken cancellationToken = default);
}
