// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.Media;

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 获取永久素材（<c>material/get_material</c>）结果信封（三形态统一载体）。
/// </summary>
/// <remarks>
/// <para>
/// <b>三形态（逐页核验官方 get_material 页 2026-10-07，按 Content-Type + 载荷形态分流）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>图文</b>：<see cref="NewsItems"/> 非 null（官方 <c>news_item</c> 数组，经
/// <c>MediaJsonContext</c> 源生成反序列化——零手写解析、AOT 安全）。</item>
/// <item><b>视频</b>：<see cref="VideoDownUrl"/> 非 null（官方 <c>title</c>/<c>description</c>/<c>down_url</c>）；
/// 下载地址由调用方自行下载（SDK 不代下载）。</item>
/// <item><b>图片 / 语音</b>（官方原文「响应的直接为素材的内容，开发者可以自行保存为文件」）：
/// <see cref="Content"/> 为响应体流。</item>
/// </list>
/// <para>
/// <b>形态判定</b>：<see cref="NewsItems"/> != null ⇒ 图文；否则 <see cref="VideoDownUrl"/> != null ⇒ 视频；
/// 否则 <see cref="Content"/> != null ⇒ 文件流。JSON 错误体（errcode != 0）不出信封——抛
/// <see cref="Abstractions.Exceptions.MpException"/>（令牌失效码走失效重试链路）。
/// </para>
/// <para>
/// <b>生命周期</b>：文件流形态实现 <see cref="IDisposable"/>（流绑定底层响应报文）；
/// 图文 / 视频形态无可释放资源。SDK <b>不做本地落盘</b>。
/// </para>
/// </remarks>
public sealed class MpPermanentMaterialResult : IDisposable
{
    /// <summary>创建图文形态结果。</summary>
    internal MpPermanentMaterialResult(IReadOnlyList<MpMaterialNewsItem> newsItems)
    {
        NewsItems = newsItems;
    }

    /// <summary>创建视频形态结果。</summary>
    internal MpPermanentMaterialResult(string? contentType, string? videoTitle, string? videoDescription, string videoDownUrl)
    {
        ContentType = contentType;
        VideoTitle = videoTitle;
        VideoDescription = videoDescription;
        VideoDownUrl = videoDownUrl;
    }

    /// <summary>创建文件流形态结果（<see cref="Content"/> 生命周期绑定 <paramref name="response"/>）。</summary>
    internal MpPermanentMaterialResult(string? contentType, string? fileName, Stream content, HttpResponseMessage response)
    {
        ContentType = contentType;
        FileName = fileName;
        Content = content;
        _response = response;
    }

    /// <summary>底层响应报文（文件流形态持有；随实例释放）。</summary>
    private readonly HttpResponseMessage? _response;

    /// <summary>获取响应体媒体类型（文件流形态；图文 / 视频形态为 <c>application/json</c>）。</summary>
    public string? ContentType { get; }

    /// <summary>获取官方响应头携带的文件名（文件流形态；<c>Content-Disposition</c> 未携带时为 <c>null</c>）。</summary>
    public string? FileName { get; }

    /// <summary>获取图文素材内容（官方 <c>news_item</c>；<b>仅图文素材非 null</b>）。</summary>
    public IReadOnlyList<MpMaterialNewsItem>? NewsItems { get; }

    /// <summary>获取视频素材标题（官方 <c>title</c>；<b>仅视频素材返回</b>）。</summary>
    public string? VideoTitle { get; }

    /// <summary>获取视频素材描述（官方 <c>description</c>；<b>仅视频素材返回</b>）。</summary>
    public string? VideoDescription { get; }

    /// <summary>获取视频下载地址（官方 <c>down_url</c>；<b>仅视频素材非 null</b>，由调用方自行下载）。</summary>
    public string? VideoDownUrl { get; }

    /// <summary>获取响应体内容流（图片 / 语音形态；图文 / 视频形态为 <c>null</c>）。</summary>
    /// <remarks>流生命周期绑定本实例：释放本实例即关闭流。不得提前关闭。</remarks>
    public Stream? Content { get; }

    /// <summary>释放底层响应报文与内容流（幂等；释放后 <see cref="Content"/> 不可再读）。</summary>
    public void Dispose()
    {
        _response?.Dispose();
    }
}
