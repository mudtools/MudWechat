// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 录制共享配置对象（修改会议录制共享设置请求 <c>sharing_config</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class RecordSharingConfig
{
    /// <summary>
    /// 获取或设置共享开关（默认为 true）：true - 开启；false - 未开启。
    /// <para>官方限制：未开启时不允许设置以下参数；修改为 false 关闭共享后，之前设置的共享设置将不保存。</para>
    /// </summary>
    [JsonPropertyName("enable_sharing")]
    public bool? EnableSharing { get; set; }

    /// <summary>
    /// 获取或设置共享权限类型：0 - 仅允许登录成员查看；1 - 仅企业内成员可查看；2 - 仅参会成员可查看；3 - 全部成员可查看；
    /// 4 - 通过权限审批的成员可查看（Rooms 不支持）；5 - 获取微信特邀链接的成员可查看（Rooms 不支持）。
    /// </summary>
    [JsonPropertyName("sharing_auth_type")]
    public int? SharingAuthType { get; set; }

    /// <summary>获取或设置是否开启密码（默认为 true）：true - 开启；false - 不开启。</summary>
    [JsonPropertyName("enable_password")]
    public bool? EnablePassword { get; set; }

    /// <summary>
    /// 获取或设置共享密码（默认随机生成）。
    /// <para>官方限制：当 <see cref="EnablePassword"/> = true 时必传；当 <see cref="EnablePassword"/> = false 时不可传。</para>
    /// </summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }

    /// <summary>获取或设置是否开启共享链接有效期（默认为 false）：true - 开启；false - 不开启。</summary>
    [JsonPropertyName("enable_sharing_expire")]
    public bool? EnableSharingExpire { get; set; }

    /// <summary>
    /// 获取或设置共享链接有效期（UNIX 时间戳，单位毫秒，默认为空）。
    /// </summary>
    /// <remarks>
    /// 官方限制：当 <see cref="EnableSharingExpire"/> = true 时必传；当 <see cref="EnableSharingExpire"/> = false 时不可传。
    /// </remarks>
    [JsonPropertyName("sharing_expire")]
    public long? SharingExpire { get; set; }

    /// <summary>获取或设置是否允许下载（默认为 false）：true - 允许下载；false - 不允许下载。</summary>
    [JsonPropertyName("allow_download")]
    public bool? AllowDownload { get; set; }
}
