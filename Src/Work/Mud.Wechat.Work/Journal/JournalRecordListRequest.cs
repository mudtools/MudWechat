// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Journal;

/// <summary>
/// 获取汇报记录号列表请求体（<c>/cgi-bin/oa/journal/get_record_list</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Journal")]
public class JournalRecordListRequest
{
    /// <summary>获取或设置查询范围开始时间戳（官方 starttime，秒级，必填）。</summary>
    [JsonPropertyName("starttime")]
    public long StartTimestamp { get; set; }

    /// <summary>获取或设置查询范围结束时间戳（官方 endtime，秒级，必填；与 starttime 的间隔不可超过一个月）。</summary>
    [JsonPropertyName("endtime")]
    public long EndTimestamp { get; set; }

    /// <summary>获取或设置翻页游标（官方 cursor，必填；官方参数表口径为「首次请求传 0，非首次请求携带上一次请求返回的 next_cursor」，故本仓以非可空的 <c>int</c> 承载，缺省即 0 = 首页）。</summary>
    [JsonPropertyName("cursor")]
    public int Cursor { get; set; }

    /// <summary>获取或设置拉取条数（官方 limit，必填；官方一次拉取调用最多返回 100 个汇报记录，超出须多次拉取）。</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    /// <summary>获取或设置筛选条件列表（官方 filters，元素形态见 <see cref="JournalRecordFilter"/>）。</summary>
    [JsonPropertyName("filters")]
    public List<JournalRecordFilter>? FilterList { get; set; }
}
