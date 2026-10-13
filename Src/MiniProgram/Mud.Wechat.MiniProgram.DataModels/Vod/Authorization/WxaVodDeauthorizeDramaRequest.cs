// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「解除剧目授权」请求体（<c>POST /wxa/sec/vod/deauthorizedrama</c>）。
/// </summary>
/// <remarks>
/// <para><b>本支参数名是 <c>authorized_appid</c></b>（与「增加剧目授权」的 <c>authorized</c> 不同，官方两页参数表如此），且 <c>drama_id</c> 同样是数组 ⇒ 两请求体字段名交集不完全相同，不共用 DTO。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizedrama/api_deauthorizedrama.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodDeauthorizeDramaRequest
{
    /// <summary>drama_id（官方类型 <c>numarray</c>，<b>必填</b>）：解除授权剧目ID。</summary>
    [JsonPropertyName("drama_id")]
    public List<long>? DramaId { get; set; }

    /// <summary>authorized_appid（官方类型 <c>string</c>，<b>必填</b>）：被授权方小程序id。</summary>
    [JsonPropertyName("authorized_appid")]
    public string? AuthorizedAppid { get; set; }
}
