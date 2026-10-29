// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Tag;

/// <summary>
/// 编辑标签请求体（<c>updateTag</c>，<c>POST /cgi-bin/tags/update</c>）。
/// </summary>
/// <remarks>
/// 官方字段表：<c>tag.id</c>（必填，标签 id）与 <c>tag.name</c>（必填，修改的标签名，UTF8 编码）。
/// 响应仅 <c>errcode</c> / <c>errmsg</c>（由 <see cref="MpResponse"/> 承载，故不设专用响应 DTO）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Tag")]
public class MpUpdateTagRequest
{
    /// <summary>标签信息（标识 + 新名称）。</summary>
    [JsonPropertyName("tag")]
    public MpTagIdName Tag { get; set; } = new();
}
