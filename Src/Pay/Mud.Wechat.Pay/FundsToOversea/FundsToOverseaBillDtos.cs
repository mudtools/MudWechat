// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.FundsToOversea;

/// <summary>获取购付汇账单文件下载链接响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476132"/></summary>
[HttpJsonSerializable(SerializerClassName = "FundsToOversea")]
public class FundsToOverseaBillDownloadUrlResponse : WechatPayResponse
{
    /// <summary>【哈希类型】哈希类型。可选取值：`SHA1` Secure Hash Algorithm 1（官方字段路径 <c>hash_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("hash_type")]
    public string? HashType { get; set; }

    /// <summary>【哈希值】原始账单（gzip需要解压缩）的摘要值，用于校验文件的完整性。 建议将实际账单文件的哈希值和该哈希值进行比对，以确认数据的完整性（官方字段路径 <c>hash_value</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("hash_value")]
    public string? HashValue { get; set; }

    /// <summary>【下载地址】下载地址5min内有效（官方字段路径 <c>download_url</c>，<b>必填</b>，类型 <c>string(2048)</c>）。</summary>
    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }

}
