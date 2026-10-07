// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 素材下载结果信封（二进制流 / 视频 <c>video_url</c> 双形态的统一载体）。
/// </summary>
/// <remarks>
/// <para>
/// <b>三形态（逐页核验官方 media/get 页 2026-10-07）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>文件流</b>（图片 / 语音 / 缩略图 / JSSDK 高清语音）：<see cref="Content"/> 为响应体流，
/// <see cref="ContentType"/> 为官方返回的媒体类型（如 <c>image/jpeg</c>），<see cref="VideoUrl"/> 为 <c>null</c>。</item>
/// <item><b>视频 JSON 形态</b>：官方原文对视频素材返回 <c>{"video_url":"DOWN_URL"}</c>
/// （**不返回文件流**，也非 302 跳转）⇒ <see cref="VideoUrl"/> 承载下载地址，
/// <see cref="Content"/> 为 <c>null</c>，由调用方自行下载该 URL（SDK 不代下载）。</item>
/// <item><b>JSON 错误</b>（如 <c>40007 invalid media_id</c>）：不出本信封——按 Content-Type
/// 分支判错后抛 <see cref="MpException"/>（令牌失效码走失效重试链路）。</item>
/// </list>
/// <para>
/// <b>生命周期</b>：实现 <see cref="IDisposable"/>——<see cref="Content"/> 流绑定在底层
/// <see cref="HttpResponseMessage"/> 上，调用方用毕须释放本实例（释放即同时关闭流与响应报文）；
/// SDK <b>不做本地落盘</b>（与「SDK 不做业务编排」红线一致）。
/// </para>
/// </remarks>
public sealed class MpMediaDownloadResult : IDisposable
{
    /// <summary>底层响应报文（文件流形态持有；随实例释放）。</summary>
    private readonly HttpResponseMessage? _response;

    /// <summary>创建文件流形态结果（<see cref="Content"/> 生命周期绑定 <paramref name="response"/>）。</summary>
    internal MpMediaDownloadResult(string? contentType, string? fileName, Stream content, HttpResponseMessage response)
    {
        ContentType = contentType;
        FileName = fileName;
        Content = content;
        _response = response;
    }

    /// <summary>创建视频 <c>video_url</c> 形态结果（无内容流、无响应报文）。</summary>
    internal MpMediaDownloadResult(string? contentType, string videoUrl)
    {
        ContentType = contentType;
        VideoUrl = videoUrl;
    }

    /// <summary>获取响应体媒体类型（如 <c>image/jpeg</c> / <c>audio/speex</c>；视频 JSON 形态为 <c>application/json</c>）。</summary>
    public string? ContentType { get; }

    /// <summary>
    /// 获取官方响应头携带的文件名（<c>Content-Disposition</c> 的 <c>filename</c>/<c>filename*</c>；
    /// 官方未携带时为 <c>null</c>，SDK 不推断）。
    /// </summary>
    public string? FileName { get; }

    /// <summary>
    /// 获取视频素材下载地址（官方 <c>video_url</c> 字段，<b>仅视频素材返回</b>；
    /// 此时 <see cref="Content"/> 为 <c>null</c>，地址由调用方自行下载）。
    /// </summary>
    public string? VideoUrl { get; }

    /// <summary>
    /// 获取响应体内容流（图片 / 语音 / 缩略图 / 高清语音形态；视频 JSON 形态为 <c>null</c>）。
    /// </summary>
    /// <remarks>流生命周期绑定本实例：释放本实例即关闭流。不得提前关闭。</remarks>
    public Stream? Content { get; }

    /// <summary>释放底层响应报文与内容流（幂等；释放后 <see cref="Content"/> 不可再读）。</summary>
    public void Dispose()
    {
        _response?.Dispose();
    }
}
