// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DataZone;

/// <summary>
/// 会话组件敏感信息隐藏设置（设置配置）请求体（<c>/cgi-bin/chatdata/set_hide_sensitiveinfo_config</c>）。
/// <para>设置成员使用会话组件时，敏感信息（手机号、身份证号、银行卡号）是否打星。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataZone")]
public class SetDataZoneHideSensitiveInfoConfigRequest
{
    /// <summary>获取或设置成员的 userid（官方必填）。</summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    /// <summary>获取或设置敏感信息隐藏配置（官方必填，<see cref="DataZoneSensitiveInfoHideConfig"/>：hide_mobile / hide_idcard / hide_bankno）。</summary>
    [JsonPropertyName("config")]
    public DataZoneSensitiveInfoHideConfig? Config { get; set; }
}
