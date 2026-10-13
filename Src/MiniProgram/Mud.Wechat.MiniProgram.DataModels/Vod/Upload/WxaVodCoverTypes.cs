// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 封面图格式取值（<c>cover_type</c>，官方列出 9 个，值为大写字面量）。
/// </summary>
/// <remarks>
/// <para><b><c>TIF</c> 与 <c>TIFF</c> 官方并列</b>（照录，勿「统一成一个」）：官方封面格式表同时给出两者。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_applyupload.html"/></para>
/// </remarks>
public static class WxaVodCoverTypes
{
    /// <summary>封面图格式 <c>JPG</c>（官方原文大写）。</summary>
    public const string JPG = "JPG";

    /// <summary>封面图格式 <c>JPEG</c>（官方原文大写）。</summary>
    public const string JPEG = "JPEG";

    /// <summary>封面图格式 <c>PNG</c>（官方原文大写）。</summary>
    public const string PNG = "PNG";

    /// <summary>封面图格式 <c>BMP</c>（官方原文大写）。</summary>
    public const string BMP = "BMP";

    /// <summary>封面图格式 <c>TIFF</c>（官方原文大写）。</summary>
    public const string TIFF = "TIFF";

    /// <summary>封面图格式 <c>AI</c>（官方原文大写）。</summary>
    public const string AI = "AI";

    /// <summary>封面图格式 <c>CDR</c>（官方原文大写）。</summary>
    public const string CDR = "CDR";

    /// <summary>封面图格式 <c>EPS</c>（官方原文大写）。</summary>
    public const string EPS = "EPS";

    /// <summary>封面图格式 <c>TIF</c>（官方原文大写）。</summary>
    public const string TIF = "TIF";
}
