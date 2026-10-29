// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceSettle;

/// <summary>申请个人收款批量结算请求·【结算信息列表】 结算信息列表，上限数量20·【结算明细单列表】 结算明细单列表，上限数量5·【金额】 金额，单位分。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012649847"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceSettle")]
public class SettleApplyRequestSettleInfoListItemSettleDetailInfoListItemAmount
{
    /// <summary>【金额】 金额，单位分（官方字段路径 <c>settle_info_list.settle_detail_info_list.amount.amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【币种】 符合ISO 4217标准的三位字母代码，填空默认：CNY。目前只支持人民币：CNY（官方字段路径 <c>settle_info_list.settle_detail_info_list.amount.currency</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>申请个人收款批量结算请求·【结算信息列表】 结算信息列表，上限数量20·【结算明细单列表】 结算明细单列表，上限数量5。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012649847"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceSettle")]
public class SettleApplyRequestSettleInfoListItemSettleDetailInfoListItem
{
    /// <summary>【商户结算明细单号】 商户结算明细单号（官方字段路径 <c>settle_info_list.settle_detail_info_list.out_settle_detail_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_settle_detail_no")]
    public string? OutSettleDetailNo { get; set; }

    /// <summary>【结算类型】 结算类型。可选取值：`RECEIVE` 收款 / `DEDUCTION` 交易款扣除平台商户相关服务费用（官方字段路径 <c>settle_info_list.settle_detail_info_list.settle_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("settle_type")]
    public string? SettleType { get; set; }

    /// <summary>【接收方类型】 接收方类型。可选取值：`MERCHANT` 微信支付商户 / `USER` 微信支付用户（官方字段路径 <c>settle_info_list.settle_detail_info_list.receiver_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("receiver_type")]
    public string? ReceiverType { get; set; }

    /// <summary>【接收方账号】 接收方账号，当接收方类型为商户时，填写接收方的商户号。当接收方类型为用户时，填写接收方的授权ID（官方字段路径 <c>settle_info_list.settle_detail_info_list.receiver_account</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("receiver_account")]
    public string? ReceiverAccount { get; set; }

    /// <summary>【金额】 金额，单位分（官方字段路径 <c>settle_info_list.settle_detail_info_list.amount</c>，<b>必填</b>，类型 <c>AmountInfo</c>）。</summary>
    [JsonPropertyName("amount")]
    public SettleApplyRequestSettleInfoListItemSettleDetailInfoListItemAmount? Amount { get; set; }

}

/// <summary>申请个人收款批量结算请求·【结算信息列表】 结算信息列表，上限数量20。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012649847"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceSettle")]
public class SettleApplyRequestSettleInfoListItem
{
    /// <summary>【商户结算单号】 商户系统内部生成的结算批次的结算单号，批次内保证唯一，只能由数字、大小写字母_-&amp;#124;\*@组成（官方字段路径 <c>settle_info_list.out_settle_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_settle_no")]
    public string? OutSettleNo { get; set; }

    /// <summary>【微信支付订单号】 微信支付订单号（官方字段路径 <c>settle_info_list.transaction_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【结算明细单列表】 结算明细单列表，上限数量5（官方字段路径 <c>settle_info_list.settle_detail_info_list</c>，<b>必填</b>，类型 <c>array[SettleDetailInfo]</c>）。</summary>
    [JsonPropertyName("settle_detail_info_list")]
    public List<SettleApplyRequestSettleInfoListItemSettleDetailInfoListItem>? SettleDetailInfoList { get; set; }

}

/// <summary>申请个人收款批量结算请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012649847"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceSettle")]
public class SettleApplyRequest
{
    /// <summary>【商户结算批次单号】 商户系统内部生成的结算批次单号，商户号下保证唯一，只能由数字、大小写字母_-&amp;#124;\*@组成。一笔商户结算批次单号可操作多笔交易订单结算，同一笔商户结算批次单号多次请求，需保证请求内容完全一致（官方字段路径 <c>out_settle_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_settle_batch_no")]
    public string? OutSettleBatchNo { get; set; }

    /// <summary>【商品单个人收款方受理授权ID】 标识一个微信用户在该平台用于个人收款的微信支付账户，平台邀请微信用户完成个人收款授权后获得此id，详见 查询个人收款方授权结果（官方字段路径 <c>individual_auth_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("individual_auth_id")]
    public string? IndividualAuthId { get; set; }

    /// <summary>【通知地址】 商户接收个人收款结算结果的回调地址，通知URL必须为外网可访问的URL，不能携带参数。仅支持域名且必须为HTTPS（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string(255)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【收款方描述】 收款方描述，用于展示在用户确认收款的页面上（官方字段路径 <c>description</c>，<b>必填</b>，类型 <c>string(255)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【交易场景】 交易场景，必须与结算信息列表中的每一笔订单支付时的结算收款类型一致（官方字段路径 <c>trade_scenario</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("trade_scenario")]
    public string? TradeScenario { get; set; }

    /// <summary>【结算信息列表】 结算信息列表，上限数量20（官方字段路径 <c>settle_info_list</c>，<b>必填</b>，类型 <c>array[SettleInfo]</c>）。</summary>
    [JsonPropertyName("settle_info_list")]
    public List<SettleApplyRequestSettleInfoListItem>? SettleInfoList { get; set; }

    /// <summary>【收款类型模板ID】 用户收款时感知到的收款原因展示内容，将根据不同的收款类型模板ID展示不同的内容，具体的ID可查看产品文档了解（官方字段路径 <c>receive_type_template_id</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("receive_type_template_id")]
    public long? ReceiveTypeTemplateId { get; set; }

}

/// <summary>申请个人收款批量结算响应·【异常结算处理结果】 异常结算处理结果，仅当批次状态为异常结算处理申请成功的时候。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012649847"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceSettle")]
public class SettleApplyResponseAbnormalSettleResult
{
    /// <summary>【处理方式】 异常结算批次单处理方式（官方字段路径 <c>abnormal_settle_result.abnormal_settle_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("abnormal_settle_type")]
    public string? AbnormalSettleType { get; set; }

    /// <summary>【申请成功时间】 遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日13点29分35秒（官方字段路径 <c>abnormal_settle_result.apply_success_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("apply_success_time")]
    public string? ApplySuccessTime { get; set; }

    /// <summary>【微信支付异常资金付款单号】 微信支付异常资金付款单号，可以通过调用【查询在途异常资金付款结果】的接口获取付款结果（官方字段路径 <c>abnormal_settle_result.abnormal_fund_receipt_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("abnormal_fund_receipt_id")]
    public string? AbnormalFundReceiptId { get; set; }

    /// <summary>【商户异常结算批次单处理单号】 商户异常结算批次单处理单号（官方字段路径 <c>abnormal_settle_result.out_abnormal_settle_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_abnormal_settle_batch_no")]
    public string? OutAbnormalSettleBatchNo { get; set; }

}

/// <summary>申请个人收款批量结算响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012649847"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceSettle")]
public class SettleApplyResponse : WechatPayResponse
{
    /// <summary>【商户结算批次单号】 商户结算批次单号（官方字段路径 <c>out_settle_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_settle_batch_no")]
    public string? OutSettleBatchNo { get; set; }

    /// <summary>【微信支付结算批次单号】 微信支付系统内部生成的结算批次单号，全局唯一对应一笔商户结算批次单号（官方字段路径 <c>settle_batch_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("settle_batch_no")]
    public string? SettleBatchNo { get; set; }

    /// <summary>【商品单个人收款方受理授权ID】 标识一个微信用户在该平台用于个人收款的微信支付账户，平台邀请微信用户完成个人收款授权后获得此id，详见 查询个人收款方授权结果（官方字段路径 <c>individual_auth_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("individual_auth_id")]
    public string? IndividualAuthId { get; set; }

    /// <summary>【收款方描述】 收款方描述（官方字段路径 <c>description</c>，<b>必填</b>，类型 <c>string(255)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【批次状态】 批次状态。可选取值：`ACCEPTED` 结算已受理 / `RECEIVED` 用户入账成功 / `SUCCESS` 结算完成 / `DEDUCT_FEE_FAIL` 用户入账成功扣费失败 / `ABNORMAL` 结算异常 / `ABNORMAL_SETTLE_SUCCESS` 异常结算处理申请成功（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【交易场景】 交易场景（官方字段路径 <c>trade_scenario</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("trade_scenario")]
    public string? TradeScenario { get; set; }

    /// <summary>【批次创建时间】 遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日13点29分35秒（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【批次完成时间】 仅在批次状态为【结算成功】或【用户入账成功扣费失败】的时候有值。 遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日13…（官方字段路径 <c>finish_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("finish_time")]
    public string? FinishTime { get; set; }

    /// <summary>【异常原因类型】 异常原因类型，仅当批次状态为结算异常的时候返回（官方字段路径 <c>abnormal_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("abnormal_type")]
    public string? AbnormalType { get; set; }

    /// <summary>【异常结算处理结果】 异常结算处理结果，仅当批次状态为异常结算处理申请成功的时候返回（官方字段路径 <c>abnormal_settle_result</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("abnormal_settle_result")]
    public SettleApplyResponseAbnormalSettleResult? AbnormalSettleResult { get; set; }

}

/// <summary>申请异常结算批次单处理请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015198012"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionEcommerceSettle")]
public class SettleApplyAbnormalRequest
{
    /// <summary>【商品单个人收款方受理授权ID】 标识一个微信用户在该平台用于个人收款的微信支付账户，平台邀请微信用户完成个人收款授权后获得此id，详见 查询个人收款方授权结果（官方字段路径 <c>individual_auth_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("individual_auth_id")]
    public string? IndividualAuthId { get; set; }

    /// <summary>【商户异常结算批次单号】 商户系统内部生成的异常结算批次单号，商户号下保证唯一，只能由数字、大小写字母_-&amp;#124;\*@组成。同一笔商户异常结算批次单号多次请求，需保证请求内容完全一致（官方字段路径 <c>out_abnormal_settle_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_abnormal_settle_batch_no")]
    public string? OutAbnormalSettleBatchNo { get; set; }

    /// <summary>【异常结算批次单处理方式】 异常结算批次单处理方式。可选取值：`TRANSFER_TO_ORIGINAL_RECEIVE_USER` 转付至原收款用户（官方字段路径 <c>abnormal_settle_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("abnormal_settle_type")]
    public string? AbnormalSettleType { get; set; }

    /// <summary>【异常处理说明】 异常处理说明，后续体现在转付流程的转账说明中（官方字段路径 <c>description</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

}
