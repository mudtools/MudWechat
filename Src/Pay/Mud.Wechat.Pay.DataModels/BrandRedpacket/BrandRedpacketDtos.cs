// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.BrandRedpacket;

/// <summary>品牌商户红包批次发放请求·【品牌红包明细列表】 品牌红包明细列表，最多10笔。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014310358"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandRedpacket")]
public class BrandRedpacketBatchCreateRequestDetailListItem
{
    /// <summary>【商家品牌红包明细单号】 品牌商户系统内部区分品牌红包批次单下不同品牌红包明细单的唯一标识，非全局唯一（在同一批次下不允许重复，不同批次下允许相同），要求此参数只能由数字、大小写字母组成（官方字段路径 <c>detail_list.out_detail_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }

    /// <summary>【红包金额（单位：分）】 红包金额单位为“分”，红包金额的最大限额取决于商户在商户平台的设置额度（官方字段路径 <c>detail_list.amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【接收红包用户OpenID】 接收红包的用户OpenID，该OpenID为用户在上方指定的AppID下的唯一标识。注：openid是微信用户在公众号appid下的唯一用户标识（appid不同，则获取到的openid就不同），可用于永久标记一个用户。 获取openid（官方字段路径 <c>detail_list.openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【接收红包用户姓名】 1、明细转账金额 &gt;= 2,000，收款用户姓名必填； 2、同一批次转账明细中，收款用户姓名字段需全部填写、或全部不填写； 3、 若传入收款用户姓名，微信支付会校验用户OpenID与姓名是否一致，并提供电子回单； 4、收款方姓名。采用标准RSA算法，公钥由微信侧提供 5、该字段需进行加密处理，加密方法详见 敏感信息加密说明。(提醒：必须在HTTP头中上送Wechatpay-Serial)（官方字段路径 <c>detail_list.user_name</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>【红包备注】 单个红包备注，会展示在客户端收款凭证的“红包说明”字段，UTF8编码，最多允许32个字符（官方字段路径 <c>detail_list.remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

}

/// <summary>品牌商户红包批次发放请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014310358"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandRedpacket")]
public class BrandRedpacketBatchCreateRequest
{
    /// <summary>【品牌ID】 品牌在微信支付进行品牌认证后的唯一标识品牌ID（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public long? BrandId { get; set; }

    /// <summary>【品牌AppID】 品牌商户在微信申请公众号/小程序或移动应用成功后分配的账号ID，需与品牌有绑定关系，使用品牌的AppID时需要填写，具体请参考 普通商户模式开发必要参数说明（官方字段路径 <c>brand_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_appid")]
    public string? BrandAppid { get; set; }

    /// <summary>【品牌红包发放场景】 品牌红包发放场景，用户可以在指定的场景领取到红包。可选取值：`CUSTOM_SEND` 自定义场景发放红包，使用已配置的自定义发放模板进行发红包（官方字段路径 <c>scene</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("scene")]
    public string? Scene { get; set; }

    /// <summary>【品牌红包模板ID】 品牌主配置的品牌红包模板ID（官方字段路径 <c>template_id</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>【商家品牌红包批次单号】 品牌商户系统内部的品牌红包批次单号，要求此参数只能由数字、大小写字母组成，在商户系统内部唯一（官方字段路径 <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>【品牌红包批次名称】 该品牌红包批次的名称，展示在用户红包领取通知的红包活动名称（官方字段路径 <c>batch_name</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("batch_name")]
    public string? BatchName { get; set; }

    /// <summary>【品牌红包批次备注】 该批次品牌红包的备注，仅用于品牌商户内部管理（官方字段路径 <c>batch_remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("batch_remark")]
    public string? BatchRemark { get; set; }

    /// <summary>【总金额】 品牌红包总金额必须与品牌红包批次内所有品牌红包明细发送金额之和保持一致，否则无法发送品牌红包（官方字段路径 <c>total_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>【总笔数】 一个品牌红包批次单最多发送10笔品牌红包明细。品牌红包总笔数必须与批次内所有品牌红包明细之和保持一致，否则无法发送品牌红包（官方字段路径 <c>total_num</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_num")]
    public long? TotalNum { get; set; }

    /// <summary>【品牌红包明细列表】 品牌红包明细列表，最多10笔（官方字段路径 <c>detail_list</c>，选填，类型 <c>array[DetailEntityForCreate]</c>）。</summary>
    [JsonPropertyName("detail_list")]
    public List<BrandRedpacketBatchCreateRequestDetailListItem>? DetailList { get; set; }

}

/// <summary>品牌商户红包批次发放响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014310358"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandRedpacket")]
public class BrandRedpacketBatchCreateResponse : WechatPayResponse
{
    /// <summary>【商家品牌红包批次单号】 品牌商户系统内部区分品牌红包批次单下不同品牌红包明细单的唯一标识，要求此参数只能由数字、大小写字母组成（官方字段路径 <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>【微信支付品牌红包批次单号】 微信批次单号，微信商家转账系统返回的唯一标识（官方字段路径 <c>batch_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("batch_no")]
    public string? BatchNo { get; set; }

    /// <summary>【品牌红包批次创建时间】 批次受理成功时返回，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示北京时间2015年05月20日13点29分35秒（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【批次单状态】 当前品牌红包批次状态。可选取值：`WAIT_PAY` 商户员工确认付款阶段 / `ACCEPTED` 批次已受理成功，若发起品牌红包的30分钟后，品牌红包批次单仍处于该状态，可能原因是商户账户余额不足等。商户可查询账户资金流水，若该笔品牌红包批次单的扣款已经发生，则表示批次已经进入发送中，请再次查单确认 / `PROCESSING` 已开始处理批次内的品牌红包明细单 / `FINISHED` 批次内的所有品牌红包明细单都已处理完成 / `CLOSED` 可查询具体的批次关闭原因确认（官方字段路径 <c>batch_state</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("batch_state")]
    public string? BatchState { get; set; }

}

/// <summary>品牌商户红包批次查询（微信批次号）响应·【品牌红包明细列表】 当批次状态为“FINISHED”（已完成），且成功查询到品。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014310369"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandRedpacket")]
public class BrandRedpacketBatchQueryResponseDetailListItem
{
    /// <summary>【微信支付品牌红包明细单号】 微信支付系统内部区分品牌红包批次单下不同品牌红包明细单的唯一标识（官方字段路径 <c>detail_list.transfer_detail_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transfer_detail_no")]
    public string? TransferDetailNo { get; set; }

    /// <summary>【商家品牌红包明细单号】 品牌商户系统内部区分品牌红包批次单下不同品牌红包明细单的唯一标识，要求此参数只能由数字、大小写字母组成（官方字段路径 <c>detail_list.out_detail_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }

    /// <summary>【品牌红包明细单状态】 品牌红包明细单的状态。可选取值：`DETAIL_INIT` 系统转账校验中 / `DETAIL_WAIT_PAY` 待商户确认，符合免密条件时，系统会自动扭转为发送中 / `DETAIL_PROCESSING` 正在处理中，品牌红包发送结果尚未明确 / `DETAIL_SUCCESS` 发送成功 / `DETAIL_FAIL` 需要确认失败原因后，再决定是否重新对该笔品牌红包明细单进行发送（并非整个品牌红包批次单）（官方字段路径 <c>detail_list.detail_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("detail_state")]
    public string? DetailState { get; set; }

}

/// <summary>品牌商户红包批次查询（微信批次号）响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014310369"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandRedpacket")]
public class BrandRedpacketBatchQueryResponse : WechatPayResponse
{
    /// <summary>【品牌主商户号】 微信服务商下特约商户的商户号，且已经认证品牌的品牌主商户号（官方字段路径 <c>brand_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_mchid")]
    public string? BrandMchid { get; set; }

    /// <summary>【微信支付品牌红包批次单号】 品牌在微信支付进行品牌认证后的唯一标识品牌ID（官方字段路径 <c>batch_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("batch_no")]
    public string? BatchNo { get; set; }

    /// <summary>【商家品牌红包批次单号】 品牌商户系统内部的品牌红包批次单号，要求此参数只能由数字、大小写字母组成，在商户系统内部唯一（官方字段路径 <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>【品牌ID】 品牌在微信支付进行品牌认证后的唯一标识品牌ID（官方字段路径 <c>brand_id</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public long? BrandId { get; set; }

    /// <summary>【品牌红包模板ID】 品牌主配置的品牌红包模板ID（官方字段路径 <c>template_id</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>【品牌AppID】 品牌商户在微信申请公众号/小程序或移动应用成功后分配的账号ID，该AppID需与品牌ID有绑定关系，具体请参考 普通商户模式开发必要参数说明（官方字段路径 <c>brand_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_appid")]
    public string? BrandAppid { get; set; }

    /// <summary>【品牌红包批次状态】 当前品牌红包批次状态。可选取值：`WAIT_PAY` 商户员工确认付款阶段 / `ACCEPTED` 批次已受理成功，若发起品牌红包的30分钟后，品牌红包批次单仍处于该状态，可能原因是商户账户余额不足等。商户可查询账户资金流水，若该笔品牌红包批次单的扣款已经发生，则表示批次已经进入发送中，请再次查单确认 / `PROCESSING` 已开始处理批次内的品牌红包明细单 / `FINISHED` 批次内的所有品牌红包明细单都已处理完成 / `CLOSED` 可查询具体的批次关闭原因确认（官方字段路径 <c>batch_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("batch_state")]
    public string? BatchState { get; set; }

    /// <summary>【品牌红包批次名称】 该批次品牌红包的备注，用于品牌商户内部管理（官方字段路径 <c>batch_name</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("batch_name")]
    public string? BatchName { get; set; }

    /// <summary>【品牌红包批次备注】 该批次品牌红包的备注，仅用于品牌商户内部管理（官方字段路径 <c>batch_remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("batch_remark")]
    public string? BatchRemark { get; set; }

    /// <summary>【品牌红包批次单关闭原因】 品牌红包批次单状态为“CLOSED”（已关闭）时返回。可选取值：`MERCHANT_REVOCATION` 商户主动撤销 / `SYSTEM_OVERDUE_CLOSE` 系统超时关闭（官方字段路径 <c>close_reason</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("close_reason")]
    public string? CloseReason { get; set; }

    /// <summary>【总金额】 品牌红包金额单位为“分”（官方字段路径 <c>total_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>【总笔数】 一个品牌红包批次单最多发送10笔品牌红包明细。品牌红包总笔数必须与批次内所有品牌红包明细之和保持一致，否则无法发送品牌红包（官方字段路径 <c>total_num</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_num")]
    public long? TotalNum { get; set; }

    /// <summary>【品牌红包批次创建时间】 品牌红包批次受理成功时返回，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示北京时间2015年05月20日13点29分35秒（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【品牌红包批次更新时间】 品牌红包批次最近一次状态变更时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示北京时间2015年05月20日13点29分35秒（官方字段路径 <c>update_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>【发放成功金额】 品牌红包发放成功的金额，单位为“分”。当批次状态为“PROCESSING”（发送中）时，发放成功金额随时可能变化（官方字段路径 <c>success_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("success_amount")]
    public long? SuccessAmount { get; set; }

    /// <summary>【发放成功笔数】 品牌红包发放成功的笔数。当批次状态为“PROCESSING”（发放中）时，发放成功笔数随时可能变化（官方字段路径 <c>success_num</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("success_num")]
    public long? SuccessNum { get; set; }

    /// <summary>【发放失败金额】 品牌红包发放失败的金额，单位为“分”（官方字段路径 <c>fail_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("fail_amount")]
    public long? FailAmount { get; set; }

    /// <summary>【发放失败笔数】 品牌红包发放失败的笔数（官方字段路径 <c>fail_num</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("fail_num")]
    public long? FailNum { get; set; }

    /// <summary>【品牌红包明细列表】 当批次状态为“FINISHED”（已完成），且成功查询到品牌红包明细单时返回。包括微信支付品牌红包明细单号、明细状态信息（官方字段路径 <c>detail_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("detail_list")]
    public List<BrandRedpacketBatchQueryResponseDetailListItem>? DetailList { get; set; }

}

/// <summary>品牌商户红包明细查询（微信明细号）响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014310384"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandRedpacket")]
public class BrandRedpacketDetailQueryResponse : WechatPayResponse
{
    /// <summary>【品牌主商户号】 微信服务商下特约商户的商户号，且已经认证品牌的品牌主商户号（官方字段路径 <c>brand_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_mchid")]
    public string? BrandMchid { get; set; }

    /// <summary>【商家品牌红包批次单号】 商户系统内部的商家品牌红包批次单号，在商户系统内部唯一（官方字段路径 <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>【微信支付品牌红包批次单号】 微信支付品牌红包批次单号，微信商家品牌红包系统返回的唯一标识（官方字段路径 <c>batch_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("batch_no")]
    public string? BatchNo { get; set; }

    /// <summary>【商家品牌红包明细单号】 品牌商户系统内部的品牌红包批次单号，要求此参数只能由数字、大小写字母组成，在商户系统内部唯一（官方字段路径 <c>out_detail_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }

    /// <summary>【微信支付品牌红包明细单号】 微信支付系统内部区分品牌红包批次单下不同品牌红包明细单的唯一标识（官方字段路径 <c>detail_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("detail_no")]
    public string? DetailNo { get; set; }

    /// <summary>【品牌红包明细单状态】 品牌红包明细单状态。可选取值：`DETAIL_INIT` 系统转账校验中 / `DETAIL_WAIT_PAY` 待商户确认，符合免密条件时，系统会自动扭转为发送中 / `DETAIL_PROCESSING` 正在处理中，品牌红包发送结果尚未明确 / `DETAIL_SUCCESS` 发送成功 / `DETAIL_FAIL` 需要确认失败原因后，再决定是否重新对该笔品牌红包明细单进行发送（并非整个品牌红包批次单）（官方字段路径 <c>detail_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("detail_state")]
    public string? DetailState { get; set; }

    /// <summary>【红包金额】 红包金额单位为“分”（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【红包备注】 单个品牌红包备注（微信用户会收到该备注），UTF8编码，最多允许32个字符（官方字段路径 <c>remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>【明细失败原因】 明细失败原因。可选取值：`ACCOUNT_FROZEN` 该用户账户被冻结 / `REAL_NAME_CHECK_FAIL` 收款人未实名认证，需要用户完成微信实名认证 / `NAME_NOT_CORRECT` 收款人姓名校验不通过，请核实信息 / `OPENID_INVALID` OpenID格式错误或者不属于商家公众账号 / `TRANSFER_QUOTA_EXCEED` 超过用户单笔收款区间，核实产品设置是否准确 / `DAY_RECEIVED_QUOTA_EXCEED` 超过用户单日收款额度，核实产品设置是否准确 / `DAY_RECEIVED_COUNT_EXCEED` 超过用户单日收款次数，核实产品设置是否准确 / `PRODUCT_AUTH_CHECK_FAIL` 未开通该权限或权限被冻结，请核实产品权限状态 / `OVERDUE_CLOSE` 该笔转账已关闭 / `ACCOUNT_NOT_EXIST` 该用户账户不存在 / `TRANSFER_RISK` 该笔转账可能存在风险，已被微信拦截 / `USER_ACCOUNT_LIMIT` 用户账户收款受限，请引导用户在微信支付查看详情 / `FAIL_REASON_UNKNOWN` 失败原因未知 / `PAYER_ACCOUNT_ABNORMAL` 商户账户付款受限，可前往商户平台获取解除功能限制指引 / `PAYEE_ACCOUNT_ABNORMAL` 用户账户收款异常，请联系用户完善其在微信支付的身份信息以继续收款 / `USER_RECEIVE_OVERDUE` 用户逾期未领取 / `REMARK_NOT_CORRECT` 红包备注设置失败，请修改后再试 / `MERCHANT_REJECT` 商户员工（转账验密人）已驳回转账 / `MERCHANT_NOT_CONFIRM` 商户员工（转账验密人）超时未验密 / `BLOCK_B2C_USERLIMITAMOUNT_BSRULE_MONTH` 超出用户单月转账收款20w限额，本月不支持继续向该用户付款（官方字段路径 <c>fail_reason</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>【接收红包用户OpenID】 接收红包的用户OpenID，OpenID为用户在对应AppID下的唯一标识（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【接收红包用户姓名】 发放品牌红包时传入的接收红包用户姓名，已使用商户的公钥加密（官方字段路径 <c>user_name</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>【品牌红包发起时间】 品牌红包发起的时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示北京时间2015年05月20日13点29分35秒（官方字段路径 <c>initiate_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("initiate_time")]
    public string? InitiateTime { get; set; }

    /// <summary>【品牌红包更新时间】 品牌红包明细单最后一次状态变更时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示北京时间2015年05月20日13点29分35秒（官方字段路径 <c>update_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>【品牌ID】 品牌在微信支付进行品牌认证后的唯一标识品牌ID（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public long? BrandId { get; set; }

    /// <summary>【品牌红包模板ID】 品牌主配置的品牌红包模板ID（官方字段路径 <c>template_id</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>【品牌AppID】 品牌商户在微信申请公众号/小程序或移动应用成功后分配的账号ID，该AppID需与品牌ID有绑定关系，具体请参考 普通商户模式开发必要参数说明（官方字段路径 <c>brand_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_appid")]
    public string? BrandAppid { get; set; }

}
