// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 手机号转外部联系人 ID 响应体（<c>/cgi-bin/externalcontact/batch_to_external_userid</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolBatchToExternalUserIdResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置转换成功列表（见 <see cref="SchoolMobileConvertSuccessItem"/>）。
    /// </summary>
    [JsonPropertyName("success_list")]
    public List<SchoolMobileConvertSuccessItem>? SuccessList { get; set; }

    /// <summary>
    /// 获取或设置转换失败列表（见 <see cref="SchoolMobileConvertFailItem"/>）。
    /// </summary>
    [JsonPropertyName("fail_list")]
    public List<SchoolMobileConvertFailItem>? FailList { get; set; }
}

/// <summary>
/// 手机号转换成功条目（<see cref="SchoolBatchToExternalUserIdResponse.SuccessList"/> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolMobileConvertSuccessItem
{
    /// <summary>
    /// 获取或设置手机号。
    /// </summary>
    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    /// <summary>
    /// 获取或设置外部联系人的 userid（家长关注后才会返回该字段）。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserid { get; set; }
}

/// <summary>
/// 手机号转换失败条目（<see cref="SchoolBatchToExternalUserIdResponse.FailList"/> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolMobileConvertFailItem
{
    /// <summary>
    /// 获取或设置返回码。
    /// </summary>
    [JsonPropertyName("errcode")]
    public int? ErrCode { get; set; }

    /// <summary>
    /// 获取或设置对返回码的文本描述内容。
    /// </summary>
    [JsonPropertyName("errmsg")]
    public string? ErrMsg { get; set; }

    /// <summary>
    /// 获取或设置手机号。
    /// </summary>
    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }
}
