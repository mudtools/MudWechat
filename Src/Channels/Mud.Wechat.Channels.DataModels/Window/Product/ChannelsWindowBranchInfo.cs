// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>橱窗商品分店信息（官方 <c>branch_info</c> 对象，<c>getwindowproduct</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsWindowBranchInfo
{
    /// <summary>获取或设置分店 ID（官方 <c>branch_id</c>）。</summary>
    [JsonPropertyName("branch_id")]
    public long? BranchId { get; set; }

    /// <summary>获取或设置分店名（官方 <c>branch_name</c>）。</summary>
    [JsonPropertyName("branch_name")]
    public string? BranchName { get; set; }

    /// <summary>获取或设置分店状态（官方 <c>branch_status</c>，见 <see cref="ChannelsWindowBranchStatuses"/>）。</summary>
    [JsonPropertyName("branch_status")]
    public int? BranchStatus { get; set; }
}
