// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 微盘文件水印设置（获取文件权限信息响应 watermark 与修改文件安全设置请求 watermark 共用结构）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约差异：写入口「修改文件安全设置」（<c>file_secure_setting</c>）官方仅开放 text /
/// margin_type / show_visitor_name / show_text 四个可写字段（不填保持原样）；
/// force_by_admin 与 force_by_space_admin 为读取回显字段，写入时不应填充
/// （<c>DefaultIgnoreCondition = WhenWritingNull</c> 保证未填充字段不会被序列化）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class WedriveFileWatermark
{
    /// <summary>获取或设置水印文字（不填保持原样）。</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>获取或设置水印密度类型：1 - 低密度水印；2 - 高密度水印（不填保持原样）。</summary>
    [JsonPropertyName("margin_type")]
    public ulong? MarginType { get; set; }

    /// <summary>获取或设置是否显示访问人名称（仅专业版支持；不填保持原样）。</summary>
    [JsonPropertyName("show_visitor_name")]
    public bool? ShowVisitorName { get; set; }

    /// <summary>获取或设置管理员是否强制使用水印（读取回显字段，写入口不开放）。</summary>
    [JsonPropertyName("force_by_admin")]
    public bool? ForceByAdmin { get; set; }

    /// <summary>获取或设置是否展示水印文本（不填保持原样）。</summary>
    [JsonPropertyName("show_text")]
    public bool? ShowText { get; set; }

    /// <summary>获取或设置空间管理员是否强制使用水印（读取回显字段，写入口不开放）。</summary>
    [JsonPropertyName("force_by_space_admin")]
    public bool? ForceBySpaceAdmin { get; set; }
}
