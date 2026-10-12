// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.OneCode;

// ---------------------------------------------------------------- 激活二维码（POST /intp/marketcode/codeactive）

/// <summary>
/// 激活二维码（<c>POST /intp/marketcode/codeactive</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>application_id</c> / <c>activity_name</c> / <c>product_brand</c> / <c>product_title</c> /
/// <c>product_code</c> / <c>wxa_appid</c> / <c>wxa_path</c> / <c>code_start</c> / <c>code_end</c> 必填；
/// <c>wxa_type</c> 可选（默认 <c>0</c> 正式版，见 <c>MpCodeWxaTypes</c>）。
/// </para>
/// <para>
/// 官方原文：<c>activity_name</c>/<c>product_brand</c>/<c>product_title</c> 均为「数据分析…区分依据，<b>请规范命名</b>」；
/// <c>product_code</c> 为「EAN 商品条码，请规范填写」。
/// </para>
/// <para>
/// <b>激活范围（官方原文）</b>：<c>code_start</c>/<c>code_end</c> 为<b>闭区间</b>（两端「包含该值」）。
/// 官方文档缺陷（照录）：未给出合法取值范围、是否允许倒序、最大跨度与申请单容量的对应关系；
/// 且说明举例上限 <c>9999</c> 与代码示例的上限 <c>200</c> 不一致。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpCodeActiveRequest
{
    /// <summary>获取或设置申请单号（官方 <c>application_id</c>，必填）。</summary>
    [JsonPropertyName("application_id")]
    public long ApplicationId { get; set; }

    /// <summary>获取或设置活动名称（官方 <c>activity_name</c>，必填；数据分析活动区分依据）。</summary>
    [JsonPropertyName("activity_name")]
    public string ActivityName { get; set; } = string.Empty;

    /// <summary>获取或设置商品品牌（官方 <c>product_brand</c>，必填；数据分析品牌区分依据）。</summary>
    [JsonPropertyName("product_brand")]
    public string ProductBrand { get; set; } = string.Empty;

    /// <summary>获取或设置商品标题（官方 <c>product_title</c>，必填；数据分析商品区分依据）。</summary>
    [JsonPropertyName("product_title")]
    public string ProductTitle { get; set; } = string.Empty;

    /// <summary>获取或设置商品条码（官方 <c>product_code</c>，必填；EAN 商品条码）。</summary>
    [JsonPropertyName("product_code")]
    public string ProductCode { get; set; } = string.Empty;

    /// <summary>获取或设置扫码跳转的小程序 appid（官方 <c>wxa_appid</c>，必填）。</summary>
    [JsonPropertyName("wxa_appid")]
    public string WxaAppId { get; set; } = string.Empty;

    /// <summary>获取或设置扫码跳转的小程序 path（官方 <c>wxa_path</c>，必填）。</summary>
    [JsonPropertyName("wxa_path")]
    public string WxaPath { get; set; } = string.Empty;

    /// <summary>获取或设置小程序版本（官方 <c>wxa_type</c>，可选；默认 <c>0</c> 正式版，见 <c>MpCodeWxaTypes</c>）。</summary>
    [JsonPropertyName("wxa_type")]
    public int? WxaType { get; set; }

    /// <summary>获取或设置激活码段起始位（官方 <c>code_start</c>，必填；<b>包含该值</b>）。</summary>
    [JsonPropertyName("code_start")]
    public int CodeStart { get; set; }

    /// <summary>获取或设置激活码段结束位（官方 <c>code_end</c>，必填；<b>包含该值</b>）。</summary>
    [JsonPropertyName("code_end")]
    public int CodeEnd { get; set; }
}
