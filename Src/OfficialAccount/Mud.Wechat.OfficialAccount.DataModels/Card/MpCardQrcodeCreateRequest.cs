// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

// ---------------------------------------------------------------- 卡券二维码（POST /card/qrcode/create）

/// <summary>
/// 创建卡券 / 礼品码二维码（<c>POST /card/qrcode/create</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>与公众号「带参二维码」域无重叠</b>：本端点路由为 <c>/card/qrcode/create</c>，
/// 与 <c>IMpQrcodeService</c> 的 <c>/cgi-bin/qrcode/create</c> 是两个不同 URI ⇒
/// 不受 <c>MpRouteCountGuard</c> RC3「同一路由不得跨接口」约束，但两域职责在接口 XML 中互相点名。
/// </para>
/// <para><b>顶层三字段</b>：<c>action_name</c>（官方动作枚举，取值<b>待逐页核验</b>）+ <c>action_info</c> + <c>expire_seconds</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQrcodeCreateRequest
{
    /// <summary>获取或设置二维码有效秒数（官方 <c>expire_seconds</c>，选填；上限数值<b>待官方逐页核验</b>）。</summary>
    [JsonPropertyName("expire_seconds")]
    public long? ExpireSeconds { get; set; }

    /// <summary>获取或设置动作类型（官方 <c>action_name</c>，必填；取值表<b>待官方逐页核验</b>，SDK 不建枚举常量）。</summary>
    [JsonPropertyName("action_name")]
    public string ActionName { get; set; } = string.Empty;

    /// <summary>获取或设置动作信息（官方 <c>action_info</c>，必填）。</summary>
    [JsonPropertyName("action_info")]
    public MpCardQrcodeActionInfo? ActionInfo { get; set; }
}
