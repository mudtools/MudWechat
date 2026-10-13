// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Journal;

/// <summary>
/// 获取汇报记录号列表响应体（<c>/cgi-bin/oa/journal/get_record_list</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Journal")]
public class JournalRecordListResponse : WechatWorkResponse
{
    /// <summary>获取或设置汇报 UUID 列表（官方 journaluuid_list，逐条传给「获取汇报记录详情」）。</summary>
    [JsonPropertyName("journaluuid_list")]
    public List<string>? JournalIdList { get; set; }

    /// <summary>获取或设置翻页是否结束（官方 endflag；<b>官方以数字 0/1 承载布尔语义</b>，本仓无数值-布尔转换器，故原样以 <c>int</c> 承载）。</summary>
    [JsonPropertyName("endflag")]
    public int? Ending { get; set; }

    /// <summary>获取或设置下一页的翻页标记（官方 next_cursor，回填至下一次请求的 cursor）。</summary>
    [JsonPropertyName("next_cursor")]
    public int? NextCursor { get; set; }
}
