// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 商家转账到零钱请求体（<c>/promotion/transfers</c>，需商户证书）。
/// </summary>
/// <remarks>
/// <para><b>字段名陷阱</b>：本端点用 <c>mch_appid</c> + <c>mchid</c>（<b>不是</b> <c>appid</c> / <c>mch_id</c>），
/// 而配套的查询端点 <c>/promotion/gettransferinfo</c> 又回到 <c>appid</c> + <c>mch_id</c>。
/// 官方两页即如此不对称，照抄不得「统一」—— 两族 DTO 因此<b>不共用</b>字段名。</para>
/// <para><b>频率</b>：官方 100 qps（同一笔重试间隔 ≥ 1 秒）；<b>金额</b>：单笔 ≥ 100 分（未实名用户 ≥ 300 分）、
/// 单笔上限 10 万元，越界官方回 <c>AMOUNT_LIMIT</c>。</para>
/// <para><b>前置</b>：调用方出口 IP <b>须在商户平台白名单内</b>，且本族只认 <c>apiclient_cert.p12</c>
/// 双向证书（<b>不支持</b>公钥证书形态）。</para>
/// <para><b>幂等</b>：<c>partner_trade_no</c> 唯一，重试沿用原单号。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2TransferToBalanceRequest
{
    /// <summary>商户 AppID（<c>mch_appid</c>，必填 string(64)）：注意本页参数名即 <c>mch_appid</c>。</summary>
    [JsonPropertyName("mch_appid")] public string? MchAppId { get; set; }

    /// <summary>商户号（<c>mchid</c>，必填 string(32)）：注意本页参数名即 <c>mchid</c>（无下划线）。</summary>
    [JsonPropertyName("mchid")] public string? MchId { get; set; }

    /// <summary>子商户号（<c>sub_mch_id</c>，选填 string(10)）：服务商模式。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>商户订单号（<c>partner_trade_no</c>，必填 string(64)）：商户系统内唯一，重试不得更换。</summary>
    [JsonPropertyName("partner_trade_no")] public string? PartnerTradeNo { get; set; }

    /// <summary>收款用户 openid（<c>openid</c>，必填 string(64)）：须与 <c>mch_appid</c> 同一应用下。</summary>
    [JsonPropertyName("openid")] public string? OpenId { get; set; }

    /// <summary>校验真实姓名选项（<c>check_name</c>，必填 string(16)）：<c>OPTION_NAME</c> 需校验 / <c>NO_CHECK</c> 不校验。</summary>
    [JsonPropertyName("check_name")] public string? CheckName { get; set; }

    /// <summary>收款用户真实姓名（<c>transfer_name</c>，条件必填 string(32)）：<c>check_name=OPTION_NAME</c> 时必填。</summary>
    [JsonPropertyName("transfer_name")] public string? TransferName { get; set; }

    /// <summary>转账金额（<c>amount</c>，必填 int）：单位为分。</summary>
    [JsonPropertyName("amount")] public int? Amount { get; set; }

    /// <summary>转账备注（<c>description</c>，必填 string(127)）：用户收款凭据展示。</summary>
    [JsonPropertyName("description")] public string? Description { get; set; }

    /// <summary>场景 ID（<c>scene_id</c>，选填 string(64)）：需在商户平台「商家转账到零钱」配置。</summary>
    [JsonPropertyName("scene_id")] public string? SceneId { get; set; }

    /// <summary>品牌 ID（<c>brand_id</c>，选填 string(64)）：品牌商户使用。</summary>
    [JsonPropertyName("brand_id")] public string? BrandId { get; set; }

    /// <summary>视频号模板 ID（<c>finder_template_id</c>，选填 string(64)）：视频号场景使用。</summary>
    [JsonPropertyName("finder_template_id")] public string? FinderTemplateId { get; set; }

    /// <summary>风控信息（<c>risk_info</c>，选填 string：JSON 串）：含用户身份字段，属敏感信息 ⇒ SDK 不落日志。</summary>
    [JsonPropertyName("risk_info")] public string? RiskInfo { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>，必填 string(32)）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>）：由证书通道自动填入。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 商家转账到零钱应答。
/// </summary>
/// <remarks><b><c>payment_no</c> 才是微信侧单号</b>：本族没有 <c>transaction_id</c> 概念。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2TransferToBalanceResponse
{
    /// <summary>通信标识（<c>return_code</c>）。</summary>
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    /// <summary>返回信息（<c>return_msg</c>）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>业务结果（<c>result_code</c>）。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    /// <summary>错误代码（<c>err_code</c>）。</summary>
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }

    /// <summary>错误代码描述（<c>err_code_des</c>）。</summary>
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }

    /// <summary>商户 AppID（<c>mch_appid</c>）。</summary>
    [JsonPropertyName("mch_appid")] public string? MchAppId { get; set; }

    /// <summary>商户号（<c>mchid</c>）。</summary>
    [JsonPropertyName("mchid")] public string? MchId { get; set; }

    /// <summary>子商户号（<c>sub_mch_id</c>）。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>商户订单号（<c>partner_trade_no</c>）。</summary>
    [JsonPropertyName("partner_trade_no")] public string? PartnerTradeNo { get; set; }

    /// <summary>微信付款单号（<c>payment_no</c>）：转账成功后返回，查询与对账以此或 <c>partner_trade_no</c> 为准。</summary>
    [JsonPropertyName("payment_no")] public string? PaymentNo { get; set; }

    /// <summary>付款成功时间（<c>payment_time</c>）：格式 <c>yyyyMMddHHmmss</c>。</summary>
    [JsonPropertyName("payment_time")] public string? PaymentTime { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 查询商家转账到零钱请求体（<c>/promotion/gettransferinfo</c>，需商户证书）。
/// </summary>
/// <remarks>
/// <para><b>只可查 30 天内</b>的转账单；本端点参数名回到 <c>appid</c> / <c>mch_id</c>
/// （与发放两支的 <c>mch_appid</c> / <c>mchid</c> 不对称，官方即如此）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2GetTransferInfoRequest
{
    /// <summary>商户 AppID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }

    /// <summary>商户号（<c>mch_id</c>，必填 string(10)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>商户订单号（<c>partner_trade_no</c>，必填 string(64)）。</summary>
    [JsonPropertyName("partner_trade_no")] public string? PartnerTradeNo { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>，必填 string(32)）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>）：由证书通道自动填入。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 查询商家转账到零钱应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2GetTransferInfoResponse
{
    /// <summary>通信标识（<c>return_code</c>）。</summary>
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    /// <summary>返回信息（<c>return_msg</c>）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>业务结果（<c>result_code</c>）。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    /// <summary>错误代码（<c>err_code</c>）。</summary>
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }

    /// <summary>错误代码描述（<c>err_code_des</c>）。</summary>
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }

    /// <summary>商户订单号（<c>partner_trade_no</c>）。</summary>
    [JsonPropertyName("partner_trade_no")] public string? PartnerTradeNo { get; set; }

    /// <summary>微信明细单号（<c>detail_id</c>）。</summary>
    [JsonPropertyName("detail_id")] public string? DetailId { get; set; }

    /// <summary>转账状态（<c>status</c>）：<c>SUCCESS</c> / <c>FAILED</c> / <c>PROCESSING</c>。</summary>
    [JsonPropertyName("status")] public string? Status { get; set; }

    /// <summary>收款用户姓名（<c>transfer_name</c>）：脱敏返回。</summary>
    [JsonPropertyName("transfer_name")] public string? TransferName { get; set; }

    /// <summary>转账金额（<c>payment_amount</c>）：单位为分。</summary>
    [JsonPropertyName("payment_amount")] public int? PaymentAmount { get; set; }

    /// <summary>转账时间（<c>transfer_time</c>）：格式 <c>yyyyMMddHHmmss</c>。</summary>
    [JsonPropertyName("transfer_time")] public string? TransferTime { get; set; }

    /// <summary>付款成功时间（<c>payment_time</c>）：格式 <c>yyyyMMddHHmmss</c>。</summary>
    [JsonPropertyName("payment_time")] public string? PaymentTime { get; set; }

    /// <summary>转账备注（<c>desc</c>）。</summary>
    [JsonPropertyName("desc")] public string? Desc { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 向企业微信成员付款请求体（<c>/promotion/paywwsptrans2pocket</c>，需商户证书 + <c>workwx_sign</c>）。
/// </summary>
/// <remarks>
/// <para><b>双签</b>：<c>workwx_sign</c> 以 <c>agentid</c> 对应自建应用 Secret 对官方<b>固定白名单</b>
/// （<c>amount, appid, desc, mch_id, nonce_str, openid, partner_trade_no, ww_msg_type</c>）
/// 取 <c>MD5(stringA + "secret=" + Secret)</c>，且<b>参与</b> <c>sign</c>；口径见
/// <c>WechatPayV2WorkWxSigner</c>。</para>
/// <para><b>官方限制</b>：单笔单日 <b>200 元</b>、商户单日 <b>10 万元</b>、每用户每日 <b>10 次</b>、
/// 同一用户两次领取间隔 <b>≥ 15 秒</b>。官方本页另注「向员工付款」功能<b>暂不支持新增</b> ⇒
/// 未开通该能力的商户调用会被拒，接入前先与微信侧确认资格。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PayWorkWxPocketRequest
{
    /// <summary>商户 AppID（<c>appid</c>，必填 string(32)）：参与 <c>workwx_sign</c> 白名单。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }

    /// <summary>商户号（<c>mch_id</c>，必填 string(10)）：参与 <c>workwx_sign</c> 白名单。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>商户订单号（<c>partner_trade_no</c>，必填 string(64)）：重试不得更换。</summary>
    [JsonPropertyName("partner_trade_no")] public string? PartnerTradeNo { get; set; }

    /// <summary>收款用户 openid（<c>openid</c>，必填 string(64)）：企业微信成员 openid。</summary>
    [JsonPropertyName("openid")] public string? OpenId { get; set; }

    /// <summary>付款金额（<c>amount</c>，必填 int）：单位为分，参与 <c>workwx_sign</c> 白名单。</summary>
    [JsonPropertyName("amount")] public int? Amount { get; set; }

    /// <summary>付款描述（<c>desc</c>，必填 string(81)）：参与 <c>workwx_sign</c> 白名单；注意本页长度上限是 81。</summary>
    [JsonPropertyName("desc")] public string? Desc { get; set; }

    /// <summary>企业消息类型（<c>ww_msg_type</c>，必填 string(16)）：<c>NORMAL_MSG</c> / <c>APPROVAL_MSG</c>，参与白名单。</summary>
    [JsonPropertyName("ww_msg_type")] public string? WwMsgType { get; set; }

    /// <summary>审批单号（<c>approval_number</c>，条件必填 string(64)）：<c>ww_msg_type=APPROVAL_MSG</c> 时必填。</summary>
    [JsonPropertyName("approval_number")] public string? ApprovalNumber { get; set; }

    /// <summary>审批类型（<c>approval_type</c>，选填 string(32)）：审批消息场景使用。</summary>
    [JsonPropertyName("approval_type")] public string? ApprovalType { get; set; }

    /// <summary>活动名称（<c>act_name</c>，选填 string(32)）：<b>不参与</b> <c>workwx_sign</c> 白名单。</summary>
    [JsonPropertyName("act_name")] public string? ActName { get; set; }

    /// <summary>企业微信应用 agentid（<c>agentid</c>，选填 string(16)）：决定 <c>workwx_sign</c> 取哪个应用的 Secret。</summary>
    [JsonPropertyName("agentid")] public string? AgentId { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>，必填 string(32)）：参与 <c>workwx_sign</c> 白名单。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>）：由证书通道自动填入。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }

    /// <summary>企业微信签名（<c>workwx_sign</c>）：由证书通道自动填入，宿主不要手工赋值。</summary>
    [JsonPropertyName("workwx_sign")] public string? WorkWxSign { get; set; }
}

/// <summary>
/// APIv2 向企业微信成员付款应答。
/// </summary>
/// <remarks><b>官方本页应答不带任何签名字段</b>（既无 <c>sign</c> 也无 <c>workwx_sign</c>）⇒
/// 证书通道对该报文自然跳过验签，属官方契约而非 SDK 漏验。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PayWorkWxPocketResponse
{
    /// <summary>通信标识（<c>return_code</c>）。</summary>
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    /// <summary>返回信息（<c>return_msg</c>）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>业务结果（<c>result_code</c>）。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    /// <summary>错误代码（<c>err_code</c>）。</summary>
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }

    /// <summary>错误代码描述（<c>err_code_des</c>）。</summary>
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }

    /// <summary>商户订单号（<c>partner_trade_no</c>）。</summary>
    [JsonPropertyName("partner_trade_no")] public string? PartnerTradeNo { get; set; }

    /// <summary>微信单号（<c>payment_no</c>）。</summary>
    [JsonPropertyName("payment_no")] public string? PaymentNo { get; set; }

    /// <summary>付款时间（<c>payment_time</c>）：格式 <c>yyyyMMddHHmmss</c>。</summary>
    [JsonPropertyName("payment_time")] public string? PaymentTime { get; set; }
}

/// <summary>
/// APIv2 查询企业微信付款记录请求体（<c>/promotion/querywwsptrans2pocket</c>，需商户证书）。
/// </summary>
/// <remarks>查询族官方<b>不要求</b> <c>workwx_sign</c> ⇒ 本 DTO 不带该字段。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2QueryWorkWxPocketRequest
{
    /// <summary>商户 AppID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }

    /// <summary>商户号（<c>mch_id</c>，必填 string(10)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>商户订单号（<c>partner_trade_no</c>，必填 string(64)）。</summary>
    [JsonPropertyName("partner_trade_no")] public string? PartnerTradeNo { get; set; }

    /// <summary>收款用户 openid（<c>openid</c>，必填 string(64)）。</summary>
    [JsonPropertyName("openid")] public string? OpenId { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>，必填 string(32)）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>）：由证书通道自动填入。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 查询企业微信付款记录应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2QueryWorkWxPocketResponse
{
    /// <summary>通信标识（<c>return_code</c>）。</summary>
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    /// <summary>返回信息（<c>return_msg</c>）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>业务结果（<c>result_code</c>）。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    /// <summary>错误代码（<c>err_code</c>）。</summary>
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }

    /// <summary>错误代码描述（<c>err_code_des</c>）。</summary>
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }

    /// <summary>商户订单号（<c>partner_trade_no</c>）。</summary>
    [JsonPropertyName("partner_trade_no")] public string? PartnerTradeNo { get; set; }

    /// <summary>付款状态（<c>status</c>）：官方取值含未领取 / 已领取 / 已退回等。</summary>
    [JsonPropertyName("status")] public string? Status { get; set; }

    /// <summary>付款金额（<c>amount</c>）：单位为分。</summary>
    [JsonPropertyName("amount")] public int? Amount { get; set; }

    /// <summary>付款时间（<c>transfer_time</c>）：格式 <c>yyyyMMddHHmmss</c>。</summary>
    [JsonPropertyName("transfer_time")] public string? TransferTime { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}
