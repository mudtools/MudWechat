// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

// 字段名一律照抄官方原文拼写；必填性按官方表格标注，SDK 侧不做前置校验
// （APIv2 的业务失败也用 HTTP 200 + return_code/result_code 两级表达，判定归调用方）。

/// <summary>
/// APIv2 解除委托代扣签约协议请求体（<b>POST</b> <c>/papay/deletecontract</c>）。
/// </summary>
/// <remarks>
/// <b>两种解约模式</b>：<c>contract_id</c> 单键，或 <c>plan_id</c> + <c>contract_code</c> 组合键，
/// 二者必择其一（条件必填），返回结果相同。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2DeleteContractRequest
{
    /// <summary>应用 ID（<c>appid</c>，必填 string(32)）：直连商户申请的公众号或移动应用 appid。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    /// <summary>商户号（<c>mch_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    /// <summary>委托代扣协议 ID（<c>contract_id</c>，条件必填 string(32)）：contract_id 解约模式必填。</summary>
    [JsonPropertyName("contract_id")] public string? ContractId { get; set; }
    /// <summary>代扣模板 ID（<c>plan_id</c>，条件必填 string(128)）：商户在微信商户平台配置的模板 id，plan_id+contract_code 模式必填。</summary>
    [JsonPropertyName("plan_id")] public string? PlanId { get; set; }
    /// <summary>签约协议号（<c>contract_code</c>，条件必填 string(128)）：商户请求签约时传入，商户侧须唯一。</summary>
    [JsonPropertyName("contract_code")] public string? ContractCode { get; set; }
    /// <summary>解约原因备注（<c>contract_termination_remark</c>，必填 string(512)）：如「签约信息有误，须重新签约」。</summary>
    [JsonPropertyName("contract_termination_remark")] public string? ContractTerminationRemark { get; set; }
    /// <summary>版本（<c>version</c>，必填 string(8)）：固定值 <c>1.0</c>。</summary>
    [JsonPropertyName("version")] public string? Version { get; set; }
    /// <summary>签名（<c>sign</c>）：由传输层签名 Handler 补写，调用方留空。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 解除委托代扣签约协议应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2DeleteContractResponse
{
    /// <summary>通信标识（<c>return_code</c>）：<c>SUCCESS</c>/<c>FAIL</c>，非交易标识。</summary>
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    /// <summary>返回信息（<c>return_msg</c>）：非空即错误原因（如签名失败）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    /// <summary>业务结果（<c>result_code</c>）：<c>SUCCESS</c>/<c>FAIL</c>。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    /// <summary>错误码（<c>err_code</c>）。</summary>
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    /// <summary>错误码描述（<c>err_code_des</c>）。</summary>
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
    /// <summary>商户号（<c>mch_id</c>）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    /// <summary>应用 ID（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }
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
/// APIv2 H5 预签约应答。
/// </summary>
/// <remarks>
/// <para>
/// <b>入参不走本 DTO</b>：官方该端点是 <b>GET</b>，参数全部在查询串里 ⇒
/// 接口方法以 <c>[Query]</c> 逐参声明（见 <c>IWechatPayV2PapayService.H5EntrustWebAsync</c>）。
/// </para>
/// <para>
/// <b>本端点应答是 XML 而非 302</b>：拿到 <see cref="RedirectUrl"/> 后由前端跳转。
/// 官方硬性要求页面加 <c>&lt;meta name="referrer" content="no-referrer-when-downgrade"&gt;</c>，
/// 且<b>不得</b>用 <c>window.location.replace</c>，须用 <c>window.location.href =</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2H5EntrustWebResponse
{
    /// <summary>通信标识（<c>return_code</c>）：非签约结果标识，签约发起是否成功看 <c>result_code</c>。</summary>
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    /// <summary>返回信息（<c>return_msg</c>）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    /// <summary>业务结果（<c>result_code</c>）。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    /// <summary>结果信息（<c>result_msg</c>）：非空即错误原因，如签名错误。</summary>
    [JsonPropertyName("result_msg")] public string? ResultMsg { get; set; }
    /// <summary>跳转签约页面 URL（<c>redirect_url</c>）：有效期 <b>10 分钟</b>；发起请求的页面地址须在微信后台（申请 H5 签约权限时）配置。</summary>
    [JsonPropertyName("redirect_url")] public string? RedirectUrl { get; set; }
}

/// <summary>
/// APIv2 APP 预签约请求体（<b>POST</b> <c>/papay/preentrustweb</c>）。
/// </summary>
/// <remarks>
/// 与 H5 形态的关键差异：本端点返回 <see cref="V2PreEntrustWebResponse.PreEntrustwebId"/>，
/// 由 APP 端 SDK（<c>OpenBusinessWebview</c> 或小程序通道）调起签约，而非直接给跳转 URL；
/// 且 <c>sign_type</c> 缺省时官方默认 <b>MD5</b>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PreEntrustWebRequest
{
    /// <summary>应用 ID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    /// <summary>商户号（<c>mch_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
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
    /// <summary>签约完成后返回 APP（<c>return_app</c>，选填 string(1)）：<c>Y</c> 表示返回 app，不传则停留微信；iOS/Android 需点完成按钮才返回。</summary>
    [JsonPropertyName("return_app")] public string? ReturnApp { get; set; }
    /// <summary>签名（<c>sign</c>）：由传输层签名 Handler 补写，调用方留空。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 APP 预签约应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PreEntrustWebResponse
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
    /// <summary>应用 ID（<c>appid</c>）：开放平台 APPID，注意与公众号 APPID 不同。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    /// <summary>商户号（<c>mch_id</c>）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    /// <summary>随机字符串（<c>nonce_str</c>）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    /// <summary>预签约 ID（<c>pre_entrustweb_id</c>）：<b>2 小时</b>内有效，交 APP 端 SDK 调起签约。</summary>
    [JsonPropertyName("pre_entrustweb_id")] public string? PreEntrustwebId { get; set; }
    /// <summary>小程序签约用户名（<c>miniprogram_username</c>）：仅「模板为 2025-09-23 及之后申请」或「已申请 WXLaunchMiniProgram 权限」时返回；返回则可用 LaunchMiniProgram SDK 签约并原样填入，否则只能 OpenBusinessWebview SDK。</summary>
    [JsonPropertyName("miniprogram_username")] public string? MiniprogramUsername { get; set; }
    /// <summary>小程序签约路径（<c>miniprogram_path</c>）：返回条件同 <see cref="MiniprogramUsername"/>，APP 调起签约需原样填入。</summary>
    [JsonPropertyName("miniprogram_path")] public string? MiniprogramPath { get; set; }
    /// <summary>签名（<c>sign</c>）：由传输层自动验签。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 查询委托代扣签约关系请求体（<b>POST</b> <c>/papay/querycontract</c>）。
/// </summary>
/// <remarks><b>频率上限</b>：官方 300qps。<b>两种查询模式</b>：<c>contract_id</c> 或 <c>plan_id</c>+<c>contract_code</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2QueryContractRequest
{
    /// <summary>应用 ID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    /// <summary>商户号（<c>mch_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
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
/// APIv2 查询委托代扣签约关系应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2QueryContractResponse
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
    /// <summary>商户号（<c>mch_id</c>）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    /// <summary>应用 ID（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }
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
    /// <c>4</c> 商户平台解约 / <c>5</c> 用户微信账户注销 / <c>7</c> 用户联系客服发起的解约。
    /// </summary>
    [JsonPropertyName("contract_termination_mode")] public int? ContractTerminationMode { get; set; }
    /// <summary>解约原因备注（<c>contract_termination_remark</c>）：<c>contract_state=1</c> 时有效。</summary>
    [JsonPropertyName("contract_termination_remark")] public string? ContractTerminationRemark { get; set; }
    /// <summary>用户标识（<c>openid</c>）：商户 appid 下的用户唯一标识。</summary>
    [JsonPropertyName("openid")] public string? OpenId { get; set; }
    /// <summary>签名（<c>sign</c>）：由传输层自动验签。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 支付中签约请求体（<b>POST</b> <c>/pay/contractorder</c>，统一下单与签约合并）。
/// </summary>
/// <remarks>
/// <para><b>双回调</b>：支付结果经 <see cref="NotifyUrl"/>、签约结果经 <see cref="ContractNotifyUrl"/>
/// 各自<b>异步通知</b> ⇒ 应答里的 <c>result_code</c> 只代表受理，两笔结果都要等回调。</para>
/// <para><b>签名在 URL encode 之前计算</b>（官方原文），由传输层保证：本 SDK 的字段表始终承载解码后的原文值。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2ContractOrderRequest
{
    /// <summary>应用 ID（<c>appid</c>，必填 string(32)）：由微信生成的应用 ID，全局唯一。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    /// <summary>商户号（<c>mch_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    /// <summary>签约商户号（<c>contract_mchid</c>，必填 string(32)）：必须与 <c>mch_id</c> 一致。</summary>
    [JsonPropertyName("contract_mchid")] public string? ContractMchId { get; set; }
    /// <summary>签约公众号（<c>contract_appid</c>，必填 string(32)）：必须与 <c>appid</c> 一致。</summary>
    [JsonPropertyName("contract_appid")] public string? ContractAppId { get; set; }
    /// <summary>商户订单号（<c>out_trade_no</c>，必填 string(32)）：32 字符内、可包含字母。</summary>
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }
    /// <summary>终端设备号（<c>device_info</c>，选填 string(32)）：PC 网页或公众号内支付传 <c>WEB</c>。</summary>
    [JsonPropertyName("device_info")] public string? DeviceInfo { get; set; }
    /// <summary>随机字符串（<c>nonce_str</c>，必填 string(32)）：不长于 32 位。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    /// <summary>商品描述（<c>body</c>，必填 string(127)）。</summary>
    [JsonPropertyName("body")] public string? Body { get; set; }
    /// <summary>商品明细（<c>detail</c>，选填 string(8192)）。</summary>
    [JsonPropertyName("detail")] public string? Detail { get; set; }
    /// <summary>附加数据（<c>attach</c>，选填 string(127)）：查询与通知中原样返回。</summary>
    [JsonPropertyName("attach")] public string? Attach { get; set; }
    /// <summary>支付回调地址（<c>notify_url</c>，必填 string(256)）：http/https 开头、外网可访问、不带参数。</summary>
    [JsonPropertyName("notify_url")] public string? NotifyUrl { get; set; }
    /// <summary>订单总金额（<c>total_fee</c>，必填 int）：单位为分。</summary>
    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }
    /// <summary>用户客户端 IP（<c>spbill_create_ip</c>，必填 string(16)）：支持 IPV4 与 IPV6。</summary>
    [JsonPropertyName("spbill_create_ip")] public string? SpbillCreateIp { get; set; }
    /// <summary>订单生成时间（<c>time_start</c>，选填 string(14)）：格式 <c>yyyyMMddHHmmss</c>。</summary>
    [JsonPropertyName("time_start")] public string? TimeStart { get; set; }
    /// <summary>订单失效时间（<c>time_expire</c>，选填 string(14)）：格式 <c>yyyyMMddHHmmss</c>。</summary>
    [JsonPropertyName("time_expire")] public string? TimeExpire { get; set; }
    /// <summary>商品标记（<c>goods_tag</c>，选填 string(32)）：代金券或立减优惠功能的参数。</summary>
    [JsonPropertyName("goods_tag")] public string? GoodsTag { get; set; }
    /// <summary>交易类型（<c>trade_type</c>，必填 string(16)）：<c>JSAPI</c> / <c>NATIVE</c> / <c>APP</c> / <c>MWEB</c>。</summary>
    [JsonPropertyName("trade_type")] public string? TradeType { get; set; }
    /// <summary>商品 ID（<c>product_id</c>，条件必填 string(32)）：<c>trade_type=NATIVE</c> 时必传。</summary>
    [JsonPropertyName("product_id")] public string? ProductId { get; set; }
    /// <summary>用户标识（<c>openid</c>，条件必填 string(128)）：<c>trade_type=JSAPI</c> 时必传。</summary>
    [JsonPropertyName("openid")] public string? OpenId { get; set; }
    /// <summary>协议模板 ID（<c>plan_id</c>，必填 int）：官方本页标注为整型（其余 papay 端点标注为字符串），照抄不得统一。</summary>
    [JsonPropertyName("plan_id")] public long? PlanId { get; set; }
    /// <summary>商户侧签约协议号（<c>contract_code</c>，必填 string(64)）：数字与大小写字母，同一商户号下唯一。</summary>
    [JsonPropertyName("contract_code")] public string? ContractCode { get; set; }
    /// <summary>商户请求签约序列号（<c>request_serial</c>，必填 int64）：要求唯一、禁止 0 开头。</summary>
    [JsonPropertyName("request_serial")] public long? RequestSerial { get; set; }
    /// <summary>签约用户名称（<c>contract_display_account</c>，必填 string(64)）：支付中签约页面「开通账号」展示。</summary>
    [JsonPropertyName("contract_display_account")] public string? ContractDisplayAccount { get; set; }
    /// <summary>签约信息回调地址（<c>contract_notify_url</c>，必填 string(256)）：http/https 开头、外网可访问、不带参数。</summary>
    [JsonPropertyName("contract_notify_url")] public string? ContractNotifyUrl { get; set; }
    /// <summary>签名（<c>sign</c>）：由传输层签名 Handler 补写，调用方留空。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 支付中签约应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2ContractOrderResponse
{
    /// <summary>通信标识（<c>return_code</c>）。</summary>
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    /// <summary>返回信息（<c>return_msg</c>）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    /// <summary>支付业务结果（<c>result_code</c>）。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }
    /// <summary>错误代码（<c>err_code</c>）。</summary>
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    /// <summary>错误信息描述（<c>err_code_des</c>）。</summary>
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
    /// <summary>预签约结果（<c>contract_result_code</c>）：<c>SUCCESS</c>/<c>FAIL</c>，仅在 return 与 result 均 SUCCESS 时返回。</summary>
    [JsonPropertyName("contract_result_code")] public string? ContractResultCode { get; set; }
    /// <summary>预签约错误代码（<c>contract_err_code</c>）。</summary>
    [JsonPropertyName("contract_err_code")] public string? ContractErrCode { get; set; }
    /// <summary>预签约错误描述（<c>contract_err_code_des</c>）。</summary>
    [JsonPropertyName("contract_err_code_des")] public string? ContractErrCodeDes { get; set; }
    /// <summary>预支付交易会话标识（<c>prepay_id</c>）：有效期 <b>2 小时</b>。</summary>
    [JsonPropertyName("prepay_id")] public string? PrepayId { get; set; }
    /// <summary>扫码支付链接（<c>code_url</c>）：<c>trade_type=NATIVE</c> 时返回。</summary>
    [JsonPropertyName("code_url")] public string? CodeUrl { get; set; }
    /// <summary>H5 收银台中间页（<c>mweb_url</c>）：有效期 <b>5 分钟</b>。</summary>
    [JsonPropertyName("mweb_url")] public string? MwebUrl { get; set; }
    /// <summary>签名（<c>sign</c>）：由传输层自动验签。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 委托代扣扣款请求体（<b>POST</b> <c>/pay/pappayapply</c>）。
/// </summary>
/// <remarks>
/// <para><b>频率上限</b>：官方 150qps。<b>异步受理</b>：<c>err_code = ORDER_ACCEPTED</c>
/// 表示扣款请求已受理，<b>不得重复发起</b>；扣款结果以 <c>notify_url</c> 异步通知为准。</para>
/// <para><b>协议类错误</b>：<c>CONTRACT_NOT_EXIST</c> 签约协议不存在；<c>CONTRACTERROR</c> 协议已过期。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PapPayApplyRequest
{
    /// <summary>应用 ID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    /// <summary>商户号（<c>mch_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
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
    /// <summary>货币种类（<c>fee_type</c>，选填 string(16)）：符合 ISO 4217 三位字母代码，默认 <c>CNY</c>。</summary>
    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }
    /// <summary>用户客户端 IP（<c>spbill_create_ip</c>，选填 string(16)）：官方商户本页标注选填（服务商页为必填），照抄。</summary>
    [JsonPropertyName("spbill_create_ip")] public string? SpbillCreateIp { get; set; }
    /// <summary>商品标记（<c>goods_tag</c>，选填 string(32)）。</summary>
    [JsonPropertyName("goods_tag")] public string? GoodsTag { get; set; }
    /// <summary>扣款结果回调地址（<c>notify_url</c>，必填 string(256)）：http/https 开头、外网可访问、不带参数。</summary>
    [JsonPropertyName("notify_url")] public string? NotifyUrl { get; set; }
    /// <summary>交易类型（<c>trade_type</c>，必填）：枚举值固定 <c>PAP</c>（微信委托代扣支付）。</summary>
    [JsonPropertyName("trade_type")] public string? TradeType { get; set; }
    /// <summary>委托代扣协议 ID（<c>contract_id</c>，必填 string(32)）：签约成功后微信返回。</summary>
    [JsonPropertyName("contract_id")] public string? ContractId { get; set; }
    /// <summary>签名（<c>sign</c>）：由传输层签名 Handler 补写，调用方留空。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 委托代扣扣款应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PapPayApplyResponse
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
    /// <summary>商户号（<c>mch_id</c>）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    /// <summary>应用 ID（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    /// <summary>随机字符串（<c>nonce_str</c>）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    /// <summary>签名（<c>sign</c>）：由传输层自动验签。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}
