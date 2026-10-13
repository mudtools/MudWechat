// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 道具上传条目（官方请求侧子表，五支<b>全部必填</b>）。
/// </summary>
/// <remarks>
/// <para><b>长度限制照官方原文</b>：<c>id</c> 与 <c>name</c> 为 (0,20]、<c>remark</c> 为 (0,1024]、<c>price</c> 须大于 0；<b>中文算一个字符</b>（官方对 <c>id</c> 明示）。SDK 不本地校验。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_start_upload_goods.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayUploadGoodsItem
{
    /// <summary>id（官方类型 <c>string</c>，<b>必填</b>）：道具id，长度(0,20]，字符只允许使用字母、数字、'\_'、'-'，中文算一个字符</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>name（官方类型 <c>string</c>，<b>必填</b>）：道具名称，长度(0，20]</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>price（官方类型 <c>number</c>，<b>必填</b>）：道具单价，单位分，需要大于0</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>remark（官方类型 <c>string</c>，<b>必填</b>）：道具备注，长度(0,1024]</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>item_url（官方类型 <c>string</c>，<b>必填</b>）：道具图片的url地址，当前仅支持jpg,png等格式</summary>
    [JsonPropertyName("item_url")]
    public string? ItemUrl { get; set; }
}
