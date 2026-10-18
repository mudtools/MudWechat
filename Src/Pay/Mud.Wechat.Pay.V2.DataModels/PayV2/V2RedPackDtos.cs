// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 发放现金红包请求体（<c>/mmpaymkttransfers/sendredpack</c>，需商户证书）。
/// </summary>
/// <remarks>
/// <para><b>金额约束</b>：单个红包 <b>[100, 20000] 分</b>；<c>total_num = 1</c> 时
/// <c>total_amount = 单个金额</c>，<c>total_num &gt; 1</c>（裂变红包）时 <c>total_amount ≥ 200 × total_num</c> 分。</para>
/// <para><b>幂等键</b>：<c>mch_billno</c>（商户订单号）唯一，<b>失败重试必须沿用原单号</b>；
/// <c>SYSTEMERROR</c> 时换单号会造成<b>重复出资</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2SendRedPackRequest
{
    /// <summary>商户订单号（<c>mch_billno</c>，必填 string(28)）：商户号 + 8 位日期 + 业务号，重试不得更换。</summary>
    [JsonPropertyName("mch_billno")] public string? MchBillNo { get; set; }

    /// <summary>商户号（<c>mch_id</c>，必填 string(10)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>子商户号（<c>sub_mch_id</c>，选填 string(10)）：服务商模式下填实际出资的子商户号。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>公众账号 ID（<c>wxappid</c>，必填 string(32)）：发放红包的 appid，须与商户号绑定。</summary>
    [JsonPropertyName("wxappid")] public string? WxAppId { get; set; }

    /// <summary>场景应用 ID（<c>msgappid</c>，选填 string(32)）：下发模板消息用的 appid，与 <c>wxappid</c> 可不同。</summary>
    [JsonPropertyName("msgappid")] public string? MsgAppId { get; set; }

    /// <summary>商户红包发送者名称（<c>send_name</c>，必填 string(32)）：红包页展示的名称。</summary>
    [JsonPropertyName("send_name")] public string? SendName { get; set; }

    /// <summary>收红包用户 openid（<c>re_openid</c>，必填 string(64)）：须与 <c>wxappid</c> 同一应用下的用户。</summary>
    [JsonPropertyName("re_openid")] public string? ReOpenId { get; set; }

    /// <summary>付款金额（<c>total_amount</c>，必填 int）：单位为分，约束见类型注释。</summary>
    [JsonPropertyName("total_amount")] public int? TotalAmount { get; set; }

    /// <summary>红包个数（<c>total_num</c>，必填 int）：为 1 即普通红包，大于 1 即裂变红包。</summary>
    [JsonPropertyName("total_num")] public int? TotalNum { get; set; }

    /// <summary>祝福语（<c>wishing</c>，必填 string(128)）：红包显示文案。</summary>
    [JsonPropertyName("wishing")] public string? Wishing { get; set; }

    /// <summary>IP 地址（<c>client_ip</c>，选填 string(32)）：请求端 IP，风控参考项。</summary>
    [JsonPropertyName("client_ip")] public string? ClientIp { get; set; }

    /// <summary>活动名称（<c>act_name</c>，必填 string(32)）：用于微信侧账单与风控展示。</summary>
    [JsonPropertyName("act_name")] public string? ActName { get; set; }

    /// <summary>备注（<c>remark</c>，必填 string(256)）。</summary>
    [JsonPropertyName("remark")] public string? Remark { get; set; }

    /// <summary>场景 ID（<c>scene_id</c>，选填 string(32)）：默认 <c>Platform-1001</c>，需在商户平台申请。</summary>
    [JsonPropertyName("scene_id")] public string? SceneId { get; set; }

    /// <summary>风控信息（<c>risk_info</c>，选填 string：JSON 串）：含用户身份字段，属敏感信息 ⇒ SDK 不落日志。</summary>
    [JsonPropertyName("risk_info")] public string? RiskInfo { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>，必填 string(32)）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>）：由证书通道自动填入，宿主不要手工赋值。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 发放现金红包应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2SendRedPackResponse
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

    /// <summary>商户订单号（<c>mch_billno</c>）：回显请求值。</summary>
    [JsonPropertyName("mch_billno")] public string? MchBillNo { get; set; }

    /// <summary>微信单号（<c>send_listid</c>）：红包流水号，后续查询以此为准。</summary>
    [JsonPropertyName("send_listid")] public string? SendListId { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>）：由证书通道自动校验。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 发放小程序红包请求体（<c>/mmpaymkttransfers/sendminiprogramhb</c>，需商户证书）。
/// </summary>
/// <remarks>
/// <para>与 <see cref="V2SendRedPackRequest"/> 的差异：字段面<b>不含</b> <c>client_ip</c> 与 <c>risk_info</c>，
/// 且 <c>notify_way</c> 为<b>必填固定值</b> <c>MINI_PROGRAM_JSAPI</c> ⇒ 不复用同一个 DTO，
/// 避免「小程序族漏填 notify_way」这种非法状态在另一族里也能编译通过。</para>
/// <para>成功应答额外返回 <c>package</c>，小程序端用它调 <c>wx.openBusinessView</c> 领取。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2SendMiniProgramRedPackRequest
{
    /// <summary>商户订单号（<c>mch_billno</c>，必填 string(28)）：重试不得更换。</summary>
    [JsonPropertyName("mch_billno")] public string? MchBillNo { get; set; }

    /// <summary>商户号（<c>mch_id</c>，必填 string(10)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>子商户号（<c>sub_mch_id</c>，选填 string(10)）。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>小程序 appid（<c>wxappid</c>，必填 string(32)）：发放红包的小程序 appid。</summary>
    [JsonPropertyName("wxappid")] public string? WxAppId { get; set; }

    /// <summary>场景应用 ID（<c>msgappid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("msgappid")] public string? MsgAppId { get; set; }

    /// <summary>通知方式（<c>notify_way</c>，必填 string(32)）：<b>固定值</b> <c>MINI_PROGRAM_JSAPI</c>。</summary>
    [JsonPropertyName("notify_way")] public string? NotifyWay { get; set; }

    /// <summary>商户红包发送者名称（<c>send_name</c>，必填 string(32)）。</summary>
    [JsonPropertyName("send_name")] public string? SendName { get; set; }

    /// <summary>收红包用户 openid（<c>re_openid</c>，必填 string(64)）：须为该小程序下的用户。</summary>
    [JsonPropertyName("re_openid")] public string? ReOpenId { get; set; }

    /// <summary>付款金额（<c>total_amount</c>，必填 int）：单位为分。</summary>
    [JsonPropertyName("total_amount")] public int? TotalAmount { get; set; }

    /// <summary>红包个数（<c>total_num</c>，必填 int）。</summary>
    [JsonPropertyName("total_num")] public int? TotalNum { get; set; }

    /// <summary>祝福语（<c>wishing</c>，必填 string(128)）。</summary>
    [JsonPropertyName("wishing")] public string? Wishing { get; set; }

    /// <summary>活动名称（<c>act_name</c>，必填 string(32)）。</summary>
    [JsonPropertyName("act_name")] public string? ActName { get; set; }

    /// <summary>备注（<c>remark</c>，必填 string(256)）。</summary>
    [JsonPropertyName("remark")] public string? Remark { get; set; }

    /// <summary>场景 ID（<c>scene_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("scene_id")] public string? SceneId { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>，必填 string(32)）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>）：由证书通道自动填入。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 发放小程序红包应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2SendMiniProgramRedPackResponse
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

    /// <summary>商户订单号（<c>mch_billno</c>）。</summary>
    [JsonPropertyName("mch_billno")] public string? MchBillNo { get; set; }

    /// <summary>微信单号（<c>send_listid</c>）。</summary>
    [JsonPropertyName("send_listid")] public string? SendListId { get; set; }

    /// <summary>领取红包资源包（<c>package</c>）：小程序端传给 <c>wx.openBusinessView</c>，<b>不得</b>自行拼装。</summary>
    [JsonPropertyName("package")] public string? Package { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 查询红包记录请求体（<c>/mmpaymkttransfers/gethbinfo</c>，需商户证书）。
/// </summary>
/// <remarks>
/// <para><b>字段名陷阱</b>：本端点请求用 <c>appid</c>（而发放两支用 <c>wxappid</c>），
/// 且 <c>bill_type</c> 为<b>必填固定值</b> <c>MCHT</c> —— 照抄官方，不做「统一」修正。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2GetRedPackInfoRequest
{
    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）：注意本页即叫 <c>appid</c>，不是 <c>wxappid</c>。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }

    /// <summary>商户号（<c>mch_id</c>，必填 string(10)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>子商户号（<c>sub_mch_id</c>，选填 string(10)）。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>商户订单号（<c>mch_billno</c>，必填 string(28)）。</summary>
    [JsonPropertyName("mch_billno")] public string? MchBillNo { get; set; }

    /// <summary>账单类型（<c>bill_type</c>，必填 string(32)）：<b>固定值</b> <c>MCHT</c>。</summary>
    [JsonPropertyName("bill_type")] public string? BillType { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>，必填 string(32)）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>）：由证书通道自动填入。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 查询红包记录应答。
/// </summary>
/// <remarks>
/// <para><b><c>hblist</c> 是 JSON 串而非 XML 子树</b>：官方把裂变红包的领取明细塞在一个字符串字段里返回，
/// 故本 DTO 保持 <see cref="string"/>；APIv2 扁平字段表无法承载其嵌套结构，反序列化交宿主按需用
/// <c>System.Text.Json</c> 自行解析。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2GetRedPackInfoResponse
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

    /// <summary>商户订单号（<c>mch_billno</c>）。</summary>
    [JsonPropertyName("mch_billno")] public string? MchBillNo { get; set; }

    /// <summary>微信红包单号（<c>detail_id</c>）。</summary>
    [JsonPropertyName("detail_id")] public string? DetailId { get; set; }

    /// <summary>商户名称（<c>mch_name</c>）。</summary>
    [JsonPropertyName("mch_name")] public string? MchName { get; set; }

    /// <summary>红包类型（<c>hb_type</c>）：<c>1</c> 普通 / <c>4</c> 裂变 / <c>10003</c> 新版现金红包等官方取值。</summary>
    [JsonPropertyName("hb_type")] public string? HbType { get; set; }

    /// <summary>发送金额（<c>total_amount</c>）：单位为分。</summary>
    [JsonPropertyName("total_amount")] public int? TotalAmount { get; set; }

    /// <summary>红包个数（<c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")] public int? TotalNum { get; set; }

    /// <summary>退款金额（<c>refund_amount</c>）：单位为分。</summary>
    [JsonPropertyName("refund_amount")] public int? RefundAmount { get; set; }

    /// <summary>红包发放类型（<c>send_type</c>）：<c>API-HONGBAO</c> 等官方取值。</summary>
    [JsonPropertyName("send_type")] public string? SendType { get; set; }

    /// <summary>发送时间（<c>send_time</c>）：格式 <c>yyyyMMddHHmmss</c>。</summary>
    [JsonPropertyName("send_time")] public string? SendTime { get; set; }

    /// <summary>退款时间（<c>refund_time</c>）：格式 <c>yyyyMMddHHmmss</c>。</summary>
    [JsonPropertyName("refund_time")] public string? RefundTime { get; set; }

    /// <summary>红包状态（<c>status</c>）：<c>SENDING</c> / <c>SENT</c> / <c>FAILED</c> / <c>RECEIVED</c> / <c>RFUND_ING</c> / <c>REFUND</c>。</summary>
    [JsonPropertyName("status")] public string? Status { get; set; }

    /// <summary>失败原因（<c>reason</c>）：仅 <c>FAILED</c> 等异常态返回。</summary>
    [JsonPropertyName("reason")] public string? Reason { get; set; }

    /// <summary>领取明细（<c>hblist</c>）：<b>JSON 数组字符串</b>，每项含 <c>openid</c> / <c>amount</c> / <c>rcv_time</c>。</summary>
    [JsonPropertyName("hblist")] public string? HbList { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 发放企业微信红包请求体（<c>/mmpaymkttransfers/sendworkwxredpack</c>，需商户证书 + <c>workwx_sign</c>）。
/// </summary>
/// <remarks>
/// <para><b>双签</b>：本端点在 <c>sign</c> 之外还要求 <c>workwx_sign</c> —— 以<b>请求里 <c>agentid</c> 对应的
/// 企业微信自建应用 Secret</b> 为密钥，对官方<b>固定字段白名单</b>
/// （<c>act_name, mch_billno, mch_id, nonce_str, re_openid, total_amount, wxappid</c>）
/// 按 <c>MD5(stringA + "secret=" + Secret)</c> 取大写十六进制。
/// 白名单与算法收在 <c>Mud.Wechat.Pay.V2.Security.WechatPayV2WorkWxSigner</c>；
/// Secret 经 <c>IWechatPayV2WorkWxSecretProvider</c> 由宿主桥接注入，<b>不进支付配置面</b>。</para>
/// <para><b>官方口径</b>：已算出的 <c>workwx_sign</c> <b>同样参与</b> <c>sign</c>
/// （官方原文「除 sign 字段外所有字段都参与签名，包括 workwx_sign」）⇒ 顺序不可颠倒，
/// 先 <c>workwx_sign</c> 后 <c>sign</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2SendWorkWxRedPackRequest
{
    /// <summary>商户订单号（<c>mch_billno</c>，必填 string(28)）：重试不得更换。</summary>
    [JsonPropertyName("mch_billno")] public string? MchBillNo { get; set; }

    /// <summary>商户号（<c>mch_id</c>，必填 string(10)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>微信企业号 CorpID（<c>wxappid</c>，必填 string(32)）：此处语义为企业微信 CorpID。</summary>
    [JsonPropertyName("wxappid")] public string? WxAppId { get; set; }

    /// <summary>企业号应用 agentid（<c>agentid</c>，选填 string(16)）：与 <c>sender_name</c> 二选一，决定 <c>workwx_sign</c> 用哪个应用的 Secret。</summary>
    [JsonPropertyName("agentid")] public string? AgentId { get; set; }

    /// <summary>发送者名称（<c>sender_name</c>，选填 string(32)）：与 <c>agentid</c> 二选一。</summary>
    [JsonPropertyName("sender_name")] public string? SenderName { get; set; }

    /// <summary>发送者头像素材（<c>sender_header_media_id</c>，选填 string(128)）：企业微信素材 ID。</summary>
    [JsonPropertyName("sender_header_media_id")] public string? SenderHeaderMediaId { get; set; }

    /// <summary>收款用户 openid（<c>re_openid</c>，必填 string(64)）：企业微信成员的 openid。</summary>
    [JsonPropertyName("re_openid")] public string? ReOpenId { get; set; }

    /// <summary>付款金额（<c>total_amount</c>，必填 int）：单位为分。</summary>
    [JsonPropertyName("total_amount")] public int? TotalAmount { get; set; }

    /// <summary>祝福语（<c>wishing</c>，必填 string(128)）。</summary>
    [JsonPropertyName("wishing")] public string? Wishing { get; set; }

    /// <summary>活动名称（<c>act_name</c>，必填 string(32)）：参与 <c>workwx_sign</c> 白名单。</summary>
    [JsonPropertyName("act_name")] public string? ActName { get; set; }

    /// <summary>备注（<c>remark</c>，必填 string(256)）。</summary>
    [JsonPropertyName("remark")] public string? Remark { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>，必填 string(32)）：参与 <c>workwx_sign</c> 白名单。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>）：由证书通道自动填入。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }

    /// <summary>企业微信签名（<c>workwx_sign</c>）：由证书通道按白名单自动填入，宿主不要手工赋值。</summary>
    [JsonPropertyName("workwx_sign")] public string? WorkWxSign { get; set; }
}

/// <summary>
/// APIv2 发放企业微信红包应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2SendWorkWxRedPackResponse
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

    /// <summary>商户订单号（<c>mch_billno</c>）。</summary>
    [JsonPropertyName("mch_billno")] public string? MchBillNo { get; set; }

    /// <summary>微信单号（<c>send_listid</c>）。</summary>
    [JsonPropertyName("send_listid")] public string? SendListId { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 查询企业微信红包应答请求体（<c>/mmpaymkttransfers/queryworkwxredpack</c>，需商户证书）。
/// </summary>
/// <remarks>查询族官方<b>不要求</b> <c>workwx_sign</c>，只有发放族需要 ⇒ 本 DTO 不带该字段。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2QueryWorkWxRedPackRequest
{
    /// <summary>微信企业号 CorpID（<c>wxappid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("wxappid")] public string? WxAppId { get; set; }

    /// <summary>商户号（<c>mch_id</c>，必填 string(10)）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>商户订单号（<c>bill_no</c>，必填 string(28)）：注意本页参数名是 <c>bill_no</c>，不是 <c>mch_billno</c>。</summary>
    [JsonPropertyName("bill_no")] public string? BillNo { get; set; }

    /// <summary>签名（<c>sign</c>）：由证书通道自动填入。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 查询企业微信红包应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2QueryWorkWxRedPackResponse
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

    /// <summary>商户订单号（<c>mch_billno</c>）。</summary>
    [JsonPropertyName("mch_billno")] public string? MchBillNo { get; set; }

    /// <summary>商户名称（<c>mch_name</c>）。</summary>
    [JsonPropertyName("mch_name")] public string? MchName { get; set; }

    /// <summary>红包名称（<c>red_packet_name</c>）。</summary>
    [JsonPropertyName("red_packet_name")] public string? RedPacketName { get; set; }

    /// <summary>金额（<c>total_amount</c>）：单位为分。</summary>
    [JsonPropertyName("total_amount")] public int? TotalAmount { get; set; }

    /// <summary>发放时间（<c>send_time</c>）：格式 <c>yyyyMMddHHmmss</c>。</summary>
    [JsonPropertyName("send_time")] public string? SendTime { get; set; }

    /// <summary>红包状态（<c>status</c>）：官方取值 <c>SENDING</c> / <c>SENT</c> / <c>RECEIVED</c> / <c>FAILED</c> 等。</summary>
    [JsonPropertyName("status")] public string? Status { get; set; }

    /// <summary>失败原因（<c>reason</c>）。</summary>
    [JsonPropertyName("reason")] public string? Reason { get; set; }

    /// <summary>收款用户 openid（<c>openid</c>）。</summary>
    [JsonPropertyName("openid")] public string? OpenId { get; set; }

    /// <summary>领取时间（<c>received_time</c>）：格式 <c>yyyyMMddHHmmss</c>。</summary>
    [JsonPropertyName("received_time")] public string? ReceivedTime { get; set; }

    /// <summary>签名（<c>sign</c>）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}
