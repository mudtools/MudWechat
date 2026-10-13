// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「解除版权授权」请求体（<c>POST /wxa/sec/vod/deauthorizecopyright</c>）：与授权侧同过滤面、无到期时间。
/// </summary>
/// <remarks>
/// <para><b>与「增加版权授权」不共用</b>：官方两页参数表差一个 <c>expire_time</c> ⇒ 分立类型，避免「解除时传到期时间」的非法态。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizecopyright/api_deauthorizecopyright.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodDeauthorizeCopyrightRequest
{
    /// <summary>authorization_type（官方类型 <c>number</c>，<b>必填</b>）：被授权方授权类型：1. 主体；2. 小程序；</summary>
    [JsonPropertyName("authorization_type")]
    public long? AuthorizationType { get; set; }

    /// <summary>authorized_appid（官方类型 <c>string</c>）：被授权方小程序id，`authorization_type` 为 2 时必填。</summary>
    [JsonPropertyName("authorized_appid")]
    public string? AuthorizedAppid { get; set; }

    /// <summary>authorized_subject_cert_no（官方类型 <c>string</c>）：被授权方主体证件号，必须为统一社会信用代码，`authorization_type` 为 1 时必填。</summary>
    [JsonPropertyName("authorized_subject_cert_no")]
    public string? AuthorizedSubjectCertNo { get; set; }

    /// <summary>drama_ids（官方类型 <c>numarray</c>）：要解除授权的受版权保护的剧目 id，最多 100 个</summary>
    [JsonPropertyName("drama_ids")]
    public List<long>? DramaIds { get; set; }
}
