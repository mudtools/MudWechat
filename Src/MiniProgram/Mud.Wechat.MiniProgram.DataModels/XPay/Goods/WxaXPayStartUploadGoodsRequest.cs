// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「批量上传道具」请求体（<c>POST /xpay/start_upload_goods</c>）。
/// </summary>
/// <remarks>
/// <para><b>任务互斥（官方错误码 <c>268490012</c>）</b>：「批量任务运行中，请等待完成后才能再次运行」⇒ 上传与发布两支各占一条任务位，须先查任务再提交。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_start_upload_goods.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayStartUploadGoodsRequest
{
    /// <summary>upload_item（官方类型 <c>objarray</c>，<b>必填</b>）：上传的商品列表</summary>
    [JsonPropertyName("upload_item")]
    public List<WxaXPayUploadGoodsItem>? UploadItem { get; set; }

    /// <summary>env（官方类型 <c>number</c>，<b>必填</b>）：0-正式环境 1-沙箱环境</summary>
    [JsonPropertyName("env")]
    public long? Env { get; set; }
}
