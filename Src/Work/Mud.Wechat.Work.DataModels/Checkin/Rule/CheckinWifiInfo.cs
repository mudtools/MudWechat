// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡地点 WiFi 打卡信息（打卡规则 <c>wifimac_infos</c> 元素，请求/响应共用形态）。
/// </summary>
/// <remarks>
/// <para>官方限制：wifiname、wifimac、bssid 均不可为空，且 wifiname 字符个数不可超过 40 个；wifimac 需要规则内唯一且合法；wifi/地点个数不可超过 500。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinWifiInfo
{
    /// <summary>获取或设置 WiFi 打卡地点名称（不可为空，字符个数不可超过 40 个）。</summary>
    [JsonPropertyName("wifiname")]
    public string? Wifiname { get; set; }

    /// <summary>获取或设置 WiFi 的无线路由器 MAC 地址/bssid（不可为空；需要规则内唯一）。</summary>
    [JsonPropertyName("wifimac")]
    public string? Wifimac { get; set; }
}
