// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Mud.Wechat.OpenPlatform.DataModels.Component;

/// <summary>
/// 查询快速注册个人小程序任务状态请求（<c>POST /wxa/component/fastregisterpersonalweapp?action=query</c>；请求体仅 <c>taskid</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/register-management/fast-registration-ind/fastRegisterPersonalMp.html"/></para>
/// <para>官方约束：<c>taskid</c> 来自创建请求的应答；查询频率同创建（5 次/分钟、100 次/天）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformFastRegisterPersonalQueryRequest
{
    /// <summary>任务 ID（官方 <c>taskid</c>，必填；由 <see cref="OpenPlatformFastRegisterPersonalResponse.TaskId"/> 返回）。</summary>
    [JsonPropertyName("taskid")]
    public string? TaskId { get; set; }
}
