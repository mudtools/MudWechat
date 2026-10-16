// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.PayrollCard;

/// <summary>发起代发薪批量转账请求·【转账明细列表】发起批量转账的明细列表，最多一千笔。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461824"/></summary>
[HttpJsonSerializable(SerializerClassName = "PayrollCard")]
public class PayrollCardTransferBatchCreateRequestTransferDetailListItem
{
    /// <summary>【商家明细单号】商户系统内部区分转账批次单下不同转账明细单的唯一标识（官方字段路径 <c>transfer_detail_list.out_detail_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }

    /// <summary>【转账金额】转账金额单位为“分”（官方字段路径 <c>transfer_detail_list.transfer_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("transfer_amount")]
    public long? TransferAmount { get; set; }

    /// <summary>【转账备注】单条转账备注（微信用户会收到该备注），UTF8编码，最多允许32个字符（官方字段路径 <c>transfer_detail_list.transfer_remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>【收款用户OpenID】收款用户OpenID。如果转账特约商户授权类型是INFORMATION_AUTHORIZATION_TYPE，对应的是特约商户公众号下的OpenID（官方字段路径 <c>transfer_detail_list.openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【收款用户姓名】收款用户姓名。采用标准RSA算法，公钥由微信侧提供（官方字段路径 <c>transfer_detail_list.user_name</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

}

/// <summary>发起代发薪批量转账请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461824"/></summary>
[HttpJsonSerializable(SerializerClassName = "PayrollCard")]
public class PayrollCardTransferBatchCreateRequest
{
    /// <summary>【特约商户号】特约商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【特约商户AppID】微信分配的特约商户公众账号ID，特约商户授权类型为INFORMATION_AUTHORIZATION_TYPE和INFORMATION_AND_FUND_AUTHORIZATION_TYPE时 需要填写（官方字段路径 <c>sub_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【特约商户授权类型】特约商户授权类型。可选取值：`INFORMATION_AUTHORIZATION_TYPE` 表示使用特约商户用户信息，出款方服务商。自 2026 年 6 月 29 日起，该授权类型将停止支持。 / `FUND_AUTHORIZATION_TYPE` 表示使用特约商户的资金，出款方为特约商户，用户信息为服务商AppID对应的OpenID / `INFORMATION_AND_FUND_AUTHORIZATION_TYPE` 表示使用特约商户的用户信息且出款方为特约商户（官方字段路径 <c>authorization_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("authorization_type")]
    public string? AuthorizationType { get; set; }

    /// <summary>【商家批次单号】商户系统内部的商家批次单号，在商户系统内部唯一（官方字段路径 <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>【批次名称】该笔批量转账的名称（官方字段路径 <c>batch_name</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("batch_name")]
    public string? BatchName { get; set; }

    /// <summary>【批次备注】转账说明，UTF8编码，最多允许32个字符（官方字段路径 <c>batch_remark</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("batch_remark")]
    public string? BatchRemark { get; set; }

    /// <summary>【转账总金额】转账金额单位为“分”。转账总金额必须与批次内所有明细转账金额之和保持一致，否则无法发起转账操作（官方字段路径 <c>total_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>【转账总笔数】一个转账批次单最多发起一千笔转账。转账总笔数必须与批次内所有明细之和保持一致，否则无法发起转账操作（官方字段路径 <c>total_num</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_num")]
    public long? TotalNum { get; set; }

    /// <summary>【转账明细列表】发起批量转账的明细列表，最多一千笔（官方字段路径 <c>transfer_detail_list</c>，<b>必填</b>，类型 <c>array[TransferDetailInput]</c>）。</summary>
    [JsonPropertyName("transfer_detail_list")]
    public List<PayrollCardTransferBatchCreateRequestTransferDetailListItem>? TransferDetailList { get; set; }

    /// <summary>【服务商的AppID】微信分配的服务商商户公众账号ID，特约商户授权类型为FUND_AUTHORIZATION_TYPE时 需要填写（官方字段路径 <c>sp_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>【用工类型】微工卡服务仅支持用于与商户有用工关系的用户，需明确用工类型；参考值：长期用工：LONG_TERM_EMPLOYMENT，短期用工：SHORT_TERM_EMPLOYMENT，合作关系：COOPERATION_EMPLOYMENT。可选取值：`LONG_TERM_EMPLOYMENT` 长期用工：有劳务合同关系的长期用工 / `SHORT_TERM_EMPLOYMENT` 短期用工：有劳务合同关系的短期用工 / `COOPERATION_EMPLOYMENT` 合作关系：有业务服务、合作关系的人员（官方字段路径 <c>employment_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("employment_type")]
    public string? EmploymentType { get; set; }

    /// <summary>【用工场景】用工场景；参考值：LOGISTICS：物流；MANUFACTURING：制造业；HOTEL：酒店；CATERING：餐饮业；EVENT：活动促销；RETAIL：零售；OTHERS：其他。可选取值：`LOGISTICS` 物流 / `MANUFACTURING` 制造业 / `HOTEL` 酒店 / `CATERING` 餐饮业 / `EVENT` 活动促销 / `RETAIL` 零售 / `OTHERS` 其他（官方字段路径 <c>employment_scene</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("employment_scene")]
    public string? EmploymentScene { get; set; }

}

/// <summary>发起代发薪批量转账响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461824"/></summary>
[HttpJsonSerializable(SerializerClassName = "PayrollCard")]
public class PayrollCardTransferBatchCreateResponse : WechatPayResponse
{
    /// <summary>【商家批次单号】商户系统内部的商家批次单号，在商户系统内部唯一（官方字段路径 <c>out_batch_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>【微信支付批次单号】微信支付批次单号，微信商家转账系统返回的唯一标识（官方字段路径 <c>batch_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("batch_id")]
    public string? BatchId { get; set; }

    /// <summary>【批次创建时间】批次受理成功时返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

}
