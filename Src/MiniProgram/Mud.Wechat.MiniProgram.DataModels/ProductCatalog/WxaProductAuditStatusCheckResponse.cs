// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 查询审核状态应答（<c>POST /product/register/check_audit_status</c>）。
/// </summary>
/// <remarks>
/// <para>状态枚举 1 当前不可操作 / 2 正常可操作 / 3 已提交审核中 / 4 审核失败被驳回 / 5 成功 / 6 冻结。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductAuditStatusCheckResponse : WxaResponse
{
    /// <summary>审核状态数据。</summary>
    [JsonPropertyName("data")]
    public WxaProductAuditStatusData? Data { get; set; }
}
