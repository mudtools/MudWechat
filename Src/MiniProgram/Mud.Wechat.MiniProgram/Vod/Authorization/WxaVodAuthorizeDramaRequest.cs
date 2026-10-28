// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「增加剧目授权」请求体（<c>GET /wxa/sec/vod/authorizedrama</c>）：一次授权一批剧目给同一个小程序。
/// </summary>
/// <remarks>
/// <para><b><c>authorized</c> 官方参数表与官方示例自相矛盾 ⇒ 线格式按示例取 <c>authorized_appid</c></b>：本页请求示例为 <c>{"authorized_appid": "wx***", "drama_id": [100200, 100205], "authz_expire_time": 0}</c>，而参数表列作 <c>authorized</c>；同域其余 12 处被授权方字段名一律是 <c>authorized_appid</c>⇒ 采信示例与域内一致性（裁决留档见接口注释，守卫 DV4 锁定该改名）。</para>
/// <para><b>业务限制（官方原文）</b>：授权方与被授权方都需申请【文娱-微短剧】类目；被授权剧目必须已审核通过；指定到期时间时授权时长不得小于 7 天（从当前时间算起）。</para>
/// <para><b>不传或传 <c>0</c> 表示永久有效</b>（<c>authz_expire_time</c>）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizedrama/api_authorizedrama.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodAuthorizeDramaRequest
{
    /// <summary>drama_id（官方类型 <c>numarray</c>，<b>必填</b>）：授权的剧目ID</summary>
    [JsonPropertyName("drama_id")]
    public List<long>? DramaId { get; set; }

    /// <summary>线格式按官方示例取 <c>authorized_appid</c>，官方参数表列作 <c>authorized</c>｜authorized_appid（官方类型 <c>string</c>，<b>必填</b>）：被授权方小程序id。</summary>
    [JsonPropertyName("authorized_appid")]
    public string? AuthorizedAppid { get; set; }

    /// <summary>authz_expire_time（官方类型 <c>number</c>）：不传或者传0，表示永久有效；否则表示授权到期的时间戳</summary>
    [JsonPropertyName("authz_expire_time")]
    public long? AuthzExpireTime { get; set; }
}
