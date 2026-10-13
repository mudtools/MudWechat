// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 道具上传结果（官方返回侧子表：请求侧五字段 + <c>upload_status</c> + <c>errmsg</c>）。
/// </summary>
/// <remarks>
/// <para><b>与请求侧条目类型不共用</b>：本表多出 <c>upload_status</c>/<c>errmsg</c> 两支逐条目结果 ⇒ 语义是「任务产物」而非「任务输入」，强行共用会把状态字段带进请求。</para>
/// <para><b><c>upload_status</c> 与发布侧 <c>publish_status</c> 取值集不同</b>（本表 0 上传中 / 1 id 已经存在 / 2 上传成功 / 3 上传失败）⇒ 官方此处只写在说明列、无枚举表，故照录进文档不建常量类。</para>
/// <para><b><c>item_url</c> 在此是「微信转存后」的地址</b>（官方原文）⇒ 与请求侧的原始 URL 不同值，对账不得比对二者。</para>
/// <para><b>本表 <c>errmsg</c> 是逐条目失败原因，不是接口级错误码</b>（接口级由基类 <see cref="Mud.Wechat.MiniProgram.DataModels.WxaResponse"/> 承载）⇒ 故须显式承载该同名键。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_upload_goods.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayUploadGoodsResult
{
    /// <summary>id（官方类型 <c>string</c>）：道具id</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>name（官方类型 <c>string</c>）：道具名称</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>price（官方类型 <c>number</c>）：道具单价，单位分</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>remark（官方类型 <c>string</c>）：道具备注</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>item_url（官方类型 <c>string</c>）：道具图片的url地址（微信转存后）</summary>
    [JsonPropertyName("item_url")]
    public string? ItemUrl { get; set; }

    /// <summary>upload_status（官方类型 <c>number</c>）：0-上传中 1-id已经存在 2-上传成功 3-上传失败</summary>
    [JsonPropertyName("upload_status")]
    public long? UploadStatus { get; set; }

    /// <summary>errmsg（官方类型 <c>string</c>）：上传失败的原因</summary>
    [JsonPropertyName("errmsg")]
    public string? Errmsg { get; set; }
}
