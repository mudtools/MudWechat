// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vip;

/// <summary>更新小程序会员信息请求体（<c>POST /wxa/vip/user/info/update</c>）。</summary>
/// <remarks>官方文档：<c>https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_wxa_updateuserinfo</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class WxaVipUpdateUserRequest
{
    /// <summary>小程序用户 openid（<c>wxa_openid</c>，必填）。</summary>
    [JsonPropertyName("wxa_openid")]
    public string? WxaOpenId { get; set; }

    /// <summary>小程序会员信息（<c>info</c>，必填；更新后小店侧同步展示最新会员信息）。</summary>
    [JsonPropertyName("info")]
    public WxaVipMemberInfo? Info { get; set; }
}
