// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vip;

/// <summary>小程序会员列表条目（<c>user/list/get</c> 返回 <c>list[]</c>）。</summary>
/// <remarks>
/// <para>
/// <b>与单查响应 <c>info</c> 的字段集差异照录</b>：列表元素<b>多</b>携带 <c>wxa_openid</c>
/// （官方原文如此），故不复用 <see cref="WxaVipUserInfo"/>。
/// </para>
/// <para>
/// <b>疑点照录</b>：官方「请求示例」JSON 中该字段拼作 <c>out_userid</c>（漏 er），
/// 与参数表 <c>outer_userid</c> 不一致——SDK 按参数表建模，序列化名以官方参数表为准。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class WxaVipUserListItem
{
    /// <summary>小程序定义的用户会员身份（<c>identity</c>）。</summary>
    [JsonPropertyName("identity")]
    public string? Identity { get; set; }

    /// <summary>小程序定义的用户会员 id（<c>outer_userid</c>；官方参数表拼写，请求示例中的 out_userid 为笔误）。</summary>
    [JsonPropertyName("outer_userid")]
    public string? OuterUserId { get; set; }

    /// <summary>小程序用户 openid（<c>wxa_openid</c>；仅列表元素携带，单查响应 info 无此字段）。</summary>
    [JsonPropertyName("wxa_openid")]
    public string? WxaOpenId { get; set; }

    /// <summary>用户已绑定的小店 appid 列表（<c>related_shop_appid</c>，官方类型 array）。</summary>
    [JsonPropertyName("related_shop_appid")]
    public List<string>? RelatedShopAppIds { get; set; }
}
