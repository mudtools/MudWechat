// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Security;

/// <summary>
/// 可信设备信息（<c>/cgi-bin/security/trustdevice/list</c>、<c>/cgi-bin/security/trustdevice/get_by_user</c> 响应 device_list 项）。
/// </summary>
/// <remarks>
/// 设备未确认为可信企业设备（status 为 2、4、6）时，MAC 地址、序列号、主板 UUID 等返回脱敏数据。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class TrustDeviceInfo
{
    /// <summary>
    /// 获取或设置设备编码。
    /// </summary>
    [JsonPropertyName("device_code")]
    public string? DeviceCode { get; set; }

    /// <summary>
    /// 获取或设置设备类型（Windows / Mac）。
    /// </summary>
    [JsonPropertyName("system")]
    public string? System { get; set; }

    /// <summary>
    /// 获取或设置 MAC 地址列表。
    /// </summary>
    [JsonPropertyName("mac_addr")]
    public List<string>? MacAddr { get; set; }

    /// <summary>
    /// 获取或设置主板 UUID。
    /// </summary>
    [JsonPropertyName("motherboard_uuid")]
    public string? MotherboardUuid { get; set; }

    /// <summary>
    /// 获取或设置硬盘序列号列表。
    /// </summary>
    [JsonPropertyName("harddisk_uuid")]
    public List<string>? HarddiskUuid { get; set; }

    /// <summary>
    /// 获取或设置 Windows 域名。
    /// </summary>
    [JsonPropertyName("domain")]
    public string? Domain { get; set; }

    /// <summary>
    /// 获取或设置 Windows 计算机名。
    /// </summary>
    [JsonPropertyName("pc_name")]
    public string? PcName { get; set; }

    /// <summary>
    /// 获取或设置 Mac 序列号。
    /// </summary>
    [JsonPropertyName("seq_no")]
    public string? SeqNo { get; set; }

    /// <summary>
    /// 获取或设置设备最后登录时间（Unix 秒）。
    /// </summary>
    [JsonPropertyName("last_login_time")]
    public long? LastLoginTime { get; set; }

    /// <summary>
    /// 获取或设置最后登录成员 userid。
    /// </summary>
    [JsonPropertyName("last_login_userid")]
    public string? LastLoginUserid { get; set; }

    /// <summary>
    /// 获取或设置设备归属/确认时间戳（Unix 秒）。
    /// </summary>
    [JsonPropertyName("confirm_timestamp")]
    public long? ConfirmTimestamp { get; set; }

    /// <summary>
    /// 获取或设置归属/确认成员 userid。
    /// </summary>
    [JsonPropertyName("confirm_userid")]
    public string? ConfirmUserid { get; set; }

    /// <summary>
    /// 获取或设置通过申报的管理员 userid。
    /// </summary>
    [JsonPropertyName("approved_userid")]
    public string? ApprovedUserid { get; set; }

    /// <summary>
    /// 获取或设置设备来源：0-未知 1-成员确认 2-管理员导入 3-成员自主申报。
    /// </summary>
    [JsonPropertyName("source")]
    public int? Source { get; set; }

    /// <summary>
    /// 获取或设置设备状态：1-已导入未登录 2-待邀请 3-待管理员确认为企业设备 4-待管理员确认为个人设备
    /// 5-已确认为可信企业设备 6-已确认为可信个人设备。
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}
