// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 版权授权记录（官方 <c>list(Array)</c> 元素，七字段同时含授权方与被授权方两侧标识）。
/// </summary>
/// <remarks>
/// <para><b>两处共用裁决</b>：「查询版权授权信息」与「查询被版权授权信息」的元素子表字段名完全相同 ⇒ 共用本类型；两个端点只是<b>视角</b>不同（我给出的 / 我得到的）。</para>
/// <para><b>同时含 <c>authorizer_appid</c> 与 <c>authorized_appid</c></b>：一字母之差方向相反 ⇒ 读写时务必按字段名而非位置取值（守卫 DV4 锁定该字段面）。</para>
/// <para><b>本类型内到期字段名是 <c>expire_time</c></b>，剧目/账号授权记录里是 <c>authz_expire_time</c> ⇒ 照录两侧原文，不「对齐」。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizecopyright/api_getcopyrightauthorizationlist.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodCopyrightAuthorization
{
    /// <summary>authorizer_appid（官方类型 <c>string</c>）：授权方小程序 id</summary>
    [JsonPropertyName("authorizer_appid")]
    public string? AuthorizerAppid { get; set; }

    /// <summary>authorization_type（官方类型 <c>number</c>）：授权类型</summary>
    [JsonPropertyName("authorization_type")]
    public long? AuthorizationType { get; set; }

    /// <summary>authorized_appid（官方类型 <c>string</c>）：被授权方小程序 id</summary>
    [JsonPropertyName("authorized_appid")]
    public string? AuthorizedAppid { get; set; }

    /// <summary>authorized_subject_cert_no（官方类型 <c>string</c>）：被授权方主体证件号</summary>
    [JsonPropertyName("authorized_subject_cert_no")]
    public string? AuthorizedSubjectCertNo { get; set; }

    /// <summary>drama_id（官方类型 <c>number</c>）：授权剧目 id</summary>
    [JsonPropertyName("drama_id")]
    public long? DramaId { get; set; }

    /// <summary>authorized_time（官方类型 <c>number</c>）：授权时间戳</summary>
    [JsonPropertyName("authorized_time")]
    public long? AuthorizedTime { get; set; }

    /// <summary>expire_time（官方类型 <c>number</c>）：授权到期时间戳，0 表示长期有效</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }
}
