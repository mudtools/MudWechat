// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Mud.Wechat.OpenPlatform.DataModels.Component;

/// <summary>
/// 快速注册个人小程序请求（<c>POST /wxa/component/fastregisterpersonalweapp?action=create</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/register-management/fast-registration-ind/fastRegisterPersonalMp.html"/></para>
/// <para>官方约束：令牌面为 <c>component_access_token</c>；频率限制 5 次/分钟、100 次/天；<c>new_version</c> 必须填 true。</para>
/// <para>官方错误码：<c>-1</c>（系统繁忙）/ <c>40001</c>（access_token 无效）/ <c>1001~1008</c>（主体/管理员违规或超限）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformFastRegisterPersonalRequest
{
    /// <summary>个人用户姓名，必填。</summary>
    [JsonPropertyName("idname")]
    public string? IdName { get; set; }

    /// <summary>个人用户微信号，必填。</summary>
    [JsonPropertyName("wxuser")]
    public string? WxUser { get; set; }

    /// <summary>第三方联系电话（建议填写，方便商家联系），可选。</summary>
    [JsonPropertyName("component_phone")]
    public string? ComponentPhone { get; set; }

    /// <summary>接口新旧版本标识，使用新接口必须填 true，不填则为 false。</summary>
    [JsonPropertyName("new_version")]
    public bool? NewVersion { get; set; }
}
