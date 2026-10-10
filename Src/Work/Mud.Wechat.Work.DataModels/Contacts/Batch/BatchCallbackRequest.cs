// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contacts.Batch;

/// <summary>
/// 异步导入任务回调信息（增量更新成员 / 全量覆盖成员 / 全量覆盖部门请求中的 <c>callback</c>）。
/// </summary>
/// <remarks>填写后任务完成时官方经回调推送事件给企业。三个字段均为可选；
/// <c>token</c> / <c>encodingaeskey</c> 属敏感凭据，调用方不得记录到日志。</remarks>
[HttpJsonSerializable(SerializerClassName = "Batch")]
public class BatchCallbackRequest
{
    /// <summary>
    /// 获取或设置企业应用接收企业微信推送请求的访问协议和地址（支持 http 或 https 协议）。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 获取或设置用于生成签名的 Token（敏感，不得记录到日志）。
    /// </summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>
    /// 获取或设置消息体的加密密钥（AES 密钥的 Base64 编码；敏感，不得记录到日志）。
    /// </summary>
    [JsonPropertyName("encodingaeskey")]
    public string? EncodingAesKey { get; set; }
}
