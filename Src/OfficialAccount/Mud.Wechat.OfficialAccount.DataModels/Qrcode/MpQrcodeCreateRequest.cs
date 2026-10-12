// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Qrcode;

/// <summary>
/// 生成带参数的二维码（<c>qrcode/create</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<c>action_name</c>/<c>action_info</c> 必填；
/// <c>expire_seconds</c> 仅临时二维码需要（<b>最大 2592000 秒即 30 天</b>）。
/// </para>
/// <para>
/// <b>二维码类型（官方引言原文）</b>：「临时二维码，是有过期时间的，最长可以设置为在二维码生成后的
/// 30 天（即 2592000 秒）后过期，但能够生成较多数量」；「永久二维码，是无过期时间的，但数量较少
/// （<b>目前为最多 10 万个</b>）」。
/// </para>
/// <para>
/// <b>scene_id / scene_str 互斥（按 action_name 形态隐含，官方无「互斥」明文——照录）</b>：
/// QR_SCENE/QR_LIMIT_SCENE → scene_id（临时 32 位非 0 整型 / 永久 1~100000）；
/// QR_STR_SCENE/QR_LIMIT_STR_SCENE → scene_str（字符串，长度 1~64）。
/// SDK 以可空超集建模（两字段并存于 <see cref="MpQrcodeScene"/>），由调用方按 action_name 携带其一。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Qrcode")]
public class MpQrcodeCreateRequest
{
    /// <summary>获取或设置二维码有效时间秒数（官方 <c>expire_seconds</c>，最大 2592000；仅临时二维码需要）。</summary>
    [JsonPropertyName("expire_seconds")]
    public int? ExpireSeconds { get; set; }

    /// <summary>获取或设置二维码类型（官方 <c>action_name</c>，四形态见 <c>MpQrcodeActionNames</c>）。</summary>
    [JsonPropertyName("action_name")]
    public string ActionName { get; set; } = string.Empty;

    /// <summary>获取或设置二维码详细信息（官方 <c>action_info</c>，必填）。</summary>
    [JsonPropertyName("action_info")]
    public MpQrcodeActionInfo ActionInfo { get; set; } = new MpQrcodeActionInfo();
}
