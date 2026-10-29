// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 车主服务用户入场通知（<c>/vehicle/partnerpay/notification</c>，服务商侧）请求体。
/// </summary>
/// <remarks>
/// <para><b>与直连版的差异只有四个标识字段</b>：<c>sub_appid</c> 与 <c>sub_mch_id</c> 均为<b>必填</b>
/// （官方：<c>sub_appid</c> 是「子商户绑定的服务号/小程序/APP 的 appid，需在服务商商户平台为子商户绑定」），
/// 场景子字段面另多一个 <c>sub_openid</c>；<c>scene_info</c> 仍是 JSON 字符串袋。</para>
/// <para><b><c>openid</c>／<c>sub_openid</c>／<c>space_number</c> 的必填性官方自相矛盾</b>：
/// 参数表把 <c>openid</c> 标「是」、<c>sub_openid</c> 标「否」，却与 <c>scene_info</c> 平铺在同表；
/// 服务商页<b>没有请求示例报文</b>可供对形态 ⇒ 本类按「表成员即顶层字段」建模，
/// 并在此留痕：若实测官方要求把它们放进 <see cref="SceneInfo"/> 的 JSON 内，
/// 调用方须自行移入，SDK 不在两种形态间猜。这是本族文档最不确定的一处。</para>
/// <para><b>权限类错误码更细</b>：<c>PAYAUTH_ERROR</c> 的官方原因列点名三项权限
/// （服务商商户号受理权限、子商户号代扣产品权限、子商户号授予父商户号代扣权限）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PartnerVehicleNotificationRequest
{
    /// <summary>服务商商户号绑定的 APPID（官方要求已授权）。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }

    /// <summary>服务商商户号。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>子商户公众账号 Id（需在服务商商户平台为子商户绑定）。</summary>
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }

    /// <summary>子商户号。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>随机字符串，不长于 32 位。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名类型：官方必填、默认 <c>HMAC-SHA256</c> ⇒ 默认值即契约面，勿清空。</summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; } = "HMAC-SHA256";

    /// <summary>版本号：官方「固定值 3.0：车场停车（PARKING）场景 2.0：其他场景」。</summary>
    [JsonPropertyName("version")] public string? Version { get; set; }

    /// <summary>交易场景：<c>PARKING</c>（车场停车）／<c>PARKING SPACE</c>（车位停车）。</summary>
    [JsonPropertyName("trade_scene")] public string? TradeScene { get; set; }

    /// <summary>场景信息：<b>JSON 字符串</b>（String(1024)）。</summary>
    [JsonPropertyName("scene_info")] public string? SceneInfo { get; set; }

    /// <summary>车牌状态变更的回调通知 URL（选填；官方要求公网 https、不带参数）。</summary>
    [JsonPropertyName("notify_url")] public string? NotifyUrl { get; set; }

    /// <summary>用户在服务商 appid 下的标识（官方标必填）。<b>不得进日志。</b></summary>
    [JsonPropertyName("openid")] public string? OpenId { get; set; }

    /// <summary>用户在子商户 appid 下的标识（官方标选填）。<b>不得进日志。</b></summary>
    [JsonPropertyName("sub_openid")] public string? SubOpenId { get; set; }

    /// <summary>车位编号（PARKING SPACE 场景用，官方标必填）。<b>不得进日志。</b></summary>
    [JsonPropertyName("space_number")] public string? SpaceNumber { get; set; }

    /// <summary>签名（由传输层签名 Handler 计算，调用方无需赋值）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 车主服务用户入场通知应答（服务商侧）。
/// </summary>
/// <remarks>
/// <para><b>官方返回表不列 <c>sub_appid</c>/<c>sub_mch_id</c></b>：与同族的服务商扣款申请
/// 与状态查询两支（两者返回表都有）不同 —— 本支照抄官方，<b>不</b>按「服务商统一形状」补齐；
/// 若实际应答带了这两个字段，会被 <c>IndexedFields</c> 之类的扩展袋吞掉，故此处刻意不建扩展袋、
/// 也不建这两个属性。</para>
/// <para><b>示例别名同直连版</b>：官方示例用 <c>user_status</c>，返回表用 <c>user_state</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PartnerVehicleNotificationResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    [JsonPropertyName("appid")] public string? AppId { get; set; }

    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>应答签名（必填，须验签）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }

    /// <summary>业务结果：SUCCESS / FAIL。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }

    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }

    /// <summary>用户状态：<c>NORMAL</c> 可用 / <c>BLOCKED</c> 不可用。</summary>
    [JsonPropertyName("user_state")] public string? UserState { get; set; }

    /// <summary><b>示例别名</b>（返回表写 <c>user_state</c>）⇒ 宽松解析用。</summary>
    [JsonPropertyName("user_status")] public string? UserStatus { get; set; }

    /// <summary>发起扣费方式：<c>PROACTIVE</c> / <c>AUTOPAY</c>。</summary>
    [JsonPropertyName("deduct_mode")] public string? DeductMode { get; set; }
}

/// <summary>
/// APIv2 车主服务申请扣款（<c>/vehicle/partnerpay/payapply</c>，服务商侧）请求体。
/// </summary>
/// <remarks>
/// <para><b>请求面没有 <c>sub_openid</c></b>：官方服务商 payapply 参数表只有
/// <c>openid</c>（特定场景必填），与另两支不同 ⇒ 照抄不得补（这是本族三支之间最不直观的差异）。</para>
/// <para><b><c>version</c> 描述在本页只写「固定值 3.0」</b>（直连页写「3.0 车场 / 2.0 其他」）
/// ⇒ 官方两页措辞不一致，本类不据此放宽：调用方仍应按场景取值。</para>
/// <para><b>与直连版同享的业务红线</b>：受理成功 ≠ 扣款成功（以支付通知为准）、同一
/// <c>out_trade_no</c> 只能在一种扣费方式使用、垫资商户 200ms 抬杆 / 非垫资需等通知、
/// 只支持蓝牌绿牌、高速 ETC 需 48 小时内送达。错误码集与直连版同为 17 项。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PartnerVehiclePayApplyRequest
{
    /// <summary>服务商商户号绑定的 APPID。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }

    /// <summary>子商户公众账号 Id（官方必填，需在服务商商户平台为子商户绑定）。</summary>
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }

    /// <summary>服务商商户号。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>子商户号（官方必填）。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>随机字符串，不长于 32 位。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名类型：官方必填、默认 <c>HMAC-SHA256</c> ⇒ 默认值即契约面，勿清空。</summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; } = "HMAC-SHA256";

    /// <summary>商品描述（String(128)）。</summary>
    [JsonPropertyName("body")] public string? Body { get; set; }

    /// <summary>商品详情（String(6000)）。</summary>
    [JsonPropertyName("detail")] public string? Detail { get; set; }

    /// <summary>附加数据（String(127)），原样返回。</summary>
    [JsonPropertyName("attach")] public string? Attach { get; set; }

    /// <summary>商户系统订单号（同一单号只能在一种扣费方式使用）。</summary>
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }

    /// <summary>总金额，单位<b>分</b>（官方类型 int）。</summary>
    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }

    /// <summary>货币类型：ISO 4217 三位字母代码，默认 <c>CNY</c>。</summary>
    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }

    /// <summary>终端 IP：API 机器交换 IP。</summary>
    [JsonPropertyName("spbill_create_ip")] public string? SpbillCreateIp { get; set; }

    /// <summary>商品标记（代金券或立减优惠参数）。</summary>
    [JsonPropertyName("goods_tag")] public string? GoodsTag { get; set; }

    /// <summary>异步接收支付结果通知的回调地址（官方必填）。</summary>
    [JsonPropertyName("notify_url")] public string? NotifyUrl { get; set; }

    /// <summary>交易类型：官方固定为 <c>PAP</c>。</summary>
    [JsonPropertyName("trade_type")] public string? TradeType { get; set; }

    /// <summary>版本号：官方本页「固定值 3.0」。</summary>
    [JsonPropertyName("version")] public string? Version { get; set; }

    /// <summary>交易场景：<c>PARKING</c> / <c>PARKING SPACE</c> / <c>GAS</c> / <c>HIGHWAY</c> / <c>BRIDGE</c>。</summary>
    [JsonPropertyName("trade_scene")] public string? TradeScene { get; set; }

    /// <summary>用户标识：特定场景必填。<b>不得进日志。</b></summary>
    [JsonPropertyName("openid")] public string? OpenId { get; set; }

    /// <summary>分账标识：大写 <c>Y</c>／<c>N</c>，默认不分账。</summary>
    [JsonPropertyName("profit_sharing")] public string? ProfitSharing { get; set; }

    /// <summary>场景信息：<b>JSON 字符串</b>（String(1024)）。</summary>
    [JsonPropertyName("scene_info")] public string? SceneInfo { get; set; }

    /// <summary>签名（由传输层签名 Handler 计算，调用方无需赋值）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 车主服务申请扣款应答（服务商侧）—— 同样只是「受理」回执。
/// </summary>
/// <remarks>
/// <para><b>与直连版的返回差异</b>：多 <c>sub_appid</c>（官方标<b>否</b>）与 <c>sub_mch_id</c>
/// （官方标<b>是</b>）—— 一个可空一个必填的不对称是官方原样，勿「顺手对齐」。
/// 其余字段面与直连版完全同一（10 行通用块）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PartnerVehiclePayApplyResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    [JsonPropertyName("appid")] public string? AppId { get; set; }

    /// <summary>子商户公众账号 Id（官方必填栏为「否」）。</summary>
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }

    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>子商户号（官方必填栏为「是」）。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    [JsonPropertyName("device_info")] public string? DeviceInfo { get; set; }

    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>应答签名（必填，须验签）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }

    /// <summary>业务结果：SUCCESS 仅表示<b>受理</b>成功 / FAIL。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }

    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
}

/// <summary>
/// APIv2 车主服务用户状态查询（<c>/vehicle/partnerpay/querystate</c>，服务商侧）请求体。
/// </summary>
/// <remarks>
/// <para><b>三个用户标识三选一</b>：官方把 <c>openid</c>／<c>sub_openid</c>／<c>plate_number</c>
/// 各自标注「三选一」（直连版只有两个字段、标「二选一」）。</para>
/// <para><b>本页 <c>trade_scene</c> 枚举少一档</b>：官方只列
/// <c>PARKING</c>／<c>PARKING SPACE</c>／<c>GAS</c>／<c>HIGHWAY</c>，
/// <b>不含</b> <c>BRIDGE</c> —— 而同族的服务商扣款申请页与直连状态查询页都含 <c>BRIDGE</c>。
/// 这是官方跨页不一致，本类按本页照抄并在接口注释留痕，<b>不做</b>「补齐成五档」的对齐。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PartnerVehicleStateQueryRequest
{
    /// <summary>服务商商户号绑定的 APPID。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }

    /// <summary>子商户公众账号 Id（官方必填，需在服务商商户平台为子商户绑定）。</summary>
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }

    /// <summary>服务商商户号。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>子商户号（官方必填）。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>随机字符串，不长于 32 位。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名类型：官方必填、默认 <c>HMAC-SHA256</c> ⇒ 默认值即契约面，勿清空。</summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; } = "HMAC-SHA256";

    /// <summary>交易场景：本页四档 <c>PARKING</c> / <c>PARKING SPACE</c> / <c>GAS</c> / <c>HIGHWAY</c>。</summary>
    [JsonPropertyName("trade_scene")] public string? TradeScene { get; set; }

    /// <summary>跳转场景：选填，不传默认小程序；支持 <c>APP</c> 与 <c>H5</c>。</summary>
    [JsonPropertyName("jump_scene")] public string? JumpScene { get; set; }

    /// <summary>用户在服务商 appid 下的标识：三选一。<b>不得进日志。</b></summary>
    [JsonPropertyName("openid")] public string? OpenId { get; set; }

    /// <summary>用户在子商户 appid 下的标识：三选一。<b>不得进日志。</b></summary>
    [JsonPropertyName("sub_openid")] public string? SubOpenId { get; set; }

    /// <summary>车牌号：仅「省份 + 车牌」；三选一。<b>不得进日志。</b></summary>
    [JsonPropertyName("plate_number")] public string? PlateNumber { get; set; }

    /// <summary>版本号：官方「固定值 3.0：车场停车（PARKING）场景 2.0：其他场景」，本页为必填。</summary>
    [JsonPropertyName("version")] public string? Version { get; set; }

    /// <summary>签名（由传输层签名 Handler 计算，调用方无需赋值）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 车主服务用户状态查询应答（服务商侧）。
/// </summary>
/// <remarks>
/// <para><b>官方本页把字段分成「通信结果 / 业务结果 / 明细」三段排版</b>，
/// 但结果码仍是 <c>return_code</c> + <c>result_code</c> 两级（三段只是「在什么条件下有值」的注释分组，
/// <b>不是</b>三级结果码），别据此设计嵌套 DTO。</para>
/// <para><b><c>deduct_mode</c> 有官方附加条件</b>：「传入 openid 时只返回 PROACTIVE」。</para>
/// <para><b>错误码比直连版多一个</b>：本页 5 项（多 <c>VEHICLE_NOT_EXIST</c>），直连版 4 项。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2PartnerVehicleStateQueryResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    [JsonPropertyName("appid")] public string? AppId { get; set; }

    /// <summary>子商户公众账号 Id（官方必填栏为「否」）。</summary>
    [JsonPropertyName("sub_appid")] public string? SubAppId { get; set; }

    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>子商户号。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>应答签名（必填，须验签）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }

    /// <summary>业务结果：SUCCESS / FAIL。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }

    /// <summary>错误代码描述（官方本页标注 String(128)）。</summary>
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }

    /// <summary>用户状态：<c>NORMAL</c> / <c>PAUSED</c> / <c>OVERDUE</c> / <c>UNAUTHORIZED</c>。</summary>
    [JsonPropertyName("user_state")] public string? UserState { get; set; }

    /// <summary><b>示例别名</b>（返回表写 <c>user_state</c>）⇒ 宽松解析用。</summary>
    [JsonPropertyName("user_status")] public string? UserStatus { get; set; }

    /// <summary>用户在服务商 appid 下的标识（入驻车主服务时返回）。<b>不得进日志。</b></summary>
    [JsonPropertyName("openid")] public string? OpenId { get; set; }

    /// <summary>用户在子商户 appid 下的标识。<b>不得进日志。</b></summary>
    [JsonPropertyName("sub_openid")] public string? SubOpenId { get; set; }

    /// <summary>发起扣费方式：<c>PROACTIVE</c> / <c>AUTOPAY</c>（传 openid 时只返回 PROACTIVE）。</summary>
    [JsonPropertyName("deduct_mode")] public string? DeductMode { get; set; }

    /// <summary>跳转车主小程序的页面路径（String(1024)）；非空时需引导用户授权。</summary>
    [JsonPropertyName("path")] public string? Path { get; set; }

    /// <summary>
    /// 车牌信息：<b>JSON 数组字符串</b>（String(512)）；官方注明 PARKING SPACE 场景下无返回。
    /// 子字段 <c>plate_number</c>／<c>channel_type</c>（仅高速，MTC／ETC／"ETC;MTC"）／
    /// <c>common_use_flag</c>（3.0 免密常用车牌）。<b>含车牌号，不落日志。</b>
    /// </summary>
    [JsonPropertyName("plate_number_info")] public string? PlateNumberInfo { get; set; }

    /// <summary><b>示例别名</b>（返回表写 <c>plate_number_info</c>）⇒ 宽松解析用。</summary>
    [JsonPropertyName("plate_number_list")] public string? PlateNumberList { get; set; }
}
