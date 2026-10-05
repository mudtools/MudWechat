// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取会议成员报名 ID 请求体（<c>/cgi-bin/meeting/enroll/query_by_tmp_openid</c>；通过会中成员的 tmp_openid 查询对应的报名 ID）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class QueryMeetingEnrollIdsRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>
    /// 获取或设置查询报名 ID 的排序规则（当该账号存在多条报名记录（手机号导入、手动报名等）时，该接口返回的顺序）：
    /// 1 - 优先查询手机号导入报名，再查询成员手动报名（默认值）；2 - 优先查询成员手动报名，再查手机号导入。
    /// </summary>
    [JsonPropertyName("sorting_rules")]
    public int? SortingRules { get; set; }

    /// <summary>
    /// 获取或设置当场会议的成员临时 ID 数组（官方必填，适用于所有成员；单次最多支持 500 条）。
    /// <para>成员的 tmp_openid 可通过「获取已参会成员列表」接口获取。</para>
    /// </summary>
    [JsonPropertyName("tmp_openid_list")]
    public List<string>? TmpOpenidList { get; set; }
}
