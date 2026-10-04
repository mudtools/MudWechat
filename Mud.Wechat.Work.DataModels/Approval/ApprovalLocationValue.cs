// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 位置控件值（Location）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalLocationValue
{
    /// <summary>
    /// 获取或设置纬度，精确到 6 位小数。
    /// </summary>
    /// <remarks>
    /// <para>官方示例按字符串传输（如 "30.547239"），故本模型以字符串承载。</para>
    /// </remarks>
    [JsonPropertyName("latitude")]
    public string? Latitude { get; set; }

    /// <summary>
    /// 获取或设置经度，精确到 6 位小数。
    /// </summary>
    /// <remarks>
    /// <para>官方示例按字符串传输（如 "104.063291"），故本模型以字符串承载。</para>
    /// </remarks>
    [JsonPropertyName("longitude")]
    public string? Longitude { get; set; }

    /// <summary>
    /// 获取或设置地点标题。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置地点详情地址。
    /// </summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>
    /// 获取或设置选择地点的时间（Unix时间戳）。
    /// </summary>
    [JsonPropertyName("time")]
    public long? Time { get; set; }
}
