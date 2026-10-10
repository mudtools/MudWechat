// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.ContactWay;

/// <summary>
/// 获取企业已配置的「联系我」列表请求体（<c>/cgi-bin/externalcontact/list_contact_way</c>）。
/// <para>该接口不包含临时会话模式的「联系我」，且仅可查询 2021-07-10 之后创建的记录。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ContactWay")]
public class ListContactWayRequest
{
    /// <summary>
    /// 获取或设置「联系我」创建的起始时间戳（默认为 90 天前）。
    /// </summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>
    /// 获取或设置「联系我」创建的结束时间戳（默认为当前时间）。
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>
    /// 获取或设置用于分页查询的游标，由上一次调用返回，首次调用不填。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置分页预期请求的数据量（默认为 100，最大值为 1000）。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
