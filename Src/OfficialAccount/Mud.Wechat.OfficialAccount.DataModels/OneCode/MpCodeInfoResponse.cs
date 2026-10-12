// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.OneCode;

/// <summary>
/// 「原始码 + 激活信息」响应（<c>codeactivequery</c> 与 <c>tickettocode</c> <b>共用</b>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方两页的响应字段表<b>语义完全相同</b>（均为「原始码数据，并返回对应的激活信息」），字段集仅相差
/// <c>wxa_type</c> 一项（<c>tickettocode</c> 页未列，<b>疑官方漏列</b>）；且 STJ 对未声明字段自动忽略
/// ⇒ SDK 以<b>超集共用</b>本 DTO（比复制 10 个字段更优；差异照录于此）。
/// </para>
/// <para>
/// <b>存在性冲突取并集</b>：两页返回<b>示例</b>均出现 <c>product_code</c>，但两页字段表<b>均未收录</b>
/// ⇒ SDK 保留 <see cref="ProductCode"/>。
/// </para>
/// <para>
/// 官方文档缺陷（照录）：<c>codeactivequery</c> 页返回示例 JSON 缺逗号（非法 JSON）；
/// <c>tickettocode</c> 页示例使用中文全角逗号；两页 <c>code_end</c> 示例值（<c>200</c>）与字段说明举例（<c>9999</c>）不一致。
/// </para>
/// <para>官方错误码：<c>40001</c>（两页错误码表均仅此行，照录）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpCodeInfoResponse : MpResponse
{
    /// <summary>获取或设置原始码（官方 <c>code</c>；九位字符串原始码，返回码数据与对应激活信息）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置申请单号（官方 <c>application_id</c>）。</summary>
    [JsonPropertyName("application_id")]
    public long? ApplicationId { get; set; }

    /// <summary>获取或设置外部单号（官方 <c>isv_application_id</c>）。</summary>
    [JsonPropertyName("isv_application_id")]
    public string? IsvApplicationId { get; set; }

    /// <summary>获取或设置活动名称（官方 <c>activity_name</c>）。</summary>
    [JsonPropertyName("activity_name")]
    public string? ActivityName { get; set; }

    /// <summary>获取或设置商品品牌（官方 <c>product_brand</c>）。</summary>
    [JsonPropertyName("product_brand")]
    public string? ProductBrand { get; set; }

    /// <summary>获取或设置商品标题（官方 <c>product_title</c>）。</summary>
    [JsonPropertyName("product_title")]
    public string? ProductTitle { get; set; }

    /// <summary>获取或设置商品条码（官方返回示例字段 <c>product_code</c>；两页字段表均未收录，照录）。</summary>
    [JsonPropertyName("product_code")]
    public string? ProductCode { get; set; }

    /// <summary>获取或设置小程序 appid（官方 <c>wxa_appid</c>）。</summary>
    [JsonPropertyName("wxa_appid")]
    public string? WxaAppId { get; set; }

    /// <summary>获取或设置小程序 path（官方 <c>wxa_path</c>）。</summary>
    [JsonPropertyName("wxa_path")]
    public string? WxaPath { get; set; }

    /// <summary>获取或设置小程序版本（官方 <c>wxa_type</c>；<c>tickettocode</c> 页字段表未列，疑漏）。</summary>
    [JsonPropertyName("wxa_type")]
    public int? WxaType { get; set; }

    /// <summary>获取或设置激活码段起始位（官方 <c>code_start</c>；包含该值）。</summary>
    [JsonPropertyName("code_start")]
    public int? CodeStart { get; set; }

    /// <summary>获取或设置激活码段结束位（官方 <c>code_end</c>；包含该值）。</summary>
    [JsonPropertyName("code_end")]
    public int? CodeEnd { get; set; }
}
