namespace Mud.Wechat.Work.DataModels.OpenHardware;

/// <summary>
/// 更新设备状态请求体（openhw/device/report_device_status，官方 95983）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class ReportDeviceStatusRequest
{
    /// <summary>
    /// 获取或设置设备在线状态：1-在线，2-离线。
    /// </summary>
    [JsonPropertyName("online_status")]
    public int OnlineStatus { get; set; }

    /// <summary>
    /// 获取或设置设备当前固件版本（官方：最长 32 字节）。
    /// </summary>
    [JsonPropertyName("cur_version")]
    public string? CurVersion { get; set; }

    /// <summary>
    /// 获取或设置当前设备可升级的固件版本（最长 32 字节）。
    /// </summary>
    [JsonPropertyName("upgradable_version")]
    public string? UpgradableVersion { get; set; }

    /// <summary>
    /// 获取或设置可升级固件的简要描述（最长 256 字节）。
    /// </summary>
    [JsonPropertyName("upgradable_version_desc")]
    public string? UpgradableVersionDesc { get; set; }
}

/// <summary>
/// 上报设备固件升级结果请求体（openhw/device/report_firmware_upgrade_result，官方 95999）。
/// </summary>
/// <remarks>
/// <para><b>字段名陷阱</b>：<see cref="ErrCode"/> 是<b>请求体</b>里的「设备侧固件升级结果」
/// （0-成功、1-设备离线、其他为厂商自定义失败），与响应基类的接口调用返回码同名不同义，
/// 二者不得互相回填。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class ReportFirmwareUpgradeResultRequest
{
    /// <summary>
    /// 获取或设置固件升级事件中携带的操作 id。
    /// </summary>
    [JsonPropertyName("oper_id")]
    public string? OperId { get; set; }

    /// <summary>
    /// 获取或设置设备侧固件升级结果：0-升级成功，1-设备离线，其他失败值由厂商自定义。
    /// </summary>
    [JsonPropertyName("errcode")]
    public uint ErrCode { get; set; }

    /// <summary>
    /// 获取或设置升级结果描述（官方：失败时请简短描述失败原因）。
    /// </summary>
    [JsonPropertyName("errmsg")]
    public string? ErrMsg { get; set; }

    /// <summary>
    /// 获取或设置设备当前固件版本。
    /// </summary>
    [JsonPropertyName("cur_version")]
    public string? CurVersion { get; set; }
}

/// <summary>
/// 上报成员识别信息变化结果请求体（openhw/device/report_bio_info_result，官方 96000）。
/// </summary>
/// <remarks>
/// <para><b>官方二选一形态</b>：企业微信客户端发起录入时以 <see cref="OperId"/> 回填结果；
/// <c>oper_id</c> 不存在或为空时，改由 <see cref="OpenUserId"/> + <see cref="UserType"/> +
/// <see cref="BioInfoType"/> 三字段定位成员与生物类型。厂商不得同时省略。</para>
/// <para><b>个人敏感信息</b>：指纹 / 人脸属生物识别信息，本 SDK 只回传「录入结果」，
/// 不承载任何生物模板数据；<see cref="OpenUserId"/> 不得写入日志。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class ReportBioInfoResultRequest
{
    /// <summary>
    /// 获取或设置识别信息录入事件的操作 id（取自企业微信回调厂商的录入事件）。
    /// </summary>
    [JsonPropertyName("oper_id")]
    public string? OperId { get; set; }

    /// <summary>
    /// 获取或设置录入结果：0-录入成功，1-设备离线，其他失败值由厂商自定义。
    /// </summary>
    [JsonPropertyName("errcode")]
    public uint ErrCode { get; set; }

    /// <summary>
    /// 获取或设置录入结果描述（如设备离线、找不到人脸等）。
    /// </summary>
    [JsonPropertyName("errmsg")]
    public string? ErrMsg { get; set; }

    /// <summary>
    /// 获取或设置录入成员的 <c>open_userid</c>（仅 <c>oper_id</c> 不存在或为空时生效）。
    /// </summary>
    [JsonPropertyName("open_userid")]
    public string? OpenUserId { get; set; }

    /// <summary>
    /// 获取或设置成员类型：0-企业成员，2-家校学生（仅 <c>oper_id</c> 不存在或为空时生效）。
    /// </summary>
    [JsonPropertyName("user_type")]
    public uint? UserType { get; set; }

    /// <summary>
    /// 获取或设置生物信息类型：1-指纹，2-人脸（仅 <c>oper_id</c> 不存在或为空时生效）。
    /// </summary>
    [JsonPropertyName("bio_info_type")]
    public uint? BioInfoType { get; set; }
}

/// <summary>
/// 上报远程开门结果请求体（openhw/device/report_remote_open_result，官方 96048）。
/// </summary>
/// <remarks>
/// <para><b>字段名陷阱</b>：<see cref="ErrCode"/> 是设备侧指令执行结果（0-成功、1-失败），
/// 与响应基类返回码同名不同义。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class ReportRemoteOpenResultRequest
{
    /// <summary>
    /// 获取或设置远程开门指令的操作 id（取自企业微信回调厂商的开门事件）。
    /// </summary>
    [JsonPropertyName("oper_id")]
    public string? OperId { get; set; }

    /// <summary>
    /// 获取或设置上报类型（官方枚举字符串，如 <c>remote_open_door</c> 门禁 app 点击远程开门、
    /// <c>scan_qrcode_*</c> 扫码开门等；拼写照抄官方原文）。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置开门指令执行结果：0-成功，1-失败。
    /// </summary>
    [JsonPropertyName("errcode")]
    public uint ErrCode { get; set; }

    /// <summary>
    /// 获取或设置执行结果描述（如设备离线等）。
    /// </summary>
    [JsonPropertyName("errmsg")]
    public string? ErrMsg { get; set; }
}

/// <summary>
/// 上报门禁通行记录请求体（openhw/device/report_access_data，官方 95997）。
/// </summary>
/// <remarks><para>官方：<see cref="AccessData"/> 单次最多批量上报 100 条。</para></remarks>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class ReportAccessDataRequest
{
    /// <summary>
    /// 获取或设置门禁通行数据列表（最多 100 条）。
    /// </summary>
    [JsonPropertyName("access_data")]
    public IList<OpenHardwareAccessData>? AccessData { get; set; }
}

/// <summary>
/// 门禁通行记录（<b>个人信息</b>：成员身份 + 通行时间 + 通行方式，不得写入日志或遥测）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class OpenHardwareAccessData
{
    /// <summary>
    /// 获取或设置通行成员的 <c>open_userid</c>。
    /// </summary>
    [JsonPropertyName("open_userid")]
    public string? OpenUserId { get; set; }

    /// <summary>
    /// 获取或设置用户类型：0-企业成员；2-学生。
    /// </summary>
    [JsonPropertyName("user_type")]
    public uint UserType { get; set; }

    /// <summary>
    /// 获取或设置通过门禁的时间戳。
    /// </summary>
    [JsonPropertyName("timestamp")]
    public uint Timestamp { get; set; }

    /// <summary>
    /// 获取或设置通行类型：1-进门；2-出门；3-无法识别方向。
    /// </summary>
    [JsonPropertyName("pass_type")]
    public uint PassType { get; set; }

    /// <summary>
    /// 获取或设置通行方法：1-人脸；2-二维码；3-远程开门；4-指纹。
    /// </summary>
    [JsonPropertyName("pass_method")]
    public uint PassMethod { get; set; }
}

/// <summary>
/// 上报考勤打卡记录请求体（openhw/device/report_checkin_data，官方 95985）。
/// </summary>
/// <remarks><para>官方：<see cref="CheckinData"/> 单次最多批量上报 100 条。</para></remarks>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class ReportCheckinDataRequest
{
    /// <summary>
    /// 获取或设置打卡数据列表（最多 100 条）。
    /// </summary>
    [JsonPropertyName("checkin_data")]
    public IList<OpenHardwareCheckinData>? CheckinData { get; set; }
}

/// <summary>
/// 打卡记录（<b>个人信息</b>：成员身份 + 打卡时间，不得写入日志或遥测）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class OpenHardwareCheckinData
{
    /// <summary>
    /// 获取或设置打卡成员的 <c>open_userid</c>。
    /// </summary>
    [JsonPropertyName("open_userid")]
    public string? OpenUserId { get; set; }

    /// <summary>
    /// 获取或设置用户类型：0-企业成员；2-学生。
    /// </summary>
    [JsonPropertyName("user_type")]
    public uint UserType { get; set; }

    /// <summary>
    /// 获取或设置打卡时间戳。
    /// </summary>
    [JsonPropertyName("timestamp")]
    public uint Timestamp { get; set; }
}

/// <summary>
/// 上报体温检测记录请求体（openhw/device/report_temperature_data，官方 95986）。
/// </summary>
/// <remarks><para>官方：<see cref="TemperatureData"/> 单次最多批量上报 100 条。</para></remarks>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class ReportTemperatureDataRequest
{
    /// <summary>
    /// 获取或设置体温数据列表（最多 100 条）。
    /// </summary>
    [JsonPropertyName("temperature_data")]
    public IList<OpenHardwareTemperatureData>? TemperatureData { get; set; }
}

/// <summary>
/// 体温检测记录（<b>健康生理信息</b>：体温读数与异常状态属个人敏感信息，
/// 不得写入日志、遥测或异常消息）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class OpenHardwareTemperatureData
{
    /// <summary>
    /// 获取或设置成员 <c>open_userid</c>。
    /// </summary>
    [JsonPropertyName("open_userid")]
    public string? OpenUserId { get; set; }

    /// <summary>
    /// 获取或设置用户类型：0-企业成员；2-学生。
    /// </summary>
    [JsonPropertyName("user_type")]
    public uint UserType { get; set; }

    /// <summary>
    /// 获取或设置用户温度（官方字段类型为 <b>string</b>，非数值，照抄官方契约以免丢精度或格式）。
    /// </summary>
    [JsonPropertyName("temperature")]
    public string? Temperature { get; set; }

    /// <summary>
    /// 获取或设置测温时间戳。
    /// </summary>
    [JsonPropertyName("timestamp")]
    public uint Timestamp { get; set; }

    /// <summary>
    /// 获取或设置测温状态：0-正常，1-发热，2-温度过低。
    /// </summary>
    [JsonPropertyName("status")]
    public uint Status { get; set; }
}

/// <summary>
/// 批量上报类端点的失败项定位键（官方 95997 / 95985 的 <c>fail_list</c> 元素形态：
/// 仅回 <c>open_userid</c> / <c>user_type</c> / <c>timestamp</c> 三键定位）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class OpenHardwareFailedDataKey
{
    /// <summary>
    /// 获取或设置失败记录的成员 <c>open_userid</c>。
    /// </summary>
    [JsonPropertyName("open_userid")]
    public string? OpenUserId { get; set; }

    /// <summary>
    /// 获取或设置失败记录的成员类型：0-企业成员；2-学生。
    /// </summary>
    [JsonPropertyName("user_type")]
    public uint UserType { get; set; }

    /// <summary>
    /// 获取或设置失败记录的时间戳。
    /// </summary>
    [JsonPropertyName("timestamp")]
    public uint Timestamp { get; set; }
}

/// <summary>
/// 体温上报失败项（官方 95986 的 <c>fail_list</c> 元素<b>比门禁 / 打卡两族多印
/// </b><c>errcode</c> / <c>errmsg</c> 两键 ⇒ 逐页取证后独立建型，不与
/// <see cref="OpenHardwareFailedDataKey"/> 共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class OpenHardwareFailedTemperatureItem
{
    /// <summary>
    /// 获取或设置失败记录的成员 <c>open_userid</c>。
    /// </summary>
    [JsonPropertyName("open_userid")]
    public string? OpenUserId { get; set; }

    /// <summary>
    /// 获取或设置失败记录的成员类型：0-企业成员；2-学生。
    /// </summary>
    [JsonPropertyName("user_type")]
    public uint UserType { get; set; }

    /// <summary>
    /// 获取或设置失败记录的时间戳。
    /// </summary>
    [JsonPropertyName("timestamp")]
    public uint Timestamp { get; set; }

    /// <summary>
    /// 获取或设置官方为该条记录单独返回的失败码（与响应顶层 <c>errcode</c> 不同层：
    /// 顶层为 0 而本列表非空时表示<b>部分</b>记录失败）。
    /// </summary>
    [JsonPropertyName("errcode")]
    public int ErrCode { get; set; }

    /// <summary>
    /// 获取或设置该条记录的失败描述。
    /// </summary>
    [JsonPropertyName("errmsg")]
    public string? ErrMsg { get; set; }
}

/// <summary>
/// 门禁通行记录上报响应体（顶层 <c>errcode</c> 为 0 且 <see cref="FailList"/> 非空 ⇒ 部分记录失败）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class ReportAccessDataResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置上报失败列表（官方仅在存在失败项时返回）。
    /// </summary>
    [JsonPropertyName("fail_list")]
    public IList<OpenHardwareFailedDataKey>? FailList { get; set; }
}

/// <summary>
/// 考勤打卡记录上报响应体（官方 95985 明示：顶层 <c>errcode</c> 非 0 ⇒ 整个接口调用失败；
/// 顶层为 0 而本列表非空 ⇒ 部分数据上报失败）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class ReportCheckinDataResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置上报失败列表（官方仅在存在失败项时返回）。
    /// </summary>
    [JsonPropertyName("fail_list")]
    public IList<OpenHardwareFailedDataKey>? FailList { get; set; }
}

/// <summary>
/// 体温检测记录上报响应体（失败项携带逐条 errcode / errmsg）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class ReportTemperatureDataResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置上报失败列表（官方仅在存在失败项时返回）。
    /// </summary>
    [JsonPropertyName("fail_list")]
    public IList<OpenHardwareFailedTemperatureItem>? FailList { get; set; }
}