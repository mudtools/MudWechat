// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 剧目内剧集引用（官方包装形态：<c>media_list</c> 数组的元素对象只有 <c>media_id</c>）。
/// </summary>
/// <remarks>
/// <para><b>不共用 <see cref="WxaVodMediaInfo"/></b>：官方此处子表只有单字段，媒资详情须另走「获取媒资详细信息」。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_getdrama.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodDramaMediaItem
{
    /// <summary>media_id（官方类型 <c>number</c>）：媒资文件id。</summary>
    [JsonPropertyName("media_id")]
    public long? MediaId { get; set; }
}
