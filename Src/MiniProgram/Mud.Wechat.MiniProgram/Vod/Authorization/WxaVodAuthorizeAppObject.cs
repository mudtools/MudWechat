// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 账号级授权记录（官方 <c>objects(Array)</c> 元素，无剧目维度）。
/// </summary>
/// <remarks>
/// <para><b>比剧目级授权记录少 <c>drama_id</c></b>（照录官方三字段面）⇒ 不得复用 <see cref="WxaVodAuthorizeObject"/>。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizeapp/api_getauthorizeapps.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodAuthorizeAppObject
{
    /// <summary>authorized_appid（官方类型 <c>string</c>）：被授权方小程序id</summary>
    [JsonPropertyName("authorized_appid")]
    public string? AuthorizedAppid { get; set; }

    /// <summary>authorized_time（官方类型 <c>number</c>）：授权时间戳</summary>
    [JsonPropertyName("authorized_time")]
    public long? AuthorizedTime { get; set; }

    /// <summary>authz_expire_time（官方类型 <c>number</c>）：授权到期时间戳。0表示长期有效。</summary>
    [JsonPropertyName("authz_expire_time")]
    public long? AuthzExpireTime { get; set; }
}
