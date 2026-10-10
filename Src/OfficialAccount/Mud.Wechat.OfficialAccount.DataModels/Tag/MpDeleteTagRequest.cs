// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Tag;

/// <summary>
/// 删除标签请求体（<c>deleteTag</c>，<c>POST /cgi-bin/tags/delete</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>tag.id</c>（标签 ID）。
/// </para>
/// <para>
/// <b>官方文档缺陷（勿「顺手修正」为可空）</b>：该页字段表把 <c>tag</c> 与 <c>tag.id</c> 的「必填」列
/// 标为「否」，但接口语义上必须提供待删标签标识（否则无从删除，官方亦给 <c>44002 empty post data</c>
/// 错误码）。本 SDK 按必填建模，<b>不</b>跟随错误的文档标注。
/// </para>
/// <para>响应仅 <c>errcode</c> / <c>errmsg</c>（由 <see cref="MpResponse"/> 承载，故不设专用响应 DTO）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Tag")]
public class MpDeleteTagRequest
{
    /// <summary>待删除的标签标识。</summary>
    [JsonPropertyName("tag")]
    public MpTagId Tag { get; set; } = new();
}
