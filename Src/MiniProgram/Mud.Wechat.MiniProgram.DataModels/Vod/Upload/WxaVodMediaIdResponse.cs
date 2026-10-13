// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「媒资 id」应答（<c>errcode</c> / <c>errmsg</c> / <c>media_id</c>），单个文件上传与确认上传共用。
/// </summary>
/// <remarks>
/// <para><b>两处共用裁决</b>：<c>singlefileupload</c> 与 <c>commitupload</c> 的返回体表逐字段相同（单字段 <c>media_id</c>）⇒ 共用本类型，与 <see cref="WxaVodMediaIdRequest"/> 的请求侧共用形成对称。</para>
/// <para><b>单文件上传请求侧无 DTO</b>：官方 <c>Content-Type</c> 为 <c>multipart/form-data</c> ⇒ 走声明式 <c>[MultipartForm] IFormContent</c> 通路（仓内既有形态：企业微信临时素材、公众号客服素材、开放平台资质文件），表单字段名须照官方原文（<c>media_name</c> / <c>media_type</c> / <c>media_data</c> / <c>cover_type</c> / <c>cover_data</c> / <c>source_context</c>）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_singlefileupload.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodMediaIdResponse : WxaResponse
{
    /// <summary>media_id（官方类型 <c>number</c>）：媒体文件id。</summary>
    [JsonPropertyName("media_id")]
    public long? MediaId { get; set; }
}
