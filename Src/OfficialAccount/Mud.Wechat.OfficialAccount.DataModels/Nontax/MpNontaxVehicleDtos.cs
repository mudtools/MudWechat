// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Nontax;

/// <summary>
/// 「查询车辆状态」请求（官方 <c>vehicle/querystate</c>；<c>plate_number</c> 与 <c>openid</c> 二选一）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxVehicleQueryStateRequest
{
    /// <summary>获取或设置公众号 <c>appid</c>（官方 <c>appid</c>，可选）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置银行 ID（官方 <c>bank_id</c>，可选）。</summary>
    [JsonPropertyName("bank_id")]
    public string? BankId { get; set; }

    /// <summary>获取或设置银行账户（官方 <c>bank_account</c>，可选）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>获取或设置商户号（官方 <c>mch_id</c>，可选）。</summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>获取或设置地区编码（官方 <c>region_code</c>，必填）。</summary>
    [JsonPropertyName("region_code")]
    public string RegionCode { get; set; } = string.Empty;

    /// <summary>获取或设置交易场景（官方 <c>trade_scene</c>，必填）。</summary>
    [JsonPropertyName("trade_scene")]
    public string TradeScene { get; set; } = string.Empty;

    /// <summary>获取或设置跳转场景（官方 <c>jump_scene</c>，可选）。</summary>
    [JsonPropertyName("jump_scene")]
    public string? JumpScene { get; set; }

    /// <summary>获取或设置用户 OpenId（官方 <c>openid</c>，可选；与车牌二选一）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置车牌号（官方 <c>plate_number</c>，可选；与 OpenId 二选一）。</summary>
    [JsonPropertyName("plate_number")]
    public string? PlateNumber { get; set; }

    /// <summary>获取或设置进区材料信息（官方 <c>material_info</c>，可选）。</summary>
    [JsonPropertyName("material_info")]
    public string? MaterialInfo { get; set; }

    /// <summary>获取或设置渠道类型（官方 <c>channel_type</c>，可选）。</summary>
    [JsonPropertyName("channel_type")]
    public string? ChannelType { get; set; }
}

/// <summary>
/// 「查询车辆状态」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxVehicleQueryStateResponse : MpResponse
{
    /// <summary>获取或设置用户状态（官方 <c>user_state</c>）。</summary>
    [JsonPropertyName("user_state")]
    public string? UserState { get; set; }

    /// <summary>获取或设置用户 OpenId（官方 <c>openid</c>，可选）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置跳转路径（官方 <c>path</c>，可选）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>获取或设置车牌信息列表（官方 <c>plate_number_info</c>，可选）。</summary>
    [JsonPropertyName("plate_number_info")]
    public string[]? PlateNumberInfo { get; set; }

    /// <summary>获取或设置授权信息（官方 <c>authorize_package</c>；键值对形态）。</summary>
    [JsonPropertyName("authorize_package")]
    public Dictionary<string, string>? AuthorizePackage { get; set; }
}

/// <summary>
/// 车辆入场场景信息（官方 <c>scene_info</c>；entrancenotify 形态）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxVehicleEntranceScene
{
    /// <summary>获取或设置入场时间（官方 <c>start_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long StartTime { get; set; }

    /// <summary>获取或设置车牌号（官方 <c>plate_number</c>，必填）。</summary>
    [JsonPropertyName("plate_number")]
    public string PlateNumber { get; set; } = string.Empty;

    /// <summary>获取或设置车型（官方 <c>car_type</c>，可选）。</summary>
    [JsonPropertyName("car_type")]
    public string? CarType { get; set; }

    /// <summary>获取或设置通知地址（官方 <c>notify_url</c>，可选）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>获取或设置停车场名称（官方 <c>parking_name</c>，可选）。</summary>
    [JsonPropertyName("parking_name")]
    public string? ParkingName { get; set; }

    /// <summary>获取或设置用户 OpenId（官方 <c>openid</c>，可选）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置免费时长（官方 <c>free_time</c>，单位秒，可选）。</summary>
    [JsonPropertyName("free_time")]
    public int? FreeTime { get; set; }
}

/// <summary>
/// 「车辆入场通知」请求（官方 <c>vehicle/entrancenotify</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxVehicleEntranceNotifyRequest
{
    /// <summary>获取或设置公众号 <c>appid</c>（官方 <c>appid</c>，可选）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置银行 ID（官方 <c>bank_id</c>，可选）。</summary>
    [JsonPropertyName("bank_id")]
    public string? BankId { get; set; }

    /// <summary>获取或设置银行账户（官方 <c>bank_account</c>，可选）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>获取或设置商户号（官方 <c>mch_id</c>，可选）。</summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>获取或设置地区编码（官方 <c>region_code</c>，必填）。</summary>
    [JsonPropertyName("region_code")]
    public string RegionCode { get; set; } = string.Empty;

    /// <summary>获取或设置交易场景（官方 <c>trade_scene</c>，必填）。</summary>
    [JsonPropertyName("trade_scene")]
    public string TradeScene { get; set; } = string.Empty;

    /// <summary>获取或设置入场场景信息（官方 <c>scene_info</c>，必填）。</summary>
    [JsonPropertyName("scene_info")]
    public MpNontaxVehicleEntranceScene SceneInfo { get; set; } = new();
}

/// <summary>
/// 「车辆入场通知」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxVehicleEntranceNotifyResponse : MpResponse
{
    /// <summary>获取或设置用户状态（官方 <c>user_state</c>）。</summary>
    [JsonPropertyName("user_state")]
    public string? UserState { get; set; }
}

/// <summary>
/// 车辆出场场景信息（官方 <c>scene_info</c>；payapply 形态）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxVehiclePayScene
{
    /// <summary>获取或设置入场时间（官方 <c>start_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long StartTime { get; set; }

    /// <summary>获取或设置出场时间（官方 <c>end_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置充电时长（官方 <c>charging_time</c>，可选）。</summary>
    [JsonPropertyName("charging_time")]
    public int? ChargingTime { get; set; }

    /// <summary>获取或设置车牌号（官方 <c>plate_number</c>，必填）。</summary>
    [JsonPropertyName("plate_number")]
    public string PlateNumber { get; set; } = string.Empty;

    /// <summary>获取或设置车型（官方 <c>car_type</c>，可选）。</summary>
    [JsonPropertyName("car_type")]
    public string? CarType { get; set; }

    /// <summary>获取或设置停车场名称（官方 <c>parking_name</c>，可选）。</summary>
    [JsonPropertyName("parking_name")]
    public string? ParkingName { get; set; }

    /// <summary>获取或设置车位号（官方 <c>space_number</c>，可选）。</summary>
    [JsonPropertyName("space_number")]
    public string? SpaceNumber { get; set; }

    /// <summary>获取或设置用户 OpenId（官方 <c>openid</c>，可选）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置入口名称（官方 <c>entrance_name</c>，可选）。</summary>
    [JsonPropertyName("entrance_name")]
    public string? EntranceName { get; set; }

    /// <summary>获取或设置出口名称（官方 <c>exit_name</c>，可选）。</summary>
    [JsonPropertyName("exit_name")]
    public string? ExitName { get; set; }

    /// <summary>获取或设置承载量（官方 <c>carrying_capacity</c>，可选）。</summary>
    [JsonPropertyName("carrying_capacity")]
    public string? CarryingCapacity { get; set; }

    /// <summary>获取或设置承载范围（官方 <c>carrying_range</c>，可选）。</summary>
    [JsonPropertyName("carrying_range")]
    public string? CarryingRange { get; set; }

    /// <summary>获取或设置渠道类型（官方 <c>channel_type</c>，可选）。</summary>
    [JsonPropertyName("channel_type")]
    public string? ChannelType { get; set; }
}

/// <summary>
/// 「车辆支付申请」请求（官方 <c>vehicle/payapply</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxVehiclePayApplyRequest
{
    /// <summary>获取或设置公众号 <c>appid</c>（官方 <c>appid</c>，可选）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置银行 ID（官方 <c>bank_id</c>，可选）。</summary>
    [JsonPropertyName("bank_id")]
    public string? BankId { get; set; }

    /// <summary>获取或设置银行账户（官方 <c>bank_account</c>，可选）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>获取或设置商户号（官方 <c>mch_id</c>，可选）。</summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>获取或设置商品描述（官方 <c>desc</c>，必填）。</summary>
    [JsonPropertyName("desc")]
    public string Description { get; set; } = string.Empty;

    /// <summary>获取或设置订单总金额（官方 <c>fee</c>，单位分，必填）。</summary>
    [JsonPropertyName("fee")]
    public int Fee { get; set; }

    /// <summary>获取或设置商户订单号（官方 <c>order_no</c>，可选）。</summary>
    [JsonPropertyName("order_no")]
    public string? OrderNo { get; set; }

    /// <summary>获取或设置通知书类型（官方 <c>payment_notice_type</c>，可选）。</summary>
    [JsonPropertyName("payment_notice_type")]
    public int? PaymentNoticeType { get; set; }

    /// <summary>获取或设置通知书号（官方 <c>payment_notice_no</c>，可选）。</summary>
    [JsonPropertyName("payment_notice_no")]
    public string? PaymentNoticeNo { get; set; }

    /// <summary>获取或设置执收单位编码（官方 <c>department_code</c>，必填）。</summary>
    [JsonPropertyName("department_code")]
    public string DepartmentCode { get; set; } = string.Empty;

    /// <summary>获取或设置执收单位名称（官方 <c>department_name</c>，必填）。</summary>
    [JsonPropertyName("department_name")]
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>获取或设置地区编码（官方 <c>region_code</c>，可选）。</summary>
    [JsonPropertyName("region_code")]
    public string? RegionCode { get; set; }

    /// <summary>获取或设置商品标记（官方 <c>goods_tag</c>，可选）。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>获取或设置交易场景（官方 <c>trade_scene</c>，必填）。</summary>
    [JsonPropertyName("trade_scene")]
    public string TradeScene { get; set; } = string.Empty;

    /// <summary>获取或设置出场场景信息（官方 <c>scene_info</c>，必填）。</summary>
    [JsonPropertyName("scene_info")]
    public MpNontaxVehiclePayScene SceneInfo { get; set; } = new();
}

/// <summary>
/// 「车辆支付申请」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxVehiclePayApplyResponse : MpResponse
{
    /// <summary>获取或设置非税订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}
