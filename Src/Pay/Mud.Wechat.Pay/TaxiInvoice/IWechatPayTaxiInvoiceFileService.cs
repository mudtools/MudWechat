// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.TaxiInvoice;

namespace Mud.Wechat.Pay.TaxiInvoice;

/// <summary>
/// 出租车电子发票<b>文件通道</b>（上传发票文件，取 <c>fapiao_media_id</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何独立于声明式端点</b>：官方《上传出租车电子发票文件》的请求体是
/// <c>multipart/form-data</c>（表单字段 <c>file</c> + <c>meta</c>），而声明式
/// <c>[HttpClientApi]</c> 端点的 <c>[Body]</c> 只承载 JSON ⇒ 形态不匹配。
/// 形态参照既有先例 <see cref="Fapiao.IWechatPayFapiaoFileService"/>（同款「非 JSON ⇒ 手写通道」），
/// 本通道<b>不计入</b>支付线端点基线。
/// </para>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692904"/>
/// （更新时间 2025.02.18，<b>支持商户【普通服务商】</b>）。
/// 请求方式 <c>POST /v3/taxi-invoice/cards/upload-file</c>。
/// </para>
/// <para>
/// <b>摘要算法字段的拼写与电子发票域不同</b>（本通道最易踩的差异）：本页官方印 <c>digest_algorithm</c>
/// （拼写<b>正常</b>），而 <see cref="Fapiao.IWechatPayFapiaoFileService"/> 那页印的是
/// <c>digest_alogrithm</c>（<b>少一个 r</b>）⇒ 两支各照各页、<b>不得互相「纠正」</b>；
/// 取值亦不同（本页 <c>DIGEST_ALGORITHM_SM3</c>，电子发票域 <c>SM3</c>）。
/// </para>
/// <para>
/// <b>签名主体是 <c>meta</c> 的 JSON 串</b>（官方「构造待签名串」节原文），而<b>非</b>整段 multipart 报文；
/// 该拼接与签名由传输层承担，本类只保证<b>表单元信息串与签名串同源</b>（同一 <c>BuildMetaJson</c> 产物）。
/// </para>
/// <para>
/// <b>文件限制</b>（官方原文）：只支持 <b>PDF</b> 格式，大小<b>不超过 2M</b>；
/// 得到的 <c>fapiao_media_id</c> <b>有效期 3 天</b>，逾期未成功插卡须重新上传。
/// </para>
/// </remarks>
public interface IWechatPayTaxiInvoiceFileService
{
    /// <summary>
    /// 上传出租车电子发票文件，得到可插卡的 <c>fapiao_media_id</c>。
    /// </summary>
    /// <param name="content">发票文件内容流（官方：仅支持 <b>PDF</b>，<b>不超过 2M</b>）。</param>
    /// <param name="fileName">文件名（随表单 <c>file</c> 部分提交；调用方应保证其不含路径分隔符）。</param>
    /// <param name="companyMchid">出租车经营主体商户号（官方 <c>meta.company_mchid</c>，string(32)，<b>必填</b>）。</param>
    /// <param name="regionId">行政区划代码（官方 <c>meta.region_id</c>，integer，<b>必填</b>，值列表见官方省市区编号对照表）。</param>
    /// <param name="sm3DigestHex">
    /// 文件摘要的 <b>16 进制</b>字符串（官方 <c>meta.digest</c>，string(64)）。
    /// <b>须由调用方计算</b>：官方摘要算法只有国密 <c>SM3</c>，而 SM3 <b>不在 .NET BCL 内</b> ⇒
    /// SDK 无法代算（请用自备国密实现，对<b>同一字节流</b>求 SM3 后转 16 进制）。
    /// </param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>文件 ID（<b>3 天内有效</b>），见 <see cref="TaxiInvoiceUploadFileResponse"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> 为 <c>null</c>。</exception>
    /// <exception cref="ArgumentException"><paramref name="fileName"/>、<paramref name="companyMchid"/> 或 <paramref name="sm3DigestHex"/> 为空白。</exception>
    /// <exception cref="Mud.Wechat.Pay.Abstractions.Exceptions.WechatPayException">官方返回非 2xx（含 <c>code</c> / <c>message</c>）。</exception>
    Task<TaxiInvoiceUploadFileResponse> UploadTaxiInvoiceFileAsync(
        Stream content,
        string fileName,
        string companyMchid,
        long regionId,
        string sm3DigestHex,
        CancellationToken cancellationToken = default);
}
