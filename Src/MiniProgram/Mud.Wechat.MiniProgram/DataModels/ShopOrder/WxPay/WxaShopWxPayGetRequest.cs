// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 查询进件状态请求（<c>/shop/wxpay/get</c>；两处官方页面口径不一致，见接口注释）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopWxPayGetRequest
{
    /// <summary>业务类型：官方 <c>wxafunds</c> 页标注「此接口固定 <c>1</c>」；<c>minishopopencomponent2</c> 页示例为空请求体 ⇒ 建模为选填。</summary>
    [JsonPropertyName("biz_type")]
    public int? BizType { get; set; }

    /// <summary>微信支付申请单号：与 <c>out_request_no</c> <b>二选一</b>。</summary>
    [JsonPropertyName("applyment_id")]
    public string? ApplymentId { get; set; }

    /// <summary>商户自定义进件单号：与 <c>applyment_id</c> <b>二选一</b>。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }
}
