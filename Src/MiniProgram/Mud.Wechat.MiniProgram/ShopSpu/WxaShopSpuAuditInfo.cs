// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopSpu;

/// <summary>商品审核信息（<c>audit_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "ShopSpu")]
public class WxaShopSpuAuditInfo
{
    /// <summary>上一次提交时间，<c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("submit_time")]
    public string? SubmitTime { get; set; }

    /// <summary>上一次审核时间，<c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("audit_time")]
    public string? AuditTime { get; set; }

    /// <summary>拒绝理由，仅 <c>edit_status = 3</c>（审核失败）时出现。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }

    /// <summary>审核单 ID。</summary>
    [JsonPropertyName("audit_id")]
    public string? AuditId { get; set; }
}
