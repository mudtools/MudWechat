// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.User;

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
