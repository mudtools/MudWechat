// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>「获取小程序会员信息」应答的 info 结构（官方字段）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class ChannelsVipUserInfoGetReplyInfo
{
    /// <summary>获取或设置是否为会员（true = 是；官方：返回 true 时本次关联走静默关联，无需用户跳转小程序确认）。</summary>
    [JsonPropertyName("is_vip")]
    public bool IsVip { get; set; }

    /// <summary>获取或设置小程序定义的用户会员身份（is_vip 为 true 时需填写）。</summary>
    [JsonPropertyName("identity")]
    public string? Identity { get; set; }

    /// <summary>获取或设置小程序定义的用户会员 id（is_vip 为 true 时可填写）。</summary>
    [JsonPropertyName("outer_userid")]
    public string? OuterUserId { get; set; }
}
