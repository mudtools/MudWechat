// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>门店运费充值请求体（<c>POST /cgi-bin/express/intracity/storecharge</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_storecharge"/>；充值后返回支付链接 <c>payurl</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressStoreChargeRequest
{
    /// <summary>微信门店编号（<c>wx_store_id</c>，必填）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>运力 ID（<c>service_trans_id</c>，必填）。</summary>
    [JsonPropertyName("service_trans_id")]
    public string? ServiceTransId { get; set; }

    /// <summary>充值金额（<c>amount</c>，必填；单位：分）。</summary>
    [JsonPropertyName("amount")]
    public int Amount { get; set; }
}
