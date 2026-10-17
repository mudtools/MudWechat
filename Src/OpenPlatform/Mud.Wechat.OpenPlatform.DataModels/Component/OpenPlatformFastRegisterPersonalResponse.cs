// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Mud.Wechat.OpenPlatform.DataModels.Component;

/// <summary>
/// 快速注册个人小程序应答（<c>POST /wxa/component/fastregisterpersonalweapp?action=create</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/register-management/fast-registration-ind/fastRegisterPersonalMp.html"/></para>
/// <para><c>taskid</c> 用于后续 query 查询任务状态；<c>authorize_url</c> 需发给用户扫码完成身份验证。</para>
/// <para><c>status</c> 枚举：0=生成任务 / 1=超时 / 2=被拒绝 / 3=同意创建 / 4~9=人脸/手机验证各阶段 / 10=提交创建 / 11=创建失败 / 12=创建成功 / 13=验证成功 / 14=验证失败 / 1001~1008=超限或黑名单。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformFastRegisterPersonalResponse : OpenPlatformResponse
{
    /// <summary>任务 ID（query 查询时需要用到）。</summary>
    [JsonPropertyName("taskid")]
    public string? TaskId { get; set; }

    /// <summary>给用户扫码认证的验证 URL。</summary>
    [JsonPropertyName("authorize_url")]
    public string? AuthorizeUrl { get; set; }

    /// <summary>任务状态（见备注枚举说明）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}
