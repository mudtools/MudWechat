// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「增加账号授权」请求体（<c>POST /wxa/sec/vod/authorizeapp</c>）：整账号级授权，无需逐剧目列举。
/// </summary>
/// <remarks>
/// <para><b>业务限制（官方原文）</b>：双方都需【文娱-微短剧】类目；指定到期时间时授权时长不得小于 7 天。</para>
/// <para><b>与「增加剧目授权」的差异</b>：本支作用域是该账号下的全部剧目（含后续新增），且是 POST；剧目级那支是 GET 且逐剧目返回结果 ⇒ 两者不可互相替代。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizeapp/api_authorizeapp.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodAuthorizeAppRequest
{
    /// <summary>authorized_appid（官方类型 <c>string</c>，<b>必填</b>）：被授权方小程序id。</summary>
    [JsonPropertyName("authorized_appid")]
    public string? AuthorizedAppid { get; set; }

    /// <summary>authz_expire_time（官方类型 <c>number</c>）：不传或者传0，表示永久有效；否则表示授权到期的时间戳。</summary>
    [JsonPropertyName("authz_expire_time")]
    public long? AuthzExpireTime { get; set; }
}
