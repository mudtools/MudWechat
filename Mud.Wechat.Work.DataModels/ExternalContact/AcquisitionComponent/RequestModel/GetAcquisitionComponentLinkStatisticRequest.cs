// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.AcquisitionComponent;

/// <summary>
/// 获客助手组件查询获客链接使用详情请求体（<c>/cgi-bin/externalcontact/customer_acquisition/statistic</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AcquisitionComponent")]
public class GetAcquisitionComponentLinkStatisticRequest
{
    /// <summary>
    /// 获取或设置获客链接 id（官方必填；组件版仅可查询授权给获客助手组件的链接）。
    /// </summary>
    [JsonPropertyName("link_id")]
    public string? LinkId { get; set; }

    /// <summary>
    /// 获取或设置统计起始时间（Unix 秒级时间戳，官方必填；统计最小粒度为天，开始日期与结束日期闭区间）。
    /// </summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>
    /// 获取或设置统计结束时间（Unix 秒级时间戳，官方必填；查询日期跨度不超过 30 天，仅支持 180 天内的数据查询）。
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}
