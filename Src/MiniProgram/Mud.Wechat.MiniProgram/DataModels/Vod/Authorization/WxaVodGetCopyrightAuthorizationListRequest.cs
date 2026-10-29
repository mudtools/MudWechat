// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「查询版权授权信息」请求体（<c>POST /wxa/sec/vod/getcopyrightauthorizationlist</c>，授权方视角）。
/// </summary>
/// <remarks>
/// <para><b>「不填即查全部」的默认值是 <c>0</c></b>（官方原文：<c>authorization_type</c> 不填默认 0 表示全部，<c>drama_id</c> 同理）⇒ 与枚举 <see cref="WxaVodAuthorizationTypes"/> 的 1/2 不重叠，0 不是合法枚举值。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizecopyright/api_getcopyrightauthorizationlist.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodGetCopyrightAuthorizationListRequest
{
    /// <summary>authorization_type（官方类型 <c>number</c>）：授权类型：1. 授权给主体；2. 授权给小程序。不填默认值为 0，表示查询全部。</summary>
    [JsonPropertyName("authorization_type")]
    public long? AuthorizationType { get; set; }

    /// <summary>authorized_appid（官方类型 <c>string</c>）：被授权方小程序id，authorization_type 为 2 时必填。</summary>
    [JsonPropertyName("authorized_appid")]
    public string? AuthorizedAppid { get; set; }

    /// <summary>authorized_subject_cert_no（官方类型 <c>string</c>）：被授权方主体证件号，必须为统一社会信用代码，authorization_type 为 1 时必填。</summary>
    [JsonPropertyName("authorized_subject_cert_no")]
    public string? AuthorizedSubjectCertNo { get; set; }

    /// <summary>drama_id（官方类型 <c>number</c>）：授权剧目 id。不填默认值为 0，表示查询全部。</summary>
    [JsonPropertyName("drama_id")]
    public long? DramaId { get; set; }

    /// <summary>offset（官方类型 <c>number</c>）：偏移值，分页请求用。默认 0。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>limit（官方类型 <c>number</c>）：分页大小，默认 100，最大值为 1000。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }
}
