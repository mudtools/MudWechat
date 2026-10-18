// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.WeData;

/// <summary>
/// 权限集项（官方权限对象，<c>perm_id</c> + <c>name</c> + <c>desc</c>；查询/设置用户权限共用）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/we-analysis/weDataGetPermList.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "WeData")]
public class OpenPlatformWeDataPermission
{
    /// <summary>权限集 ID（官方 <c>perm_id</c>）。</summary>
    [JsonPropertyName("perm_id")]
    public string? PermissionId { get; set; }

    /// <summary>权限集名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>权限集描述（官方 <c>desc</c>）。</summary>
    [JsonPropertyName("desc")]
    public string? Description { get; set; }
}
