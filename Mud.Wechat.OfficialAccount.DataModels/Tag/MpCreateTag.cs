// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Tag;

/// <summary>
/// 创建标签请求体（<c>createTag</c>，<c>POST /cgi-bin/tags/create</c>）。
/// </summary>
/// <remarks>官方请求示例：<c>{ "tag": { "name": "广东" } }</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Tag")]
public class MpCreateTagRequest
{
    /// <summary>标签信息（仅需名称）。</summary>
    [JsonPropertyName("tag")]
    public MpTagName Tag { get; set; } = new();
}

/// <summary>
/// 创建标签响应（<c>createTag</c>）。
/// </summary>
/// <remarks>官方返回示例：<c>{ "tag": { "id": 134, "name": "广东" } }</c>（<b>不含 <c>count</c></b>）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Tag")]
public class MpCreateTagResponse : MpResponse
{
    /// <summary>新建的标签（仅含 <c>id</c> 与 <c>name</c>）。</summary>
    [JsonPropertyName("tag")]
    public MpTag? Tag { get; set; }
}
