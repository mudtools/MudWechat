// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopSharer;

/// <summary>
/// 获取分享员总带货数据请求（<c>POST /shop/sharer/get_sharer_data_summary</c>，请求体只有 <c>openid</c> 一支键、必填）。
/// </summary>
/// <remarks><b>敏感面</b>：<c>openid</c> 禁入日志、遥测与异常消息。</remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerDataSummaryRequest
{
    /// <summary>分享员 openid（<c>openid</c>，string，必填）。禁落日志。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}
