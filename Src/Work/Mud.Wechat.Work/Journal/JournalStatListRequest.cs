// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Journal;

/// <summary>
/// 获取汇报统计数据请求体（<c>/cgi-bin/oa/journal/get_stat_list</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Journal")]
public class JournalStatListRequest
{
    /// <summary>获取或设置统计范围开始时间戳（官方 starttime，秒级，必填）。</summary>
    [JsonPropertyName("starttime")]
    public long StartTimestamp { get; set; }

    /// <summary>获取或设置统计范围结束时间戳（官方 endtime，秒级，必填；与 starttime 的时间区间最大长度为一年）。</summary>
    [JsonPropertyName("endtime")]
    public long EndTimestamp { get; set; }

    /// <summary>获取或设置汇报表单 ID（官方 template_id，<b>必填</b>、不多于 256 字节；本接口按单一汇报表单取统计，表单 id 的获取路径见接口文档）。</summary>
    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; } = string.Empty;
}
