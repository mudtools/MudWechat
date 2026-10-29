// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.TransferToQqWallet;

/// <summary>发起转账到QQ钱包请求·【转账场景报备信息】 各转账场景下需报备的内容，商户需要按照所属转账场景规则传参。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014297386"/></summary>
[HttpJsonSerializable(SerializerClassName = "TransferToQqWallet")]
public class TransferToQqWalletBillCreateRequestTransferSceneReportInfosItem
{
    /// <summary>【信息类型】 不能超过15个字符，商户所属转账场景下的信息类型，此字段内容为固定值，需严格按照 转账场景报备信息字段说明 传参（官方字段路径 <c>transfer_scene_report_infos.info_type</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("info_type")]
    public string? InfoType { get; set; }

    /// <summary>【信息内容】 不能超过32个字符，商户所属转账场景下的信息内容，商户可按实际业务场景自定义传参，需严格按照 转账场景报备信息字段说明 传参（官方字段路径 <c>transfer_scene_report_infos.info_content</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("info_content")]
    public string? InfoContent { get; set; }

}

/// <summary>发起转账到QQ钱包请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014297386"/></summary>
[HttpJsonSerializable(SerializerClassName = "TransferToQqWallet")]
public class TransferToQqWalletBillCreateRequest
{
    /// <summary>【商户AppID】 QQ互联开放平台为开发者的应用程序提供的唯一标识。请确保该 `appid` 与商户号有绑定关系。详见： 普通商户模式开发必要参数说明（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【商户单号】 商户系统内部的商家单号，要求此参数只能由数字、大小写字母组成，在商户系统内部唯一（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>【转账场景ID】 该笔转账使用的转账场景，可前往“商户平台-产品中心-商家转账”中申请。如：1401（活动奖励）等（官方字段路径 <c>transfer_scene_id</c>，<b>必填</b>，类型 <c>string(36)</c>）。</summary>
    [JsonPropertyName("transfer_scene_id")]
    public string? TransferSceneId { get; set; }

    /// <summary>【收款用户OpenID】 收款用户在商户appid下的唯一标识。发起转账前需获取到用户的 `OpenID`，获取方式详见 参数说明。与收款用户QQ号二选一必填（官方字段路径 <c>openid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【收款用户QQ号】 收款用户QQ号。与收款用户 `OpenID` 二选一必填（官方字段路径 <c>user_qq</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("user_qq")]
    public string? UserQq { get; set; }

    /// <summary>【收款用户姓名】 需要加密传入，支持标准RSA算法和国密算法，公钥由微信侧提供。 转账金额 &gt;= 2,000元时，该笔明细必须填写 若商户传入收款用户姓名，微信支付会校验收款用户与输入姓名是否一致（官方字段路径 <c>user_name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>【转账金额】 单位为“分”（官方字段路径 <c>transfer_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transfer_amount")]
    public long? TransferAmount { get; set; }

    /// <summary>【转账备注】 用户收款时可见该备注信息，UTF8编码，最多允许32个字符（官方字段路径 <c>transfer_remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>【通知地址】 异步接收商家充值结果的回调地址，通知url必须为公网可访问的URL，必须为HTTPS，不能携带参数（官方字段路径 <c>notify_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【转账场景报备信息】 各转账场景下需报备的内容，商户需要按照所属转账场景规则传参，详见 产品文档 （官方字段路径 <c>transfer_scene_report_infos</c>，<b>必填</b>，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("transfer_scene_report_infos")]
    public List<TransferToQqWalletBillCreateRequestTransferSceneReportInfosItem>? TransferSceneReportInfos { get; set; }

}

/// <summary>发起转账到QQ钱包响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014297386"/></summary>
[HttpJsonSerializable(SerializerClassName = "TransferToQqWallet")]
public class TransferToQqWalletBillCreateResponse : WechatPayResponse
{
    /// <summary>【商户号】 微信支付分配的商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【商户转账单号】 商户系统生成的转账单号，只能由数字、大小写字母组成，在商户系统内部唯一（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>【微信支付转账单号】 微信支付生成的转账单号，转账单的唯一标识（官方字段路径 <c>transfer_bill_no</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transfer_bill_no")]
    public string? TransferBillNo { get; set; }

    /// <summary>【转账状态】 商家转账订单状态。可选取值：`ACCEPTED` 转账已受理 / `PROCESSING` 转账锁定资金中。如果一直停留在该状态，建议检查账户余额是否足够，如余额不足，可充值后再原单重试。 / `WAIT_USER_CONFIRM` 待收款用户确认，收款用户可在QQ客户端确认收款 / `TRANSFERING` 转账中，收款用户可在QQ客户端再次重试确认收款 / `SUCCESS` 转账成功（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【单据创建时间】 单据受理成功时返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

}

/// <summary>转账到QQ钱包结果查询响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014297440"/></summary>
[HttpJsonSerializable(SerializerClassName = "TransferToQqWallet")]
public class TransferToQqWalletBillQueryResponse : WechatPayResponse
{
    /// <summary>【商户号】 微信支付分配的商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【商户转账单号】 商户系统生成的转账单号，只能由数字、大小写字母组成，在商户系统内部唯一（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>【微信支付转账单号】 微信支付生成的转账单号，转账单的唯一标识（官方字段路径 <c>transfer_bill_no</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transfer_bill_no")]
    public string? TransferBillNo { get; set; }

    /// <summary>【商户AppID】 QQ互联开放平台为开发者的应用程序提供的唯一标识（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【转账状态】 商家转账订单状态。可选取值：`ACCEPTED` 转账已受理 / `PROCESSING` 转账锁定资金中。如果一直停留在该状态，建议检查账户余额是否足够，如余额不足，可充值后再原单重试。 / `WAIT_USER_CONFIRM` 待收款用户确认，收款用户可在QQ客户端确认收款 / `TRANSFERING` 转账中，收款用户可在QQ客户端再次重试确认收款 / `SUCCESS` 转账成功 / `FAIL` 转账失败 / `CANCELING` 商户撤销请求受理成功，该笔转账正在撤销中 / `CANCELLED` 转账撤销完成（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【转账金额】 单位为“分”（官方字段路径 <c>transfer_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transfer_amount")]
    public long? TransferAmount { get; set; }

    /// <summary>【转账备注】 转账备注（收款用户会收到该备注），UTF8编码，最多允许32个字符（官方字段路径 <c>transfer_remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>【转账失败原因】 转账失败时，会返回 失败原因（官方字段路径 <c>fail_reason</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>【收款用户OpenID】 收款用户在商户appid下的唯一标识。与收款用户QQ号二选一（官方字段路径 <c>openid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【收款用户QQ号】 收款用户QQ号。与收款用户OpenID二选一（官方字段路径 <c>user_qq</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("user_qq")]
    public string? UserQq { get; set; }

    /// <summary>【收款用户姓名】 收款方真实姓名，支持标准RSA算法和国密算法，公钥由微信支付侧提供。转账金额 &gt;= 2,000元时，该笔转账必须填写（官方字段路径 <c>user_name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>【单据创建时间】 单据受理成功时返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【最后一次状态变更时间】 单据最后更新时间，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>update_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

}

/// <summary>撤销转账到QQ钱包响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014297396"/></summary>
[HttpJsonSerializable(SerializerClassName = "TransferToQqWallet")]
public class TransferToQqWalletBillCancelResponse : WechatPayResponse
{
    /// <summary>【商户号】 微信支付分配的商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【商户转账单号】 商户系统生成的转账单号，只能由数字、大小写字母组成，在商户系统内部唯一（官方字段路径 <c>out_bill_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>【微信支付转账单号】 微信支付生成的转账单号，转账单的唯一标识（官方字段路径 <c>transfer_bill_no</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("transfer_bill_no")]
    public string? TransferBillNo { get; set; }

    /// <summary>【转账状态】 商家转账订单状态。可选取值：`CANCELING` 商户撤销请求受理成功，该笔转账正在撤销中 / `CANCELLED` 转账撤销完成（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【最后一次状态变更时间】 单据最后更新时间，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>update_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

}
