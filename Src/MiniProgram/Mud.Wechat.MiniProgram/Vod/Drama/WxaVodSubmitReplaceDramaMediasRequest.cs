// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「提交替换剧集审核」请求体（<c>POST /wxa/sec/vod/submitreplacedramamedias</c>）。
/// </summary>
/// <remarks>
/// <para><b>前置态（官方原文）</b>：剧目必须审核通过后才允许提交替换剧集审核，且审核完成后会发送事件通知（本线无回调工程）。</para>
/// <para><b>与 <c>auditdrama</c> 的替换项共用 <see cref="WxaVodReplaceMediaItem"/></b>（官方子表字段名相同）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_submitreplacemedias.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodSubmitReplaceDramaMediasRequest
{
    /// <summary>drama_id（官方类型 <c>number</c>，<b>必填</b>）：剧目id</summary>
    [JsonPropertyName("drama_id")]
    public long? DramaId { get; set; }

    /// <summary>replace_media_list（官方类型 <c>objarray</c>，<b>必填</b>）：替换的剧集信息。</summary>
    [JsonPropertyName("replace_media_list")]
    public List<WxaVodReplaceMediaItem>? ReplaceMediaList { get; set; }
}
