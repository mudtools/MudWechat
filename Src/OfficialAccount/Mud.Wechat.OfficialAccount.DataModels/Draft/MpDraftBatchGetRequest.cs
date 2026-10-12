// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Draft;

/// <summary>获取草稿列表（<c>draft/batchget</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftBatchGetRequest
{
    /// <summary>获取或设置偏移位置（官方 <c>offset</c>，0 表示从第一个素材返回）。</summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>获取或设置返回数量（官方 <c>count</c>，取值 1~20）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>获取或设置是否不返回 content 字段（官方 <c>no_content</c>：1 不返回 / 0 正常返回，默认 0）。</summary>
    [JsonPropertyName("no_content")]
    public int? NoContent { get; set; }
}
