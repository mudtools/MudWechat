// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>撤销类目审核（<c>cancelauditcategory</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCancelCategoryAuditRequest
{
    /// <summary>
    /// 获取或设置审核单 id（官方 <c>audit_id</c>，必填；提交审核时返回）——
    /// 本端点官方类型为 string，与「获取审核单详情」的 number 型不一致，照抄官方。
    /// </summary>
    [JsonPropertyName("audit_id")]
    public string AuditId { get; set; } = string.Empty;
}
