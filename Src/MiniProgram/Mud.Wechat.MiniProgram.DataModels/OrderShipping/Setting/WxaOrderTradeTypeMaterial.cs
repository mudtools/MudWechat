// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「交易类型变更申请材料」（<c>material_list</c> 数组元素）。
/// </summary>
/// <remarks>
/// <para><b><c>media_id</c> 来自「新增临时素材」</b>（官方原文）：临时素材有效期 3 天，
/// 申请流程若跨天须重新上传。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_setwxatradetypecgi.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderTradeTypeMaterial
{
    /// <summary>type（官方类型 <c>number</c>，必填）：材料类型：1-图片，2-视频。取值见 <see cref="WxaOrderTradeTypeMaterialTypeValues"/></summary>
    [JsonPropertyName("type")]
    public long? Type { get; set; }

    /// <summary>media_id（官方类型 <c>string</c>，必填）：通过「新增临时素材」接口上传后获取的 media_id</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }
}
