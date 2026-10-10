// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 小程序码下载结果（流式；由调用方负责释放）。
/// </summary>
/// <remarks>
/// 形态参照公众号线 <c>MpMediaDownloadResult</c>：图片可能较大，故<b>不缓冲全部字节</b>，
/// 把响应流交给调用方按需读取；<see cref="Dispose"/> 同时释放内容流与底层响应。
/// </remarks>
public sealed class WxaCodeResult : IDisposable
{
    /// <summary>创建结果。</summary>
    /// <param name="contentType">响应内容类型（如 <c>image/jpeg</c> / <c>image/png</c>）。</param>
    /// <param name="fileName">官方 <c>Content-Disposition</c> 携带的文件名（未携带时为 <c>null</c>）。</param>
    /// <param name="content">图片内容流。</param>
    /// <param name="response">底层响应消息（与内容流同生命周期）。</param>
    internal WxaCodeResult(string? contentType, string? fileName, Stream content, HttpResponseMessage response)
    {
        ContentType = contentType;
        FileName = fileName;
        Content = content;
        Response = response;
    }

    /// <summary>响应内容类型（未携带时为 <c>null</c>）。</summary>
    public string? ContentType { get; }

    /// <summary>官方携带的文件名（未携带时为 <c>null</c>；落盘前请自行净化，勿做安全假设）。</summary>
    public string? FileName { get; }

    /// <summary>图片内容流（JPEG / PNG，取决于 <c>is_hyaline</c>）。</summary>
    public Stream Content { get; }

    /// <summary>底层响应消息（供读取状态码 / 响应头等诊断信息）。</summary>
    public HttpResponseMessage Response { get; }

    /// <summary>释放内容流与底层响应。</summary>
    public void Dispose() => Response.Dispose();
}
