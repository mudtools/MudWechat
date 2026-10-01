// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contracts.Users;

/// <summary>
/// 创建成员请求体（<c>/cgi-bin/user/create</c>，仅通讯录同步助手或第三方通讯录应用可调用）。
/// </summary>
/// <remarks>每个部门下的部门、成员总数不能超过 3 万个；建议创建部门与创建成员串行处理。</remarks>
public class CreateUserRequest
{
    /// <summary>
    /// 获取或设置成员 UserID（企业内唯一，1~64 字节，仅数字/字母及 "_-@."，首字符须为数字或字母，唯一性检查忽略大小写）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置成员名称（1~64 个 UTF-8 字符）。
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置成员英文名（1~64 字节，由字母、数字、点(.)、减号(-)、空格或下划线(_)组成。官方现行参数表未单列，为对齐 Senparc 契约保留）。
    /// </summary>
    [JsonPropertyName("english_name")]
    public string? EnglishName { get; set; }

    /// <summary>
    /// 获取或设置成员别名（1~64 个 UTF-8 字符）。
    /// </summary>
    [JsonPropertyName("alias")]
    public string? Alias { get; set; }

    /// <summary>
    /// 获取或设置手机号（企业内唯一，mobile/email 不能同时为空；中国大陆可省略 +86，其他地区须带国际码）。
    /// </summary>
    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    /// <summary>
    /// 获取或设置所属部门 ID 列表（不超过 100 个；不填或 ID 为 0 则放入「其他（待设置部门）」；部门不存在时会自动新建）。
    /// </summary>
    [JsonPropertyName("department")]
    public List<int>? Department { get; set; }

    /// <summary>
    /// 获取或设置部门内排序值（默认 0，须与 Department 个数一致，官方范围 [0, 2^32)，数值越大越靠前，故以 64 位整数承载）。
    /// </summary>
    [JsonPropertyName("order")]
    public List<long>? Order { get; set; }

    /// <summary>
    /// 获取或设置职务（0~128 字符）。
    /// </summary>
    [JsonPropertyName("position")]
    public string? Position { get; set; }

    /// <summary>
    /// 获取或设置性别（官方示例按字符串传输："1" 男，"2" 女）。
    /// </summary>
    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    /// <summary>
    /// 获取或设置邮箱（6~64 字节，企业内唯一，境外成员可用此邮箱登录）。
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// 获取或设置腾讯企业邮账号（6~64 字节，企业内唯一；未填则系统生成默认企业邮箱，可修改一次）。
    /// </summary>
    [JsonPropertyName("biz_mail")]
    public string? BizMail { get; set; }

    /// <summary>
    /// 获取或设置座机（32 字节内，由纯数字、"-"、"+" 或 "," 组成）。
    /// </summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    /// <summary>
    /// 获取或设置是否部门负责人（个数须与 Department 一致；1 是，0 否）。
    /// </summary>
    [JsonPropertyName("is_leader_in_dept")]
    public List<int>? IsLeaderInDept { get; set; }

    /// <summary>
    /// 获取或设置直属上级 UserID（最多设置 1 个）。
    /// </summary>
    [JsonPropertyName("direct_leader")]
    public List<string>? DirectLeader { get; set; }

    /// <summary>
    /// 获取或设置头像媒体文件 ID（通过素材管理接口上传图片获得）。
    /// </summary>
    [JsonPropertyName("avatar_mediaid")]
    public string? AvatarMediaId { get; set; }

    /// <summary>
    /// 获取或设置启用状态（1 启用，0 禁用）。
    /// </summary>
    [JsonPropertyName("enable")]
    public int? Enable { get; set; }

    /// <summary>
    /// 获取或设置扩展属性（须先在企业 WEB 管理端添加对应字段，否则忽略）。
    /// </summary>
    [JsonPropertyName("extattr")]
    public UserExtAttr? ExtAttr { get; set; }

    /// <summary>
    /// 获取或设置是否邀请该成员使用企业微信（默认 true；每天自动下发一次邀请，最多 3 个工作日）。
    /// </summary>
    [JsonPropertyName("to_invite")]
    public bool? ToInvite { get; set; }

    /// <summary>
    /// 获取或设置成员对外属性。
    /// </summary>
    [JsonPropertyName("external_profile")]
    public UserExternalProfile? ExternalProfile { get; set; }

    /// <summary>
    /// 获取或设置对外职务（12 个汉字以内；设置后优先于 Position 对外展示）。
    /// </summary>
    [JsonPropertyName("external_position")]
    public string? ExternalPosition { get; set; }

    /// <summary>
    /// 获取或设置视频号名字（须从企业绑定的视频号中选择）。
    /// </summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>
    /// 获取或设置地址（最大 128 字符）。
    /// </summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>
    /// 获取或设置主部门 ID。
    /// </summary>
    [JsonPropertyName("main_department")]
    public int? MainDepartment { get; set; }
}
