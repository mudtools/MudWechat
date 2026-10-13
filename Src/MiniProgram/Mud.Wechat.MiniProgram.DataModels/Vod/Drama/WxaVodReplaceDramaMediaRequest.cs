// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「替换审核通过的剧集」请求体（<c>POST /wxa/sec/vod/replacedramamedia</c>）：扁平三参数。
/// </summary>
/// <remarks>
/// <para><b>与「提交替换剧集审核」是两支不同端点</b>：本支一次换一集、直接生效于已审核通过的剧目（官方要求新剧集也已审核通过），那支是提交一批并重新走审核 ⇒ 参数形态（扁平 <c>old_media_id</c>/<c>new_media_id</c> vs 对象数组）也不同，不得混用。</para>
/// <para><b>官方参数名拼写为 <c>meida_id</c> 的是「说明」列（旧剧集meida_id）而非参数名</b> ⇒ 参数名照录 <c>old_media_id</c>/<c>new_media_id</c>。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_replacedramamedia.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodReplaceDramaMediaRequest
{
    /// <summary>drama_id（官方类型 <c>number</c>，<b>必填</b>）：剧目id</summary>
    [JsonPropertyName("drama_id")]
    public long? DramaId { get; set; }

    /// <summary>old_media_id（官方类型 <c>number</c>，<b>必填</b>）：旧剧集meida_id</summary>
    [JsonPropertyName("old_media_id")]
    public long? OldMediaId { get; set; }

    /// <summary>new_media_id（官方类型 <c>number</c>，<b>必填</b>）：新剧集meida_id</summary>
    [JsonPropertyName("new_media_id")]
    public long? NewMediaId { get; set; }
}
