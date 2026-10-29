// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「确认上传」请求体（<c>POST /wxa/sec/vod/commitupload</c>）：提交各分片的 <c>etag</c> 供官方合并。
/// </summary>
/// <remarks>
/// <para><b>官方表类型标注为 <c>object</c>，返回示例证明是数组</b>（<c>"media_part_infos": [ {part_number, etag}, … ]</c>）⇒ 本 SDK 按示例落 <c>List</c>，与 <c>getcdnlogs</c> 的 <c>domestic_cdn_logs</c> 同判据（示例优先于表格）。</para>
/// <para><b>顺序契约（官方原文）</b>：<c>media_part_infos</c> 与 <c>cover_part_infos</c> 必须按 <c>part_number</c> 从小到大排序、从 1 开始、连续且不重复。SDK 不本地校验（照录官方错误码 <c>10093014</c>）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_commitupload.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodCommitUploadRequest
{
    /// <summary>upload_id（官方类型 <c>string</c>，<b>必填</b>）：一次分片上传的唯一标识，由申请分片上传接口返回。</summary>
    [JsonPropertyName("upload_id")]
    public string? UploadId { get; set; }

    /// <summary>media_part_infos（官方类型 <c>object</c>，<b>必填</b>）：本次分片上传中媒体文件每个分片的信息。</summary>
    [JsonPropertyName("media_part_infos")]
    public List<WxaVodPartInfo>? MediaPartInfos { get; set; }

    /// <summary>cover_part_infos（官方类型 <c>object</c>）：本次分片上传中封面图片文件每个分片的信息。</summary>
    [JsonPropertyName("cover_part_infos")]
    public List<WxaVodPartInfo>? CoverPartInfos { get; set; }
}
