// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「查询名称审核状态」（<c>api_wxa_querynickname</c>）请求。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/getNickNameStatus.html"/>。
/// </summary>
/// <remarks>本端点官方以 <c>audit_id</c>（带下划线，来自 <c>setnickname</c> 应答）查询审核状态，与代码审核域的 <c>auditid</c> 字段名不同，各自照抄。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpQueryNicknameRequest
{
    /// <summary>名称审核单号（官方必填 <c>audit_id</c>，number）。</summary>
    [JsonPropertyName("audit_id")]
    public long? AuditId { get; set; }
}
