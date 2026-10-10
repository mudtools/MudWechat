// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.User;

/// <summary>
/// 批量获取用户基本信息请求体（<c>batchUserinfo</c>，<c>POST /cgi-bin/user/info/batchget</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>user_list</c>（objarray，必填）——元素含 <c>openid</c>（必填）与
/// <c>lang</c>（可选：<c>zh_CN</c> 简体 / <c>zh_TW</c> 繁体 / <c>en</c> 英语，默认为 zh-CN）。
/// </para>
/// <para><b>单次数量上限：最多一次拉取 100 条</b>（超限错误码 <c>40032</c>）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "User")]
public class MpBatchGetUserInfoRequest
{
    /// <summary>待查询的用户列表（单次最多 100 条）。</summary>
    [JsonPropertyName("user_list")]
    public List<MpBatchUserInfoItem> UserList { get; set; } = new();
}

/// <summary>批量查询项（<c>user_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "User")]
public class MpBatchUserInfoItem
{
    /// <summary>用户的标识，对当前公众号唯一（必须是<b>已关注</b>用户的 openid）。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>国家地区语言版本（<c>zh_CN</c> / <c>zh_TW</c> / <c>en</c>；留空表示官方默认 zh-CN）。</summary>
    [JsonPropertyName("lang")]
    public string? Lang { get; set; }
}

/// <summary>
/// 批量获取用户基本信息响应（<c>batchUserinfo</c>）。
/// </summary>
/// <remarks>
/// 官方字段表：<c>user_info_list</c>（objarray）——元素字段与「获取用户基本信息」响应<b>完全同集</b>
/// （故元素类型共用 <see cref="MpUserInfo"/>，避免两份会漂移的声明）。
/// 官方返回示例说明：列表内可能同时包含<b>已关注</b>与<b>未关注</b>用户（后者 <c>subscribe = 0</c>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "User")]
public class MpBatchGetUserInfoResponse : MpResponse
{
    /// <summary>用户信息列表（失败时缺省为 <c>null</c>）。</summary>
    [JsonPropertyName("user_info_list")]
    public List<MpUserInfo>? UserInfoList { get; set; }
}
