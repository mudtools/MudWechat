// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 进件审核驳回明细（进件状态回显）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopWxPayAuditDetail
{
    /// <summary>被驳回的字段名。</summary>
    [JsonPropertyName("param_name")]
    public string? ParamName { get; set; }

    /// <summary>驳回原因（可能含个人信息的自然语言描述，不得进日志）。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }
}
