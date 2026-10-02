// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Security;

/// <summary>
/// 文件操作记录明细（<c>/cgi-bin/security/get_file_oper_record</c> 响应 record_list 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class FileOperRecordItem
{
    /// <summary>
    /// 获取或设置操作时间（Unix 秒）。
    /// </summary>
    [JsonPropertyName("time")]
    public long? Time { get; set; }

    /// <summary>
    /// 获取或设置操作者 userid（企业内部用户时返回）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置操作者为企业外部用户时的信息（企业外部用户时返回）。
    /// </summary>
    [JsonPropertyName("external_user")]
    public FileOperRecordExternalUser? ExternalUser { get; set; }

    /// <summary>
    /// 获取或设置操作明细（类型与来源）。
    /// </summary>
    [JsonPropertyName("operation")]
    public FileOperRecordOperation? Operation { get; set; }

    /// <summary>
    /// 获取或设置文件操作说明。
    /// </summary>
    [JsonPropertyName("file_info")]
    public string? FileInfo { get; set; }

    /// <summary>
    /// 获取或设置文件 MD5。
    /// </summary>
    [JsonPropertyName("file_md5")]
    public string? FileMd5 { get; set; }

    /// <summary>
    /// 获取或设置文件大小（字节）。
    /// </summary>
    [JsonPropertyName("file_size")]
    public long? FileSize { get; set; }

    /// <summary>
    /// 获取或设置申请人姓名（操作类型为下载申请/拒绝下载申请时返回）。
    /// </summary>
    [JsonPropertyName("applicant_name")]
    public string? ApplicantName { get; set; }

    /// <summary>
    /// 获取或设置设备类型：1-企业可信设备 2-个人可信设备（仅下载操作返回）。
    /// </summary>
    [JsonPropertyName("device_type")]
    public int? DeviceType { get; set; }

    /// <summary>
    /// 获取或设置设备编码（仅下载且操作者为内部成员时返回）。
    /// </summary>
    [JsonPropertyName("device_code")]
    public string? DeviceCode { get; set; }
}
