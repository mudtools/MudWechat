// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.PlatsolutionInsuranceBatch;

/// <summary>受理商家转账请求·【转账明细列表】 发起批量转账的明细列表，最多一千笔。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504220"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionInsuranceBatch")]
public class InsuranceBatchApplyRequestTransferDetailListItem
{
    /// <summary>【商家明细单号】 服务商系统内部区分转账批次单下不同转账明细单的唯一标识，要求此参数只能由数字、大小写字母组成（官方字段路径 <c>transfer_detail_list.out_detail_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }

    /// <summary>【转账金额】 转账金额单位为“分”，若预约时指定了该次预约为准确的金额，则转账金额必须等于预计转账金额，若没有指定为准确的金额，转账金额可以小于等于预计转账金额（官方字段路径 <c>transfer_detail_list.transfer_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transfer_amount")]
    public long? TransferAmount { get; set; }

    /// <summary>【转账备注】 单条转账备注（微信用户会收到该备注），UTF8编码，最多允许32个字符（官方字段路径 <c>transfer_detail_list.transfer_remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>【微信转账预约单号】 微信商家转账系统返回的预约记录唯一标识（官方字段路径 <c>transfer_detail_list.reservation_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("reservation_id")]
    public string? ReservationId { get; set; }

    /// <summary>【收款用户OpenID】 使用预约单号发起转账时，需填入预约时传入的openID（官方字段路径 <c>transfer_detail_list.openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

}

/// <summary>受理商家转账请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504220"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionInsuranceBatch")]
public class InsuranceBatchApplyRequest
{
    /// <summary>【二级商户号】 微信支付分配的商户号，出资商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【服务商AppID】 服务商绑定的公众号ID。使用预约单号发起转账时，需要与预约时传入的服务商AppID为同一个（官方字段路径 <c>sp_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>【二级商户AppID】 二级商户授权服务商绑定的公众号ID。使用预约单号发起转账时，需要与预约时传入的二级商户AppID为同一个（官方字段路径 <c>sub_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【商家批次单号】 服务商系统内部的批次单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方字段路径 <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>【批次名称】 该笔批量转账的名称（官方字段路径 <c>batch_name</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("batch_name")]
    public string? BatchName { get; set; }

    /// <summary>【批次备注】 转账说明，UTF8编码，最多允许32个字符（官方字段路径 <c>batch_remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("batch_remark")]
    public string? BatchRemark { get; set; }

    /// <summary>【转账总金额】 转账金额单位为“分”。转账总金额必须与批次内所有明细转账金额之和保持一致，否则无法发起转账操作（官方字段路径 <c>total_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>【转账总笔数】 一个转账批次单最多发起一千笔转账。转账总笔数必须与批次内所有明细之和保持一致，否则无法发起转账操作（官方字段路径 <c>total_num</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_num")]
    public long? TotalNum { get; set; }

    /// <summary>【转账场景ID】 指定该笔转账使用的转账场景ID，使用预约单号发起转账时，需要与预约时传入的转账场景ID为同一个（官方字段路径 <c>transfer_scene_id</c>，<b>必填</b>，类型 <c>string(36)</c>）。</summary>
    [JsonPropertyName("transfer_scene_id")]
    public string? TransferSceneId { get; set; }

    /// <summary>【转账明细列表】 发起批量转账的明细列表，最多一千笔（官方字段路径 <c>transfer_detail_list</c>，<b>必填</b>，类型 <c>array[TransferDetailInfo]</c>）。</summary>
    [JsonPropertyName("transfer_detail_list")]
    public List<InsuranceBatchApplyRequestTransferDetailListItem>? TransferDetailList { get; set; }

    /// <summary>【商户回调地址】 异步接收微信支付转账结果通知的回调地址，通知URL必须为公网可访问的URL，必须为HTTPS，不能携带参数（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

}

/// <summary>受理商家转账响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504220"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionInsuranceBatch")]
public class InsuranceBatchApplyResponse : WechatPayResponse
{
    /// <summary>【商家批次单号】 服务商系统内部的批次单号，在服务商系统内部唯一（官方字段路径 <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>【商家转账批次单号】 商家转账批次单号，微信商家转账系统返回的唯一标识（官方字段路径 <c>batch_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("batch_id")]
    public string? BatchId { get; set; }

    /// <summary>【批次创建时间】 批次受理成功时返回 使用rfc3339所定义的格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC8小时，即北京时间）。例如：2023-02-23T13:29:35+08:00表示，北京时间2023年2月23日 13点29分35秒（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【批次状态】 商家转账批次单状态。可选取值：`ACCEPTED` 批次已受理成功，若发起批量转账的30分钟后，转账批次单仍处于该状态，可能原因是出资商户账户余额不足等。商户可查询账户资金流水，若该笔转账批次单的扣款已经发生，则表示批次已经进入转账中，请再次查单确认 / `PROCESSING` 已开始处理批次内的转账明细单 / `COMPLETED` 批次内的所有转账明细单都已处理完成 / `CLOSED` 可根据 close_reason 字段确认具体的批次关闭原因确认（官方字段路径 <c>batch_state</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("batch_state")]
    public string? BatchState { get; set; }

}

/// <summary>商户转账批次单号查询批次单响应·【转账批次单】 转账批次单基本信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504222"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionInsuranceBatch")]
public class InsuranceBatchQueryResponseTransferBatch
{
    /// <summary>【商家批次单号】 服务商系统内部的批次单号，在服务商系统内部唯一（官方字段路径 <c>transfer_batch.out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>【商家转账批次单号】 商家转账批次单号，微信商家转账系统返回的唯一标识（官方字段路径 <c>transfer_batch.batch_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("batch_id")]
    public string? BatchId { get; set; }

    /// <summary>【服务商应用ID】 微信分配的商户公众账号ID（官方字段路径 <c>transfer_batch.sp_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>【二级商户号】 二级商户号（官方字段路径 <c>transfer_batch.sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【二级商户应用ID】 微信分配的商户公众账号ID（官方字段路径 <c>transfer_batch.sub_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【批次状态】 商家转账批次单状态。可选取值：`ACCEPTED` 批次已受理成功，若发起批量转账的30分钟后，转账批次单仍处于该状态，可能原因是出资商户账户余额不足等。商户可查询账户资金流水，若该笔转账批次单的扣款已经发生，则表示批次已经进入转账中，请再次查单确认 / `PROCESSING` 已开始处理批次内的转账明细单 / `COMPLETED` 批次内的所有转账明细单都已处理完成 / `CLOSED` 可根据 close_reason 字段确认具体的批次关闭原因确认（官方字段路径 <c>transfer_batch.batch_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("batch_state")]
    public string? BatchState { get; set; }

    /// <summary>【批次名称】 该笔商家转账的名称（官方字段路径 <c>transfer_batch.batch_name</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("batch_name")]
    public string? BatchName { get; set; }

    /// <summary>【批次备注】 转账说明，UTF8编码，最多允许32个字符（官方字段路径 <c>transfer_batch.batch_remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("batch_remark")]
    public string? BatchRemark { get; set; }

    /// <summary>【批次关闭原因】 如果批次状态为“CLOSED”（已关闭），则有关闭原因。可选取值：`MERCHANT_REVOCATION` 商户主动撤销（页面方式） / `OVERDUE_CLOSE` 系统超时关闭，可能原因账户余额不足或其他错误（官方字段路径 <c>transfer_batch.close_reason</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("close_reason")]
    public string? CloseReason { get; set; }

    /// <summary>【转账总金额】 转账金额单位为“分”（官方字段路径 <c>transfer_batch.total_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>【转账总笔数】 一个转账批次单最多发起1000笔转账（官方字段路径 <c>transfer_batch.total_num</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_num")]
    public long? TotalNum { get; set; }

    /// <summary>【批次创建时间】 批次创建时间 使用rfc3339所定义的格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC8小时，即北京时间）。例如：2023-02-23T13:29:35+08:00表示，北京时间2023年2月23日 13点29分35秒（官方字段路径 <c>transfer_batch.create_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【转账成功金额】 转账成功的金额，单位为“分”。当批次状态为“PROCESSING”（转账中）时，转账成功金额随时可能变化（官方字段路径 <c>transfer_batch.success_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("success_amount")]
    public long? SuccessAmount { get; set; }

    /// <summary>【转账成功笔数】 转账成功的笔数。当批次状态为“PROCESSING”（转账中）时，转账成功笔数随时可能变化（官方字段路径 <c>transfer_batch.success_num</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("success_num")]
    public long? SuccessNum { get; set; }

    /// <summary>【转账失败金额】 转账失败的金额，单位为“分”（官方字段路径 <c>transfer_batch.failed_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("failed_amount")]
    public long? FailedAmount { get; set; }

    /// <summary>【转账失败笔数】 转账失败的笔数（官方字段路径 <c>transfer_batch.failed_num</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("failed_num")]
    public long? FailedNum { get; set; }

    /// <summary>【转账场景ID】 指定的转账场景ID（官方字段路径 <c>transfer_batch.transfer_scene_id</c>，<b>必填</b>，类型 <c>string(36)</c>）。</summary>
    [JsonPropertyName("transfer_scene_id")]
    public string? TransferSceneId { get; set; }

}

/// <summary>商户转账批次单号查询批次单响应·【转账明细单列表】 当批次状态为“FINISHED”（已完成），且成功查询到转账。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504222"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionInsuranceBatch")]
public class InsuranceBatchQueryResponseDataItem
{
    /// <summary>【商家转账明细单号】 微信商家转账系统内部区分转账批次单下不同转账明细单的唯一标识（官方字段路径 <c>data.detail_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("detail_id")]
    public string? DetailId { get; set; }

    /// <summary>【商家明细单号】 商户系统内部区分转账批次单下不同转账明细单的唯一标识（官方字段路径 <c>data.out_detail_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }

    /// <summary>【明细状态】 商家转账明细单状态。可选取值：`INIT` 系统转账校验中 / `WAIT_PAY` 待二级商户员工（转账验密人）确认, 符合免确认条件时, 系统会自动扭转为转账中 / `PROCESSING` 正在处理中，转账结果尚未明确 / `SUCCESS` 转账成功 / `FAIL` 需要确认失败原因后，再决定是否重新发起对该笔明细单的转账（并非整个转账批次单） / `BANK_REFUND` 银行返回转账成功后，拒绝入账，资金会返回商户账户。（官方字段路径 <c>data.detail_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("detail_state")]
    public string? DetailState { get; set; }

}

/// <summary>商户转账批次单号查询批次单响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504222"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionInsuranceBatch")]
public class InsuranceBatchQueryResponse : WechatPayResponse
{
    /// <summary>【转账批次单】 转账批次单基本信息（官方字段路径 <c>transfer_batch</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("transfer_batch")]
    public InsuranceBatchQueryResponseTransferBatch? TransferBatch { get; set; }

    /// <summary>【转账明细单列表】 当批次状态为“FINISHED”（已完成），且成功查询到转账明细单时返回。包括微信明细单号、明细状态信息（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<InsuranceBatchQueryResponseDataItem>? Data { get; set; }

    /// <summary>【请求资源起始位置】 该次请求资源（转账明细单）的起始位置，从0开始，默认值为0（官方字段路径 <c>offset</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【最大资源条数】 该次请求可返回的最大资源（转账明细单）条数，最小20条，最大100条，不传则默认20条。不足20条按实际条数返回（官方字段路径 <c>limit</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

}

/// <summary>商户明细单号查询明细单响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504224"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionInsuranceBatch")]
public class InsuranceBatchDetailQueryResponse : WechatPayResponse
{
    /// <summary>【商家批次单号】 服务商系统内部的批次单号，在服务商系统内部唯一（官方字段路径 <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>【商家转账批次单号】 商家转账批次单号，微信商家转账系统返回的唯一标识（官方字段路径 <c>batch_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("batch_id")]
    public string? BatchId { get; set; }

    /// <summary>【商家明细单号】 服务商系统内部区分转账批次单下不同转账明细单的唯一标识（官方字段路径 <c>out_detail_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }

    /// <summary>【商家转账明细单号】 微信商家转账系统内部区分转账批次单下不同转账明细单的唯一标识（官方字段路径 <c>detail_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("detail_id")]
    public string? DetailId { get; set; }

    /// <summary>【服务商应用ID】 微信分配的商户公众账号ID（官方字段路径 <c>sp_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>【二级商户号】 二级商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【二级商户应用ID】 微信分配的商户公众账号ID（官方字段路径 <c>sub_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【转账发起时间】 转账发起的时间 使用rfc3339所定义的格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC8小时，即北京时间）。例如：2023-02-23T13:29:35+08:00表示，北京时间2023年2月23日 13点29分35秒（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【明细状态】 商家转账明细单状态。可选取值：`INIT` 系统转账校验中 / `WAIT_PAY` 待二级商户员工（转账验密人）确认, 符合免确认条件时, 系统会自动扭转为转账中 / `PROCESSING` 正在处理中，转账结果尚未明确 / `SUCCESS` 转账成功 / `FAIL` 需要确认失败原因后，再决定是否重新发起对该笔明细单的转账（并非整个转账批次单） / `BANK_REFUND` 银行返回转账成功后，拒绝入账，资金会返回商户账户。（官方字段路径 <c>detail_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("detail_state")]
    public string? DetailState { get; set; }

    /// <summary>【转账金额】 转账金额单位为“分”（官方字段路径 <c>transfer_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transfer_amount")]
    public long? TransferAmount { get; set; }

    /// <summary>【转账备注】 单条转账备注（微信用户会收到该备注），UTF8编码，最多允许32个字符（官方字段路径 <c>transfer_remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>【明细失败原因】 如果转账失败则有失败原因。可选取值：`ACCOUNT_FROZEN` 该用户账户被冻结 / `REAL_NAME_CHECK_FAIL` 收款人未实名认证，需要用户完成微信实名认证 / `NAME_NOT_CORRECT` 收款人姓名校验不通过，请核实信息 / `OPENID_INVALID` OpenID格式错误或者不属于商家公众账号 / `TRANSFER_QUOTA_EXCEED` 超过用户单笔收款额度，核实产品设置是否准确 / `DAY_RECEIVED_QUOTA_EXCEED` 超过用户单日收款额度，核实产品设置是否准确 / `MONTH_RECEIVED_QUOTA_EXCEED` 超过用户单月收款额度，核实产品设置是否准确 / `DAY_RECEIVED_COUNT_EXCEED` 超过用户单日收款次数，核实产品设置是否准确 / `PRODUCT_AUTH_CHECK_FAIL` 未开通该权限或权限被冻结，请核实产品权限状态 / `OVERDUE_CLOSE` 超过系统重试期，系统自动关闭 / `ID_CARD_NOT_CORRECT` 收款人身份证校验不通过，请核实信息 / `ACCOUNT_NOT_EXIST` 该用户账户不存在 / `TRANSFER_RISK` 该笔转账可能存在风险，已被微信拦截 / `PAYROLL_CARD_ALREADY_LOGOUT` 该用户的务工卡已经注销 / `PAYROLL_CARD_ALREADY_FROZEN` 该用户的务工卡已经被冻结 / `PAYROLL_CARD_UNAUTHORIZED` 该用户的务工卡未授权该商户 / `PAYROLL_CARD_USER_NOT_OPEN` 该用户没有开通务工卡 / `PAYROLL_CARD_NAME_CARD_NOT_MATCH` 务工卡实名和用户实名不一致 / `PAYROLL_CARD_ID_CARD_NOT_MATCH` 务工卡身份证和用户身份证不一致 / `PAYROLL_CARD_BANKCARD_UNBUNDLING` 务工卡所选银行卡已解绑 / `BANK_CARD_COLLECTIONS_ABOVE_QUOTA` 银行卡属二/三类卡，达到收款限额无法入账 / `BANK_CARD_ACCOUNT_ABNORMAL` 银行卡已被销户、冻结、作废、挂失等致无法入账 / `BANK_CARD_STATUS_ABNORMAL` 银行卡状态异常，无法入账 / `BANK_CARD_BANK_INFO_WRONG` 登记的银行名称或分支行信息有误 / `BANK_CARD_CARD_INFO_WRONG` 银行卡户名或卡号有误 / `OTHER_FAIL_REASON_TYPE` 其它失败原因 / `REALNAME_ACCOUNT_RECEIVED_QUOTA_EXCEED` 请引导用户在微信支付查看详情 / `RECEIVE_ACCOUNT_NOT_PERMMIT` 请在产品设置中调整，添加该用户为收款人 / `PAYEE_ACCOUNT_ABNORMAL` 请联系用户完善其在微信支付的身份信息以继续收款 / `PAYER_ACCOUNT_ABNORMAL` 可前往商户平台获取解除功能限制指引 / `TRANSFER_REMARK_SET_FAIL` 转账备注设置失败，请调整后重新再试 / `TRANSFER_SCENE_INVALID` 该转账场景暂不可用，请确认转账场景ID是否正确 / `TRANSFER_SCENE_UNAVAILABLE` 你尚未获取该转账场景，请确认转账场景ID是否正确 / `RECEIVE_ACCOUNT_NOT_CONFIGURE` 请前往商户平台-商家转账到零钱-前往功能-转账场景中添加 / `BLOCK_B2C_USERLIMITAMOUNT_BSRULE_MONTH` 超出用户单月转账收款20w限额，本月不支持继续向该用户付款 / `BLOCK_B2C_USERLIMITAMOUNT_MONTH` 用户账户存在风险收款受限，本月不支持继续向该用户付款 / `MERCHANT_REJECT` 商户员工（转账验密人）已驳回转账 / `MERCHANT_NOT_CONFIRM` 商户员工（转账验密人）超时未验密 / `BANK_CARD_PARAM_ERROR` 用户收款卡错误，请核实信息 / `BANK_CARD_TYPE_NOT_SUPPORTED` 用户收款卡类型非储值卡，不支持转账 / `RESERVATION_STATE_INVALID` 预约转账单状态异常，请检查 / `EXCEEDED_ESTIMATED_AMOUNT` 转账金额超过预约金额范围，请检查 / `RESERVATION_INFO_NOT_MATCH` 转账信息，如用户OpenID等参数，与预约时传入的信息不一致，请检查 / `RESERVATION_SCENE_NOT_MATCH` 该预约单的转账场景与发起转账时传入的不同，请检查（官方字段路径 <c>fail_reason</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>【收款用户OpenID】 发起转账时提供的收款用户OpenID（官方字段路径 <c>openid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【收款银行类型】 收款银行类型，采用字符串类型的银行标识。 银行标识请参考《 银行类型对照表》（官方字段路径 <c>bank_type</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>【收款用户姓名】 收款方姓名，采用标准RSA算法，公钥由微信侧提供 商户转账时传入了收款用户姓名、查询时会返回收款用户姓名（官方字段路径 <c>user_name</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>【转账完成时间】 转账完成时间，当状态为转账成功或者转账失败时返回，使用rfc3339所定义的格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC8小时，即北京时间）。例如：2023-02-23T13:29:35+08:00表示，北京时间2023年2月23日 13点29分35…（官方字段路径 <c>transfer_finish_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transfer_finish_time")]
    public string? TransferFinishTime { get; set; }

    /// <summary>【银行退票时间】 银行退票时间，当状态为时返回， 使用rfc3339所定义的格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC8小时，即北京时间）。例如：2023-02-23T13:29:35+08:00表示，北京时间2023年2月23日 13点29分35秒（官方字段路径 <c>bank_refund_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("bank_refund_time")]
    public string? BankRefundTime { get; set; }

}
