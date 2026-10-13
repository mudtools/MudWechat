// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 获取直播间分享二维码应答（<c>GET /wxaapi/broadcast/room/getsharedcode</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getsharedcode.html"/>。
/// </para>
/// <para>
/// <b>官方返回体表无 <c>errmsg</c></b>（只列 <c>errcode</c> / <c>cdnUrl</c> / <c>pagePath</c> / <c>posterUrl</c>）
/// ⇒ 继承 <see cref="WxaResponse"/> 后 <c>errmsg</c> 恒为 <c>null</c>，判成败只看 <c>errcode</c>。
/// </para>
/// <para>
/// <b>三字段对应三种分享载体</b>：<see cref="CdnUrl"/> 为二维码图片、<see cref="PagePath"/> 为小程序页面路径
/// （可携带请求里的自定义 <c>params</c>）、<see cref="PosterUrl"/> 为分享海报 —— 宿主按需取用，不得假设三者同时可用。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveSharedCodeResponse : WxaResponse
{
    /// <summary>分享二维码 CDN 地址（<c>cdnUrl</c>，官方原文「分享二维码 cdn url」）。</summary>
    [JsonPropertyName("cdnUrl")]
    public string? CdnUrl { get; set; }

    /// <summary>分享路径（<c>pagePath</c>）。</summary>
    [JsonPropertyName("pagePath")]
    public string? PagePath { get; set; }

    /// <summary>分享海报地址（<c>posterUrl</c>）。</summary>
    [JsonPropertyName("posterUrl")]
    public string? PosterUrl { get; set; }
}
