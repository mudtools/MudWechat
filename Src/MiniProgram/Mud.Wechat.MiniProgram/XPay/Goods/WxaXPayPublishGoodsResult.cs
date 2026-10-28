// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 道具发布结果（官方 <c>Res.publish_item</c>）。
/// </summary>
/// <remarks>
/// <para><b>本表没有 <c>name</c>/<c>price</c></b>（与上传结果侧不同）⇒ 官方发布侧只回 id + 状态 + 失败原因。</para>
/// <para><b><c>errmsg</c> 是逐条目失败原因</b>，不是接口级错误码（接口级由基类 <see cref="Mud.Wechat.MiniProgram.DataModels.WxaResponse"/> 承载）⇒ 二者不可互替。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_publish_goods.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayPublishGoodsResult
{
    /// <summary>id（官方类型 <c>string</c>）：道具id</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>publish_status（官方类型 <c>number</c>）：0-上传中 1-id已经存在 2-发布成功 3-发布失败</summary>
    [JsonPropertyName("publish_status")]
    public long? PublishStatus { get; set; }

    /// <summary>errmsg（官方类型 <c>string</c>）：发布失败的原因</summary>
    [JsonPropertyName("errmsg")]
    public string? Errmsg { get; set; }
}
