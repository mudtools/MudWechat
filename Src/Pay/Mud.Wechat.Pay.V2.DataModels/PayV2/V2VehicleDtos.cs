// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 车主服务用户入场通知（<c>/vehicle/pay/notification</c>，直连商户侧）请求体。
/// </summary>
/// <remarks>
/// <para><b><c>scene_info</c> 是 JSON 字符串袋，不是 XML 子树</b>：官方请求示例把
/// <c>{"scene_info":{…}}</c> 整段塞进一个节点 ⇒ 场景子字段（<c>start_time</c>／
/// <c>plate_number</c>／<c>free_time</c>／<c>car_type</c>／<c>parking_name</c>，
/// 车位场景另加 <c>space_number</c>／<c>openid</c>）由调用方自行拼 JSON，
/// 本类只提供 <see cref="SceneInfo"/> 一个字符串字段。官方参数表把这些子字段平铺在同一张表里
/// 只是排版（缩进子行），<b>不是</b> XML 顶层元素 —— 误当顶层字段上传即 <c>PARAM_ERROR</c>。</para>
/// <para><b><c>sign_type</c> 必填且默认即算法</b>：官方「签名类型，默认为HMAC-SHA256」，
/// 本页不给 MD5 备选 ⇒ 默认值钉为 <c>HMAC-SHA256</c>（清空即被签名器按 MD5 计算 ⇒ 必然 <c>SIGNERROR</c>）。</para>
/// <para><b><c>version</c> 是「固定值」字段</b>：官方「3.0：车场停车（PARKING）场景 2.0：其他场景」
/// ⇒ 按 <see cref="TradeScene"/> 取值，不能一律填 3.0。</para>
/// <para><b>业务语义（官方原文）</b>：调用本接口<b>微信侧不会向用户下发入场通知消息，消息由商户下发</b>；
/// 车场停车场景<b>必须</b>调用本接口（注册车牌状态变更通知）。本页未写频率上限与幂等口径。</para>
/// <para><b><c>notify_url</c> 的排版歧义留痕</b>：官方把它印在与 <c>scene_info</c> 同一张表内
/// （必填栏「否」），但场景子字段清单里不列它、请求示例里也不含它 ⇒ 本类按
/// 「XML 顶层选填字段」建模（表成员即契约面）；若实测须进 JSON 袋，由调用方移入
/// <see cref="SceneInfo"/>，SDK 不猜。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2VehicleNotificationRequest
{
    /// <summary>微信支付分配的公众账号 Id，<b>此 appid 需为已授权 appid</b>（官方原文）。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }

    /// <summary>微信支付分配的商户号。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>随机字符串，不长于 32 位。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名类型：官方必填、默认 <c>HMAC-SHA256</c> ⇒ 默认值即契约面，勿清空。</summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; } = "HMAC-SHA256";

    /// <summary>版本号：官方「固定值 3.0：车场停车（PARKING）场景 2.0：其他场景」。</summary>
    [JsonPropertyName("version")] public string? Version { get; set; }

    /// <summary>交易场景：委托代扣的场景值，目前支持 <c>PARKING</c>（车场停车）与 <c>PARKING SPACE</c>（车位停车）。</summary>
    [JsonPropertyName("trade_scene")] public string? TradeScene { get; set; }

    /// <summary>
    /// 场景信息：<b>JSON 字符串</b>（String(1024)），不同场景装不同子字段
    /// （PARKING：<c>start_time</c>/<c>plate_number</c>/<c>parking_name</c>/<c>free_time</c>，
    /// 可选 <c>car_type</c>；PARKING SPACE：另含 <c>space_number</c>/<c>openid</c>）。
    /// </summary>
    [JsonPropertyName("scene_info")] public string? SceneInfo { get; set; }

    /// <summary>
    /// 回调通知 URL：用于接收当前处于停车场内所有车辆的车牌状态变更通知。
    /// 官方要求公网域名必须为 https、URL 不可携带参数（专线接入或私有回调域名可用 http）。
    /// </summary>
    [JsonPropertyName("notify_url")] public string? NotifyUrl { get; set; }

    /// <summary>签名（由传输层签名 Handler 按字段表计算，调用方无需赋值）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 车主服务用户入场通知应答。
/// </summary>
/// <remarks>
/// <para><b>结果码只有两级</b>：<c>return_code</c> + <c>result_code</c>，加
/// <c>err_code</c>/<c>err_code_des</c>，没有第三级成功块。</para>
/// <para><b>官方表格与示例自相矛盾（照抄留痕）</b>：返回表写 <c>user_state</c>，
/// 官方应答示例却印 <c>user_status</c> ⇒ 两个节点都建模、解析保持宽松，
/// 以 <see cref="UserState"/> 为契约面、<see cref="UserStatus"/> 为示例别名。</para>
/// <para><b><c>openid</c> 属用户身份标识</b>，不得进日志、遥测或异常消息（PAY-B7 同线口径）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2VehicleNotificationResponse
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

    /// <summary>错误代码描述（官方本页标注 String(32)，与同族另两页的 String(128) 不一致，照抄）。</summary>
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }

    /// <summary>用户状态：<c>NORMAL</c> 可使用车主服务 / <c>BLOCKED</c> 不可使用。</summary>
    [JsonPropertyName("user_state")] public string? UserState { get; set; }

    /// <summary>
    /// <b>示例别名</b>：官方应答示例用的节点名（返回表写的是 <c>user_state</c>）⇒ 宽松解析用，勿主动读。
    /// </summary>
    [JsonPropertyName("user_status")] public string? UserStatus { get; set; }

    /// <summary>发起扣费方式：<c>PROACTIVE</c> 用户主动免密 / <c>AUTOPAY</c> 用户无感支付。</summary>
    [JsonPropertyName("deduct_mode")] public string? DeductMode { get; set; }
}

/// <summary>
/// APIv2 车主服务申请扣款（<c>/vehicle/pay/payapply</c>，直连商户侧）请求体。
/// </summary>
/// <remarks>
/// <para><b>受理成功 ≠ 扣款成功</b>（官方原文「以支付通知为准」）：本支返回只表示请求被受理，
/// 未开通垫资的商户必须等异步通知再放行车辆（官方口径：垫资商户 200ms 抬杆、非垫资 5s 等通知，
/// 以受理成功放行造成的资金损失由商户自行承担）⇒ 不要把本应答当扣款凭证。</para>
/// <para><b>同一 <c>out_trade_no</c> 只能在一种扣费方式使用</b>：官方明示并发下车主服务扣款
/// 「可能仅受理成功，但不继续执行扣款动作」⇒ 幂等键复用是商户侧责任，SDK 不代做去重。</para>
/// <para><b><c>total_fee</c> 是整数分</b>（官方类型 <c>int</c>，非字符串）。</para>
/// <para><b><c>openid</c> 条件必填</b>：官方「<c>PARKING SPACE</c> 或 <c>PROACTIVE</c> 时必填」。</para>
/// <para><b><c>profit_sharing</c> 取大写 <c>Y</c>/<c>N</c></b>，默认不分账。</para>
/// <para><b><c>scene_info</c> 同为 JSON 字符串袋</b>（String(1024)）：五个场景
/// （<c>PARKING</c>/<c>PARKING SPACE</c>/<c>GAS</c>/<c>HIGHWAY</c>/<c>BRIDGE</c>）
/// 各自的必填子字段集不同 —— PARKING 需 <c>start_time</c>/<c>charging_time</c>/
/// <c>plate_number</c>/<c>parking_name</c>/<c>deduct_mode</c>/<c>support_deduct_mode</c>；
/// GAS 需 <c>gas_station</c>/<c>gas_label_name</c>/<c>gas_type</c>/<c>gas_standard</c>/
/// <c>gas_amount</c>(ml)/<c>gas_gun_no</c>；HIGHWAY 需 <c>start_time</c>/<c>end_time</c>/
/// <c>plate_number</c>/<c>car_type</c>/<c>entrance_name</c>(60)/<c>exit_name</c>(90)/
/// <c>channel_type</c>；BRIDGE 需 <c>exit_name</c>(64，官方要求必须展示)。
/// <c>deduct_mode</c> 与 <c>support_deduct_mode</c> 在各场景均必填，但<b>官方示例报文里也没写</b>
/// （文档自相矛盾，已在此留痕）⇒ 以参数表为准。</para>
/// <para><b>高速 ETC 时限</b>：官方「高速场景，ETC 通道需要在 48 小时内将交易送达」
/// （超时对应 <c>TRADE_TIME_INVALID</c>）；<c>channel_type</c> 必须与车牌通道标识一致
/// （不一致为 <c>CHANNEL_TYPE_INVALID</c>）；车牌只支持蓝牌、绿牌。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2VehiclePayApplyRequest
{
    /// <summary>微信支付分配的公众账号 Id（需已授权）。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }

    /// <summary>微信支付分配的商户号。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>随机字符串，不长于 32 位。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名类型：官方必填、默认 <c>HMAC-SHA256</c> ⇒ 默认值即契约面，勿清空。</summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; } = "HMAC-SHA256";

    /// <summary>商品描述（简要描述，String(128)）。</summary>
    [JsonPropertyName("body")] public string? Body { get; set; }

    /// <summary>商品详情（String(6000)，明细列表）。</summary>
    [JsonPropertyName("detail")] public string? Detail { get; set; }

    /// <summary>附加数据（String(127)），异步通知与查询原样返回。</summary>
    [JsonPropertyName("attach")] public string? Attach { get; set; }

    /// <summary>商户系统订单号（同一次扣款唯一；同一单号只能在一种扣费方式使用）。</summary>
    [JsonPropertyName("out_trade_no")] public string? OutTradeNo { get; set; }

    /// <summary>总金额，单位<b>分</b>（官方类型为 int 整数，非字符串）。</summary>
    [JsonPropertyName("total_fee")] public int? TotalFee { get; set; }

    /// <summary>货币类型：符合 ISO 4217 标准的三位字母代码，默认人民币 <c>CNY</c>。</summary>
    [JsonPropertyName("fee_type")] public string? FeeType { get; set; }

    /// <summary>终端 IP：API 机器交换 IP。</summary>
    [JsonPropertyName("spbill_create_ip")] public string? SpbillCreateIp { get; set; }

    /// <summary>商品标记（代金券或立减优惠功能的参数）。</summary>
    [JsonPropertyName("goods_tag")] public string? GoodsTag { get; set; }

    /// <summary>异步接收微信支付结果通知的回调地址（官方必填）。</summary>
    [JsonPropertyName("notify_url")] public string? NotifyUrl { get; set; }

    /// <summary>交易类型：官方固定为 <c>PAP</c>（微信委托代扣支付）。</summary>
    [JsonPropertyName("trade_type")] public string? TradeType { get; set; }

    /// <summary>版本号：官方「固定值 3.0：车场停车（PARKING）场景 2.0：其他场景」。</summary>
    [JsonPropertyName("version")] public string? Version { get; set; }

    /// <summary>交易场景：<c>PARKING</c> / <c>PARKING SPACE</c> / <c>GAS</c> / <c>HIGHWAY</c> / <c>BRIDGE</c>。</summary>
    [JsonPropertyName("trade_scene")] public string? TradeScene { get; set; }

    /// <summary>用户标识：官方条件必填 —— <c>PARKING SPACE</c> 场景或 <c>PROACTIVE</c> 扣费方式下必填。<b>不得进日志。</b></summary>
    [JsonPropertyName("openid")] public string? OpenId { get; set; }

    /// <summary>分账标识：大写 <c>Y</c>／<c>N</c>，默认不分账。</summary>
    [JsonPropertyName("profit_sharing")] public string? ProfitSharing { get; set; }

    /// <summary>场景信息：<b>JSON 字符串</b>（String(1024)），子字段随 <see cref="TradeScene"/> 变化。</summary>
    [JsonPropertyName("scene_info")] public string? SceneInfo { get; set; }

    /// <summary>签名（由传输层签名 Handler 计算，调用方无需赋值）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 车主服务申请扣款应答 —— <b>只是「受理」回执，不是扣款结果</b>。
/// </summary>
/// <remarks>
/// <para>本支应答体没有任何金额或交易状态字段：官方返回表就通用 10 行
/// （<c>return_code</c>/<c>return_msg</c>/<c>appid</c>/<c>mch_id</c>/<c>device_info</c>/
/// <c>nonce_str</c>/<c>sign</c>/<c>result_code</c>/<c>err_code</c>/<c>err_code_des</c>）
/// ⇒ 扣款结果必须走异步通知或订单查询，<b>不能</b>据 <c>result_code=SUCCESS</c> 放行车辆。</para>
/// <para><b>本页错误码是下划线形态的 <c>SIGN_ERROR</c></b>（示例值即写 <c>SIGN_ERROR</c>），
/// 与基础支付各族的 <c>SIGNERROR</c> 不同；本族共 17 个错误码，
/// 其中 <c>SYSTEMERROR</c> 官方给出 10 次退避序列 30s,60s,60s,120s,120s,180s,180s,360s,360s,600s。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2VehiclePayApplyResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    [JsonPropertyName("appid")] public string? AppId { get; set; }

    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>设备号（官方说明：PC 网站可填写 "WEB"）。</summary>
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
/// APIv2 车主服务用户状态查询（<c>/vehicle/pay/querystate</c>，直连商户侧）请求体。
/// </summary>
/// <remarks>
/// <para><b><c>openid</c> 与 <c>plate_number</c> 二选一</b>（官方各自标注「必传其一」）：
/// 两者都不传不是合法请求。</para>
/// <para><b>本页请求面没有 <c>scene_info</c>／<c>notify_url</c></b>：只多一个选填
/// <c>jump_scene</c>（不传默认小程序，支持 <c>APP</c> 与公众号 <c>H5</c>）。</para>
/// <para><b>应答里的 <c>path</c> 是引导入口</b>：用户状态为 <c>OVERDUE</c>／<c>UNAUTHORIZED</c>
/// 时官方返回跳转路径，商户须引导用户跳转授权后才能继续扣款。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2VehicleStateQueryRequest
{
    /// <summary>微信支付分配的公众账号 Id（需已授权）。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }

    /// <summary>微信支付分配的商户号。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>随机字符串，不长于 32 位。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名类型：官方必填、默认 <c>HMAC-SHA256</c> ⇒ 默认值即契约面，勿清空。</summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; } = "HMAC-SHA256";

    /// <summary>交易场景：<c>PARKING</c> / <c>PARKING SPACE</c> / <c>GAS</c> / <c>HIGHWAY</c> / <c>BRIDGE</c>。</summary>
    [JsonPropertyName("trade_scene")] public string? TradeScene { get; set; }

    /// <summary>跳转场景：选填，不传默认小程序；支持 <c>APP</c> 与 <c>H5</c>。</summary>
    [JsonPropertyName("jump_scene")] public string? JumpScene { get; set; }

    /// <summary>用户标识：与 <see cref="PlateNumber"/> 二选一。<b>不得进日志。</b></summary>
    [JsonPropertyName("openid")] public string? OpenId { get; set; }

    /// <summary>车牌号：仅「省份 + 车牌」，不含特殊字符；与 <see cref="OpenId"/> 二选一。</summary>
    [JsonPropertyName("plate_number")] public string? PlateNumber { get; set; }

    /// <summary>版本号：官方「固定值 3.0：车场停车（PARKING）场景 2.0：其他场景」，本页为必填。</summary>
    [JsonPropertyName("version")] public string? Version { get; set; }

    /// <summary>签名（由传输层签名 Handler 计算，调用方无需赋值）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 车主服务用户状态查询应答。
/// </summary>
/// <remarks>
/// <para><b><c>user_state</c> 四档</b>：<c>NORMAL</c> 正常 / <c>PAUSED</c> 暂停 /
/// <c>OVERDUE</c> 欠费 / <c>UNAUTHORIZED</c> 未授权 —— 与入场通知的两档
/// （<c>NORMAL</c>/<c>BLOCKED</c>）<b>不是同一套枚举</b>，两页 DTO 不得共用注释。</para>
/// <para><b>示例别名照抄留痕</b>：官方应答示例印 <c>user_status</c> 与 <c>plate_number_list</c>，
/// 返回表写 <c>user_state</c> 与 <c>plate_number_info</c> ⇒ 四个节点都建模、解析宽松。</para>
/// <para><b><c>plate_number_info</c> 是 JSON 数组字符串</b>（String(512)，形如
/// <c>{"plate_number_info":[{"plate_number":"粤B888888","channel_type":"ETC"}]}</c>），
/// 子字段为 <c>plate_number</c>／<c>channel_type</c>（仅高速场景，取值 MTC／ETC／"ETC;MTC"）／
/// <c>common_use_flag</c>（3.0 接口下免密常用车牌才返回）⇒ 本 SDK 原样交回字符串，不猜结构。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2VehicleStateQueryResponse
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

    /// <summary>错误代码描述（官方本页标注 String(128)）。</summary>
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }

    /// <summary>用户状态：<c>NORMAL</c> / <c>PAUSED</c> / <c>OVERDUE</c> / <c>UNAUTHORIZED</c>。</summary>
    [JsonPropertyName("user_state")] public string? UserState { get; set; }

    /// <summary><b>示例别名</b>（返回表写 <c>user_state</c>）⇒ 宽松解析用。</summary>
    [JsonPropertyName("user_status")] public string? UserStatus { get; set; }

    /// <summary>用户标识（入驻车主服务时返回）。<b>不得进日志。</b></summary>
    [JsonPropertyName("openid")] public string? OpenId { get; set; }

    /// <summary>发起扣费方式：<c>PROACTIVE</c> / <c>AUTOPAY</c>。</summary>
    [JsonPropertyName("deduct_mode")] public string? DeductMode { get; set; }

    /// <summary>跳转车主小程序的页面路径（String(1024)）；状态异常时非空，商户须引导授权。</summary>
    [JsonPropertyName("path")] public string? Path { get; set; }

    /// <summary>车牌信息：<b>JSON 数组字符串</b>（String(512)）。<b>含车牌号，属车主关联信息，不落日志。</b></summary>
    [JsonPropertyName("plate_number_info")] public string? PlateNumberInfo { get; set; }

    /// <summary><b>示例别名</b>（返回表写 <c>plate_number_info</c>，示例里是逗号车牌串）⇒ 宽松解析用。</summary>
    [JsonPropertyName("plate_number_list")] public string? PlateNumberList { get; set; }
}
