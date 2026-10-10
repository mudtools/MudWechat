// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Tag;

/// <summary>
/// 获取标签响应（<c>getTags</c>，<c>GET /cgi-bin/tags/get</c>）。
/// </summary>
/// <remarks>
/// 官方返回示例：<c>{ "tags": [ { "id": 1, "name": "…", "count": 0 }, … ] }</c>；
/// 接口<b>无请求体</b>（故不设请求 DTO）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Tag")]
public class MpGetTagsResponse : MpResponse
{
    /// <summary>标签列表（失败时官方返回 <c>errcode</c>/<c>errmsg</c>，本字段缺省为 <c>null</c>）。</summary>
    [JsonPropertyName("tags")]
    public List<MpTag>? Tags { get; set; }
}
