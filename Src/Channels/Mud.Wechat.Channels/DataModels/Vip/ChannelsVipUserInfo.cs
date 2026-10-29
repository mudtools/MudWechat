// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Vip;

/// <summary>
/// 会员用户信息（官方 <c>info</c> 对象，<c>info/get</c>；官方 <c>list</c> 数组元素，<c>list/get</c>）——
/// 两处字段集完全一致（openid / unionid / user_grade_info）⇒ 共用同一 DTO。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class ChannelsVipUserInfo
{
    /// <summary>获取或设置用户 openid（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置用户 unionid（官方 <c>unionid</c>）。</summary>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }

    /// <summary>获取或设置用户等级信息（官方 <c>user_grade_info</c>）。</summary>
    [JsonPropertyName("user_grade_info")]
    public ChannelsVipUserGradeInfo? UserGradeInfo { get; set; }
}
