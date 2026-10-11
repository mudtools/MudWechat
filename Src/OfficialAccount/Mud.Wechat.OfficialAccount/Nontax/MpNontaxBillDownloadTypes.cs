// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 非税缴费对账单下载结果（CSV 文本流；调用方负责释放 <see cref="Content"/> 与本对象）。
/// </summary>
public sealed class MpNontaxBillDownloadResult : IDisposable
{
    private readonly HttpResponseMessage? _response;

    internal MpNontaxBillDownloadResult(string? contentType, string? fileName, Stream content, HttpResponseMessage response)
    {
        ContentType = contentType;
        FileName = fileName;
        Content = content;
        _response = response;
    }

    /// <summary>获取响应内容类型（官方为 CSV 文本形态）。</summary>
    public string? ContentType { get; }

    /// <summary>获取官方 Content-Disposition 携带的文件名（未携带时为 <c>null</c>，不推断）。</summary>
    public string? FileName { get; }

    /// <summary>获取账单内容流（CSV 文本；调用方读取完毕后须释放本对象）。</summary>
    public Stream? Content { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        _response?.Dispose();
    }
}

/// <summary>
/// 非税缴费对账单下载通道（<c>/nontax/downloadbill</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何独立于声明式管线（对齐素材下载 I3 裁决）</b>：本端点成功响应是 <b>CSV 文本</b>
/// （官方 FAQ 原文「以 csv 格式的文本返回」），失败才是 JSON <c>errcode</c> 形态——
/// 生成管线只承载 JSON 响应端点，原始字节流端点走独立通道。
/// </para>
/// </remarks>
public interface IMpNontaxBillDownloadService
{
    /// <summary>
    /// 下载缴费对账单。
    /// </summary>
    /// <param name="request">下载请求（<c>bill_date</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>账单 CSV 内容流（调用方读取完毕后须释放）。</returns>
    /// <exception cref="MpException">官方返回 JSON 错误体（含 <c>errcode</c>）时抛出。</exception>
    Task<MpNontaxBillDownloadResult> DownloadBillAsync(
        DataModels.Nontax.MpNontaxDownloadBillRequest request,
        CancellationToken cancellationToken = default);
}
