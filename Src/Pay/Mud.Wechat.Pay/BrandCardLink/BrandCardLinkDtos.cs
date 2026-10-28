// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.BrandCardLink;

/// <summary>交易连接名片规则列表响应·【已生效连接列表】 返回符合查询条件的已生效交易连接名片列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016366785"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardLink")]
public class BrandCardLinkListResponseActiveLinkListItem
{
    /// <summary>【交易场景类型】 交易场景类型是指微信支付提供的帮助商家在不同场景中快速收款的工具（如小程序支付、付款码支付等）。 不同类型的交易场景下，建立交易连接名片规则依据的账号类型不同： 1、当交易场景类型为 `MINI_PROGRAM`、 `APP` 时， 返回 `appid` 字段； 2、当交易场景类型为 `PAYMENT_SCORE` 时，返回 `service_id` 字段； 3、当交易场景类型为 `PAYMENT_CODE` 时， 返回 `card_li…。可选取值：`MINI_PROGRAM` 小程序支付：用户在品牌的小程序中选择商品下单，跳转微信支付确认验密页面，完成支付，详细可查看 [产品介绍](https://pay.weixin.qq.com/doc/v3/partner/4012085810.md) / `APP` APP 支付：用户在品牌的App中选择商品下单，跳转到微信支付确认验密页面，完成支付，详细可查看 [产品介绍](https://pay.weixin.qq.com/doc/v3/partner/4013080227.md) / `PAYMENT_SCORE` 支付分支付：微信支付分提供先使用服务，再付款的能力，如免押金借用充电宝、电商先用后付等，详细可查看 [产品介绍](https://pay.weixin.qq.com/doc/v3/partner/4012586132.md) / `PAYMENT_CODE` 付款码支付：用户线下展示付款码，商户收银员用扫码设备扫描用户的付款码，用户确认支付，详细可查看 [场景介绍](https://pay.weixin.qq.com/doc/v2/partner/4011941052.md)（官方字段路径 <c>active_link_list.payment_scene</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("payment_scene")]
    public string? PaymentScene { get; set; }

    /// <summary>【公众账号ID列表】 公众账号ID也称 AppID，是（微信开放平台、微信公众平台）为开发者提供的一个唯一标识，用于识别开发者的应用程序（APP、小程序、公众号）。 1、若交易场景类型为小程序支付、APP支付时，此字段返回对应的 AppID； 2、若交易场景类型为小程序支付且对应的 AppID 为收银类平台小程序时，此字段可返回多个 AppID； 3、其他交易场景类型时，此字段返回空值（官方字段路径 <c>active_link_list.appid_list</c>，选填，类型 <c>array[string(20)]</c>）。</summary>
    [JsonPropertyName("appid_list")]
    public List<string>? AppidList { get; set; }

    /// <summary>【商户号】 商户号是商户在 商户平台 申请的唯一身份标识，微信支付通过商户号确认商户身份。 1、当交易场景为付款码支付，或小程序支付且对应的 AppID 为收银类平台小程序时，此字段返回对应的商户号； 2、其他交易场景类型时，此字段返回空值（官方字段路径 <c>active_link_list.card_link_mchid</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("card_link_mchid")]
    public string? CardLinkMchid { get; set; }

    /// <summary>【服务ID】 支付分服务ID是商户支付分服务的唯一标识，由32位数字组成。支付分产品权限申请审核通过后，微信支付运营会向商户提供支付分服务ID。 1、当交易场景类型为支付分支付时，此字段返回对应的支付分服务ID； 2、其他交易场景类型时，此字段返回空值（官方字段路径 <c>active_link_list.service_id</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

}

/// <summary>交易连接名片规则列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016366785"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardLink")]
public class BrandCardLinkListResponse : WechatPayResponse
{
    /// <summary>【品牌ID】 1、商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符； 2、数字，长度不超过 20 个字符; 3、暂不支持银行品牌（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【总个数】 符合查询条件的已生效交易连接名片场景总数（官方字段路径 <c>total_num</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_num")]
    public long? TotalNum { get; set; }

    /// <summary>【已生效连接列表】 返回符合查询条件的已生效交易连接名片列表（官方字段路径 <c>active_link_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("active_link_list")]
    public List<BrandCardLinkListResponseActiveLinkListItem>? ActiveLinkList { get; set; }

    /// <summary>【查询页码】 查询页码。从1开始，不传默认返回首页数据（官方字段路径 <c>page_index</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("page_index")]
    public long? PageIndex { get; set; }

    /// <summary>【查询页大小】 查询页大小。单次拉取的数据条数上限，不填默认为20，最大值50（官方字段路径 <c>page_size</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("page_size")]
    public long? PageSize { get; set; }

}

/// <summary>交易连接名片规则申请请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016333302"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardLink")]
public class BrandCardLinkApplyRequest
{
    /// <summary>【品牌业务申请编号】 1、只能由数字、字母或下划线组成； 2、服务商自定义的请求流水号，用于标识每一个交易连接名片关联关系配置，需确保同一品牌下不同账号ID配置的流水号唯一。​（官方字段路径 <c>business_code</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("business_code")]
    public string? BusinessCode { get; set; }

    /// <summary>【品牌ID】 1、商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符； 2、数字，长度不超过 20 个字符； 3、暂不支持银行品牌（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【交易场景类型】 交易场景类型是指微信支付提供的帮助商家在不同场景中快速收款的工具（如小程序支付、付款码支付等）。 不同类型的交易场景下，建立交易连接名片规则依据的账号类型不同： 1、当交易场景类型填 `MINI_PROGRAM`、 `APP` 时， `appid` 字段必填； 2、当交易场景类型填 `PAYMENT_SCORE` 时， `service_id` 字段必填； 3、当交易场景类型填 `PAYMENT_CODE` 时， `card_link_m…。可选取值：`MINI_PROGRAM` 小程序支付：用户在品牌的小程序中选择商品下单，跳转微信支付确认验密页面，完成支付，详细可查看 [产品介绍](https://pay.weixin.qq.com/doc/v3/partner/4012085810.md) / `APP` APP 支付：用户在品牌的App中选择商品下单，跳转到微信支付确认验密页面，完成支付，详细可查看 [产品介绍](https://pay.weixin.qq.com/doc/v3/partner/4013080227.md) / `PAYMENT_SCORE` 支付分支付：微信支付分提供先使用服务，再付款的能力，如免押金借用充电宝、电商先用后付等，详细可查看 [产品介绍](https://pay.weixin.qq.com/doc/v3/partner/4012586132.md) / `PAYMENT_CODE` 付款码支付：用户线下展示付款码，商户收银员用扫码设备扫描用户的付款码，用户确认支付，详细可查看 [场景介绍](https://pay.weixin.qq.com/doc/v2/partner/4011941052.md)（官方字段路径 <c>payment_scene</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("payment_scene")]
    public string? PaymentScene { get; set; }

    /// <summary>【公众账号ID】 公众账号ID也称 AppID，是（微信开放平台、微信公众平台）为开发者提供的一个唯一标识，用于识别开发者的应用程序（APP、小程序、公众号）。 开发者需要在微信开放平台或微信公众平台中申请ID。 1、若交易场景类型为小程序支付、APP支付时，此字段必填，其他交易场景类型时不支持填写； 2、请提供与品牌经营场景有关的下单小程序（如点餐小程序、电商下单小程序等）或APP，请勿提供与品牌经营场景无关的小程序（如不发生交易支付的广告小程序等）或A…（官方字段路径 <c>appid</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【商户号】 商户号是商户在 商户平台 申请的唯一身份标识，所有接口在调用时都必须传商户号，微信支付通过商户号确认商户身份。 注意：若交易场景为付款码支付，或小程序支付且提供的 AppID 为收银类平台小程序时，此字段必填（仅支持填写普通商户、特约商户类型商户号），其他交易场景类型不支持填写（官方字段路径 <c>card_link_mchid</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("card_link_mchid")]
    public string? CardLinkMchid { get; set; }

    /// <summary>【服务ID】 1、商户支付分服务的唯一标识，由32位数字组成。支付分产品权限审核通过后，微信支付运营会向商户提供该ID； 2、交易场景为支付分支付时必填，其他交易场景不支持填写（官方字段路径 <c>service_id</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

}

/// <summary>交易连接名片规则申请响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016333302"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardLink")]
public class BrandCardLinkApplyResponse : WechatPayResponse
{
    /// <summary>【品牌业务申请编号】 1、只能由数字、字母或下划线组成； 2、服务商自定义的请求流水号，用于标识每一个交易连接名片关联关系配置，需确保同一品牌下不同账号ID配置的流水号唯一。​（官方字段路径 <c>business_code</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("business_code")]
    public string? BusinessCode { get; set; }

    /// <summary>【品牌ID】 1、商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符； 2、数字，长度不超过 20 个字符; 3、暂不支持银行品牌（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【交易场景类型】 交易场景类型是指微信支付提供的帮助商家在不同场景中快速收款的工具（如小程序支付、付款码支付等）。 不同类型的交易场景下，建立交易连接名片规则依据的账号类型不同： 1、当枚举值选择 `MINI_PROGRAM`、 `APP` 时， 返回 `appid` 字段； 2、当枚举值选择 `PAYMENT_SCORE` 时，返回 `service_id` 字段； 3、当枚举值选择 `PAYMENT_CODE` 时， 返回 `card_link_mch…。可选取值：`MINI_PROGRAM` 小程序支付：用户在品牌的小程序中选择商品下单，跳转微信支付确认验密页面，完成支付，详细可查看 [产品介绍](https://pay.weixin.qq.com/doc/v3/partner/4012085810.md) / `APP` APP 支付：用户在品牌的App中选择商品下单，跳转到微信支付确认验密页面，完成支付，详细可查看 [产品介绍](https://pay.weixin.qq.com/doc/v3/partner/4013080227.md) / `PAYMENT_SCORE` 支付分支付：微信支付分提供先使用服务，再付款的能力，如免押金借用充电宝、电商先用后付等，详细可查看 [产品介绍](https://pay.weixin.qq.com/doc/v3/partner/4012586132.md) / `PAYMENT_CODE` 付款码支付：用户线下展示付款码，商户收银员用扫码设备扫描用户的付款码，用户确认支付，详细可查看 [场景介绍](https://pay.weixin.qq.com/doc/v2/partner/4011941052.md)（官方字段路径 <c>payment_scene</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("payment_scene")]
    public string? PaymentScene { get; set; }

    /// <summary>【公众账号ID】 公众账号ID也称 AppID，是（微信开放平台、微信公众平台）为开发者提供的一个唯一标识，用于识别开发者的应用程序（APP、小程序、公众号）。 1、若交易场景类型为小程序支付、APP支付时，此字段返回对应的AppID； 2、其他交易场景类型时，此字段返回空值（官方字段路径 <c>appid</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【商户号】 商户号是商户在 商户平台 申请的唯一身份标识，微信支付通过商户号确认商户身份。 1、当交易场景为付款码支付，或小程序支付且对应的 AppID 为收银类平台小程序时，此字段返回对应的商户号； 2、其他交易场景类型时，此字段返回空值（官方字段路径 <c>card_link_mchid</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("card_link_mchid")]
    public string? CardLinkMchid { get; set; }

    /// <summary>【服务ID】 支付分服务ID是商户支付分服务的唯一标识，由32位数字组成。支付分产品权限申请审核通过后，微信支付运营会向商户提供支付分服务ID。 1、当交易场景类型为支付分支付时，此字段返回对应的支付分服务ID； 2、其他交易场景类型时，此字段返回空值（官方字段路径 <c>service_id</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

}

/// <summary>交易连接名片规则申请状态响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016366816"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardLink")]
public class BrandCardLinkStateResponse : WechatPayResponse
{
    /// <summary>【品牌业务申请编号】 1、只能由数字、字母或下划线组成； 2、服务商自定义的请求流水号，用于标识每一个交易连接名片关联关系配置，需确保同一品牌下不同账号ID配置的流水号唯一。​（官方字段路径 <c>business_code</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("business_code")]
    public string? BusinessCode { get; set; }

    /// <summary>【品牌ID】 1、商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符； 2、数字，长度不超过 20 个字符; 3、暂不支持银行品牌（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【交易场景类型】 交易场景类型是指微信支付提供的帮助商家在不同场景中快速收款的工具（如小程序支付、付款码支付等）。 不同类型的交易场景下，建立交易连接名片规则依据的账号类型不同： 1、当枚举值选择 `MINI_PROGRAM`、 `APP` 时， 返回 `appid` 字段； 2、当枚举值选择 `PAYMENT_SCORE` 时，返回 `service_id` 字段； 3、当枚举值选择 `PAYMENT_CODE` 时， 返回 `card_link_mch…。可选取值：`MINI_PROGRAM` 小程序支付：用户在品牌的小程序中选择商品下单，跳转微信支付确认验密页面，完成支付，详细可查看 [产品介绍](https://pay.weixin.qq.com/doc/v3/partner/4012085810.md) / `APP` APP 支付：用户在品牌的App中选择商品下单，跳转到微信支付确认验密页面，完成支付，详细可查看 [产品介绍](https://pay.weixin.qq.com/doc/v3/partner/4013080227.md) / `PAYMENT_SCORE` 支付分支付：微信支付分提供先使用服务，再付款的能力，如免押金借用充电宝、电商先用后付等，详细可查看 [产品介绍](https://pay.weixin.qq.com/doc/v3/partner/4012586132.md) / `PAYMENT_CODE` 付款码支付：用户线下展示付款码，商户收银员用扫码设备扫描用户的付款码，用户确认支付，详细可查看 [场景介绍](https://pay.weixin.qq.com/doc/v2/partner/4011941052.md)（官方字段路径 <c>payment_scene</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("payment_scene")]
    public string? PaymentScene { get; set; }

    /// <summary>【公众账号ID】 公众账号ID也称 APPID，是（微信开放平台、微信公众平台）为开发者提供的一个唯一标识，用于识别开发者的应用程序（APP、小程序、公众号）。 1、若交易场景类型为小程序支付、APP支付时，此字段返回对应的 AppID； 2、其他交易场景类型时，此字段返回空值（官方字段路径 <c>appid</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【商户号】 商户号是商户在 商户平台 申请的唯一身份标识，微信支付通过商户号确认商户身份。 1、当交易场景为付款码支付，或小程序支付且对应的 appid 为收银类平台小程序时，此字段返回对应的商户号； 2、其他交易场景类型时，此字段返回空值（官方字段路径 <c>card_link_mchid</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("card_link_mchid")]
    public string? CardLinkMchid { get; set; }

    /// <summary>【服务ID】 支付分服务ID是商户支付分服务的唯一标识，由32位数字组成。支付分产品权限申请审核通过后，微信支付运营会向商户提供支付分服务ID。 1、当交易场景类型为支付分支付时，此字段返回对应的支付分服务ID； 2、其他交易场景类型时，此字段返回空值（官方字段路径 <c>service_id</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>【交易连接名片配置状态】 该字段返回交易连接名片的配置状态。可选取值：`STATE_UNKNOWN` 「状态未知，非终态」连接规则建立中，请稍后 / `WAITING_AUDIT` 「审核中，非终态」创建交易链路请求受理后，平台将进行审核，即流转到该状态 / `AUDIT_REJECT` 「审核驳回，终态」审核阶段被驳回，会附带具体的驳回原因，请根据驳回原因修改调整，如仍需添加请使用原业务申请编号重新调用 [配置交易连接名片](https://pay.weixin.qq.com/doc/v3/partner/4016333302.md) 接口 / `WAITING_CONFIRMATION` 「待商户号超管确认，非终态」添加付款码支付/收银类小程序支付场景时，若提供的商户号主体与品牌不一致，则需商户号超管确认才能完成规则配置，请提醒对方超管查看「微信支付商家助手」继续确认 / `MERCHANT_ADMIN_REJECT` 「商户号超管已拒绝，终态」付款码支付/收银类小程序支付场景，绑定异主体商户号，商户号超管已拒绝或超时，如仍需绑定请使用原业务申请编号重新调用 [配置交易连接名片](https://pay.weixin.qq.com/doc/v3/partner/4016333302.md) 接口 / `IN_EFFECT` 「生效中，终态」交易链路建立成功 / `CANCELED` 「已撤销，终态」交易链路请求被撤销，如仍需添加请更换业务申请编号重新调用 [配置交易连接名片](https://pay.weixin.qq.com/doc/v3/partner/4016333302.md) 接口（官方字段路径 <c>configuration_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("configuration_state")]
    public string? ConfigurationState { get; set; }

    /// <summary>【驳回原因】 若交易连接名片规则配置请求被驳回时，则会提供驳回原因，如仍需配置，请根据驳回原因修改和调整后，重新发起请求（官方字段路径 <c>reject_reason</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }

}

/// <summary>交易连接名片配置申请撤销请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016366797"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardLink")]
public class BrandCardLinkCancelRequest
{
    /// <summary>【品牌业务申请编号】 1、只能由数字、字母或下划线组成； 2、服务商自定义的请求流水号，用于标识每一个交易连接名片关联关系配置，需确保同一品牌下不同账号ID配置的流水号唯一； 3、填写调用 配置交易连接名片 时传递的业务申请编号（官方字段路径 <c>business_code</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("business_code")]
    public string? BusinessCode { get; set; }

    /// <summary>【品牌ID】 1、商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符； 2、数字，长度不超过 20 个字符; 3、暂不支持银行品牌（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>交易连接名片配置申请撤销响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016366797"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardLink")]
public class BrandCardLinkCancelResponse : WechatPayResponse
{
    /// <summary>【品牌业务申请编号】 1、只能由数字、字母或下划线组成； 2、服务商自定义的请求流水号，用于标识每一个交易连接名片关联关系配置，需确保同一品牌下不同账号ID配置的流水号唯一（官方字段路径 <c>business_code</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("business_code")]
    public string? BusinessCode { get; set; }

    /// <summary>【品牌ID】 1、商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符； 2、数字，长度不超过 20 个字符; 3、暂不支持银行品牌（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>交易连接名片场景解除请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016366804"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardLink")]
public class BrandCardLinkUnbindRequest
{
    /// <summary>【品牌ID】 1、商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符； 2、数字，长度不超过 20 个字符; 3、暂不支持银行品牌（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【交易场景类型】 交易场景类型是指微信支付提供的帮助商家在不同场景中快速收款的工具（如小程序支付、付款码支付等）。 不同类型的交易场景下，建立交易连接名片规则依据的账号类型不同： 1、当交易场景类型填 `MINI_PROGRAM`、 `APP` 时， `appid` 字段必填； 2、当交易场景类型填 `PAYMENT_SCORE` 时， `service_id` 字段必填； 3、当交易场景类型填 `PAYMENT_CODE` 时， `card_link_m…。可选取值：`MINI_PROGRAM` 小程序支付：用户在品牌的小程序中选择商品下单，跳转微信支付确认验密页面，完成支付，详细可查看 [产品介绍](https://pay.weixin.qq.com/doc/v3/partner/4012085810.md) / `APP` APP 支付：用户在品牌的App中选择商品下单，跳转到微信支付确认验密页面，完成支付，详细可查看 [产品介绍](https://pay.weixin.qq.com/doc/v3/partner/4013080227.md) / `PAYMENT_SCORE` 支付分支付：微信支付分提供先使用服务，再付款的能力，如免押金借用充电宝、电商先用后付等，详细可查看 [产品介绍](https://pay.weixin.qq.com/doc/v3/partner/4012586132.md) / `PAYMENT_CODE` 付款码支付：用户线下展示付款码，商户收银员用扫码设备扫描用户的付款码，用户确认支付，详细可查看 [场景介绍](https://pay.weixin.qq.com/doc/v2/partner/4011941052.md)（官方字段路径 <c>payment_scene</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("payment_scene")]
    public string? PaymentScene { get; set; }

    /// <summary>【公众账号ID】 公众账号ID也称 AppID，是（微信开放平台、微信公众平台）为开发者提供的一个唯一标识，用于识别开发者的应用程序（APP、小程序、公众号）。 开发者需要在微信开放平台或微信公众平台中申请ID。 1、若交易场景类型为小程序支付、APP支付时，此字段必填，其他交易场景类型时不支持填写； 2、请提供与品牌经营场景有关的下单小程序（如点餐小程序、电商下单小程序等）或APP，请勿提供与品牌经营场景无关的小程序（如不发生交易支付的广告小程序等）或A…（官方字段路径 <c>appid</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【商户号】 商户号是商户在 商户平台 申请的唯一身份标识，所有接口在调用时都必须传商户号，微信支付通过商户号确认商户身份。 注意：若交易场景为付款码支付，或小程序支付且提供的 AppID 为收银类平台小程序时，此字段必填（仅支持填写普通商户、特约商户类型商户号），其他交易场景类型不支持填写（官方字段路径 <c>card_link_mchid</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("card_link_mchid")]
    public string? CardLinkMchid { get; set; }

    /// <summary>【服务ID】 1、商户支付分服务的唯一标识，由32位数字组成。支付分产品权限审核通过后，微信支付运营会向商户提供该ID； 2、交易场景为支付分支付时必填，其他交易场景不支持填写（官方字段路径 <c>service_id</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

}

/// <summary>交易连接名片场景解除响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016366804"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardLink")]
public class BrandCardLinkUnbindResponse : WechatPayResponse
{
    /// <summary>【品牌ID】 1、商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符； 2、数字，长度不超过 20 个字符; 3、暂不支持银行品牌（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【交易场景类型】 交易场景类型是指微信支付提供的帮助商家在不同场景中快速收款的工具（如小程序支付、付款码支付等）。 不同类型的交易场景下，建立交易连接名片规则依据的账号类型不同： 1、当枚举值选择 `MINI_PROGRAM`、 `APP` 时， 返回 `appid` 字段； 2、当枚举值选择 `PAYMENT_SCORE` 时，返回 `service_id` 字段； 3、当枚举值选择 `PAYMENT_CODE` 时， 返回 `card_link_mch…。可选取值：`MINI_PROGRAM` 小程序支付：用户在品牌的小程序中选择商品下单，跳转微信支付确认验密页面，完成支付，详细可查看 [产品介绍](https://pay.weixin.qq.com/doc/v3/partner/4012085810.md) / `APP` APP 支付：用户在品牌的App中选择商品下单，跳转到微信支付确认验密页面，完成支付，详细可查看 [产品介绍](https://pay.weixin.qq.com/doc/v3/partner/4013080227.md) / `PAYMENT_SCORE` 支付分支付：微信支付分提供先使用服务，再付款的能力，如免押金借用充电宝、电商先用后付等，详细可查看 [产品介绍](https://pay.weixin.qq.com/doc/v3/partner/4012586132.md) / `PAYMENT_CODE` 付款码支付：用户线下展示付款码，商户收银员用扫码设备扫描用户的付款码，用户确认支付，详细可查看 [场景介绍](https://pay.weixin.qq.com/doc/v2/partner/4011941052.md)（官方字段路径 <c>payment_scene</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("payment_scene")]
    public string? PaymentScene { get; set; }

    /// <summary>【公众账号ID】 公众账号ID也称 AppID，是（微信开放平台、微信公众平台）为开发者提供的一个唯一标识，用于识别开发者的应用程序（APP、小程序、公众号）。 1、若交易场景类型为小程序支付、APP支付时，此字段返回对应的 AppID； 2、其他交易场景类型时，此字段返回空值（官方字段路径 <c>appid</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【商户号】 商户号是商户在 商户平台 申请的唯一身份标识，微信支付通过商户号确认商户身份。 1、当交易场景为付款码支付，或小程序支付且对应的 AppID 为收银类平台小程序时，此字段返回对应的商户号； 2、其他交易场景类型时，此字段返回空值（官方字段路径 <c>card_link_mchid</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("card_link_mchid")]
    public string? CardLinkMchid { get; set; }

    /// <summary>【服务ID】 支付分服务ID是商户支付分服务的唯一标识，由32位数字组成。支付分产品权限申请审核通过后，微信支付运营会向商户提供支付分服务ID。 1、当交易场景类型为支付分支付时，此字段返回对应的支付分服务ID； 2、其他交易场景类型时，此字段返回空值（官方字段路径 <c>service_id</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

}
