// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 图文群发总数据条目（官方 <c>getarticletotal</c> 的 list 元素；官方声明已停止维护）。
/// </summary>
/// <remarks>
/// 官方原文：「每天对应的数值为该文章到该日为止的<b>总量</b>（而不是当日的量）」；
/// 「最多统计发表日后 7 天数据」。官方文档缺陷（照录）：返回参数表未列出 details/msgid/title/ref_date
/// 字段行，仅出现在返回示例——SDK 按示例形态建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpArticleTotalItem
{
    /// <summary>获取或设置群发日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置图文消息标识（官方 <c>msgid</c>）。</summary>
    [JsonPropertyName("msgid")]
    public string? MsgId { get; set; }

    /// <summary>获取或设置图文消息的标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置逐日累计统计（官方 <c>details</c>；每项为该文章到该日为止的总量）。</summary>
    [JsonPropertyName("details")]
    public List<MpArticleTotalStat>? Details { get; set; }
}
