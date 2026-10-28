// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>查询索要单请求（business.getPayForOrder，官方页顶部标注「接口已废弃」但文档存活）。三支参数官方全部必填。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment/business.getPayForOrder.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameGetPayForOrderRequest
{
    /// <summary>小游戏 appId（官方必填）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>微信索要单号（官方必填）。</summary>
    [JsonPropertyName("order_no")]
    public string? OrderNo { get; set; }

    /// <summary>游戏业务单号（官方必填）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }
}
