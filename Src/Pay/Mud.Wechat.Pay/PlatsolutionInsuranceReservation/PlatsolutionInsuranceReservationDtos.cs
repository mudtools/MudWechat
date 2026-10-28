// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.PlatsolutionInsuranceReservation;

/// <summary>受理单次预约商家转账请求·【场景报备信息】 特殊的转账场景，当转账金额大于场景规定的金额时，需要传入场景对·【保险保单报备信息】 保险理赔、保险取现等场景需要报备的字段。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504216"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionInsuranceReservation")]
public class InsuranceReservationApplyRequestSceneReportDataInsurancePolicy
{
    /// <summary>【保险产品备案编号】 保险产品备案编号（官方字段路径 <c>scene_report_data.insurance_policy.product_registration_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("product_registration_no")]
    public string? ProductRegistrationNo { get; set; }

    /// <summary>【保险单号】 保险单号（官方字段路径 <c>scene_report_data.insurance_policy.policy_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("policy_no")]
    public string? PolicyNo { get; set; }

    /// <summary>【保险名称】 保险名称（官方字段路径 <c>scene_report_data.insurance_policy.name</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【取现单号】 用户取现单号（官方字段路径 <c>scene_report_data.insurance_policy.withdrawal_no</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("withdrawal_no")]
    public string? WithdrawalNo { get; set; }

    /// <summary>【理赔单号】 理赔单号（官方字段路径 <c>scene_report_data.insurance_policy.claim_no</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("claim_no")]
    public string? ClaimNo { get; set; }

}

/// <summary>受理单次预约商家转账请求·【场景报备信息】 特殊的转账场景，当转账金额大于场景规定的金额时，需要传入场景对。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504216"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionInsuranceReservation")]
public class InsuranceReservationApplyRequestSceneReportData
{
    /// <summary>【保险保单报备信息】 保险理赔、保险取现等场景需要报备的字段（官方字段路径 <c>scene_report_data.insurance_policy</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("insurance_policy")]
    public InsuranceReservationApplyRequestSceneReportDataInsurancePolicy? InsurancePolicy { get; set; }

}

/// <summary>受理单次预约商家转账请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504216"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionInsuranceReservation")]
public class InsuranceReservationApplyRequest
{
    /// <summary>【二级商户号】 微信支付分配的商户号，出资商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【服务商AppID】 服务商绑定的公众号ID（官方字段路径 <c>sp_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>【二级商户AppID】 二级商户授权服务商绑定的公众号ID。若填写了该字段，收款用户OpenID必须对应二级商户AppID（官方字段路径 <c>sub_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【商户预约单号】 商户系统内部的预约商家转账单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方字段路径 <c>out_reservation_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_reservation_no")]
    public string? OutReservationNo { get; set; }

    /// <summary>【转账场景ID】 该笔转账使用的转账场景，可在「平台商家转账 \- 产品设置」中查看已获得场景和申请新场景（官方字段路径 <c>transfer_scene_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("transfer_scene_id")]
    public string? TransferSceneId { get; set; }

    /// <summary>【预计最晚转账日期】 需在发起预约当日的31天内，预约当日为第1天，最晚可预约第31天。遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>estimated_date</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("estimated_date")]
    public string? EstimatedDate { get; set; }

    /// <summary>【预计最大转账金额】 预计最大转账金额单位为“分”。若传入该字段，用户确认时不会展示该金额，实际转账时可传入的金额可小于等于预计最大转账金额。最大金额和准确金额中必须传入其中一个（官方字段路径 <c>estimated_max_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("estimated_max_amount")]
    public long? EstimatedMaxAmount { get; set; }

    /// <summary>【准确转账金额】 准确转账金额单位为“分”。若传入该金额，用户确认时会展示，且实际转账时传入的金额必须等于准确转账金额。最大金额和准确金额中必须传入其中一个（官方字段路径 <c>exact_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exact_amount")]
    public long? ExactAmount { get; set; }

    /// <summary>【收款用户OpenID】 商户AppID下用户的唯一标识。若填写了二级商户AppID，则收款用户OpenID必须对应二级商户AppID。否则需要对应服务商AppID（官方字段路径 <c>openid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【收款用户姓名】 经过加密后的收款用户姓名信息。 预计转账金额 &gt;= 2,000元时，必须填写用户姓名 若商户传入收款用户姓名，微信支付会校验收款用户的微信支付实名与姓名是否一致，并提供电子回单（官方字段路径 <c>user_real_name</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("user_real_name")]
    public string? UserRealName { get; set; }

    /// <summary>【预约备注】 将在预约页面对用户展示，商户可描述与预约时间、预约金额等相关的信息说明（官方字段路径 <c>reserve_remark</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("reserve_remark")]
    public string? ReserveRemark { get; set; }

    /// <summary>【商户回调地址】 异步接收用户确认预约结果的回调通知地址，通知URL必须为公网可访问的URL，必须为HTTPS，不能携带参数（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【场景报备信息】 特殊的转账场景，当转账金额大于场景规定的金额时，需要传入场景对应业务字段进行报备。具体字段及约束详情可查看产品指引文档（官方字段路径 <c>scene_report_data</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("scene_report_data")]
    public InsuranceReservationApplyRequestSceneReportData? SceneReportData { get; set; }

}

/// <summary>受理单次预约商家转账响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504216"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionInsuranceReservation")]
public class InsuranceReservationApplyResponse : WechatPayResponse
{
    /// <summary>【二级商户号】 微信支付分配的商户号，出资商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【商户预约单号】 商户系统内部的预约商家转账单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方字段路径 <c>out_reservation_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_reservation_no")]
    public string? OutReservationNo { get; set; }

    /// <summary>【微信转账预约单号】 微信商家转账系统返回的预约记录唯一标识（官方字段路径 <c>reservation_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("reservation_id")]
    public string? ReservationId { get; set; }

    /// <summary>【预约记录受理时间】 单据受理成功时会返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>accept_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("accept_time")]
    public string? AcceptTime { get; set; }

    /// <summary>【预约记录状态】 预约记录状态，受理成功的记录状态只有ACCEPTED。可选取值：`ACCEPTED` 预约转账已受理，待用户确认预约 / `CONFIRMED` 用户已确认预约，且未超过预计最晚转账日期 / `CLOSED` 商户主动关闭，或者系统关闭（用户未及时确认、转账完成、超期未转账等原因）。（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【跳转微信支付用户确认预约页的package信息】 单据受理成功时会返回，跳转微信支付用户确认预约页的package信息，该数据在24小时内有效（官方字段路径 <c>package_info</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("package_info")]
    public string? PackageInfo { get; set; }

}

/// <summary>商户预约单号查询预约商家转账记录响应·【预约记录的关闭信息】 当状态为已关闭时返回，预约记录的关闭信息，如关闭原因等。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504218"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionInsuranceReservation")]
public class InsuranceReservationQueryResponseCloseInfo
{
    /// <summary>【预约记录关闭的时间】 当状态为已关闭时返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>close_info.close_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("close_time")]
    public string? CloseTime { get; set; }

    /// <summary>【预约记录关闭的原因】 当状态为已关闭时返回。可选取值：`USER_OVERDUE_UNCONFIRMED` 用户超时未确认，系统自动关闭 / `CLOSE_CONFIRMED_VIA_MCH_API` 商户通过API主动关闭已预约的记录 / `CLOSE_ACCEPTED_VIA_MCH_API` 商户通过API主动关闭已受理的记录 / `TRANSFER_FINISHED` 转账成功后，系统自动关闭 / `OVERDUE_NOT_TRANSFER` 商户在预计最晚转账日期之前，未发起或发起转账但转账失败，系统自动关闭。 / `RESERVE_FAIL` 因业务规则限制或者预约流程不合法，系统自动关闭 / `USER_CLOSE` 用户确认后，主动关闭预约（官方字段路径 <c>close_info.close_reason</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("close_reason")]
    public string? CloseReason { get; set; }

}

/// <summary>商户预约单号查询预约商家转账记录响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504218"/></summary>
[HttpJsonSerializable(SerializerClassName = "PlatsolutionInsuranceReservation")]
public class InsuranceReservationQueryResponse : WechatPayResponse
{
    /// <summary>【服务商商户号】 微信支付分配的商户号（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【二级商户号】 微信支付分配的商户号，出资商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【服务商AppID】 服务商的公众号ID（官方字段路径 <c>sp_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>【二级商户AppID】 二级商户授权服务商绑定的公众号ID（官方字段路径 <c>sub_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【微信转账预约单号】 微信商家转账系统返回的预约记录唯一标识（官方字段路径 <c>reservation_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("reservation_id")]
    public string? ReservationId { get; set; }

    /// <summary>【商户预约单号】 商户系统内部的预约商家转账单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方字段路径 <c>out_reservation_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_reservation_no")]
    public string? OutReservationNo { get; set; }

    /// <summary>【转账场景ID】 该笔转账使用的转账场景（官方字段路径 <c>transfer_scene_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("transfer_scene_id")]
    public string? TransferSceneId { get; set; }

    /// <summary>【预计最晚转账日期】 商户申请预约时传入的预计最晚转账日期。遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>estimated_date</c>，选填，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("estimated_date")]
    public string? EstimatedDate { get; set; }

    /// <summary>【预计最大转账金额】 商户申请预约时传入的预计最大转账金额，单位为“分”。最大金额和准确金额中有且只返回其中一个（官方字段路径 <c>estimated_max_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("estimated_max_amount")]
    public long? EstimatedMaxAmount { get; set; }

    /// <summary>【准确转账金额】 商户申请预约时传入的准确转账金额，单位为“分”。最大金额和准确金额中有且只返回其中一个（官方字段路径 <c>exact_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("exact_amount")]
    public long? ExactAmount { get; set; }

    /// <summary>【收款用户OpenID】 商户申请预约时传入的OpenID（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【收款银行类型】 收款银行类型，采用字符串类型的银行标识。 银行标识请参考《 银行类型对照表》（官方字段路径 <c>bank_type</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>【预约记录状态】 预约记录状态。可选取值：`ACCEPTED` 预约转账已受理，待用户确认预约 / `CONFIRMED` 用户已确认预约，且未超过预计最晚转账日期 / `CLOSED` 商户主动关闭，或者系统关闭（用户未及时确认、转账完成、超期未转账等原因）。（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【预约记录受理的时间】 预约记录受理成功时返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>accept_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("accept_time")]
    public string? AcceptTime { get; set; }

    /// <summary>【预约记录用户确认预约的时间】 当状态为已预约时返回，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>reserve_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("reserve_time")]
    public string? ReserveTime { get; set; }

    /// <summary>【预约记录的关闭信息】 当状态为已关闭时返回，预约记录的关闭信息，如关闭原因等（官方字段路径 <c>close_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("close_info")]
    public InsuranceReservationQueryResponseCloseInfo? CloseInfo { get; set; }

}
