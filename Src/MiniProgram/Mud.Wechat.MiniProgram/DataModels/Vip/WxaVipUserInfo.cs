// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vip;

/// <summary>小程序会员信息（响应侧 <c>info</c>，<c>user/info/get</c> 参数表）。</summary>
/// <remarks>
/// <b>字段集边界</b>：本 DTO 照官方 <c>user/info/get</c> 参数表<b>不含</b> <c>wxa_openid</c>
/// （该字段仅 <c>user/list/get</c> 列表元素携带）——响应侧与请求侧、单查与列表的
/// 字段集差异均精确表达，禁止「顺手统一」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class WxaVipUserInfo
{
    /// <summary>小程序定义的用户会员身份（<c>identity</c>）。</summary>
    [JsonPropertyName("identity")]
    public string? Identity { get; set; }

    /// <summary>小程序定义的用户会员 id（<c>outer_userid</c>）。</summary>
    [JsonPropertyName("outer_userid")]
    public string? OuterUserId { get; set; }

    /// <summary>用户当前已关联的小店 appid 列表（<c>related_shop_appid</c>，官方类型 array）。</summary>
    [JsonPropertyName("related_shop_appid")]
    public List<string>? RelatedShopAppIds { get; set; }
}
