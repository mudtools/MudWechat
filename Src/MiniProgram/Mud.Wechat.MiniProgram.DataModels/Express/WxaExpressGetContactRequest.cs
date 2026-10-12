// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>
/// 获取面单联系人信息请求体（<c>POST /cgi-bin/express/delivery/contact/get</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_getcontact"/>。</para>
/// <para>本组接口面向<b>服务商（微信物流服务商）</b>：<c>token</c> 为商户侧下单事件中推送的 Token 字段（非模糊查询令牌），凭运单号获取面单联系人。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetContactRequest
{
    /// <summary>商户侧下单事件中推送的 Token 字段（<c>token</c>，必填）。</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>运单 ID（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }
}
