// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ProviderAuthentication;

/// <summary>
/// 获取登录用户信息请求体（<c>/cgi-bin/service/get_login_info</c>，官方文档 91154）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ProviderAuthentication")]
public class GetLoginInfoRequest
{
    /// <summary>
    /// 获取或设置扫码登录授权返回的临时授权码（官方必填；最长 512 字节，
    /// 单次使用、5 分钟内有效，过期作废）。
    /// </summary>
    [JsonPropertyName("auth_code")]
    public string AuthCode { get; set; } = string.Empty;
}
