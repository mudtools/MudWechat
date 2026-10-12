// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>设置保费告警余额请求体（<c>POST /wxa/business/insurance_freight/update_notify_funds</c>）。</summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_update_notify_funds"/>
/// （接口英文名 <c>insurance_freight_update_notify_funds</c>）。
/// <b>官方原文</b>：自定义余额为 xx 时通知小程序管理员（余额无改动情况下 24h 内通知一次），通知消息经<b>微信公众平台</b>下发。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUpdateNotifyFundsRequest
{
    /// <summary>通知的金额（<c>notify_funds</c>，必填；单位：分；设置为 <c>0</c> 表示不通知）。</summary>
    [JsonPropertyName("notify_funds")]
    public long NotifyFunds { get; set; }
}
