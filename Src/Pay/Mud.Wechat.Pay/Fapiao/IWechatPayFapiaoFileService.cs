// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Fapiao;

namespace Mud.Wechat.Pay.Fapiao;

/// <summary>
/// 微信支付电子发票<b>文件通道</b>（上传发票文件）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何独立于声明式端点</b>：官方《上传电子发票文件》的请求体是
/// <c><b>multipart/form-data</b></c>（表单字段 <c>file</c> + <c>meta</c>），
/// 而声明式 <c>[HttpClientApi]</c> 端点的 <c>[Body]</c> 只承载 JSON —— 形态不匹配。
/// 参照本仓既有先例：账单下载通道（<c>IWechatPayBillDownloadService</c>）同为
/// 「非 JSON ⇒ 手写通道」。
/// </para>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/docs/merchant/apis/fapiao/fapiao-applications/upload-fapiao-file.html"/>
/// （2026-10-09 核验；更新时间 2025.09.26）。<b>支持商户：【普通商户】</b>。
/// </para>
/// <para>
/// <b>两段式</b>：上传得到 <c>fapiao_media_id</c>（<b>三天内有效</b>）后，
/// 须尽快调用 <see cref="IWechatPayFapiaoService.InsertFapiaoCardsAsync"/> 插卡。
/// </para>
/// <para>
/// <b>⚠️ 下载发票文件<b>刻意不在本通道</b></b>（裁决，非遗漏）：官方《下载发票文件》页给出的示例 URL 是
/// <c>https://<b>pay.wechatpay.cn</b>/invoicing/fapiao/fapiao-file?token=…</c>
/// —— <b>文件域名与 API 接入点不同</b>，而本仓的进程级网络白名单
/// （<c>WechatApiHosts.AllowedBaseUrlDomains</c>）只含 <c>weixin.qq.com</c> / <c>work.weixin.qq.com</c>，
/// 且该白名单条目数被守卫（MP-X8）锁定为「两条新线零改动」。
/// 因此在本进程内请求 <c>pay.wechatpay.cn</c> 会被 URL 校验器拒绝 ⇒
/// <b>新增该主机＝进程级安全面变更</b>，须由宿主显式决策，不能由 SDK 顺手放开。
/// 当前落点：<see cref="IWechatPayFapiaoService.GetFapiaoFilesAsync"/> 返回 <c>download_url</c>，
/// 由宿主自行取用（该下载<b>不支持签名和验签</b>，URL 有效期 <b>30s</b>，须原样使用、严禁改写）。
/// </para>
/// </remarks>
public interface IWechatPayFapiaoFileService
{
    /// <summary>
    /// 上传电子发票文件，得到可插卡的 <c>fapiao_media_id</c>。
    /// </summary>
    /// <param name="content">发票文件内容流（官方：仅支持 <b>PDF</b> 与 <b>OFD</b> 格式，<b>不超过 2M</b>）。</param>
    /// <param name="fileName">文件名（随表单 <c>file</c> 部分提交；调用方应保证其不含路径分隔符）。</param>
    /// <param name="sm3DigestHex">
    /// 文件摘要的 <b>16 进制</b>字符串（官方 <c>meta.digest</c>，string(64)）。
    /// <b>须由调用方计算</b>：官方摘要算法只有 <c>SM3</c>，而 SM3 <b>不在 .NET BCL 内</b> ⇒
    /// SDK 无法代算（请用自备国密实现，对<b>同一字节流</b>求 SM3 后转 16 进制小写）。
    /// </param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>文件 ID（<b>三天内有效</b>），见 <see cref="FapiaoUploadFileResponse"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> 为 <c>null</c>。</exception>
    /// <exception cref="ArgumentException"><paramref name="fileName"/> 或 <paramref name="sm3DigestHex"/> 为空白。</exception>
    /// <exception cref="Mud.Wechat.Pay.Abstractions.Exceptions.WechatPayException">官方返回非 2xx（含 <c>code</c> / <c>message</c>）。</exception>
    Task<FapiaoUploadFileResponse> UploadFapiaoFileAsync(
        Stream content,
        string fileName,
        string sm3DigestHex,
        CancellationToken cancellationToken = default);
}
