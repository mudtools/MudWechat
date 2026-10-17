// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>获取推客的注册状态以及和机构的绑定状态请求（官方 <c>promoter/get_promoter_register_and_bind_status</c> 请求体）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterGetRegisterAndBindStatusRequest
{
    /// <summary>获取或设置推客在小程序中的 openid（官方 <c>sharer_openid</c>，必须是机构绑定小程序的用户 openid，与 <c>sharer_appid</c> 二选一）。</summary>
    [JsonPropertyName("sharer_openid")]
    public string? SharerOpenId { get; set; }

    /// <summary>获取或设置推客在微信电商平台注册的身份标识（官方 <c>sharer_appid</c>，与 <c>sharer_openid</c> 二选一）。</summary>
    [JsonPropertyName("sharer_appid")]
    public string? SharerAppId { get; set; }

    /// <summary>获取或设置是否走简易版本注册（官方 <c>is_simple_register</c>，必填。简易注册不要求推客开通商户号，但分佣只能走机构自己分佣；非简易注册要求推客开通商户号作为收款账户，可平台分佣）。</summary>
    [JsonPropertyName("is_simple_register")]
    public bool IsSimpleRegister { get; set; }
}
