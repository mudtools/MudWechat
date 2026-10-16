// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.Fapiao;

/// <summary>创建电子发票卡券模板请求·【卡券模板信息】 卡券模板信息·【卡券自定义cell位配置】 卡券自定义cell位配置，需要在卡券详情中增加。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529382"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474026"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4015792562"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoCardTemplateRequestCardTemplateInformationCustomCell
{
    /// <summary>【cell位文字】 展示在卡券详情页自定义cell位上的文字（官方字段路径 <c>card_template_information.custom_cell.words</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("words")]
    public string? Words { get; set; }

    /// <summary>【cell位描述】 展示在卡券详情页自定义cell位右侧的描述（官方字段路径 <c>card_template_information.custom_cell.description</c>，<b>必填</b>，类型 <c>string(6)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【点击cell位跳转的地址】 点击卡券详情页自定义cell位后跳转的页面链接，用户跳转时会自动在链接后加上参数 `encrypt_code` 与 `card_id`，商户需要调用code解码接口解码 `encrypt_code` 得到真实的code（官方字段路径 <c>card_template_information.custom_cell.jump_url</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("jump_url")]
    public string? JumpUrl { get; set; }

    /// <summary>【点击cell位跳转的小程序的用户名】 用于指定点击卡券详情页自定义cell位后跳转的小程序，该小程序必须与商户插卡的AppID绑定（官方字段路径 <c>card_template_information.custom_cell.miniprogram_user_name</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("miniprogram_user_name")]
    public string? MiniprogramUserName { get; set; }

    /// <summary>【点击cell位跳转的小程序的页面路径】 用于指定点击卡券详情页自定义cell位后跳转的小程序页面路径，跳转小程序后，开发者可以通过页面 `onShow` 方法的query中获取到发起跳转的 `card_id`、 `encrypt_code` 和用户的公众号OpenID，商户需要调用code解码接口解码 `encrypt_code` 得到真实的code（官方字段路径 <c>card_template_information.custom_cell.miniprogram_path</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("miniprogram_path")]
    public string? MiniprogramPath { get; set; }

}

/// <summary>创建电子发票卡券模板请求·【卡券模板信息】 卡券模板信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529382"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474026"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4015792562"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoCardTemplateRequestCardTemplateInformation
{
    /// <summary>【收款方名称】 收款方名称，显示在电子发票卡券信息中，若不传则默认取商户名称（官方字段路径 <c>card_template_information.payee_name</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("payee_name")]
    public string? PayeeName { get; set; }

    /// <summary>【卡券logo地址】 卡券logo地址， 请点击参考此文档（官方字段路径 <c>card_template_information.logo_url</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("logo_url")]
    public string? LogoUrl { get; set; }

    /// <summary>【卡券自定义cell位配置】 卡券自定义cell位配置，需要在卡券详情中增加（官方字段路径 <c>card_template_information.custom_cell</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("custom_cell")]
    public FapiaoCardTemplateRequestCardTemplateInformationCustomCell? CustomCell { get; set; }

}

/// <summary>创建电子发票卡券模板请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529382"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474026"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4015792562"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoCardTemplateRequest
{
    /// <summary>【插卡公众号AppID】 插卡公众号AppID。若是服务商模式，则可以是服务商申请的AppId，也可以是子商户申请的AppId；若是直连模式，则是直连商户申请的AppId（官方字段路径 <c>card_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_appid")]
    public string? CardAppid { get; set; }

    /// <summary>【卡券模板信息】 卡券模板信息（官方字段路径 <c>card_template_information</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("card_template_information")]
    public FapiaoCardTemplateRequestCardTemplateInformation? CardTemplateInformation { get; set; }

    /// <summary>【子商户号】 微信支付分配的子商户号，服务商模式下必传（官方字段路径 <c>sub_mchid</c>，选填，仅<b>服务商面</b>官方页印发，服务商面页标必填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>创建电子发票卡券模板响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529382"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474026"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4015792562"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoCardTemplateResponse : WechatPayResponse
{
    /// <summary>【插卡公众号AppID】 卡券模板所属的AppID（官方字段路径 <c>card_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_appid")]
    public string? CardAppid { get; set; }

    /// <summary>【卡券模板ID】 卡券模板ID（官方字段路径 <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

}
