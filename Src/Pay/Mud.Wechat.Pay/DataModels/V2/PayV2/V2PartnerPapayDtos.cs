// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

// 服务商（特约商户）侧委托代扣族。与商户侧同名的端点<b>路径亦相同</b>
// （解约官方 path 就是 /papay/deletecontract，无 /papay/partner/ 段），
// 差异只在字段面多了 sub_appid / sub_mch_id（与个别风控参数）⇒ DTO 不共用，路径共用。

/// <summary>
/// APIv2 服务商解除委托代扣签约协议请求体（<b>POST</b> <c>/papay/deletecontract</c>）。
/// </summary>
/// <remarks><b>两种解约模式</b>：<c>contract_id</c> 单键，或 <c>plan_id</c> + <c>contract_code</c> 组合键。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PartnerDeleteContractRequest
{
    /// <summary>应用 ID（<c>appid</c>，必填 string(32)）：服务商申请的公众号或移动应用 appid。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    /// <summary>服务商商户号（<c>mch_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    /// <summary>子商户绑定的 appid（<c>sub_appid</c>，选填 string(32)）：需服务商在商户平台为子商户绑定。</summary>
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    /// <summary>子商户号（<c>sub_mch_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    /// <summary>委托代扣协议 ID（<c>contract_id</c>，条件必填 string(32)）：contract_id 模式必填。</summary>
    [JsonPropertyName("contract_id")] public string? ContractId { get; set; }
    /// <summary>代扣模板 ID（<c>plan_id</c>，条件必填 string(128)）：plan_id+contract_code 模式必填。</summary>
    [JsonPropertyName("plan_id")] public string? PlanId { get; set; }
    /// <summary>签约协议号（<c>contract_code</c>，条件必填 string(128)）：商户侧须唯一。</summary>
    [JsonPropertyName("contract_code")] public string? ContractCode { get; set; }
    /// <summary>解约原因备注（<c>contract_termination_remark</c>，必填 string(512)）。</summary>
    [JsonPropertyName("contract_termination_remark")] public string? ContractTerminationRemark { get; set; }
    /// <summary>版本（<c>version</c>，必填 string(8)）：固定值 <c>1.0</c>。</summary>
    [JsonPropertyName("version")] public string? Version { get; set; }
    /// <summary>签名（<c>sign</c>）：由传输层签名 Handler 补写，调用方留空。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 服务商解除委托代扣签约协议应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PartnerDeleteContractResponse
{
    /// <summary>通信标识（<c>return_code</c>）：非交易标识。</summary>
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    /// <summary>返回信息（<c>return_msg</c>）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    /// <summary>业务结果（<c>result_code</c>）。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    /// <summary>错误码（<c>err_code</c>）。</summary>
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    /// <summary>错误码描述（<c>err_code_des</c>）。</summary>
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
    /// <summary>服务商 appid（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    /// <summary>服务商商户号（<c>mch_id</c>）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    /// <summary>子商户绑定的 appid（<c>sub_appid</c>）。</summary>
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    /// <summary>子商户号（<c>sub_mch_id</c>）。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    /// <summary>委托代扣协议 ID（<c>contract_id</c>）。</summary>
    [JsonPropertyName("contract_id")] public string? ContractId { get; set; }
    /// <summary>协议模板 ID（<c>plan_id</c>）。</summary>
    [JsonPropertyName("plan_id")] public string? PlanId { get; set; }
    /// <summary>签约协议号（<c>contract_code</c>）。</summary>
    [JsonPropertyName("contract_code")] public string? ContractCode { get; set; }
    /// <summary>签名（<c>sign</c>）：由传输层自动验签。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 服务商 H5 预签约应答。
/// </summary>
/// <remarks>
/// <para><b>入参不走本 DTO</b>：官方该端点是 <b>GET</b> ⇒ 接口方法以 <c>[Query]</c> 逐参声明。</para>
/// <para><b>风控参数面比商户侧宽</b>（<c>deviceid</c>/<c>mobile</c>/<c>email</c>/<c>qq</c>/
/// <c>openid</c>/<c>creid</c>/<c>outerid</c> 仅服务商页给出）：这些是<b>用户身份信息</b>，
/// 本 SDK 不记录其值，宿主须自行确保收集与传输已合法征得用户授权。</para>
/// <para><b>权限前提</b>：官方注明该接口暂未全面开放，需单独申请。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PartnerH5EntrustWebResponse
{
    /// <summary>通信标识（<c>return_code</c>）：非签约结果标识，签约发起是否成功看 <c>result_code</c>。</summary>
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    /// <summary>返回信息（<c>return_msg</c>）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    /// <summary>业务结果（<c>result_code</c>）。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    /// <summary>结果信息（<c>result_msg</c>）：非空即错误原因，如签名错误。</summary>
    [JsonPropertyName("result_msg")] public string? ResultMsg { get; set; }
    /// <summary>跳转签约页面 URL（<c>redirect_url</c>）：发起请求的页面地址须在微信后台（申请 H5 签约权限时）配置。</summary>
    [JsonPropertyName("redirect_url")] public string? RedirectUrl { get; set; }
}

/// <summary>
/// APIv2 服务商 APP 预签约请求体（<b>POST</b> <c>/papay/partner/preentrustweb</c>）。
/// </summary>
/// <remarks><c>sign_type</c> 缺省时官方本页默认 <b>MD5</b>（H5 族默认 HMAC-SHA256，两页不同，照抄不统一）。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PartnerPreEntrustWebRequest
{
    /// <summary>应用 ID（<c>appid</c>，必填 string(32)）：服务商申请的公众号或移动应用 appid。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    /// <summary>服务商商户号（<c>mch_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    /// <summary>子商户绑定的 appid（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    /// <summary>子商户号（<c>sub_mch_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    /// <summary>协议模板 ID（<c>plan_id</c>，必填 string(28)）。</summary>
    [JsonPropertyName("plan_id")] public string? PlanId { get; set; }
    /// <summary>商户侧签约协议号（<c>contract_code</c>，必填 string(128)）：数字与大小写字母，同一商户号下唯一。</summary>
    [JsonPropertyName("contract_code")] public string? ContractCode { get; set; }
    /// <summary>商户请求签约序列号（<c>request_serial</c>，必填 int64）：唯一、禁止 0 开头、纯数字。</summary>
    [JsonPropertyName("request_serial")] public long? RequestSerial { get; set; }
    /// <summary>签约用户名称（<c>contract_display_account</c>，必填 string(128)）：不支持 UTF8 非 3 字节编码字符。</summary>
    [JsonPropertyName("contract_display_account")] public string? ContractDisplayAccount { get; set; }
    /// <summary>签约成功回调地址（<c>notify_url</c>，必填 string(256)）：外网可访问、不带参数。</summary>
    [JsonPropertyName("notify_url")] public string? NotifyUrl { get; set; }
    /// <summary>版本（<c>version</c>，必填 string(8)）：固定值 <c>1.0</c>。</summary>
    [JsonPropertyName("version")] public string? Version { get; set; }
    /// <summary>系统当前时间（<c>timestamp</c>，必填 string(10)）。</summary>
    [JsonPropertyName("timestamp")] public string? Timestamp { get; set; }
    /// <summary>签名算法（<c>sign_type</c>，选填 string(32)）：本页缺省默认 <c>MD5</c>。</summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }
    /// <summary>签约完成后返回 APP（<c>return_app</c>，选填 string(1)）：<c>Y</c> 表示返回 app，不传则停留微信。</summary>
    [JsonPropertyName("return_app")] public string? ReturnApp { get; set; }
    /// <summary>签名（<c>sign</c>）：由传输层签名 Handler 补写，调用方留空。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 服务商 APP 预签约应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PartnerPreEntrustWebResponse
{
    /// <summary>通信标识（<c>return_code</c>）：非签约结果标识。</summary>
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    /// <summary>返回信息（<c>return_msg</c>）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    /// <summary>业务结果（<c>result_code</c>）。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    /// <summary>错误码（<c>err_code</c>）。</summary>
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    /// <summary>错误码描述（<c>err_code_des</c>）。</summary>
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
    /// <summary>服务商 appid（<c>appid</c>）：开放平台审核通过的应用 APPID，注意与公众号 APPID 不同。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    /// <summary>服务商商户号（<c>mch_id</c>）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    /// <summary>子商户绑定的 appid（<c>sub_appid</c>）。</summary>
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    /// <summary>子商户号（<c>sub_mch_id</c>）。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    /// <summary>随机字符串（<c>nonce_str</c>）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    /// <summary>预签约 ID（<c>pre_entrustweb_id</c>）：<b>2 小时</b>内有效，交 APP 端 SDK 调起签约。</summary>
    [JsonPropertyName("pre_entrustweb_id")] public string? PreEntrustwebId { get; set; }
    /// <summary>小程序签约用户名（<c>miniprogram_username</c>）：仅「模板为 2025-09-23 及之后申请」或「已申请 WXLaunchMiniProgram 权限」时返回；返回则可用 LaunchMiniProgram SDK 签约并原样填入。</summary>
    [JsonPropertyName("miniprogram_username")] public string? MiniprogramUsername { get; set; }
    /// <summary>小程序签约路径（<c>miniprogram_path</c>）：返回条件同 <see cref="MiniprogramUsername"/>，APP 调起签约需原样填入。</summary>
    [JsonPropertyName("miniprogram_path")] public string? MiniprogramPath { get; set; }
    /// <summary>签名（<c>sign</c>）：由传输层自动验签。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 服务商查询委托代扣签约关系请求体（<b>POST</b> <c>/papay/partner/querycontract</c>）。
/// </summary>
/// <remarks><b>频率上限</b>：官方默认 300qps。<b>两种查询模式</b>：<c>contract_id</c> 或 <c>plan_id</c>+<c>contract_code</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PartnerQueryContractRequest
{
    /// <summary>应用 ID（<c>appid</c>，必填 string(32)）：服务商申请的公众号或移动应用 appid。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    /// <summary>服务商商户号（<c>mch_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    /// <summary>子商户绑定的 appid（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    /// <summary>子商户号（<c>sub_mch_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    /// <summary>委托代扣协议 ID（<c>contract_id</c>，条件必填 string(32)）：contract_id 查询模式必填。</summary>
    [JsonPropertyName("contract_id")] public string? ContractId { get; set; }
    /// <summary>代扣模板 ID（<c>plan_id</c>，条件必填 string(128)）：plan_id+contract_code 模式必填。</summary>
    [JsonPropertyName("plan_id")] public string? PlanId { get; set; }
    /// <summary>签约协议号（<c>contract_code</c>，条件必填 string(128)）：商户侧须唯一。</summary>
    [JsonPropertyName("contract_code")] public string? ContractCode { get; set; }
    /// <summary>版本（<c>version</c>，必填 string(8)）：固定值 <c>1.0</c>。</summary>
    [JsonPropertyName("version")] public string? Version { get; set; }
    /// <summary>签名（<c>sign</c>）：由传输层签名 Handler 补写，调用方留空。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 服务商查询委托代扣签约关系应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PartnerQueryContractResponse
{
    /// <summary>通信标识（<c>return_code</c>）。</summary>
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    /// <summary>返回信息（<c>return_msg</c>）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    /// <summary>业务结果（<c>result_code</c>）。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    /// <summary>错误码（<c>err_code</c>）。</summary>
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    /// <summary>错误码描述（<c>err_code_des</c>）。</summary>
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
    /// <summary>服务商商户号（<c>mch_id</c>）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    /// <summary>服务商 appid（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    /// <summary>子商户绑定的 appid（<c>sub_appid</c>）。</summary>
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    /// <summary>子商户号（<c>sub_mch_id</c>）。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    /// <summary>委托代扣协议 ID（<c>contract_id</c>）。</summary>
    [JsonPropertyName("contract_id")] public string? ContractId { get; set; }
    /// <summary>协议模板 ID（<c>plan_id</c>）。</summary>
    [JsonPropertyName("plan_id")] public string? PlanId { get; set; }
    /// <summary>商户请求签约序列号（<c>request_serial</c>，int64）：主要用于排序，不作查询条件。</summary>
    [JsonPropertyName("request_serial")] public long? RequestSerial { get; set; }
    /// <summary>签约协议号（<c>contract_code</c>）。</summary>
    [JsonPropertyName("contract_code")] public string? ContractCode { get; set; }
    /// <summary>签约用户名称（<c>contract_display_account</c>）。</summary>
    [JsonPropertyName("contract_display_account")] public string? ContractDisplayAccount { get; set; }
    /// <summary>协议状态（<c>contract_state</c>）：<c>0</c> 已签约 / <c>1</c> 未签约 / <c>9</c> 签约进行中。</summary>
    [JsonPropertyName("contract_state")] public int? ContractState { get; set; }
    /// <summary>协议签署时间（<c>contract_signed_time</c>）。</summary>
    [JsonPropertyName("contract_signed_time")] public string? ContractSignedTime { get; set; }
    /// <summary>协议到期时间（<c>contract_expired_time</c>）：目前协议长期有效，可忽略该字段。</summary>
    [JsonPropertyName("contract_expired_time")] public string? ContractExpiredTime { get; set; }
    /// <summary>协议解约时间（<c>contract_terminated_time</c>）：<c>contract_state=1</c> 时有效。</summary>
    [JsonPropertyName("contract_terminated_time")] public string? ContractTerminatedTime { get; set; }
    /// <summary>
    /// 解约方式（<c>contract_termination_mode</c>）：<c>contract_state=1</c> 时有效。
    /// <c>1</c> 有效期过自动解约（预留）/ <c>2</c> 用户主动解约 / <c>3</c> 商户 API 解约 /
    /// <c>4</c> 商户平台解约 / <c>5</c> 用户微信账户注销 / <c>7</c> 用户联系客服发起解约。
    /// </summary>
    [JsonPropertyName("contract_termination_mode")] public int? ContractTerminationMode { get; set; }
    /// <summary>解约原因备注（<c>contract_termination_remark</c>）：<c>contract_state=1</c> 时有效。</summary>
    [JsonPropertyName("contract_termination_remark")] public string? ContractTerminationRemark { get; set; }
    /// <summary>用户标识（<c>openid</c>）：服务商 appid 下的用户唯一标识。</summary>
    [JsonPropertyName("openid")] public string? OpenId { get; set; }
    /// <summary>子商户用户标识（<c>sub_openid</c>）：<c>sub_appid</c> 下的用户唯一标识。</summary>
    [JsonPropertyName("sub_openid")] public string? SubOpenId { get; set; }
    /// <summary>签名（<c>sign</c>）：由传输层自动验签。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 服务商委托代扣扣款请求体（<b>POST</b> <c>/pay/partner/pappayapply</c>）。
/// </summary>
/// <remarks>
/// <para><b>频率上限</b>：官方 150qps。<b>异步受理</b>：<c>err_code = ORDER_ACCEPTED</c> 表示已受理，
/// <b>不得重复发起</b>；同一商户 + 同一订单号只扣一次（幂等）。</para>
/// <para><b>扣款预约规则</b>：扣款顺序优先零钱、次选信用卡/借记卡；新签约 12 小时内可直接扣款，
/// 之后遵循预约规则，预约扣款限北京时间 <b>07:00–22:00</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PartnerPapPayApplyRequest
{
    /// <summary>应用 ID（<c>appid</c>，必填 string(32)）：服务商申请的公众号或移动应用 appid。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    /// <summary>服务商商户号（<c>mch_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    /// <summary>子商户绑定的 appid（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    /// <summary>子商户号（<c>sub_mch_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    /// <summary>随机字符串（<c>nonce_str</c>，必填 string(32)）：不长于 32 位。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    /// <summary>商品描述（<c>body</c>，必填 string(127)）。</summary>
    [JsonPropertyName("body")] public string? Body { get; set; }
    /// <summary>商品明细（<c>detail</c>，选填 string(8192)）。</summary>
    [JsonPropertyName("detail")] public string? Detail { get; set; }
    /// <summary>附加数据（<c>attach</c>，选填 string(128)）：查询与通知中原样返回。</summary>
    [JsonPropertyName("attach")] public string? Attach { get; set; }
    /// <summary>商户订单号（<c>out_trade_no</c>，必填 string(32)）：32 字符内、可包含字母。</summary>
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    /// <summary>订单总金额（<c>total_fee</c>，必填 int）：单位为分，只能为整数。</summary>
    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }
    /// <summary>货币种类（<c>fee_type</c>，选填 string(16)）：ISO 4217 三位字母代码，默认 <c>CNY</c>。</summary>
    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }
    /// <summary>用户客户端 IP（<c>spbill_create_ip</c>，必填 string(16)）：官方服务商本页标注为必填（商户页为选填），照抄。</summary>
    [JsonPropertyName("spbill_create_ip")] public string? SpbillCreateIp { get; set; }
    /// <summary>商品标记（<c>goods_tag</c>，选填 string(32)）。</summary>
    [JsonPropertyName("goods_tag")] public string? GoodsTag { get; set; }
    /// <summary>扣款结果回调地址（<c>notify_url</c>，必填 string(256)）：外网可访问、不带参数。</summary>
    [JsonPropertyName("notify_url")] public string? NotifyUrl { get; set; }
    /// <summary>交易类型（<c>trade_type</c>，必填 string(8)）：枚举值固定 <c>PAP</c>。</summary>
    [JsonPropertyName("trade_type")] public string? TradeType { get; set; }
    /// <summary>委托代扣协议 ID（<c>contract_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("contract_id")] public string? ContractId { get; set; }
    /// <summary>电子发票开票入口（<c>receipt</c>，选填 string(8)）：传 <c>Y</c> 时支付成功消息与详情页出现开票入口，需开通电子发票功能才生效。</summary>
    [JsonPropertyName("receipt")] public string? Receipt { get; set; }
    /// <summary>签名（<c>sign</c>）：由传输层签名 Handler 补写，调用方留空。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 服务商委托代扣扣款应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PartnerPapPayApplyResponse
{
    /// <summary>通信标识（<c>return_code</c>）：非交易标识。</summary>
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    /// <summary>返回信息（<c>return_msg</c>）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    /// <summary>业务结果（<c>result_code</c>）。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    /// <summary>错误码（<c>err_code</c>）：<c>ORDER_ACCEPTED</c> 表示已受理、勿重复发起。</summary>
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    /// <summary>错误码描述（<c>err_code_des</c>）。</summary>
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
    /// <summary>服务商 appid（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    /// <summary>服务商商户号（<c>mch_id</c>）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    /// <summary>子商户绑定的 appid（<c>sub_appid</c>）。</summary>
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }
    /// <summary>子商户号（<c>sub_mch_id</c>）。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
    /// <summary>随机字符串（<c>nonce_str</c>）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    /// <summary>签名（<c>sign</c>）：由传输层自动验签。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}
