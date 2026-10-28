// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「消息跳转路径设置」请求体（<c>POST /wxa/sec/order/set_msg_jump_path</c>）。
/// </summary>
/// <remarks>
/// <para><b>空路径或小程序中不存在的路径不会报错</b>（官方注意事项）：点击仍跳转平台默认确认收货页
/// ⇒ 「设置成功」不等于「用户会进到我的小程序」。</para>
/// <para><b>平台会往该路径追加 Query 参数</b>（官方原文）：<c>transaction_id</c>、<c>merchant_id</c>、
/// <c>merchant_trade_no</c>，存在二级商户号时再加 <c>sub_merchant_id</c> ⇒ 自定义 Query 须避开这四个键名。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_setmsgjumppath.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderMsgJumpPathRequest
{
    /// <summary>path（官方类型 <c>string</c>，必填）：商户自定义跳转路径。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }
}
