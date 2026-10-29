// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 视频格式取值（<c>media_type</c>，官方列出 14 个，值为大写字面量）。
/// </summary>
/// <remarks>
/// <para><b>类型是 <c>string</c> 不是数字</b>：官方以大写缩写表达（<c>MP4</c>…），错误码 <c>10090001</c>「视频类型不支持」。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_applyupload.html"/></para>
/// </remarks>
public static class WxaVodMediaTypes
{
    /// <summary>视频格式 <c>MP4</c>（官方原文大写）。</summary>
    public const string MP4 = "MP4";

    /// <summary>视频格式 <c>TS</c>（官方原文大写）。</summary>
    public const string TS = "TS";

    /// <summary>视频格式 <c>MOV</c>（官方原文大写）。</summary>
    public const string MOV = "MOV";

    /// <summary>视频格式 <c>MXF</c>（官方原文大写）。</summary>
    public const string MXF = "MXF";

    /// <summary>视频格式 <c>MPG</c>（官方原文大写）。</summary>
    public const string MPG = "MPG";

    /// <summary>视频格式 <c>FLV</c>（官方原文大写）。</summary>
    public const string FLV = "FLV";

    /// <summary>视频格式 <c>WMV</c>（官方原文大写）。</summary>
    public const string WMV = "WMV";

    /// <summary>视频格式 <c>AVI</c>（官方原文大写）。</summary>
    public const string AVI = "AVI";

    /// <summary>视频格式 <c>M4V</c>（官方原文大写）。</summary>
    public const string M4V = "M4V";

    /// <summary>视频格式 <c>F4V</c>（官方原文大写）。</summary>
    public const string F4V = "F4V";

    /// <summary>视频格式 <c>MPEG</c>（官方原文大写）。</summary>
    public const string MPEG = "MPEG";

    /// <summary>视频格式 <c>3GP</c>（官方原文大写）。</summary>
    public const string F3GP = "3GP";

    /// <summary>视频格式 <c>ASF</c>（官方原文大写）。</summary>
    public const string ASF = "ASF";

    /// <summary>视频格式 <c>MKV</c>（官方原文大写）。</summary>
    public const string MKV = "MKV";
}
