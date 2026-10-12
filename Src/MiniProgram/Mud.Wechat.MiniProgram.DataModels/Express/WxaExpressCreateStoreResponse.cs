// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>创建门店应答（<c>out_store_id</c> / <c>wx_store_id</c> / <c>appid</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_createstore"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressCreateStoreResponse : WxaResponse
{
    /// <summary>自定义门店编号（<c>out_store_id</c>）。</summary>
    [JsonPropertyName("out_store_id")]
    public string? OutStoreId { get; set; }

    /// <summary>微信门店编号（<c>wx_store_id</c>；后续运力订单凭此定位）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>小程序 AppId（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }
}
