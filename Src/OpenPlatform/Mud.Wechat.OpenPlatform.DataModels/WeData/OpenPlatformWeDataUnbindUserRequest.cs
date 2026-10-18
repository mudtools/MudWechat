// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.WeData;

/// <summary>
/// 用户解绑请求（<c>POST /wedata/wedata_unbind_user</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/we-analysis/weDataUnbindUser.html"/></para>
/// <para>官方约束：解绑会删除该用户的成员身份（管理员与数据管理员除外）；操作不可撤销。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "WeData")]
public class OpenPlatformWeDataUnbindUserRequest
{
    /// <summary>用户在服务商系统的唯一标识（官方 <c>uid</c>，必填）。</summary>
    [JsonPropertyName("uid")]
    public string UserId { get; set; } = string.Empty;
}
