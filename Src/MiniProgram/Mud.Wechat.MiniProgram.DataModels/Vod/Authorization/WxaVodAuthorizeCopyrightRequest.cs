// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「增加版权授权」请求体（<c>POST /wxa/sec/vod/authorizecopyright</c>）：可授权给主体或小程序。
/// </summary>
/// <remarks>
/// <para><b>按 <c>authorization_type</c> 二选一必填</b>（官方原文）：<c>2</c> ⇒ <c>authorized_appid</c> 必填；<c>1</c> ⇒ <c>authorized_subject_cert_no</c>（统一社会信用代码）必填。SDK 不做本地校验（做成可空属性）。</para>
/// <para><b>批量上限（官方原文）</b>：<c>drama_ids</c> 最多 100 个；只有「受版权保护的剧目」才能授权（提审时经 <c>copyright.apply_for_copyright_protection=1</c> 申请）。</para>
/// <para><b>按主体授权是「传染性」的</b>（官方原文）：该主体下所有具备【文娱-微短剧】或【文娱-互动微短剧】类目的小程序<b>自动获得授权</b>。</para>
/// <para><b>业务限制</b>：双方都需上述类目；指定到期时间时授权时长不得小于 7 天；<c>expire_time</c> 不传或传 0 表示永久。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizecopyright/api_authorizecopyright.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodAuthorizeCopyrightRequest
{
    /// <summary>authorization_type（官方类型 <c>number</c>，<b>必填</b>）：授权类型：1. 授权给主体；2. 授权给小程序；</summary>
    [JsonPropertyName("authorization_type")]
    public long? AuthorizationType { get; set; }

    /// <summary>authorized_appid（官方类型 <c>string</c>）：被授权方小程序id，`authorization_type` 为 2 时必填。</summary>
    [JsonPropertyName("authorized_appid")]
    public string? AuthorizedAppid { get; set; }

    /// <summary>authorized_subject_cert_no（官方类型 <c>string</c>）：被授权方主体证件号，必须为统一社会信用代码，`authorization_type` 为 1 时必填。</summary>
    [JsonPropertyName("authorized_subject_cert_no")]
    public string? AuthorizedSubjectCertNo { get; set; }

    /// <summary>drama_ids（官方类型 <c>numarray</c>）：要授权的受版权保护的剧目 id，最多 100 个。</summary>
    [JsonPropertyName("drama_ids")]
    public List<long>? DramaIds { get; set; }

    /// <summary>expire_time（官方类型 <c>number</c>）：不传或者传 0，表示永久有效；否则表示授权到期的时间戳。如果指定授权到期时间，授权时长不能小于 7 天（从当前时间算起）。默认值为 0。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }
}
