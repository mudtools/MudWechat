// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Draft;

/// <summary>获取草稿列表（<c>draft/batchget</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftBatchGetResponse : MpResponse
{
    /// <summary>获取或设置草稿素材总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    /// <summary>获取或设置本次获取数量（官方 <c>item_count</c>）。</summary>
    [JsonPropertyName("item_count")]
    public int ItemCount { get; set; }

    /// <summary>获取或设置素材列表（官方 <c>item</c>）。</summary>
    [JsonPropertyName("item")]
    public List<MpDraftListItem>? Items { get; set; }
}
