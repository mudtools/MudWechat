// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.Multiuse;

/// <summary>下载批次发放明细响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012539756"/></summary>
[HttpJsonSerializable(SerializerClassName = "Multiuse")]
public class MultiuseFlowUrlResponse : WechatPayResponse
{
    /// <summary>【下载链接】流水文件下载链接，30s内有效（官方字段路径 <c>url</c>，<b>必填</b>，类型 <c>string(2048)</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

}
