// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 分片信息（<c>part_number</c> + <c>etag</c>）。
/// </summary>
/// <remarks>
/// <para><b>共用裁决</b>：<c>media_part_infos</c> 与 <c>cover_part_infos</c> 的官方子表字段名完全相同（仅必填标记不同：媒体侧必填、封面侧选填）⇒ 共用本类型，靠「属性可空 + 调用方按官方必填面赋值」承载差异。</para>
/// <para><b><c>etag</c> 值自带双引号</b>（官方示例 <c>"\"d899fbd1…\""</c>）⇒ 须原样回填 <c>uploadpart</c> 返回的值，不得自行加剥引号。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_commitupload.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodPartInfo
{
    /// <summary>part_number（官方类型 <c>number</c>，<b>必填</b>）：分片编号。</summary>
    [JsonPropertyName("part_number")]
    public long? PartNumber { get; set; }

    /// <summary>etag（官方类型 <c>string</c>，<b>必填</b>）：使用上传分片接口上传成功后返回的 etag 的值</summary>
    [JsonPropertyName("etag")]
    public string? Etag { get; set; }
}
