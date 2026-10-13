// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「查询授权信息」请求体（<c>POST /wxa/sec/vod/getauthorizeobjects</c>，授权方视角：查我授出去的权利）。
/// </summary>
/// <remarks>
/// <para><b>四个过滤参数全选填</b>（官方原文）：不填 <c>drama_id</c> 表示查所有剧目、不填 <c>authorized_appid</c> 表示查所有被授权方。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizedrama/api_getauthorizeobjects.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodGetAuthorizeObjectsRequest
{
    /// <summary>drama_id（官方类型 <c>number</c>）：查询的剧目ID。不填表示查询所有剧目。</summary>
    [JsonPropertyName("drama_id")]
    public long? DramaId { get; set; }

    /// <summary>authorized_appid（官方类型 <c>string</c>）：查询的被授权方小程序id。不填表示查询所有被授权方。</summary>
    [JsonPropertyName("authorized_appid")]
    public string? AuthorizedAppid { get; set; }

    /// <summary>offset（官方类型 <c>number</c>）：偏移值，分页请求用。默认0。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>limit（官方类型 <c>number</c>）：分页大小，默认100，最大值为1000。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }
}
