// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Vip;

/// <summary>小店获取关联小程序信息（<c>v3/wxa/info/get</c>）响应。</summary>
/// <remarks>官方契约：<b>POST</b> + 空请求体；令牌为小店 access_token（第三方代调用权限集 id 186）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class ChannelsVipGetWxaInfoResponse : ChannelsResponse
{
    /// <summary>获取或设置关联的小程序 appid（官方 <c>wxa_appid</c>）。</summary>
    [JsonPropertyName("wxa_appid")]
    public string? WxaAppId { get; set; }

    /// <summary>获取或设置关联小程序信息（官方 <c>info</c>）。</summary>
    [JsonPropertyName("info")]
    public ChannelsVipWxaInfo? Info { get; set; }

    /// <summary>获取或设置上线状态（官方 <c>state</c>，见 <see cref="ChannelsVipWxaStates"/>；为 2/3 时仅白名单用户可在小店看到会员入口）。</summary>
    [JsonPropertyName("state")]
    public int? State { get; set; }
}
