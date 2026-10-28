// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Mud.Wechat.OpenPlatform.DataModels.Component;

/// <summary>
/// 快速创建试用小程序请求（<c>POST /wxa/component/fastregisterbetaweapp</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/register-management/fast-regist-beta/registerBetaMiniprogram.html"/></para>
/// <para>官方约束：令牌面为 <c>component_access_token</c>；注册为异步任务，结果经 <c>notify_third_fastregisterbetaapp</c> 事件推送。</para>
/// <para>官方错误码：<c>-1</c>（系统繁忙）/ <c>40001</c>（access_token 无效）/ <c>750001~750004</c>（授权/身份异常）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Component")]
public class OpenPlatformFastRegisterBetaWeappRequest
{
    /// <summary>小程序名称（4-30 字符，强制后缀"的体验小程序"），必填。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>微信用户的 openid（非微信号，将设为管理员），必填。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}
