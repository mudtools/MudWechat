namespace Mud.Wechat.Work.DataModels.OpenHardware;

/// <summary>
/// 全量获取设备成员列表请求体（openhw/device/get_userinfo_by_page，官方 95984）。
/// </summary>
/// <remarks>
/// <para><b>官方短路语义</b>：<see cref="OnlyPermVersion"/> 为 <c>true</c> 时官方<b>忽略其余全部入参</b>，
/// 只返回云端通讯录版本号 <c>perm_version</c>。用于设备侧判断是否需要全量拉取。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class GetHardwareUserPageRequest
{
    /// <summary>
    /// 获取或设置分页游标（上一次调用返回的 <c>next_cursor</c>；首次调用可不填）。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置单次返回的最大记录数（官方：最大值 200，默认 100）。
    /// </summary>
    [JsonPropertyName("limit")]
    public uint? Limit { get; set; }

    /// <summary>
    /// 获取或设置是否仅获取 <c>perm_version</c>（为 <c>true</c> 时官方忽略其他入参）。
    /// </summary>
    [JsonPropertyName("only_perm_version")]
    public bool? OnlyPermVersion { get; set; }
}

/// <summary>
/// 按 id 列表获取设备成员信息请求体（openhw/device/get_userinfo_by_ids，官方 96037）。
/// </summary>
/// <remarks><para>官方：<see cref="UserItem"/> 最多 100 条。</para></remarks>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class GetHardwareUserByIdsRequest
{
    /// <summary>
    /// 获取或设置待查询成员列表（最多 100 条）。
    /// </summary>
    [JsonPropertyName("user_item")]
    public IList<OpenHardwareUserKey>? UserItem { get; set; }
}

/// <summary>
/// 设备成员查询键（官方 96037 的 <c>user_item</c> 数组元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class OpenHardwareUserKey
{
    /// <summary>
    /// 获取或设置成员 <c>open_userid</c>。
    /// </summary>
    [JsonPropertyName("open_userid")]
    public string? OpenUserId { get; set; }

    /// <summary>
    /// 获取或设置成员类型：0-企业成员；2-学生。
    /// </summary>
    [JsonPropertyName("user_type")]
    public uint UserType { get; set; }
}

/// <summary>
/// 全量获取设备成员列表响应体（分页游标 + 通讯录版本号 + 成员列表）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class GetHardwareUserPageResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置下一页游标（官方：无更多数据时返回空）。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>
    /// 获取或设置当前云端通讯录版本号（配合 <c>only_perm_version</c> 做增量判定）。
    /// </summary>
    [JsonPropertyName("perm_version")]
    public long PermVersion { get; set; }

    /// <summary>
    /// 获取或设置成员列表包装对象（官方在顶层包一层 <c>userinfo</c>）。
    /// </summary>
    [JsonPropertyName("userinfo")]
    public OpenHardwareUserInfo? UserInfo { get; set; }
}

/// <summary>
/// 按 id 列表获取设备成员信息响应体（官方本端点不返回游标与版本号）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class GetHardwareUserByIdsResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置成员列表包装对象（官方在顶层包一层 <c>userinfo</c>）。
    /// </summary>
    [JsonPropertyName("userinfo")]
    public OpenHardwareUserInfo? UserInfo { get; set; }
}

/// <summary>
/// 设备成员列表包装对象（官方字段名 <c>userinfo</c>，内含 <c>useritems</c>）。
/// </summary>
/// <remarks>
/// <para><b>个人信息约束</b>：<see cref="OpenHardwareUserItem.UserName"/> 为成员姓名，属用户身份信息，
/// 不得写入日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class OpenHardwareUserInfo
{
    /// <summary>
    /// 获取或设置成员条目列表。
    /// </summary>
    [JsonPropertyName("useritems")]
    public IList<OpenHardwareUserItem>? UserItems { get; set; }
}

/// <summary>
/// 设备成员条目（官方 95984 / 96037 响应字段表；两页字段名一致，故共用一型）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class OpenHardwareUserItem
{
    /// <summary>
    /// 获取或设置成员 <c>open_userid</c>。
    /// </summary>
    [JsonPropertyName("open_userid")]
    public string? OpenUserId { get; set; }

    /// <summary>
    /// 获取或设置成员姓名（官方 96037 该字段描述为「成员 userid」，字段名仍为 <c>user_name</c>）。
    /// </summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>
    /// 获取或设置成员类型：0-企业成员；2-学生。
    /// </summary>
    [JsonPropertyName("user_type")]
    public uint UserType { get; set; }

    /// <summary>
    /// 获取或设置放行规则列表（<c>id</c> 为编号、<c>rule</c> 为规则串，
    /// 其解析语法见官方门禁放行规则语法说明附录）。
    /// </summary>
    [JsonPropertyName("pass_rule")]
    public IList<OpenHardwarePassRule>? PassRule { get; set; }
}

/// <summary>
/// 门禁放行规则条目。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "OpenHardware")]
public class OpenHardwarePassRule
{
    /// <summary>
    /// 获取或设置规则编号。
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// 获取或设置规则串（语法解析见官方附录，本 SDK 不解析、原样透传）。
    /// </summary>
    [JsonPropertyName("rule")]
    public string? Rule { get; set; }
}
