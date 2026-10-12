// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vip;

/// <summary>新增小程序会员信息请求体（<c>POST /wxa/vip/user/info/add</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_wxa_adduserinfo</c>。</para>
/// <para>
/// <b><c>session_id</c> 时效（官方原文）</b>：用户操作的会话 id，由小店传递给小程序，
/// <b>有效期 10 分钟</b>——宿主不得持久化复用，过期调用报参数异常（errcode 1）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class WxaVipAddUserRequest
{
    /// <summary>用户操作的会话 id（<c>session_id</c>，必填；由小店传递给小程序，有效期 10 分钟）。</summary>
    [JsonPropertyName("session_id")]
    public string? SessionId { get; set; }

    /// <summary>小程序用户 openid（<c>wxa_openid</c>，必填；须为本小程序 appid 的用户，否则报 errcode 2）。</summary>
    [JsonPropertyName("wxa_openid")]
    public string? WxaOpenId { get; set; }

    /// <summary>小程序会员信息（<c>info</c>，必填），见 <see cref="WxaVipMemberInfo"/>。</summary>
    [JsonPropertyName("info")]
    public WxaVipMemberInfo? Info { get; set; }
}
