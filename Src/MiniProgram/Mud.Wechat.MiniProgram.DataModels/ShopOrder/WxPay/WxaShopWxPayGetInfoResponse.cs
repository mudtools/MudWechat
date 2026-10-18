// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 查询进件信息应答（<c>/shop/wxpay/get_info</c>，官方本页无参数表，键名取自回包示例）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopWxPayGetInfoResponse : WxaResponse
{
    /// <summary>进件信息（回包示例键名 <c>pay_req</c>；其中姓名 / 证件号 / 手机号已按官方脱敏展示，仍属个人信息，不得进日志）。</summary>
    [JsonPropertyName("pay_req")]
    public WxaShopWxPayReqInfo? PayReq { get; set; }
}
