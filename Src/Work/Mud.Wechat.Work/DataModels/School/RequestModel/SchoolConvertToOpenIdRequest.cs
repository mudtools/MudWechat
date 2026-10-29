// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 外部联系人 openid 转换请求体（<c>/cgi-bin/externalcontact/convert_to_openid</c>，家校沟通）。
/// <para>
/// 官方业务限制：将微信外部联系人的 userid 转为微信 openid，用于调用支付相关接口；
/// 暂不支持企业微信外部联系人（ExternalUserid 为 wo 开头）的 userid 转 openid。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolConvertToOpenIdRequest
{
    /// <summary>
    /// 获取或设置外部联系人的 userid（官方必填，注意不是企业成员的账号）。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserid { get; set; }
}
