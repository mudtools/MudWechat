// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>发表内容发表详细数据条目（官方 <c>getarticletotaldetail</c> 的 list 元素）。</summary>
/// <remarks>
/// 官方文档缺陷（照录）：返回参数表未列出 ref_date/msgid/title/content_url 字段行，
/// 仅出现在返回示例与变更日志（2026-03-03 补充文章标题和链接）——SDK 按示例形态建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpDataArticleTotalDetailItem
{
    /// <summary>获取或设置文章发表日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置图文消息标识（官方 <c>msgid</c>）。</summary>
    [JsonPropertyName("msgid")]
    public string? MsgId { get; set; }

    /// <summary>获取或设置发表类型（官方 <c>publish_type</c>：0 已通知 / 1 未开启通知）。</summary>
    [JsonPropertyName("publish_type")]
    public int? PublishType { get; set; }

    /// <summary>获取或设置逐日详细数据（官方 <c>detail_list</c>；每篇文章仅统计发表日期起 30 天内）。</summary>
    [JsonPropertyName("detail_list")]
    public List<MpDataTotalDetailStat>? DetailList { get; set; }

    /// <summary>获取或设置文章标题（官方 <c>title</c>；2026-03-03 变更日志补充）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置文章链接（官方 <c>content_url</c>；2026-03-03 变更日志补充）。</summary>
    [JsonPropertyName("content_url")]
    public string? ContentUrl { get; set; }
}
