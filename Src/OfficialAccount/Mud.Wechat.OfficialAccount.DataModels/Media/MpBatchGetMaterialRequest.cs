// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Media;

/// <summary>获取永久素材列表（<c>material/batchget_material</c>）请求体。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：<b>POST</b>，字段表逐字段核验——<c>type</c>（image/video/voice/news）、
/// <c>offset</c>（从全部素材的该偏移位置开始返回，0 表示从第一个素材返回）、
/// <c>count</c>（返回素材的数量，取值在 <b>1 到 20 之间</b>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpBatchGetMaterialRequest
{
    /// <summary>获取或设置素材的类型（官方 <c>type</c>：<c>MpMediaTypes</c> 四类）。</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>获取或设置偏移位置（官方 <c>offset</c>，0 表示从第一个素材返回）。</summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>获取或设置返回数量（官方 <c>count</c>，取值 1~20）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }
}
