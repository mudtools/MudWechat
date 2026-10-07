// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels;

/// <summary>
/// 企业微信统一响应基底。
/// </summary>
/// <remarks>
/// 实现公用层判错契约 <see cref="Mud.Wechat.Abstractions.Contracts.IWechatApiResponse"/>，
/// 使判错出口（<c>WechatWorkException.ThrowIfFailed</c>）可在跨产品线层面统一。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Common")]
public class WechatWorkResponse : Mud.Wechat.Abstractions.Contracts.IWechatApiResponse
{
    /// <summary>
    /// 获取或设置企业微信 API 返回的错误码（0 表示成功）。
    /// </summary>
    [JsonPropertyName("errcode")]
    public virtual int ErrorCode { get; set; }

    /// <summary>
    /// 获取或设置企业微信 API 返回的错误信息。
    /// </summary>
    [JsonPropertyName("errmsg")]
    public virtual string? ErrorMessage { get; set; }

    /// <summary>
    /// 获取一个值，该值表示本次调用是否成功（errcode == 0）。
    /// </summary>
    [JsonIgnore]
    public bool IsSuccess => ErrorCode == 0;
}

/// <summary>
/// 企业微信统一响应的泛型基类（携带 data 负载）。
/// </summary>
public class WechatChatbotResponse<TData> : WechatWorkResponse
    where TData : class
{
    /// <summary>
    /// 获取或设置返回的数据。
    /// </summary>
    [JsonPropertyName("data")]
    public virtual TData? Data { get; set; }
}
