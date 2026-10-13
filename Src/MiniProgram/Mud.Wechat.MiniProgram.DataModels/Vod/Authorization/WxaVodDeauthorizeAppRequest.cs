// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「解除账号授权」请求体（<c>POST /wxa/sec/vod/deauthorizeapp</c>）。
/// </summary>
/// <remarks>
/// <para><b>共用裁决不成立</b>：本支只有 <c>authorized_appid</c> 一参数，「增加账号授权」多一个 <c>authz_expire_time</c> ⇒ 字段集不同、分立两类型（守卫 DV4 锁定）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizeapp/api_deauthorizeapp.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodDeauthorizeAppRequest
{
    /// <summary>authorized_appid（官方类型 <c>string</c>，<b>必填</b>）：被授权方小程序id。</summary>
    [JsonPropertyName("authorized_appid")]
    public string? AuthorizedAppid { get; set; }
}
