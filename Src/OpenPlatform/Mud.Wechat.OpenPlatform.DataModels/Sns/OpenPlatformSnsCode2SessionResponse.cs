// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Mud.Wechat.OpenPlatform.DataModels.Sns;

/// <summary>
/// 第三方平台代小程序登录凭证校验应答（<c>GET /sns/component/jscode2session</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/login/thirdpartyCode2Session.html"/></para>
/// <para>官方约束：该接口仅支持服务商获取已有授权关系的小程序信息；若小程序尚未授权给第三方平台，则报 <c>61003</c>。</para>
/// <para>官方错误码：<c>40029</c>（code 无效）/ <c>41021</c>（missing component_access_token）/ <c>45011</c>（频率超限）/ <c>61003</c>（未授权）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Sns")]
public class OpenPlatformSnsCode2SessionResponse : OpenPlatformResponse
{
    /// <summary>会话密钥（用于解密用户数据）。</summary>
    [JsonPropertyName("session_key")]
    public string? SessionKey { get; set; }

    /// <summary>用户在开放平台的唯一标识符（若小程序已绑定到开放平台账号下会返回）。</summary>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }

    /// <summary>用户唯一标识（小程序 openid）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}
