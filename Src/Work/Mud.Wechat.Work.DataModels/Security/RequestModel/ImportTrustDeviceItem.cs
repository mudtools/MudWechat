// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Security;

/// <summary>
/// 导入可信企业设备的单设备信息（<c>/cgi-bin/security/trustdevice/import</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class ImportTrustDeviceItem
{
    /// <summary>
    /// 获取或设置设备类型（Windows / Mac）。
    /// </summary>
    [JsonPropertyName("system")]
    public string System { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置 MAC 地址列表（Windows 必填、Mac 选填；每个设备最多 100 个）。
    /// </summary>
    [JsonPropertyName("mac_addr")]
    public List<string>? MacAddr { get; set; }

    /// <summary>
    /// 获取或设置主板 UUID（仅 Windows）。
    /// </summary>
    [JsonPropertyName("motherboard_uuid")]
    public string? MotherboardUuid { get; set; }

    /// <summary>
    /// 获取或设置硬盘序列号列表（仅 Windows；每个设备最多 100 个）。
    /// </summary>
    [JsonPropertyName("harddisk_uuid")]
    public List<string>? HarddiskUuid { get; set; }

    /// <summary>
    /// 获取或设置 Windows 域名（仅 Windows）。
    /// </summary>
    [JsonPropertyName("domain")]
    public string? Domain { get; set; }

    /// <summary>
    /// 获取或设置 Windows 计算机名（仅 Windows）。
    /// </summary>
    [JsonPropertyName("pc_name")]
    public string? PcName { get; set; }

    /// <summary>
    /// 获取或设置 Mac 序列号（system 为 Mac 时必填）。
    /// </summary>
    [JsonPropertyName("seq_no")]
    public string? SeqNo { get; set; }
}
