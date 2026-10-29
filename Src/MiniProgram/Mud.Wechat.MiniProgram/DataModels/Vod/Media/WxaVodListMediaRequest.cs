// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「获取媒资列表」请求体（<c>POST /wxa/sec/vod/listmedia</c>）。
/// </summary>
/// <remarks>
/// <para><b>两个名称匹配参数的分工（官方原文）</b>：<c>media_name</c> 精确匹配、支持前置或后置 <c>%</c> 通配，但文件太多时模糊匹配可能拿不到结果；<c>media_name_fuzzy</c> 仅模糊匹配、<b>不需要</b>加 <c>%</c>。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_media/api_listmedia.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodListMediaRequest
{
    /// <summary>drama_id（官方类型 <c>number</c>）：根据剧目id获取剧集信息</summary>
    [JsonPropertyName("drama_id")]
    public long? DramaId { get; set; }

    /// <summary>media_name（官方类型 <c>string</c>）：媒资文件名，支持精确匹配、模糊匹配。文件太多时使用该参数进行模糊匹配可能无法得到结果，推荐使用 media_name_fuzzy 参数</summary>
    [JsonPropertyName("media_name")]
    public string? MediaName { get; set; }

    /// <summary>media_name_fuzzy（官方类型 <c>string</c>）：媒资文件名，模糊匹配</summary>
    [JsonPropertyName("media_name_fuzzy")]
    public string? MediaNameFuzzy { get; set; }

    /// <summary>start_time（官方类型 <c>number</c>）：媒资上传时间&gt;=start_time</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>end_time（官方类型 <c>number</c>）：媒资上传时间</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>limit（官方类型 <c>number</c>）：分页拉取的最大返回结果数。最大值：100</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>offset（官方类型 <c>number</c>）：分页拉取的起始偏移量</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }
}
